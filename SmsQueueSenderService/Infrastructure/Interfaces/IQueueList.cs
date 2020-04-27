using System.Collections.Generic;

namespace SmsQueueSenderService.Infrastructure.Interfaces
{
    public interface IQueueList
    {
        Dictionary<string, string> Queues { get; set;  }
        bool IsSendingQueue(KeyValuePair<string, string> queue);
        bool IsBatchQueue(KeyValuePair<string, string> queue);
    }
}
