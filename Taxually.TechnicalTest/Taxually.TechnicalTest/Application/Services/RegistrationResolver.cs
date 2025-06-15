using Taxually.TechnicalTest.Application.Interfaces;

namespace Taxually.TechnicalTest.Application.Services
{
    public class RegistrationResolver
    {
        private readonly IEnumerable<IRegistrationStrategy> _strategies;

        public RegistrationResolver(IEnumerable<IRegistrationStrategy> strategies)
        {
            _strategies = strategies;
        }

        public IRegistrationStrategy Resolve(string country)
        {
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentNullException(nameof(country), "Value cannot be null or empty.");

            return _strategies.FirstOrDefault(c => c.IsCountrySupported(country))
                ?? throw new NotSupportedException($"{country} not supported.");
        }
    }
}
