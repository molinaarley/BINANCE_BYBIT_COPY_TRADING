using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanArchitecture.Application.UnitTests.Services
{
    public class BinanceCacheByBitSymbolServiceXUnitTests
    {
        private readonly BinanceCacheByBitSymbolService _service;
        //private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<ILogger<BinanceCacheByBitSymbolService>> _mockLogger;
        private readonly Microsoft.Extensions.Caching.Memory.MemoryCache _memoryCache;




        public BinanceCacheByBitSymbolServiceXUnitTests()
        {
           // _mockCache = new Mock<IMemoryCache>();
            _mockLogger = new Mock<ILogger<BinanceCacheByBitSymbolService>>();
            _memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new MemoryCacheOptions());

            _service = new BinanceCacheByBitSymbolService(_mockLogger.Object, _memoryCache);

              // Inicialización de MemoryCache



        }

      /*  [Fact]
        public async Task Add_ShouldAddSymbolToCache()
        {
            // Arrange
            string symbol = "BTCUSDT";
            object cacheValue = null;
           // _mockCache.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(false);

            // Act
            var result = await _service.Add(symbol);

            // Assert
            Assert.True(result);
            _mockCache.Verify(c => c.Set(symbol, symbol, It.IsAny<MemoryCacheEntryOptions>()), Times.Once);
        }*/


        [Fact]
        public async Task HasSymbolInCache_ShouldReturnTrue_IfSymbolIsInCache()
        {
            // Arrange
            string symbol = "BTCUSDT";
            object cacheValue = symbol;
           // _mockCache.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(true);

            // Act
            var result = await _service.HasSymbolInCache(symbol);

            // Assert
            Assert.True(result);
           // _mockCache.Verify(c => c.TryGetValue(It.IsAny<object>(), out cacheValue), Times.Once);
        }

        [Fact]
        public async Task HasSymbolInCache_ShouldReturnFalse_IfSymbolIsNotInCache()
        {
            // Arrange
            string symbol = "BTCUSDT";
            object cacheValue = null;
           // _mockCache.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(false);

            // Act
            var result = await _service.HasSymbolInCache(symbol);

            // Assert
            Assert.False(result);
           // _mockCache.Verify(c => c.TryGetValue(It.IsAny<object>(), out cacheValue), Times.Once);
        }

    }

}
