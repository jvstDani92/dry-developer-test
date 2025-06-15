namespace Taxually.TechnicalTest.Application.Dtos
{
    public record VatRegistrationRequest
    {
        public string? CompanyName { get; init; }
        public string? CompanyId { get; init; }
        public string? Country { get; init; }

        public VatRegistrationRequest() { }

        public VatRegistrationRequest(string? companyName, string? companyId, string? country)
        {
            CompanyName = companyName;
            CompanyId = companyId;
            Country = country;
        }
    }

}
