using SmsQueueSenderService.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;

namespace SmsQueueSenderService.Infrastructure
{
    public sealed class AsyncManager : IAsyncManager
    {
        private Action<Task> DefaultExeptionHandler = t =>
        {
            try { t.Wait(); }
            catch { /* Swallow the exception */ }
        };

        public Task Run(Action action, Action<Exception> exceptionHandler = null)
        {
            if (action == null) { throw new ArgumentNullException("Async manager exception"); }

            var task = new Task(action);

            Action<Task> handler = exceptionHandler != null ?
                new Action<Task>(t => exceptionHandler(t.Exception.GetBaseException())) :
                DefaultExeptionHandler;

            var continuation = task.ContinueWith(handler,
                TaskContinuationOptions.ExecuteSynchronously
                | TaskContinuationOptions.OnlyOnFaulted);
            task.Start();

            return continuation;
        }

        public Task Run(Action action, TaskCreationOptions options, Action<Exception> exceptionHandler = null)
        {
            var task = new Task(action, options);
            task.ConfigureAwait(false);

            Action<Task> handler = exceptionHandler != null ?
                new Action<Task>(t => exceptionHandler(t.Exception.GetBaseException())) :
                DefaultExeptionHandler;

            var continuation = task.ContinueWith(handler,
                TaskContinuationOptions.ExecuteSynchronously
                | TaskContinuationOptions.OnlyOnFaulted);
            task.Start();

            return continuation;
        }
    }
}
