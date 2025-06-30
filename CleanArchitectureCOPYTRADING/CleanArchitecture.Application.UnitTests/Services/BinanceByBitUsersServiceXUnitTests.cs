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
    public class BinanceByBitUsersServiceXUnitTests
    {
        private readonly BinanceByBitUsersService _service;
        private readonly Mock<IBinanceByBitUsersRepository> _mockUserRepository;
        private readonly Mock<ILogger<BinanceByBitUsersService>> _mockLogger;

        public BinanceByBitUsersServiceXUnitTests()
        {
            _mockUserRepository = new Mock<IBinanceByBitUsersRepository>();
            _mockLogger = new Mock<ILogger<BinanceByBitUsersService>>();
            _service = new BinanceByBitUsersService(_mockLogger.Object, _mockUserRepository.Object);
        }


      
    }


}


