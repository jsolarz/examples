using BusinessLogic.Dtos.SmsSenderService;
using System.Dynamic;

namespace SmsQueueSenderService.Infrastructure.Interfaces
{
    public interface IAppSettingsWrapper
    {
        bool TryGetMember(GetMemberBinder binder, out object result);
    }
}