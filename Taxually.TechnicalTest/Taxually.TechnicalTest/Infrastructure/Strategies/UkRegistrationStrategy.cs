using Taxually.TechnicalTest.Application.Dtos;
using Taxually.TechnicalTest.Application.Interfaces;

namespace Taxually.TechnicalTest.Infrastructure.Strategies
{
    public class UkRegistrationStrategy : IRegistrationStrategy
    {
        private readonly IHttpPoster _httpPoster;

        public UkRegistrationStrategy(IHttpPoster httpPoster)
        {
            _httpPoster = httpPoster;
        }

        public bool IsCountrySupported(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                return false;

            return countryCode.Equals("GB", StringComparison.CurrentCultureIgnoreCase);
        }

        public async Task RegisterAsync(VatRegistrationRequest request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request, $"{nameof(request)} - Value cannot be null.");

            if (string.IsNullOrWhiteSpace(request.CompanyName) || string.IsNullOrWhiteSpace(request.CompanyId))
                throw new ArgumentException("CompanyName and CompanyId are required for VAT registration.");

            try
            {
                const string ukVatApiUrl = "https://api.uktax.gov.uk";

                await _httpPoster.PostAsync(ukVatApiUrl, request, ct);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("Error when processing UK VAT registration request.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred during UK VAT registration.", ex);
            }
        }
    }
}
