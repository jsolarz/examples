using Nancy.ViewEngines.Razor;
using StructureMap;
using System.Collections.Generic;

namespace Activetrail.SmsQueueSenderService.Infrastructure
{
    public class RazorRegistry : Registry
    {
        public RazorRegistry()
        {
            ForSingletonOf<IRazorConfiguration>().Use<RazorConfig>();
        }
    }



    public class RazorConfig : IRazorConfiguration
    {
        public IEnumerable<string> GetAssemblyNames()
        {
            yield return "System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
            yield return "System.Web.Abstractions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35";
            yield return "System.Web.Helpers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35";
            yield return "System.Web.Routing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35";
            yield return "System.Web.Mvc, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35";
            yield return "System.Web.WebPages, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35";
            yield return "ActiveTrail.BusinessLogic";
            yield return "ActiveTrail.Framework";
            yield return "Activetrail.SmsQueueSenderService";
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            //contains html extensions
            yield return "System.Web.Helpers";
            yield return "System.Web.Mvc";
            yield return "System.Globalization";
            yield return "System.Collections.Generic";
            yield return "System.Linq";
            yield return "ActiveTrail.Framework.Core";
            yield return "ActiveTrail.BusinessLogic.Billing.SMS.ATSS";
            yield return "Activetrail.SmsQueueSenderService.Dashboard.Models";
        }

        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }
    }
}
