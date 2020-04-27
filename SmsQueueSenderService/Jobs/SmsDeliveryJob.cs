using Activetrail.SmsQueueSenderService.Infrastructure;
using Activetrail.SmsQueueSenderService.Infrastructure.Interfaces;
using ActiveTrail.BusinessLogic;
using ActiveTrail.BusinessLogic.Billing.SMS.ATSS;
using ActiveTrail.Framework;
using Quartz;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace Activetrail.SmsQueueSenderService.Jobs
{
    public class SmsDeliveryJob : IJob
    {
        private dynamic _appSettings;
        private ISms019Proxy _sms019Proxy;
        private static HttpClient _httpClient;
        private readonly IAsyncManager _asyncManager;

        public SmsDeliveryJob(IAppSettingsWrapper appSettingsWrapper, ISms019Proxy sms019Proxy, HttpClient httpClient, IAsyncManager asyncManager)
        {
            _appSettings = appSettingsWrapper;
            _asyncManager = asyncManager;
            _sms019Proxy = sms019Proxy;

            if (_sms019Proxy.Settings == null)
            {
                _sms019Proxy.Settings = Program.Settings;
                _sms019Proxy.SetServicePoint();
            }

            _httpClient = httpClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            if (Program.Settings == null)
            {
                LogWriter.LogEntry(new LogWriterEntry("ATSS settings is null") { EntryType = LogWriterEntryType.Error, LogType = LogWriterLogType.File, IsLogExternally = true });
                return;
            }

            try
            {
                var from = DateTime.Now.AddMinutes(-20).ToString("dd/MM/yy HH:mm", CultureInfo.InvariantCulture);
                var to = DateTime.Now.AddDays(1).ToString("dd/MM/yy HH:mm", CultureInfo.InvariantCulture);
                LogWriter.LogEntry(new LogWriterEntry(string.Format("Getting dlrs from {0} to {1}", from, to)) { EntryType = LogWriterEntryType.Information, LogType = LogWriterLogType.File });

                _asyncManager.Run(async () =>
                {
                    try
                    {
                        var dlr = await _sms019Proxy.GetDeliveryReport(from, to, Program.Settings);

                        if (dlr != null && dlr.Transactions.Transaction != null && dlr.Transactions.Transaction.Count > 0)
                        {
                            ATSSSMSProvider provider = new ATSSSMSProvider();

                            foreach (var item in dlr.Transactions.Transaction)
                            {
                                var externalId = item.External_id;
                                var status = Convert.ToInt32(item.Status);
                                var resultType = (status == 102 || status == 0) ? SMSResultType.Succeeded : SMSResultType.Failed;
                                var message = item.En_message;
                                var batchId = item.Shipment_id;

                                if (!string.IsNullOrEmpty(externalId))
                                {
                                    provider.ProcessConfirmationLog(externalId, resultType, message, batchId);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWriter.LogException(ex, "ATSS there was an error while Execute sms delivery job");
                    }
                }, TaskCreationOptions.LongRunning, ex => LogWriter.LogException(ex, "ATSS there was an error while Execute sms delivery job"));
            }
            catch (Exception ex)
            {

                LogWriter.LogException(ex, "There was an error while Execute SF");
            }
        }
    }
}
