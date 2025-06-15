using Taxually.TechnicalTest.Application.Interfaces;

namespace Taxually.TechnicalTest
{
    public class TaxuallyHttpClient : IHttpPoster
    {
        public Task PostAsync<TRequest>(string url, TRequest request, CancellationToken ct)
        {
            // Actual HTTP call removed for purposes of this exercise
            return Task.CompletedTask;
        }
    }
}
