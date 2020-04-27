using SmsQueueSenderService.Infrastructure.Interfaces;
using BusinessLogic.Billing.SMS.ATSS;
using Framework;
using Framework.DataAccess;
using Framework.Security;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace SmsQueueSenderService.Model
{
    public class SmsSendingList : ISmsSendingList
    {
        const string INSERT = @"INSERT INTO sms_messages (campaignId, recipientNumber, fromName, content, proxyType, token, status, addedOn, updatedOn)
                                VALUES (@campaignId, @recipientNumber, @fromName, @content, @proxyType, @token, 1, GETDATE(), GETDATE())";

        const string QUERY = @"SELECT TOP 500 id, campaignId, recipientNumber, fromName, content, proxyType, token, addedOn, updatedOn, status
                               FROM sms_messages 
                               WHERE status IN(1,2) AND addedon > '2018-02-19 10:00:00' AND updatedOn < DATEADD(MINUTE, -30, GETDATE()) ORDER BY id";

        const string UPDATE = @"UPDATE sms_messages SET status = @status, updatedOn = GETDATE() WHERE token IN ({0})";

        private string _connString;

        public static long LastId = 0;

        public SmsSendingList()
        {
            string masterConEnc = ConfigurationManager.AppSettings["MasterConnectionString"];

            if (String.IsNullOrEmpty(masterConEnc))
            {
                throw new DataAccessException("There is no MasterConnectionString defined in the config file");
            }

            _connString = KeyHandler.GetDecryptor(KeyHandler.GetIV()).Decrypt(masterConEnc);
        }

        public void BulkAdd(List<SMSMessageQueueInfo> batch)
        {
            try
            {
                //using (TransactionScope scope = CreateTransactionScope())
                //{
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();

                    using (SqlBulkCopy copy = new SqlBulkCopy(conn))
                    {
                        copy.DestinationTableName = "sms_messages";
                        copy.BatchSize = batch.Count;

                        copy.ColumnMappings.Add("campaignId", "CampaignId");
                        copy.ColumnMappings.Add("recipientNumber", "RecipientNumber");
                        copy.ColumnMappings.Add("fromName", "FromName");
                        copy.ColumnMappings.Add("content", "Content");
                        copy.ColumnMappings.Add("proxyType", "ProxyType");
                        copy.ColumnMappings.Add("token", "Token");
                        copy.ColumnMappings.Add("status", "Status");
                        copy.ColumnMappings.Add("addedOn", "AddedOn");
                        copy.ColumnMappings.Add("updatedOn", "UpdatedOn");

                        DataTable table = new DataTable("sms_messages");

                        table.Columns.Add("campaignId", typeof(long));
                        table.Columns.Add("recipientNumber", typeof(string));
                        table.Columns.Add("fromName", typeof(string));
                        table.Columns.Add("content", typeof(string));
                        table.Columns.Add("proxyType", typeof(short));
                        table.Columns.Add("token", typeof(long));
                        table.Columns.Add("status", typeof(short));
                        table.Columns.Add("addedOn", typeof(DateTime));
                        table.Columns.Add("updatedOn", typeof(DateTime));

                        foreach (var item in batch)
                        {
                            table.Rows.Add(item.CampaignId, item.RecipientNumber, item.FromName, item.Content, item.ProxyType, item.Token, 2, DateTime.Now, DateTime.Now);
                        }

                        copy.WriteToServer(table);
                    }

                    //scope.Complete();
                }
                //}

                LogWriter.LogEntry("Saved to DB: " + string.Join(",", batch.Select(c => string.Format("[{0}-{1}-{2}]", c.CampaignId, c.Token, c.RecipientNumber)).ToArray()));
            }
            catch (Exception ex)
            {
                LogWriter.LogException(ex, "There was an error saving to the db");
            }
        }

        public void Add(SMSMessageQueueInfo po)
        {
            try
            {
                using (TransactionScope scope = CreateTransactionScope())
                {
                    using (SqlConnection conn = new SqlConnection(_connString))
                    {
                        conn.Open();
                        conn.Execute(INSERT, po);
                        LogWriter.LogEntry(string.Format("Added message to Log DB - CampaignId: {0} - MessageId: {1} - Mobile: {2}; ", po.CampaignId, po.Token, po.RecipientNumber));
                        scope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.LogException(ex, "There was an error saving to the db");
            }
        }

        public List<SMSMessageQueueInfo> Get()
        {
            List<SMSMessageQueueInfo> cloned = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();

                    cloned = conn.Query<SMSMessageQueueInfo>(QUERY, new { status = SmsMessageStatus.PENDING, lastId = SmsSendingList.LastId }).ToList();
                    LogWriter.LogEntry("Batch messages: " + string.Join(",", cloned.Select(c => string.Format("[{0}-{1}-{2}]", c.CampaignId, c.Token, c.RecipientNumber)).ToArray()));
                }
            }
            catch (Exception ex)
            {
                LogWriter.LogException(ex, "There was an error getting messages from the db");
            }

            return cloned;
        }

        private TransactionScope CreateTransactionScope()
        {
            var transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            transactionOptions.Timeout = TransactionManager.MaximumTimeout;
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }

        public void UpdateStatus(SmsMessageStatus smsStatus, IEnumerable<SMSMessageQueueInfo> messages)
        {
            if (messages.Count() == 0)
                return;

            try
            {
                //using (TransactionScope scope = CreateTransactionScope())
                //{
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    var query = string.Format(UPDATE, string.Join(",", messages.Select(x => x.Token).ToArray()));
                    conn.Execute(query, new { status = smsStatus });
                    LogWriter.LogEntry("Batch messages processed: " + string.Join(",", messages.Select(c => string.Format("[{0}-{1}-{2}]", c.CampaignId, c.Token, c.RecipientNumber)).ToArray()));
                    //scope.Complete();
                }
                //}
            }
            catch (Exception ex)
            {
                LogWriter.LogException(ex, "There was an error updating messages");
            }
        }

        public void UpdateStatus(SmsMessageStatus smsStatus, SMSMessageQueueInfo message)
        {
            if (message == null)
                return;

            try
            {
                //using (TransactionScope scope = CreateTransactionScope())
                //{
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    var query = string.Format(UPDATE, message.Token);
                    conn.Execute(query, new { status = smsStatus });
                    LogWriter.LogEntry("Batch messages processed: " + string.Format("[{0}-{1}-{2}]", message.CampaignId, message.Token, message.RecipientNumber));
                    //scope.Complete();
                }
                //}
            }
            catch (Exception ex)
            {
                LogWriter.LogException(ex, "There was an error updating message");
            }
        }

        #region Disposable
        // Flag: Has Dispose already been called?
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                //handle.Dispose();
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
        #endregion
    }
}
