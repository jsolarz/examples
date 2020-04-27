//using Activetrail.SmsQueueSenderService.Dashboard.Models;
//using Activetrail.SmsQueueSenderService.Infrastructure.Db;
//using ActiveTrail.BusinessLogic.Billing.SMS.ATSS;
//using ActiveTrail.Framework.Core;
//using Raven.Client;
//using System.Collections.Generic;
//using System.Linq;
//using System.Transactions;

//namespace Activetrail.SmsQueueSenderService.Dashboard.Repositories
//{
//    public class SmsCounterRepository : ISmsCounterRepository
//    {
//        private readonly IDocumentStore _store;
//        int pageSize = 1000;
//        int pageNumber = 0;
//        public SmsCounterRepository()
//        {
//            //IDocumentStore store
//            //_store = store;
//            //_store.Initialize();
//        }

//        public List<SmsCounter> List()
//        {
//            List<SmsCounter> counter = new List<SmsCounter>()
//            {
//                new SmsCounter
//                {
//                    Proxy = TypesSmsProxyTypes.Sms019
//                },
//                new SmsCounter
//                {
//                    Proxy = TypesSmsProxyTypes.Nexmo
//                }
//            };

//            using (TransactionScope scope = CreateTransactionScope())
//            using (var session = _store.OpenSession(database: "messages"))
//            {
//                var result = session
//                    .Query<SMSMessageQueueInfo_ByProxy.Result, SMSMessageQueueInfo_ByProxy>().ToList();

//                var sms019Results = result.Where(x => x.ProxyType == TypesSmsProxyTypes.Sms019).SingleOrDefault();
//                var nexmoResults = result.Where(x => x.ProxyType == TypesSmsProxyTypes.Nexmo).SingleOrDefault();

//                counter.First().Total = sms019Results?.Total ?? 0;
//                counter.First().TotalSent = sms019Results?.Sent ?? 0;

//                counter.Last().Total = nexmoResults?.Total ?? 0;
//                counter.Last().TotalSent = nexmoResults?.Sent ?? 0;

//                scope.Complete();
//            }

//            return counter;
//        }

//        public List<SMSMessageQueueInfo> GetBy(TypesSmsProxyTypes proxy, string status, int page)
//        {
//            List<SMSMessageQueueInfo> list;
            
//            using (TransactionScope scope = CreateTransactionScope())
//            using (var session = _store.OpenSession(database: "messages"))
//            {
//                switch (status)
//                {
//                    case "Pending":
//                        list = session.Query<SMSMessageQueueInfo, SMSMessageQueueInfo_By>()
//                        .Where(x => x.ProxyType == proxy && x.Status == 1)
//                        .Skip((pageNumber * pageSize))
//                        .Take(pageSize)
//                        .ToList();
//                        break;
//                    case "Sent":
//                        list = session.Query<SMSMessageQueueInfo, SMSMessageQueueInfo_By>()
//                        .Where(x => x.ProxyType == proxy && x.Status == 2)
//                        .Skip((pageNumber * pageSize))
//                        .Take(pageSize)
//                        .ToList();
//                        break;
//                    case "All":
//                    default:
//                        list = session.Query<SMSMessageQueueInfo, SMSMessageQueueInfo_By>()
//                        .Where(x => x.ProxyType == proxy)
//                        .Skip((pageNumber * pageSize))
//                        .Take(pageSize)
//                        .ToList();
//                        break;
//                }

//                scope.Complete();
//            }

//            return list ?? new List<SMSMessageQueueInfo>();
//        }

//        private TransactionScope CreateTransactionScope()
//        {
//            var transactionOptions = new TransactionOptions
//            {
//                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
//                Timeout = TransactionManager.MaximumTimeout
//            };
//            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
//        }
//    }
//}
