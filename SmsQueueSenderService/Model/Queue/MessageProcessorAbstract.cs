using Activetrail.SmsQueueSenderService.Infrastructure;
using Activetrail.SmsQueueSenderService.Infrastructure.Interfaces;
using ActiveTrail.Framework;
using ActiveTrail.Framework.Data;
using System;
using System.Diagnostics;
using System.Linq;
using System.Messaging;
using System.Threading;

namespace Activetrail.SmsQueueSenderService.Model.Queue
{
    public abstract class MessageProcessor<TMessage> : IMessageProcessor
    {
        private readonly MessageQueue[] Receivers;        
        private bool IsClosing;
        protected static object _syncObj = new object();
        protected ISmsServiceConfiguration _config;

        public MessageProcessor(string path, ISmsServiceConfiguration config) : this(path, 1,config) { }

        public MessageProcessor(string path, int count, ISmsServiceConfiguration config) : base()
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (!MessageQueue.Exists(path))
                MessageQueue.Create(path, true);

            _config = config;

            this.Receivers = Enumerable.Range(0, (count <= 0) ? 1 : count)
                .Select(i =>
                {
                    var queue = new MessageQueue(path, QueueAccessMode.Receive)
                    {
                        Formatter = new JsonMessageFormatter()
                    };
                    queue.MessageReadPropertyFilter.SetAll();
                    return queue;
                })
                .ToArray();
        }        

        public void Close()
        {
            this.IsClosing = true;

            this.OnClosing();

            foreach (var queue in this.Receivers)
            {
                queue.PeekCompleted -= queue_PeekCompleted;
                queue.Close();
            }

            while (this.IsProcessing)
                Thread.Sleep(100);

            this.IsClosing = this.IsOpen = false;
            this.OnClosed();
        }

        public bool IsOpen { get; private set; }

        protected bool IsProcessing
        {
            get { return Counter.Value > 0; }
        }

        protected virtual void OnClosing() { }
        protected virtual void OnClosed() { }
        protected virtual void OnOpening() { }
        protected virtual void OnOpened() { }

        public void Open()
        {
            if (this.IsOpen)
                throw new Exception("This processor is already open.");

            this.OnOpening();

            foreach (var queue in this.Receivers)
            {
                queue.PeekCompleted += queue_PeekCompleted;
                queue.BeginPeek();
            }

            this.IsOpen = true;
            this.OnOpened();
        }

        protected abstract void Process(TMessage @object);

        private void Handle(Message message)
        {
            Trace.Assert(null != message);

            Counter.Increment();
            try
            {
                Process((TMessage)message.Body);
            }
            finally
            {
                Counter.Decrement();
            }
        }

        private void queue_PeekCompleted(object sender, PeekCompletedEventArgs e)
        {
            var queue = (MessageQueue)sender;

            var transaction = new MessageQueueTransaction();
            transaction.Begin();
            try
            {
                // if the queue closes after the transaction begins,
                // but before the call to Receive, then an exception
                // will be thrown and the transaction will be aborted
                // leaving the message to be processed next time
                this.Handle(queue.Receive(transaction));
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Abort();
                LogWriter.LogException(ex, string.Format("ATSS message process transaction aborted error: {0}", ex.Message));
                Trace.WriteLine(ex.Message);
            }
            finally
            {
                if (!this.IsClosing)
                    queue.BeginPeek();
            }
        }
    }
}
