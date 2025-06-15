using Moq;
using Taxually.TechnicalTest.Application.Dtos;
using Taxually.TechnicalTest.Application.Interfaces;
using Taxually.TechnicalTest.Infrastructure.Strategies;

namespace Taxually.TechnicalTest.Tests
{
    public class UkRegistrationStrategyTests
    {
        private readonly Mock<IHttpPoster> _httpPosterMock;
        private readonly UkRegistrationStrategy _strategy;

        public UkRegistrationStrategyTests()
        {
            _httpPosterMock = new Mock<IHttpPoster>();
            _strategy = new UkRegistrationStrategy(_httpPosterMock.Object);
        }

        [Fact]
        public void IsCountrySupported_GB_ReturnsTrue()
        {
            // Arrange
            var countryCode = "GB";

            //Act
            var result = _strategy.IsCountrySupported(countryCode);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsCountrySupported_NonGB_ReturnFalse()
        {
            // Arrange
            var countryCode = "FR";

            // Act
            var result = _strategy.IsCountrySupported(countryCode);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RegisterAsync_RequestIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            VatRegistrationRequest request = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _strategy.RegisterAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task RegisterAsync_ValidRequest_CallsHttpPoster()
        {
            // Arrange
            var request = new VatRegistrationRequest("Test Company", "123456789", "GB");

            const string expectedUrl = "https://api.uktax.gov.uk";

            _httpPosterMock
                .Setup(x => x.PostAsync(expectedUrl, request, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _strategy.RegisterAsync(request, CancellationToken.None);

            // Assert
            _httpPosterMock.Verify(
                x => x.PostAsync(expectedUrl, request, CancellationToken.None),
                Times.Once
            );
        }

        [Fact]
        public async Task RegisterAsync_CompanyNameOrCompanyIdIsEmpty_ThowsArgumentException()
        {
            // Arrange
            var request = new VatRegistrationRequest("", "", "GB");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _strategy.RegisterAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task RegisterAsync_HttpRequestFails_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new VatRegistrationRequest("Test Company", "12456", "GB");

            _httpPosterMock
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<VatRegistrationRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _strategy.RegisterAsync(request, CancellationToken.None));
        }
    }
}
