using SmsQueueSenderService.Infrastructure.Interfaces;
using SmsQueueSenderService.Infrastructure.IoC;
using BusinessLogic.Billing.SMS.ATSS;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SmsQueueSenderService.Model
{
    public class MessagePipeline : IMessagePipeline
    {
        private readonly dynamic _appSettings;

        TransformBlock<string, List<SMSMessageQueueInfo>> _dequeue;
        TransformBlock<List<SMSMessageQueueInfo>, List<SMSMessageQueueInfo>> _changeStatusToPending;
        WriteOnceBlock<List<SMSMessageQueueInfo>> _broadcaster;
        TransformManyBlock<List<SMSMessageQueueInfo>, SMSMessageQueueInfo> _nexmo;
        TransformBlock<List<SMSMessageQueueInfo>, List<SMSMessageQueueInfo>> _sms019;
        ActionBlock<SMSMessageQueueInfo> _sendToNexmo;
        ActionBlock<List<SMSMessageQueueInfo>> _sendToSms019;

        public MessagePipeline(IAppSettingsWrapper appSettings)
        {
            _appSettings = appSettings;

            var _messageCounter = Convert.ToInt32(_appSettings.MessagesCount);

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            //Dequeueing from specified queue and returning all the messages we got
            _dequeue = new TransformBlock<string, List<SMSMessageQueueInfo>>(queue =>
            {
                List<SMSMessageQueueInfo> list = null;
                using (var nested = IoCBootstrapper.Instance.Container.GetNestedContainer())
                using (var queueWorker = nested.GetInstance<ISmsMessageQueueWorker>())
                {
                    try
                    {
                        list = queueWorker.Dequeue(queue, _messageCounter);

                        if (list.Count > 0)
                            LogWriter.LogEntry(string.Format("Processing {0} messages from {1}", list.Count, queue), LoggerType.Log4Net);
                    }
                    catch (Exception ex)
                    {
                        LogWriter.LogException(ex, "Not able to dequeue", LoggerType.Log4Net);
                    }
                }

                return list;
            });

            //saving dequeued messages to db
            _changeStatusToPending = new TransformBlock<List<SMSMessageQueueInfo>, List<SMSMessageQueueInfo>>(toSave =>
            {
                if (toSave == null || toSave.Count == 0)
                    return toSave;

                using (var nested = IoCBootstrapper.Instance.Container.GetNestedContainer())
                using (var smsSendingList = nested.GetInstance<ISmsSendingList>())
                {
                    LogWriter.LogEntry("Saving to DB: " + string.Join(",", toSave.Select(c => string.Format("[{0}-{1}-{2}]", c.CampaignId, c.Token, c.RecipientNumber)).ToArray()), LoggerType.Log4Net);
                    smsSendingList.BulkAdd(toSave);
                }

                return toSave;
            });

            _broadcaster = new WriteOnceBlock<List<SMSMessageQueueInfo>>(messages => messages);

            //separating messages by proxy type nexmo
            _nexmo = new TransformManyBlock<List<SMSMessageQueueInfo>, SMSMessageQueueInfo>(messages =>
            {
                if (messages == null || messages.Count == 0)
                    return messages;
                return messages.Where(m => m.ProxyType == Framework.Core.TypesSmsProxyTypes.Nexmo).ToList();
            });

            //separating messages by proxy type sms019
            _sms019 = new TransformBlock<List<SMSMessageQueueInfo>, List<SMSMessageQueueInfo>>(messages =>
            {
                if (messages == null || messages.Count == 0)
                    return messages;
                return messages.Where(m => m.ProxyType == Framework.Core.TypesSmsProxyTypes.Sms019).ToList();
            });

            //sending each message to nexmo
            _sendToNexmo = new ActionBlock<SMSMessageQueueInfo>(async message =>
            {
                if (message == null)
                    return;

                using (var nested = IoCBootstrapper.Instance.Container.GetNestedContainer())
                using (var nexmoProxy = nested.GetInstance<INexmoProxy>())
                {
                    try
                    {
                        nexmoProxy.Settings = Program.Settings;
                        nexmoProxy.SetServicePoint();

                        LogWriter.LogEntry(string.Format("Sending through {3} - CampaignId: {0} - MessageId: {1} - Mobile: {2}; ", message.CampaignId, message.Token, message.RecipientNumber, message.ProxyType), LoggerType.Log4Net);
                        var sent = await nexmoProxy.SendAsync(message, Program.Settings);

                        if (sent)
                        {
                            using (var smsSendingList = nested.GetInstance<ISmsSendingList>())
                            {
                                smsSendingList.UpdateStatus(SmsMessageStatus.SUCCESS, message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWriter.LogException(ex, "There was an error while sending to nexmo: " + ex.Message, LoggerType.Log4Net);
                    }
                }
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount - 1
            });

            //sending batch messages to 019
            _sendToSms019 = new ActionBlock<List<SMSMessageQueueInfo>>(messages =>
            {
                if (messages == null || messages.Count == 0)
                    return;

                using (var nested = IoCBootstrapper.Instance.Container.GetNestedContainer())
                using (var sms019Proxy = nested.GetInstance<ISms019Proxy>())
                {
                    sms019Proxy.Settings = Program.Settings;
                    sms019Proxy.SetServicePoint();

                    try
                    {
                        var sent = sms019Proxy.SendBatch(messages, Program.Settings);

                        using (var smsSendingList = nested.GetInstance<ISmsSendingList>())
                        {
                            smsSendingList.UpdateStatus(SmsMessageStatus.SUCCESS, messages);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWriter.LogException(ex, "There was an error while sending to sms 019: " + ex.Message, LoggerType.Log4Net);
                    }
                }
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount - 1
            });

            _dequeue.LinkTo(_changeStatusToPending, linkOptions);
            _changeStatusToPending.LinkTo(_broadcaster, linkOptions);
            _broadcaster.LinkTo(_nexmo, linkOptions);
            _broadcaster.LinkTo(_sms019, linkOptions);
            _nexmo.LinkTo(_sendToNexmo, linkOptions);
            _sms019.LinkTo(_sendToSms019, linkOptions);
        }

        public void Process(string queue, CancellationToken token)
        {
            try
            {
                _dequeue.Post(queue);
                _dequeue.Complete();

                Task.WaitAll(new Task[] { _sendToNexmo.Completion, _sendToSms019.Completion }, token);
            }
            catch (AggregateException ex)
            {
                var e = ex.Flatten();

                foreach (var error in e.InnerExceptions)
                {
                    LogWriter.LogException(ex, "There was an error in the pipeline while processing messages: " + error.Message, LoggerType.Log4Net);
                }
            }
        }

        #region Disposable
        // Flag: Has Dispose already been called?
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                //handle.Dispose();
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
        #endregion
    }
}
