using Moq;
using Taxually.TechnicalTest.Application.Interfaces;
using Taxually.TechnicalTest.Application.Services;

namespace Taxually.TechnicalTest.Tests
{
    public class RegistrationResolverTests
    {
        [Theory]
        [InlineData("GB")]
        [InlineData("FR")]
        public void Resolve_ExistingStrategy_ReturnsStrategy(string countryCode)
        {
            //Arrange 
            var strategy = new Mock<IRegistrationStrategy>();
            strategy.Setup(s => s.IsCountrySupported(countryCode)).Returns(true);

            var resolver = new RegistrationResolver(new[] { strategy.Object });

            //Act
            var result = resolver.Resolve(countryCode);

            //Assert
            Assert.Same(strategy.Object, result);
        }

        [Fact]
        public void Resolve_NonExistingStrategy_ThrowsNotSupportedException()
        {
            //Arrange
            var strategy = new Mock<IRegistrationStrategy>();
            strategy.Setup(s => s.IsCountrySupported(It.IsAny<string>())).Returns(false);

            var resolver = new RegistrationResolver(new[] { strategy.Object });

            // Act & Assert
            var exception = Assert.Throws<NotSupportedException>(() => resolver.Resolve("XX"));
            Assert.Equal("XX not supported.", exception.Message);
        }

        [Fact]
        public void Resolve_MultipleStrategies_ReturnsCorrectStrategy()
        {
            // Arrange
            var strategy1 = new Mock<IRegistrationStrategy>();
            strategy1.Setup(s => s.IsCountrySupported("GB")).Returns(true);

            var strategy2 = new Mock<IRegistrationStrategy>();
            strategy2.Setup(s => s.IsCountrySupported("FR")).Returns(true);

            var resolver = new RegistrationResolver(new[] { strategy1.Object, strategy2.Object });

            // Act
            var result = resolver.Resolve("FR");

            // Assert
            Assert.Same(strategy2.Object, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Resolve_InvalidCountryCode_ThrowsArgumentNullException(string countryCode)
        {
            // Arrange
            var resolver = new RegistrationResolver(new Mock<IEnumerable<IRegistrationStrategy>>().Object);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => resolver.Resolve(countryCode));
            Assert.Equal("Value cannot be null or empty. (Parameter 'country')", exception.Message);
        }
    }
}