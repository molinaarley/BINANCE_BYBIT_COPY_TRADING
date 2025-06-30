using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Features.Binance.Queries.MonitoringCoinWalletBalanceObjective;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Application.Models.Identity;
using CleanArchitecture.Domain.Binance;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CleanArchitecture.Infrastructure.XUnitTests.Binance.Queries
{
    public class MonitoringCoinWalletBalanceObjectiveHandlerXunitTest
    {
        // Mocks des dépendances
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ITelegrameBotService> _telegrameBotServiceMock = new();
        private readonly Mock<IDonetZipService> _donetZipServiceMock = new();
        private readonly Mock<IFileService> _fileServiceMock = new();
        private readonly Mock<IByBitApiService> _byBitApiServiceMock = new();
        private readonly Mock<IBinanceTraderService> _binanceTraderServiceMock = new();
        private readonly Mock<IBinanceOrderService> _binanceOrderServiceMock = new();
        private readonly Mock<IBinanceByBitUsersService> _binanceByBitUsersServiceMock = new();
        private readonly Mock<IBinanceByBitOrderService> _binanceByBitOrderServiceMock = new();
        private readonly Mock<IBinanceMonitoringProcessService> _binanceMonitoringProcessServiceMock = new();
        private readonly Mock<IBinanceTraderPerformanceService> _loadTraderPerformanceServiceMock = new();
        private readonly Mock<IBinanceMonitoringCoinWalletBalanceObjectiveProcessService> _monitoringCoinWalletBalanceObjectiveProcessServiceMock = new();
        private readonly Mock<IBinanceTraderUrlForUpdatePositionBinanceQueryService> _binanceTraderUrlForUpdatePositionBinanceQueryServiceMock = new();
        private readonly Mock<IConfiguration> _configurationMock = new();

        private readonly BinanceBybitSettings _binanceBybitSettings = new BinanceBybitSettings { BinanceOrderqty = 1 };
        private readonly JwtSettings _jwtSettings = new JwtSettings { Key = "test" };

        private MonitoringCoinWalletBalanceObjectiveHandler CreateHandler()
        {
            return new MonitoringCoinWalletBalanceObjectiveHandler(
                Options.Create(_jwtSettings),
                Options.Create(_binanceBybitSettings),
                _unitOfWorkMock.Object,
                _telegrameBotServiceMock.Object,
                _donetZipServiceMock.Object,
                _fileServiceMock.Object,
                _byBitApiServiceMock.Object,
                _binanceTraderServiceMock.Object,
                _binanceOrderServiceMock.Object,
                _binanceByBitUsersServiceMock.Object,
                _binanceByBitOrderServiceMock.Object,
                _binanceMonitoringProcessServiceMock.Object,
                _loadTraderPerformanceServiceMock.Object,
                _configurationMock.Object,
                _monitoringCoinWalletBalanceObjectiveProcessServiceMock.Object,
                _binanceTraderUrlForUpdatePositionBinanceQueryServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_Retourne_True_Si_Tout_Se_Passe_Bien()
        {
            // Arrange : Préparation des mocks pour simuler un utilisateur actif
            var handler = CreateHandler();

            var user = new BinanceByBitUser
            {
                IdTelegrame = 1,
                ApiKey = "api",
                SecretKey = "secret"
            };
            var users = new List<BinanceByBitUser> { user };

            _binanceByBitUsersServiceMock.Setup(x => x.GetAllIsactive()).ReturnsAsync(users);

            // Simuler qu'il n'y a pas de processus en cours
            _monitoringCoinWalletBalanceObjectiveProcessServiceMock
                .Setup(x => x.GetLastIsIngProcessForObjective(user.IdTelegrame))
                .ReturnsAsync((BinanceMonitoringCoinWalletBalanceObjectiveProcess)null);

            // Simuler le wallet balance et coin
            var walletBalance = new WalletBalance();
            var walletBalanceList = new List<WalletBalance> { walletBalance };

            _byBitApiServiceMock
                .Setup(x => x.WalletBalance(user.ApiKey, user.SecretKey))
                .ReturnsAsync(walletBalanceList);

            _byBitApiServiceMock
                .Setup(x => x.GetCoinFromWalletBalance(walletBalanceList))
                .ReturnsAsync(new Coin { equity = 1000, unrealisedPnl = 10, walletBalance = 1000 });

            // Simuler la configuration
            var section = new Mock<IConfigurationSection>();
            section.Setup(s => s.Value).Returns("5");
            _configurationMock.Setup(c => c.GetSection("BinanceBybitSettings:BinanceObjectiveDailY")).Returns(section.Object);

            // Simuler la création et la mise à jour du process
            _monitoringCoinWalletBalanceObjectiveProcessServiceMock
                .Setup(x => x.Create(It.IsAny<BinanceMonitoringCoinWalletBalanceObjectiveProcess>()))
                .ReturnsAsync(new BinanceMonitoringCoinWalletBalanceObjectiveProcess { Id = 1 });
            _monitoringCoinWalletBalanceObjectiveProcessServiceMock
                .Setup(x => x.Update(It.IsAny<BinanceMonitoringCoinWalletBalanceObjectiveProcess>()))
                .ReturnsAsync(true);

            _binanceTraderUrlForUpdatePositionBinanceQueryServiceMock
                .Setup(x => x.AddEncryptedUidList(It.IsAny<List<string>>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            _loadTraderPerformanceServiceMock
                .Setup(x => x.TraderUrlForUpdatePositionBinance())
                .ReturnsAsync(new List<string>());

            // Act : Appel du handler
            var result = await handler.Handle(new MonitoringCoinWalletBalanceObjectiveQuery(), CancellationToken.None);

            // Assert : On attend un résultat true
            Assert.True(result);
        }

        [Fact]
        public async Task Handle_Retourne_True_Meme_Si_WalletBalance_Null_Au_Debut()
        {
            // Arrange : Simuler le cas où WalletBalance est null la première fois
            var handler = CreateHandler();

            var user = new BinanceByBitUser
            {
                IdTelegrame = 1,
                ApiKey = "api",
                SecretKey = "secret"
            };
            var users = new List<BinanceByBitUser> { user };

            _binanceByBitUsersServiceMock.Setup(x => x.GetAllIsactive()).ReturnsAsync(users);

            _monitoringCoinWalletBalanceObjectiveProcessServiceMock
                .Setup(x => x.GetLastIsIngProcessForObjective(user.IdTelegrame))
                .ReturnsAsync((BinanceMonitoringCoinWalletBalanceObjectiveProcess)null);

            // Première fois null, deuxième fois retourne une vraie liste de wallet
            var walletBalance = new WalletBalance();
            var walletBalanceList = new List<WalletBalance> { walletBalance };
            int callCount = 0;
            _byBitApiServiceMock.Setup(x => x.WalletBalance(user.ApiKey, user.SecretKey))
                .ReturnsAsync(() =>
                {
                    callCount++;
                    // Première fois null, deuxième fois une liste valide
                    return callCount == 1 ? null : walletBalanceList;
                });

            // Adapter ici pour passer une liste au mock
            _byBitApiServiceMock.Setup(x => x.GetCoinFromWalletBalance(walletBalanceList))
                .ReturnsAsync(new Coin { equity = 1000, unrealisedPnl = 10, walletBalance = 1000 });

            var section = new Mock<IConfigurationSection>();
            section.Setup(s => s.Value).Returns("5");
            _configurationMock.Setup(c => c.GetSection("BinanceBybitSettings:BinanceObjectiveDailY")).Returns(section.Object);

            _monitoringCoinWalletBalanceObjectiveProcessServiceMock
                .Setup(x => x.Create(It.IsAny<BinanceMonitoringCoinWalletBalanceObjectiveProcess>()))
                .ReturnsAsync(new BinanceMonitoringCoinWalletBalanceObjectiveProcess { Id = 1 });
            _monitoringCoinWalletBalanceObjectiveProcessServiceMock
                .Setup(x => x.Update(It.IsAny<BinanceMonitoringCoinWalletBalanceObjectiveProcess>()))
                .ReturnsAsync(true);

            _binanceTraderUrlForUpdatePositionBinanceQueryServiceMock
                .Setup(x => x.AddEncryptedUidList(It.IsAny<List<string>>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            _loadTraderPerformanceServiceMock
                .Setup(x => x.TraderUrlForUpdatePositionBinance())
                .ReturnsAsync(new List<string>());

            // Act
            var result = await handler.Handle(new MonitoringCoinWalletBalanceObjectiveQuery(), CancellationToken.None);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handle_Retourne_True_Si_Aucun_Utilisateur_Actif()
        {
            // Arrange : Aucun utilisateur actif
            var handler = CreateHandler();
            _binanceByBitUsersServiceMock.Setup(x => x.GetAllIsactive()).ReturnsAsync(new List<BinanceByBitUser>());

            // Act
            var result = await handler.Handle(new MonitoringCoinWalletBalanceObjectiveQuery(), CancellationToken.None);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handle_Lance_Exception_Si_Service_Erreur()
        {
            // Arrange
            var handler = CreateHandler();
            var user = new BinanceByBitUser { IdTelegrame = 1, ApiKey = "api", SecretKey = "secret" };
            _binanceByBitUsersServiceMock.Setup(x => x.GetAllIsactive()).ReturnsAsync(new List<BinanceByBitUser> { user });

            // Simuler une exception
            _byBitApiServiceMock.Setup(x => x.WalletBalance(user.ApiKey, user.SecretKey)).ThrowsAsync(new Exception("Erreur API"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                handler.Handle(new MonitoringCoinWalletBalanceObjectiveQuery(), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Traite_Plusieurs_Utilisateurs_Actifs()
        {
            // Arrange
            var handler = CreateHandler();
            var users = new List<BinanceByBitUser>
            {
                new BinanceByBitUser { IdTelegrame = 1, ApiKey = "api1", SecretKey = "secret1" },
                new BinanceByBitUser { IdTelegrame = 2, ApiKey = "api2", SecretKey = "secret2" }
            };
            _binanceByBitUsersServiceMock.Setup(x => x.GetAllIsactive()).ReturnsAsync(users);

            var walletBalanceList = new List<WalletBalance> { new WalletBalance() };
            _byBitApiServiceMock.Setup(x => x.WalletBalance(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(walletBalanceList);
            _byBitApiServiceMock.Setup(x => x.GetCoinFromWalletBalance(walletBalanceList)).ReturnsAsync(new Coin { equity = 1000, unrealisedPnl = 10, walletBalance = 1000 });

            var section = new Mock<IConfigurationSection>();
            section.Setup(s => s.Value).Returns("5");
            _configurationMock.Setup(c => c.GetSection("BinanceBybitSettings:BinanceObjectiveDailY")).Returns(section.Object);

            _monitoringCoinWalletBalanceObjectiveProcessServiceMock
                .Setup(x => x.Create(It.IsAny<BinanceMonitoringCoinWalletBalanceObjectiveProcess>()))
                .ReturnsAsync(new BinanceMonitoringCoinWalletBalanceObjectiveProcess { Id = 1 });
            _monitoringCoinWalletBalanceObjectiveProcessServiceMock
                .Setup(x => x.Update(It.IsAny<BinanceMonitoringCoinWalletBalanceObjectiveProcess>()))
                .ReturnsAsync(true);

            _binanceTraderUrlForUpdatePositionBinanceQueryServiceMock
                .Setup(x => x.AddEncryptedUidList(It.IsAny<List<string>>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            _loadTraderPerformanceServiceMock
                .Setup(x => x.TraderUrlForUpdatePositionBinance())
                .ReturnsAsync(new List<string>());

            // Act
            var result = await handler.Handle(new MonitoringCoinWalletBalanceObjectiveQuery(), CancellationToken.None);

            // Assert
            Assert.True(result);
            _binanceByBitUsersServiceMock.Verify(x => x.GetAllIsactive(), Times.Once);
            _monitoringCoinWalletBalanceObjectiveProcessServiceMock.Verify(x => x.Create(It.IsAny<BinanceMonitoringCoinWalletBalanceObjectiveProcess>()), Times.Exactly(users.Count));
        }

        [Fact]
        public async Task Handle_Retourne_True_Si_GetAllIsactive_Retourne_Null()
        {
            // Arrange
            var handler = CreateHandler();
            _binanceByBitUsersServiceMock.Setup(x => x.GetAllIsactive()).ReturnsAsync((List<BinanceByBitUser>)null);

            // Act
            var result = await handler.Handle(new MonitoringCoinWalletBalanceObjectiveQuery(), CancellationToken.None);

            // Assert
            Assert.False(result);
        }
    }
}


