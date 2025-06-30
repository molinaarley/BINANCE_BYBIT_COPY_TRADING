using Xunit;
using Moq;
using System.Threading.Tasks;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Infrastructure.Services;
using CleanArchitecture.Domain.Binance;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using CleanArchitecture.Application.Models;

namespace CleanArchitecture.Application.UnitTests.Services
{
    public class BinanceMonitoringCoinWalletBalanceObjectiveProcessServiceXUnitTests
    {
        private readonly Mock<IBinanceMonitoringCoinWalletBalanceObjectiveProcessRepository> _mockRepository;
        private readonly Mock<ILogger<BinanceMonitoringCoinWalletBalanceObjectiveProcessService>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly BinanceMonitoringCoinWalletBalanceObjectiveProcessService _service;

        public BinanceMonitoringCoinWalletBalanceObjectiveProcessServiceXUnitTests()
        {
            _mockRepository = new Mock<IBinanceMonitoringCoinWalletBalanceObjectiveProcessRepository>();
            _mockLogger = new Mock<ILogger<BinanceMonitoringCoinWalletBalanceObjectiveProcessService>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _service = new BinanceMonitoringCoinWalletBalanceObjectiveProcessService(
                _mockLogger.Object, _mockRepository.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task Create_ShouldCallRepositoryCreate()
        {
            // Arrange
            var process = new BinanceMonitoringCoinWalletBalanceObjectiveProcess();
            _mockRepository.Setup(repo => repo.Create(process)).ReturnsAsync(process);

            // Act
            var result = await _service.Create(process);

            // Assert
            Assert.Equal(process, result);
            _mockRepository.Verify(repo => repo.Create(process), Times.Once);
        }

        [Fact]
        public async Task IsInObjective_ShouldReturnTrue_WhenEquityIsBelowThreshold()
        {
            // Arrange
            var process = new BinanceMonitoringCoinWalletBalanceObjectiveProcess { Equity = 1000 };
            var coin = new Coin { equity = 800 };

            _mockRepository.Setup(repo => repo.GetLastIsIngProcessForObjective(It.IsAny<long>())).ReturnsAsync(process);
            _mockConfiguration.Setup(config => config.GetSection("BinanceBybitSettings:GeneralStopLossAuthorized").Value).Returns("10");
            _mockConfiguration.Setup(config => config.GetSection("BinanceBybitSettings:BinanceObjectiveDailY").Value).Returns("5");

            // Act
            var result = await _service.IsInObjective(1234, coin);

            // Assert
            Assert.True(result);
        }
    }
}
