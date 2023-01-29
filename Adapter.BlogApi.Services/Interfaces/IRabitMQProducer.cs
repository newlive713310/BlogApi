namespace Adapter.BlogApi.Services.Interfaces
{
    public interface IRabitMQProducer
    {
        public void SendProductMessage<T>(T message);
    }
}
