using BusinessLogic.Dtos.SmsSenderService;
using System.Threading.Tasks;
using Topshelf;

namespace SmsQueueSenderService.Infrastructure.Interfaces
{
    public interface ISmsSenderService
    {        
        bool Start(HostControl host);
        bool Stop(HostControl host);
    }
}