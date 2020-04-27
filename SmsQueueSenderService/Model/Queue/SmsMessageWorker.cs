using SmsQueueSenderService.Infrastructure.Interfaces;
using BusinessLogic.Billing.SMS.ATSS;
using Framework;
using Framework.Data;
using System;
using System.Collections.Generic;
using System.Messaging;

namespace SmsQueueSenderService.Model.Queue
{
    public class SmsMessageQueueWorker : ISmsMessageQueueWorker
    {        
        public List<SMSMessageQueueInfo> Dequeue(string queuePath, int count)
        {
            List<SMSMessageQueueInfo> collection = new List<SMSMessageQueueInfo>();

            using (var queue = new MessageQueue(queuePath, QueueAccessMode.Receive))
            {
                queue.Formatter = new JsonMessageFormatter();
                queue.MessageReadPropertyFilter.SetAll();
                queue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);

                using (MessageEnumerator me = queue.GetMessageEnumerator2())
                {
                    try
                    {
                        while (me.MoveNext() && count > 0)
                        {

                            Message msg = queue.Receive(MessageQueueTransactionType.Single);
                            collection.Add((SMSMessageQueueInfo)msg.Body);
                            count--;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWriter.LogException(ex, "Invalid operation when receiving from queue: " + queuePath);
                    }

                    me.Close();
                }

                queue.Close();
            }

            return collection;
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
