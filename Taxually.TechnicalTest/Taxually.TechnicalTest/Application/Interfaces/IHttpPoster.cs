namespace Taxually.TechnicalTest.Application.Interfaces
{
    public interface IHttpPoster
    {
        Task PostAsync<TRequest>(string url, TRequest request, CancellationToken ct);
    }
}
