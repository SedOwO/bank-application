namespace BankAPI.Messaging
{
    public interface IRabbitMqPublisher
    {
        Task PublishMessage(string message);
    }
}
