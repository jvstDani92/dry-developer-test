using System.Text;
using System.Xml.Serialization;
using Taxually.TechnicalTest.Application.Dtos;
using Taxually.TechnicalTest.Application.Interfaces;

namespace Taxually.TechnicalTest.Infrastructure.Strategies
{
    public class GermanRegistrationStrategy : IRegistrationStrategy
    {
        private readonly IQueuePublisher _queuePublisher;

        public GermanRegistrationStrategy(IQueuePublisher queuePublisher)
        {
            _queuePublisher = queuePublisher;
        }

        public bool IsCountrySupported(string countryCode)
        {
            return countryCode.Equals("DE", StringComparison.CurrentCultureIgnoreCase);
        }

        public async Task RegisterAsync(VatRegistrationRequest request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request, $"{nameof(request)} - Value cannot be null.");

            if (string.IsNullOrWhiteSpace(request.CompanyName) || string.IsNullOrWhiteSpace(request.CompanyId))
                throw new ArgumentException("CompanyName and CompanyId are required for VAT registration.");

            try
            {
                var xml = SerializeXml(request);
                await _queuePublisher.EnqueueAsync("vat-registration-xml", Encoding.UTF8.GetBytes(xml), ct);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Error when processing German VAT registration request.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred during German VAT registration.", ex);
            }
        }

        public static string SerializeXml(VatRegistrationRequest request)
        {
            ArgumentNullException.ThrowIfNull(nameof(request), "Value cannot be null.");

            using var writer = new StringWriter();
            var xmlSeriealizar = new XmlSerializer(typeof(VatRegistrationRequest));

            try
            {
                xmlSeriealizar.Serialize(writer, request);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Error when serializing object to XML.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred during XML serialization.", ex);
            }

            return writer.ToString();
        }
    }
}
