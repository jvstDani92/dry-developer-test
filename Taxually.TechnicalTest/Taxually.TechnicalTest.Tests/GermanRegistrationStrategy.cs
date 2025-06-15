using Moq;
using System.Text;
using Taxually.TechnicalTest.Application.Dtos;
using Taxually.TechnicalTest.Application.Interfaces;
using Taxually.TechnicalTest.Infrastructure.Strategies;

namespace Taxually.TechnicalTest.Tests
{
    public class GermanRegistrationStrategyTest
    {
        private readonly Mock<IQueuePublisher> _queuePublisherMock;
        private readonly GermanRegistrationStrategy _strategy;

        public GermanRegistrationStrategyTest()
        {
            _queuePublisherMock = new Mock<IQueuePublisher>();
            _strategy = new GermanRegistrationStrategy(_queuePublisherMock.Object);
        }

        [Fact]
        public void IsCountrySupported_DE_ReturnsTrue()
        {
            // Arrange
            var countryCode = "DE";

            // Act
            var result = _strategy.IsCountrySupported(countryCode);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsCountrySupported_NonDE_ReturnsFalse()
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
        public async Task RegisterAsync_CompanyNameOrCompanyIdIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            var request = new VatRegistrationRequest("", "", "DE");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _strategy.RegisterAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task RegisterAsync_ValidRequest_CallsEnqueueAsyncWithCorrectXml()
        {
            // Arrange
            var request = new VatRegistrationRequest("Test Company", "12345", "DE");

            var expectedXml = GermanRegistrationStrategy.SerializeXml(request);

            var expectedQueueName = "vat-registration-xml";

            _queuePublisherMock
                .Setup(x => x.EnqueueAsync(expectedQueueName, It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _strategy.RegisterAsync(request, CancellationToken.None);

            // Assert
            _queuePublisherMock.Verify(
                x => x.EnqueueAsync(expectedQueueName, It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == expectedXml), CancellationToken.None),
                Times.Once
            );
        }

        [Fact]
        public async Task RegisterAsync_EnqueueAsyncFails_ThrowsException()
        {
            // Arrange
            var request = new VatRegistrationRequest("Test Company", "12345", "DE");

            _queuePublisherMock
                .Setup(x => x.EnqueueAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Queue error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _strategy.RegisterAsync(request, CancellationToken.None));
        }
    }
}
