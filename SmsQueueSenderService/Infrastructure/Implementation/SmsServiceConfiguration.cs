using SmsQueueSenderService.Infrastructure.Interfaces;

namespace SmsQueueSenderService.Infrastructure
{
    public class SmsServiceConfiguration : ISmsServiceConfiguration
    {
        public dynamic AppSettings { get; private set; }

        public SmsServiceConfiguration(IAppSettingsWrapper appSettingsWrapper)
        {
            AppSettings = appSettingsWrapper;
        }
    }
}
