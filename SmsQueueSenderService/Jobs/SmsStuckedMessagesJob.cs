using Activetrail.SmsQueueSenderService.Infrastructure.Interfaces;
using ActiveTrail.Framework;
using Quartz;
using System;

namespace Activetrail.SmsQueueSenderService.Jobs
{
    public class SmsStuckedMessagesJob : IJob
    {
        private dynamic _appSettings;
        private readonly ISmsSendingList _smsSendingList;
        private readonly ISmsMessageHandler _handler;

        public SmsStuckedMessagesJob(IAppSettingsWrapper appSettingsWrapper, ISmsSendingList smsSendingList, ISmsMessageHandler handler)
        {
            _appSettings = appSettingsWrapper;
            _smsSendingList = smsSendingList;
            _handler = handler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                bool hasStuckedMessages = true;
                while (hasStuckedMessages)
                {
                    var list = _smsSendingList.Get();

                    if (list == null || list.Count == 0)
                    {
                        hasStuckedMessages = false;
                        continue;
                    }

                    _handler.HandleMessages(list);
                }
            }
            catch (Exception ex)
            {

                LogWriter.LogException(ex, "There was an error while Execute SF");
            }
        }
    }
}
