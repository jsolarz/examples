//using Nancy;
//using Nancy.Bootstrapper;
////using Nancy.Bootstrappers.StructureMap;
//using Nancy.Conventions;
//using StructureMap;

//namespace Activetrail.SmsQueueSenderService.Dashboard.Configuration
//{
//    public class CustomBootstrapper : StructureMapNancyBootstrapper
//    {
//        protected override void ApplicationStartup(IContainer container, IPipelines pipelines)
//        {
//            // No registrations should be performed in here, however you may
//            // resolve things that are needed during application startup.

//            //Prevent errors on Linux
//            StaticConfiguration.DisableErrorTraces = false;                  
//        }

//        protected override void ConfigureApplicationContainer(IContainer existingContainer)
//        {
//            existingContainer.Configure(x =>
//            {
//                x.Scan(scan =>
//                {
//                    scan.TheCallingAssembly();
//                    scan.WithDefaultConventions();
//                    scan.LookForRegistries();
//                });
//            });
//            base.ConfigureApplicationContainer(existingContainer);
//        }

//        protected override void ConfigureRequestContainer(IContainer container, NancyContext context)
//        {
//            base.ConfigureApplicationContainer(container);
//        }

//        protected override void RequestStartup(IContainer container, IPipelines pipelines, NancyContext context)
//        {
//            // No registrations should be performed in here, however you may
//            // resolve things that are needed during request startup.
//        }

        
//        protected override void ConfigureConventions(NancyConventions conventions)
//        {
//            base.ConfigureConventions(conventions);
//            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Dashboard/Assets/js"));
//            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Dashboard/Assets/css"));
//            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Dashboard/Assets/fonts"));
//            conventions.ViewLocationConventions.Clear();
//            conventions.ViewLocationConventions.Add((viewName, model, context) => string.Concat("Dashboard/Views/", viewName));

//            //ResourceViewLocationProvider.RootNamespaces.Add(Assembly.GetAssembly(typeof(MainModule)), "Activetrail.SmsQueueSenderService.Dashboard.Views");
//        }
//    }
//}
