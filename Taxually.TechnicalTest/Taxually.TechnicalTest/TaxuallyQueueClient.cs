using Taxually.TechnicalTest.Application.Interfaces;

namespace Taxually.TechnicalTest
{
    public class TaxuallyQueueClient : IQueuePublisher
    {
        public Task EnqueueAsync<TPayload>(string queueName, TPayload payload, CancellationToken ct)
        {
            // Code to send to message queue removed for brevity
            return Task.CompletedTask;
        }
    }
}
