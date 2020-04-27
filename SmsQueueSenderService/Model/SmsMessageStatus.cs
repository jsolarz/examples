namespace SmsQueueSenderService.Model
{
    public enum SmsMessageStatus
    {
        PENDING = 1,
        PROCESSING = 2,
        SUCCESS = 3,
        FAILED = 4,
        UNKNOWN = 5
    }
}
