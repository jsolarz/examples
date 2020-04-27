using StructureMap;
using System;

namespace SmsQueueSenderService.Infrastructure.IoC
{
    /// <summary>
    /// This is required to wire up your types - it should always happen when you app is starting up
    /// </summary>
    public sealed class IoCBootstrapper
    {
        private static readonly Lazy<IoCBootstrapper> lazy = new Lazy<IoCBootstrapper>(() => new IoCBootstrapper());
        /// <summary>
        /// This is required to wire up your types - it should always happen when you app is starting up
        /// </summary>
        public static IoCBootstrapper Instance { get { return lazy.Value; } }
        public Container Container { get; private set; }

        private IoCBootstrapper()
        {
            // this is required to wire up your types - it should always happen when you app is starting up
            //var registry = new Registry();
            //registry.IncludeRegistry<SmsSenderServiceRegistry>();
            //registry.IncludeRegistry<RavenDbWebRegistry>();

            Container = new Container(cfg =>
            {
                cfg.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                    scan.LookForRegistries();
                });
            });

            Container.AssertConfigurationIsValid();
        }
    }
}
