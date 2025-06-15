using System.Text;
using Taxually.TechnicalTest.Application.Dtos;
using Taxually.TechnicalTest.Application.Interfaces;

namespace Taxually.TechnicalTest.Infrastructure.Strategies
{
    public class FranceRegistrationStrategy : IRegistrationStrategy
    {

        private readonly IQueuePublisher _queuePublisher;

        public FranceRegistrationStrategy(IQueuePublisher queuePublisher)
        {
            _queuePublisher = queuePublisher;
        }

        public bool IsCountrySupported(string countryCode)
        {
            return countryCode.Equals("FR", StringComparison.CurrentCultureIgnoreCase);
        }

        public async Task RegisterAsync(VatRegistrationRequest request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request, $"{nameof(request)} - Value cannot be null.");

            if (string.IsNullOrWhiteSpace(request.CompanyName) || string.IsNullOrWhiteSpace(request.CompanyId))
                throw new ArgumentException("CompanyName and CompanyId are required for VAT registration.");

            try
            {
                var csvBuilder = new StringBuilder();
                csvBuilder.AppendLine("CompanyName,CompanyId");
                csvBuilder.AppendLine($"{request.CompanyName},{request.CompanyId}");
                var csv = Encoding.UTF8.GetBytes(csvBuilder.ToString());

                await _queuePublisher.EnqueueAsync("vat-registration-csv", csv, ct);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Error when processing France VAT registration request.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred during France VAT registration.", ex);
            }
        }
    }
}
