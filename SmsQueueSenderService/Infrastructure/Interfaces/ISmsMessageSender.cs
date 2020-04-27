using BusinessLogic.Billing.SMS.ATSS;
using System.Threading.Tasks;

namespace SmsQueueSenderService.Infrastructure.Interfaces
{
    public interface ISmsMessageSender
    {
        void Process(SMSMessageQueueInfo message);
        Task ProcessAsync(SMSMessageQueueInfo message);
    }
}