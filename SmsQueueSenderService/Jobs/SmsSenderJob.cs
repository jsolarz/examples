using SmsQueueSenderService.Infrastructure.Interfaces;
using BusinessLogic.Billing.SMS.ATSS;
using Framework;
using Quartz;
using System;

namespace SmsQueueSenderService.Jobs
{
    public class SmsSenderJob : IJob
    {
        private ISmsServiceConfiguration _config;
        private readonly ISmsSendingList _smsSendingList;
        private readonly ISms019Proxy _sms019;

        public SmsSenderJob(ISmsServiceConfiguration config, ISmsSendingList smsSendingList, INexmoProxy nexmo, ISms019Proxy sms019)
        {
            _config = config;
            _smsSendingList = smsSendingList;
            _sms019 = sms019;

            _sms019.Settings = Program.Settings;
            _sms019.SetServicePoint();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            if (Program.Settings == null)
            {
                LogWriter.LogEntry("ATSS settings is null");
                return;
            }

            try
            {
                var list = _smsSendingList.Get();

                if (list == null || list.Count == 0)
                    return;

                _sms019.SendBatch(list, Program.Settings);
            }
            catch (Exception ex)
            {
                LogWriter.LogException(ex, "ATSS there was an error while Execute sms sender job");
            }
        }
    }
}
