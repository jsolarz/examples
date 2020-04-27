using SmsQueueSenderService.Infrastructure.Interfaces;
using System.Collections.Specialized;
using System.Configuration;
using System.Dynamic;

namespace SmsQueueSenderService.Infrastructure
{
    public class AppSettingsWrapper : DynamicObject, IAppSettingsWrapper
    {
        private NameValueCollection _items;

        public AppSettingsWrapper()
        {
            _items = ConfigurationManager.AppSettings;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _items[binder.Name];
            return result != null;
        }
    }
}
