using SmsQueueSenderService.Infrastructure.Interfaces;
using SmsQueueSenderService.Infrastructure.IoC;
using SmsQueueSenderService.Jobs;
using BusinessLogic.Dtos.SmsSenderService;
using Framework;
using Quartz;
using System;
using System.Configuration;
using Topshelf;
using Topshelf.Quartz.StructureMap;
using Topshelf.StructureMap;

namespace SmsQueueSenderService
{
    public class Program
    {
        public static SmsSendersSettingsDTO Settings { get; set; }

        public static void Main(string[] args)
        {
            var jobWaitingTime = int.Parse(ConfigurationManager.AppSettings["SendingTime"]);
            var deliveryJobWaitingTime = int.Parse(ConfigurationManager.AppSettings["DeliveryTime"]);

            HostFactory.Run(x =>
            {
                // Init StructureMap container 
                x.UseStructureMap(IoCBootstrapper.Instance.Container);

                x.Service<ISmsSenderService>(s =>
                {
                    s.ConstructUsing(name => IoCBootstrapper.Instance.Container.GetInstance<ISmsSenderService>());
                    s.WhenStarted((tc, hc) => tc.Start(hc));
                    s.WhenStopped((tc, hc) => tc.Stop(hc));

                    //Construct IJob instance with StructureMap
                    s.UseQuartzStructureMap();

                    //s.ScheduleQuartzJob(q =>
                    //    q.WithJob(() => JobBuilder.Create<SmsSenderJob>().Build())
                    //        .AddTrigger(() => TriggerBuilder.Create()
                    //            .StartAt(new DateTimeOffset(DateTime.Now.AddMinutes(2)))
                    //            .WithSimpleSchedule(t => t
                    //                .WithIntervalInSeconds(jobWaitingTime)
                    //                .RepeatForever())
                    //            .Build()));

                    //s.ScheduleQuartzJob(q =>
                    //    q.WithJob(() => JobBuilder.Create<SmsDeliveryJob>().Build())
                    //        .AddTrigger(() => TriggerBuilder.Create()
                    //            .StartAt(new DateTimeOffset(DateTime.Now.AddMinutes(2)))
                    //            .WithSimpleSchedule(t => t
                    //                .WithIntervalInMinutes(deliveryJobWaitingTime)
                    //                .RepeatForever())
                    //            .Build()));                    

                    //s.WithNancyEndpoint(x, c =>
                    //{
                    //    c.AddHost(port: 1337);
                    //    c.CreateUrlReservationsOnInstall();
                    //});
                });

                x.RunAsLocalSystem();

                x.BeforeUninstall(() =>
                {
                    // There is an issue where the service will not be removed properly if any user has the Services console running (otherwise a reboot is required)
                    foreach (var process in System.Diagnostics.Process.GetProcessesByName("mmc"))
                    {
                        process.Kill();
                    }
                });

                x.EnableServiceRecovery(r =>
                {
                    //you can have up to three of these                        
                    r.RestartService(0);
                    r.RestartService(0);
                    r.RestartService(0);

                    //should this be true for crashed or non-zero exits
                    r.OnCrashOnly();

                    //number of days until the error count resets
                    r.SetResetPeriod(1);
                });
               
                x.EnableShutdown();

                x.SetDescription("Services.SmsQueueSender"); //7
                x.SetDisplayName("Services.SmsQueueSender"); //8
                x.SetServiceName("Services.SmsQueueSender"); //9

                x.DependsOnMsmq(); // Microsoft Message Queueing

                x.SetStartTimeout(TimeSpan.FromSeconds(10));
                x.SetStopTimeout(TimeSpan.FromSeconds(10));

                x.OnException((exception) =>
                {
                    LogWriter.LogException(exception, string.Format("Exception thrown - {0}", exception.Message), LoggerType.Log4Net);
                });
            });
        }
    }
}
