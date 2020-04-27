using System.Threading;

namespace SmsQueueSenderService.Infrastructure.Interfaces
{
    public interface IProcessorManager
    {
        void Start(CancellationToken cancellationToken);
        void Stop(CancellationToken cancellationToken);
    }
}