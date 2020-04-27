using BusinessLogic.Dtos.SmsSenderService;
using System.ServiceModel;
using System.Xml.Linq;

namespace SmsQueueSenderService.Model
{
    public interface IQueueProcessor
    {
        SmsSendersSettingsDTO Settings { get; set; }
        void SetSettings(SmsSendersSettingsDTO settings);
        void ProcessQueue();
        void Dispose();
    }
}
