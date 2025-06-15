namespace Taxually.TechnicalTest.Application.Interfaces
{
    public interface IQueuePublisher
    {
        Task EnqueueAsync<TPayload>(string queueName, TPayload payload, CancellationToken ct);
    }
}
