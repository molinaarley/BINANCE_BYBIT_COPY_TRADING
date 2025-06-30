using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanArchitecture.Application.UnitTests.Services
{
    public class BinanceByBitOrderServiceXUnitTests
    {
        private readonly BinanceByBitOrderService _service;
        private readonly Mock<IBinanceByBitOrderRepository> _mockOrderRepository;
        private readonly Mock<ILogger<BinanceByBitOrderService>> _mockLogger;

        public BinanceByBitOrderServiceXUnitTests()
        {
            _mockOrderRepository = new Mock<IBinanceByBitOrderRepository>();
            _mockLogger = new Mock<ILogger<BinanceByBitOrderService>>();
            _service = new BinanceByBitOrderService(_mockLogger.Object, _mockOrderRepository.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedOrder()
        {
            // Arrange
            var newOrder = new BinanceByBitOrder { Id = 1 };
            _mockOrderRepository.Setup(repo => repo.Create(It.IsAny<BinanceByBitOrder>())).ReturnsAsync(newOrder);

            // Act
            var result = await _service.Create(newOrder);

            // Assert
            Assert.Equal(newOrder, result);
            _mockOrderRepository.Verify(repo => repo.Create(It.IsAny<BinanceByBitOrder>()), Times.Once);
        }

        [Fact]
        public async Task GetOrder_ShouldReturnOrder()
        {
            // Arrange
            var order = new BinanceByBitOrder { Id = 1 };
            _mockOrderRepository.Setup(repo => repo.GetOrder(It.IsAny<BinanceByBitOrder>())).ReturnsAsync(order);

            // Act
            var result = await _service.GetOrder(order);

            // Assert
            Assert.Equal(order, result);
            _mockOrderRepository.Verify(repo => repo.GetOrder(It.IsAny<BinanceByBitOrder>()), Times.Once);
        }

    }
}
