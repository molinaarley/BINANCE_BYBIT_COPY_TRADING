using System;
using System.Threading.Tasks;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanArchitecture.Infrastructure.XUnitTests.Binance.Services
{
    /// <summary>
    /// Tests unitaires pour le service BinanceMonitoringCoinWalletBalanceObjectiveProcessService
    /// </summary>
    public class BinanceMonitoringCoinWalletBalanceObjectiveProcessServiceXunitTest
    {
        private readonly Mock<IBinanceMonitoringCoinWalletBalanceObjectiveProcessRepository> _repoMock;
        private readonly Mock<ILogger<BinanceMonitoringCoinWalletBalanceObjectiveProcessService>> _loggerMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly BinanceMonitoringCoinWalletBalanceObjectiveProcessService _service;

        public BinanceMonitoringCoinWalletBalanceObjectiveProcessServiceXunitTest()
        {
            // Création des mocks pour les dépendances
            _repoMock = new Mock<IBinanceMonitoringCoinWalletBalanceObjectiveProcessRepository>();
            _loggerMock = new Mock<ILogger<BinanceMonitoringCoinWalletBalanceObjectiveProcessService>>();
            _configMock = new Mock<IConfiguration>();

            // Instanciation du service à tester
            _service = new BinanceMonitoringCoinWalletBalanceObjectiveProcessService(
                _loggerMock.Object,
                _repoMock.Object,
                _configMock.Object
            );
        }

        [Fact]
        public async Task Create_Appelle_Repository_Et_Retourne_Resultat()
        {
            // Arrange : Création d'un objet de test
            var process = new BinanceMonitoringCoinWalletBalanceObjectiveProcess { Id = 1, Equity = 1000 };
            _repoMock.Setup(r => r.Create(process)).ReturnsAsync(process);

            // Act : Appel de la méthode à tester
            var result = await _service.Create(process);

            // Assert : Vérification du résultat et de l'appel au mock
            Assert.Equal(process, result);
            _repoMock.Verify(r => r.Create(process), Times.Once);
        }

        [Fact]
        public async Task Update_Appelle_Repository_Et_Retourne_Resultat()
        {
            // Arrange
            var process = new BinanceMonitoringCoinWalletBalanceObjectiveProcess { Id = 2, Equity = 2000 };
            _repoMock.Setup(r => r.Update(process)).ReturnsAsync(true);

            // Act
            var result = await _service.Update(process);

            // Assert
            Assert.True(result);
            _repoMock.Verify(r => r.Update(process), Times.Once);
        }

        [Fact]
        public async Task GetIsIngProcessForObjective_Appelle_Repository_Et_Retourne_Resultat()
        {
            // Arrange
            _repoMock.Setup(r => r.GetIsIngProcessForObjective()).ReturnsAsync(true);

            // Act
            var result = await _service.GetIsIngProcessForObjective();

            // Assert
            Assert.True(result);
            _repoMock.Verify(r => r.GetIsIngProcessForObjective(), Times.Once);
        }

        [Fact]
        public async Task GetLastIsIngProcessForObjective_Appelle_Repository_Et_Retourne_Resultat()
        {
            // Arrange
            var process = new BinanceMonitoringCoinWalletBalanceObjectiveProcess { Id = 3, IdTelegrame = 123 };
            _repoMock.Setup(r => r.GetLastIsIngProcessForObjective(123)).ReturnsAsync(process);

            // Act
            var result = await _service.GetLastIsIngProcessForObjective(123);

            // Assert
            Assert.Equal(process, result);
            _repoMock.Verify(r => r.GetLastIsIngProcessForObjective(123), Times.Once);
        }

        [Fact]
        public async Task IsInObjective_Retourne_True_Si_Pas_De_Processus_Ou_EndDate()
        {
            // Arrange : Cas où il n'y a pas de processus précédent
            _repoMock.Setup(r => r.GetLastIsIngProcessForObjective(It.IsAny<long>())).ReturnsAsync((BinanceMonitoringCoinWalletBalanceObjectiveProcess)null);
            var coin = new Coin { equity = 500 };

            // Act
            var result = await _service.IsInObjective(1, coin);

            // Assert
            Assert.True(result);

            // Arrange : Cas où EndDate est défini
            var process = new BinanceMonitoringCoinWalletBalanceObjectiveProcess { EndDate = DateTime.Now };
            _repoMock.Setup(r => r.GetLastIsIngProcessForObjective(It.IsAny<long>())).ReturnsAsync(process);

            // Act
            result = await _service.IsInObjective(1, coin);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsInObjective_Calcule_Autorisation_Et_Objectif()
        {
            // Arrange : Cas où le calcul d'autorisation doit être fait
            var process = new BinanceMonitoringCoinWalletBalanceObjectiveProcess { Equity = 1000, EndDate = null };
            var coin = new Coin { equity = 800 };

            _repoMock.Setup(r => r.GetLastIsIngProcessForObjective(It.IsAny<long>())).ReturnsAsync(process);

            // Mock de la configuration pour les paramètres de calcul
            var sectionStopLoss = new Mock<IConfigurationSection>();
            sectionStopLoss.Setup(s => s.Value).Returns("10");
            _configMock.Setup(c => c.GetSection("BinanceBybitSettings:GeneralStopLossAuthorized")).Returns(sectionStopLoss.Object);

            var sectionObjective = new Mock<IConfigurationSection>();
            sectionObjective.Setup(s => s.Value).Returns("5");
            _configMock.Setup(c => c.GetSection("BinanceBybitSettings:BinanceObjectiveDailY")).Returns(sectionObjective.Object);

            // Act
            var result = await _service.IsInObjective(1, coin);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsIngProcessForObjective_Retourne_Negation_De_IsInObjective()
        {
            // Arrange : IsInObjective retourne true, donc IsIngProcessForObjective doit retourner false
            _repoMock.Setup(r => r.GetLastIsIngProcessForObjective(It.IsAny<long>())).ReturnsAsync((BinanceMonitoringCoinWalletBalanceObjectiveProcess)null);
            var coin = new Coin { equity = 1000 };

            // Act
            var result = await _service.IsIngProcessForObjective(1, coin);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetInitDateLoadPosition_Retourne_CreatedOn_Si_Existe()
        {
            // Arrange : Cas où CreatedOn est défini
            var process = new BinanceMonitoringCoinWalletBalanceObjectiveProcess { CreatedOn = DateTime.Today };
            _repoMock.Setup(r => r.GetLastIsIngProcessForObjective(It.IsAny<long>())).ReturnsAsync(process);

            // Act
            var result = await _service.GetInitDateLoadPosition(1);

            // Assert
            Assert.Equal(DateTime.Today, result);
        }

        [Fact]
        public async Task GetInitDateLoadPosition_Retourne_Now_Si_Pas_De_Processus()
        {
            // Arrange : Cas où il n'y a pas de processus
            _repoMock.Setup(r => r.GetLastIsIngProcessForObjective(It.IsAny<long>())).ReturnsAsync((BinanceMonitoringCoinWalletBalanceObjectiveProcess)null);

            // Act
            var before = DateTime.Now;
            var result = await _service.GetInitDateLoadPosition(1);
            var after = DateTime.Now;

            // Assert : On vérifie que la date retournée est bien "maintenant"
            Assert.InRange(result, before, after);
        }
    }
}
