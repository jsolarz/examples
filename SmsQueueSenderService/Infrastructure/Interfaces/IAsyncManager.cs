using System;
using System.Threading.Tasks;

namespace SmsQueueSenderService.Infrastructure.Interfaces
{
    public interface IAsyncManager
    {
        Task Run(Action action, Action<Exception> exceptionHandler = null);
        Task Run(Action action, TaskCreationOptions options, Action<Exception> exceptionHandler = null);
    }
}