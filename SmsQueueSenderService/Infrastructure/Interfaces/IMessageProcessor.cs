namespace SmsQueueSenderService.Infrastructure.Interfaces
{
    public interface IMessageProcessor
    {
        bool IsOpen { get; }
        void Close();
        void Open();
    }
}