using ActiveTrail.BusinessLogic.Billing.SMS.ATSS;
using ActiveTrail.Framework.Core;
using Raven.Client.Indexes;
using System.Linq;

namespace Activetrail.SmsQueueSenderService.Infrastructure.Db
{

    public class SMSMessageQueueInfo_By : AbstractIndexCreationTask<SMSMessageQueueInfo>
    {
        public SMSMessageQueueInfo_By()
        {
            Map = messages => from message in messages
                              select new
                              {
                                  message.ProxyType,
                                  message.Status
                              };

            Index(x => x.ProxyType, Raven.Abstractions.Indexing.FieldIndexing.NotAnalyzed);
            Index(x => x.Status, Raven.Abstractions.Indexing.FieldIndexing.NotAnalyzed);
        }
    }


    public class SMSMessageQueueInfo_ByStatus : AbstractIndexCreationTask<SMSMessageQueueInfo>
    {
        public SMSMessageQueueInfo_ByStatus()
        {
            Map = messages => from message in messages
                              select new { message.Status };

            Index(x => x.Status, Raven.Abstractions.Indexing.FieldIndexing.NotAnalyzed);
        }
    }

    public class SMSMessageQueueInfo_ByProxy : AbstractIndexCreationTask<SMSMessageQueueInfo, SMSMessageQueueInfo_ByProxy.Result>
    {
        public class Result
        {
            public TypesSmsProxyTypes ProxyType { get; set; }
            public int Total { get; set; }
            public int Pending { get; set; }
            public int Sent { get; set; }
        }

        public SMSMessageQueueInfo_ByProxy()
        {
            Map = messages => from message in messages
                              select new
                              {
                                  message.ProxyType,
                                  Pending = message.Status == 1 ? 1 : 0,
                                  Sent = message.Status == 2 ? 1 : 0,
                                  Total = 1
                              };

            Reduce = results => from result in results
                                group result by result.ProxyType into g
                                let total = g.Sum(x => x.Total)
                                let pending = g.Sum(x => x.Pending)
                                let sent = g.Sum(x => x.Sent)
                                select new
                                {
                                    ProxyType = g.Key,
                                    Total = total,
                                    Pending = pending,
                                    Sent = sent
                                };
        }
    }

    public class SMSMessageQueueInfo_ByProxyAndStatus : AbstractIndexCreationTask<SMSMessageQueueInfo, SMSMessageQueueInfo_ByProxyAndStatus.Result>
    {
        public class Result
        {
            public TypesSmsProxyTypes ProxyType { get; set; }
            public short Status { get; set; }
            public int Count { get; set; }
        }

        public SMSMessageQueueInfo_ByProxyAndStatus()
        {
            Map = messages => from message in messages
                              select new
                              {
                                  message.ProxyType,
                                  message.Status,
                                  Count = 1
                              };

            Reduce = results => from result in results
                                group result by new
                                {
                                    result.ProxyType,
                                    result.Status
                                }
                                into g
                                select new
                                {
                                    ProxyType = g.Key.ProxyType,
                                    Status = g.Key.Status,
                                    Count = g.Sum(x => x.Count)
                                };

        }
    }
}
