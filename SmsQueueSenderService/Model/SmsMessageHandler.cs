using SmsQueueSenderService.Infrastructure;
using SmsQueueSenderService.Infrastructure.Interfaces;
using BusinessLogic;
using BusinessLogic.Billing.SMS.ATSS;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SmsQueueSenderService.Model
{
    public class SmsMessageHandler : ISmsMessageHandler
    {
        private readonly INexmoProxy _nexmo;
        private readonly ISms019Proxy _sms019;
        private readonly ISmsSendingList _smsSendingList;
        private readonly IAsyncManager _asyncManager;
        private dynamic _appSettings;

        private int _worksCount;

        public SmsMessageHandler(IAppSettingsWrapper appSettings, INexmoProxy nexmo, ISms019Proxy sms019, ISmsSendingList smsSendingList, IAsyncManager asyncManager)
        {
            _appSettings = appSettings;
            _nexmo = nexmo;
            _sms019 = sms019;
            _smsSendingList = smsSendingList;
            _asyncManager = asyncManager;

            _worksCount = Convert.ToInt32(_appSettings.WorkersCount);
        }

        /// <summary>
        /// Handle a single message. Saves it to the DB or send it right away
        /// </summary>
        /// <param name="queueType"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool HandleMessage(SmsCampaignSendType queueType, SMSMessageQueueInfo message)
        {
            try
            {
                _nexmo.Settings = Program.Settings;
                _sms019.Settings = Program.Settings;
                _sms019.SetServicePoint();

                if (message.ProxyType == Framework.Core.TypesSmsProxyTypes.Nexmo)
                {
                    _nexmo.SendAsync(message, Program.Settings).Wait();
                    return true;
                }

                switch (queueType)
                {
                    case SmsCampaignSendType.Regular:
                    case SmsCampaignSendType.Campaign:
                        LogWriter.LogEntry(string.Format("Adding to list for 019 - CampaignId: {0} - MessageId: {1} - Mobile: {2}; ", message.CampaignId, message.Token, message.RecipientNumber));
                        _smsSendingList.Add(message);
                        break;
                    case SmsCampaignSendType.Automation:
                    case SmsCampaignSendType.TMM:
                    case SmsCampaignSendType.Test:
                        LogWriter.LogEntry(string.Format("Sending - queue: {3} - CampaignId: {0} - MessageId: {1} - Mobile: {2}; ", message.CampaignId, message.Token, message.RecipientNumber, queueType));
                        _sms019.Send(message, Program.Settings);
                        break;
                    default:
                        LogWriter.LogEntry(string.Format("Adding to list for 019 - CampaignId: {0} - MessageId: {1} - Mobile: {2}; ", message.CampaignId, message.Token, message.RecipientNumber));
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogWriter.LogException(ex, "In Tma worker, exception message : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Handle a list of messages to be sent
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public bool HandleMessages(List<SMSMessageQueueInfo> messages)
        {

            //try catch inside
            _smsSendingList.UpdateStatus(SmsMessageStatus.PROCESSING, messages);

            _asyncManager.Run(() =>
            {
                _sms019.Settings = Program.Settings;
                _sms019.SetServicePoint();

                try
                {
                    var sms019 = messages.Where(m => m.ProxyType == Framework.Core.TypesSmsProxyTypes.Sms019).ToList();

                    var sent = _sms019.SendBatch(sms019, Program.Settings);

                    if (sent)
                    {
                        _smsSendingList.UpdateStatus(SmsMessageStatus.SUCCESS, messages);
                    }
                }
                catch (Exception ex)
                {
                    LogWriter.LogException(ex, "There was an error while sending to sms 019: " + ex.Message);
                }
            }, System.Threading.Tasks.TaskCreationOptions.LongRunning, ex => LogWriter.LogException(ex, "There was an error while sending to sms 019: " + ex.Message));

            _asyncManager.Run(() =>
            {
                _nexmo.Settings = Program.Settings;
                _nexmo.SetServicePoint();

                var nexmo = messages.Where(m => m.ProxyType == Framework.Core.TypesSmsProxyTypes.Nexmo).ToList();

                foreach (var message in nexmo)
                {
                    while (Counter.Value == _worksCount)
                    {
                        Thread.Sleep(100);
                    }

                    Counter.Increment();

                    try
                    {
                        LogWriter.LogEntry(string.Format("Sending through {3} - CampaignId: {0} - MessageId: {1} - Mobile: {2}; ", message.CampaignId, message.Token, message.RecipientNumber, message.ProxyType));
                        var sent = _nexmo.Send(message, Program.Settings);

                        if (sent)
                        {
                            _smsSendingList.UpdateStatus(SmsMessageStatus.SUCCESS, message);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWriter.LogException(ex, "There was an error while sending to nexmo: " + ex.Message);
                    }

                    Counter.Decrement();
                }
            }, System.Threading.Tasks.TaskCreationOptions.LongRunning, ex => LogWriter.LogException(ex, "There was an error while sending to nexmo " + ex.Message));

            return true;
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
