//using Hangfire;
//using Hangfire.SqlServer;
using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartup(typeof(Activetrail.SmsQueueSenderService.Infrastructure.Startup))]

namespace Activetrail.SmsQueueSenderService.Infrastructure
{

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseErrorPage();
            app.UseWelcomePage("/");

            //string masterConEnc = ConfigurationManager.AppSettings["MasterConnectionString"];
            //string masterConDec = KeyHandler.GetDecryptor(KeyHandler.GetIV()).Decrypt(masterConEnc);

            //GlobalConfiguration.Configuration.UseSqlServerStorage("HangFireConn", new SqlServerStorageOptions { QueuePollInterval = TimeSpan.FromSeconds(1) });

            //app.UseHangfireDashboard();
            //app.UseHangfireServer();            

            //RecurringJob.AddOrUpdate(
            //    () => Console.WriteLine("{0} Recurring job completed successfully!", DateTime.Now.ToString()),
            //    Cron.Minutely);
        }
    }
}
