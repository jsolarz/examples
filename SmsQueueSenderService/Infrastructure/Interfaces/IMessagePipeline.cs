using System;
using System.Threading;

namespace SmsQueueSenderService.Infrastructure.Interfaces
{
    public interface IMessagePipeline : IDisposable
    {
        void Process(string queue, CancellationToken token);
    }
}
