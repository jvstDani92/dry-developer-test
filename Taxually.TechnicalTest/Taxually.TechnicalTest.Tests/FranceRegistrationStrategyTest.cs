using Moq;
using System.Text;
using Taxually.TechnicalTest.Application.Dtos;
using Taxually.TechnicalTest.Application.Interfaces;
using Taxually.TechnicalTest.Infrastructure.Strategies;

namespace Taxually.TechnicalTest.Tests
{
    public class FranceRegistrationStrategyTest
    {
        private readonly Mock<IQueuePublisher> _queuePublisherMock;
        private readonly FranceRegistrationStrategy _strategy;

        public FranceRegistrationStrategyTest()
        {
            _queuePublisherMock = new Mock<IQueuePublisher>();
            _strategy = new FranceRegistrationStrategy(_queuePublisherMock.Object);
        }

        [Fact]
        public void IsCountrySupported_FR_ReturnsTrue()
        {
            // Arrange
            var countryCode = "FR";

            // Act
            var result = _strategy.IsCountrySupported(countryCode);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsCountrySupported_NonFR_ReturnsFalse()
        {
            // Arrange
            var countryCode = "DE";

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
        public async Task RegisterAsync_CompanyNameOrCompanyIdIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            var request = new VatRegistrationRequest("", "", "FR");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _strategy.RegisterAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task RegisterAsync_ValidRequest_CallsEnqueueAsyncWithCorrectCsv()
        {
            // Arrange
            var request = new VatRegistrationRequest("Test Company", "12345", "FR");
            var newLine = Environment.NewLine;
            var expectedCsv = $"CompanyName,CompanyId{newLine}Test Company,12345{newLine}";

            _queuePublisherMock
                .Setup(x => x.EnqueueAsync("vat-registration-csv", It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _strategy.RegisterAsync(request, CancellationToken.None);

            // Assert
            _queuePublisherMock.Verify(
                x => x.EnqueueAsync("vat-registration-csv", It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == expectedCsv), CancellationToken.None),
                Times.Once
            );
        }

        [Fact]
        public async Task RegisterAsync_EnqueueAsyncFails_ThrowsException()
        {
            // Arrange
            var request = new VatRegistrationRequest("Test Company", "12345", "FR");

            _queuePublisherMock
                .Setup(x => x.EnqueueAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Queue error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _strategy.RegisterAsync(request, CancellationToken.None));
        }
    }
}
