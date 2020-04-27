using Activetrail.SmsQueueSenderService.Infrastructure;
using ActiveTrail.BusinessLogic.Billing.SMS.ATSS;
using Quartz;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Activetrail.SmsQueueSenderService.Jobs
{
    public class MessagesRemovalJob : IJob
    {
        private ISmsServiceConfiguration _config;
        private static IDocumentStore _store;

        public MessagesRemovalJob(ISmsServiceConfiguration config, IDocumentStore store)
        {
            _config = config;
            _store = store;
        }

        public void Execute(IJobExecutionContext context)
        {
            using (TransactionScope scope = CreateTransactionScope())
            using (var session = _store.OpenSession(database: "messages"))
            {
                
                var toRemove = session.Query<SMSMessageQueueInfo>().Where(x => (DateTime.Today - x.AddedOn).TotalDays > 7).ToList();
                foreach (var item in toRemove)
                {
                    session.Delete(item.Id);
                }

                session.SaveChanges();
                scope.Complete();
            }
        }

        private TransactionScope CreateTransactionScope()
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.MaximumTimeout
            };
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }
    }
}
