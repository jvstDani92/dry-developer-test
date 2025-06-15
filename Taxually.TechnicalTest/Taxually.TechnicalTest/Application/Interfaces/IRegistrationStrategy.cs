using Taxually.TechnicalTest.Application.Dtos;

namespace Taxually.TechnicalTest.Application.Interfaces
{
    public interface IRegistrationStrategy
    {
        bool IsCountrySupported(string countryCode);

        Task RegisterAsync(VatRegistrationRequest request, CancellationToken ct);
    }
}
