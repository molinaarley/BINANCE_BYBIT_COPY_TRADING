using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Kernel;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CleanArchitecture.Infrastructure.XUnitTests.Binance.Helpers;
using CleanArchitecture.Infrastructure.Persistence;
using Shouldly;

namespace CleanArchitecture.Infrastructure.XUnitTests.Binance.Repositories
{
    /// <summary>
    /// Tests unitaires pour le repository BinanceMonitoringCoinWalletBalanceObjectiveProcessRepository
    /// Ces tests vérifient le comportement du repository sans accéder à la base de données réelle
    /// </summary>
    public class BinanceMonitoringCoinWalletBalanceObjectiveProcessRepositoryXunitTest
    {
        // Mocks des dépendances
        private readonly Mock<ILogger<BinanceMonitoringCoinWalletBalanceObjectiveProcessRepository>> _loggerMock;
        private readonly Mock<IBinanceContext> _binanceContextMock;
        private readonly BinanceMonitoringCoinWalletBalanceObjectiveProcessRepository _repository;
        private readonly Fixture _fixture;

        /// <summary>
        /// Constructeur qui initialise les mocks et configure AutoFixture
        /// </summary>
        public BinanceMonitoringCoinWalletBalanceObjectiveProcessRepositoryXunitTest()
        {
            // Initialisation des mocks
            _loggerMock = new Mock<ILogger<BinanceMonitoringCoinWalletBalanceObjectiveProcessRepository>>();
            _binanceContextMock = new Mock<IBinanceContext>();
            _repository = new BinanceMonitoringCoinWalletBalanceObjectiveProcessRepository(_loggerMock.Object, _binanceContextMock.Object);
            
            // Configuration d'AutoFixture pour gérer les références circulaires
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            // Configuration pour ignorer les propriétés de navigation
            _fixture.Customize<BinanceMonitoringCoinWalletBalanceObjectiveProcess>(composer => composer
                .Do(x => 
                {
                    x.IdTelegrameNavigation = null;
                    x.BinanceTraderUrlForUpdatePositionBinanceQueries = new HashSet<BinanceTraderUrlForUpdatePositionBinanceQuery>();
                }));
        }

        /// <summary>
        /// Test de création réussie d'un processus
        /// Vérifie que l'objet est correctement créé et que les méthodes appropriées sont appelées
        /// </summary>
        [Fact]
        public async Task Create_ShouldCreateAndReturnProcess_WhenValidDataProvided()
        {
            // Arrange - Préparation des données de test
            // Création d'un objet de test avec AutoFixture en ignorant les propriétés de navigation
            var process = _fixture.Build<BinanceMonitoringCoinWalletBalanceObjectiveProcess>()
                .Without(x => x.IdTelegrameNavigation)
                .Without(x => x.BinanceTraderUrlForUpdatePositionBinanceQueries)
                .Create();

            // Création du mock du DbSet qui simule la table de base de données
            var dbSetMock = new Mock<DbSet<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>();
            
            // Configuration du mock du contexte pour retourner notre DbSet simulé
            _binanceContextMock.Setup(x => x.BinanceMonitoringCoinWalletBalanceObjectiveProcesses)
                .Returns(dbSetMock.Object);

            // Act - Exécution de la méthode à tester
            // Appel de la méthode Create du repository
            var result = await _repository.Create(process);

            // Assert - Vérification des résultats
            // Vérification que l'objet retourné n'est pas null
            Assert.NotNull(result);
            // Vérification que toutes les propriétés sont correctement copiées
            Assert.Equal(process.Id, result.Id);
            Assert.Equal(process.CreatedOn, result.CreatedOn);
            Assert.Equal(process.Equity, result.Equity);
            Assert.Equal(process.EquityObjective, result.EquityObjective);
            Assert.Equal(process.UnrealisedPnl, result.UnrealisedPnl);
            Assert.Equal(process.WalletBalance, result.WalletBalance);
            Assert.Equal(process.EndDate, result.EndDate);
            Assert.Equal(process.IdTelegrame, result.IdTelegrame);
            
            // Vérification que la méthode AddAsync a été appelée exactement une fois
            dbSetMock.Verify(x => x.AddAsync(process, default), Times.Once);
            // Vérification que la méthode SaveChangesAsync a été appelée exactement une fois
            _binanceContextMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        /// <summary>
        /// Test de création en cas d'erreur
        /// Vérifie que le repository gère correctement les exceptions
        /// </summary>
        [Fact]
        public async Task Create_ShouldReturnNull_WhenExceptionOccurs()
        {
            // Arrange - Préparation des données de test
            var process = _fixture.Create<BinanceMonitoringCoinWalletBalanceObjectiveProcess>();
            var dbSetMock = new Mock<DbSet<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>();
            
            // Configuration du mock pour simuler une exception lors de la sauvegarde
            _binanceContextMock.Setup(x => x.BinanceMonitoringCoinWalletBalanceObjectiveProcesses)
                .Returns(dbSetMock.Object);
            _binanceContextMock.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new Exception("Test exception"));

            // Act - Exécution de la méthode à tester
            var result = await _repository.Create(process);

            // Assert - Vérification des résultats
            // Vérification que null est retourné en cas d'erreur
            Assert.Null(result);
            // Vérification que l'erreur a été correctement journalisée
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        /// <summary>
        /// Test de vérification d'un processus actif
        /// Vérifie que la méthode retourne true quand un processus actif existe
        /// </summary>
        [Fact]
        public async Task GetIsIngProcessForObjective_ShouldReturnTrue_WhenActiveProcessExists()
        {
            // Arrange - Préparation des données de test
            var process = _fixture.Create<BinanceMonitoringCoinWalletBalanceObjectiveProcess>();
            process.EndDate = null; // Un processus est actif si EndDate est null
            
            var dbSetMock = new Mock<DbSet<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>();
            var data = new List<BinanceMonitoringCoinWalletBalanceObjectiveProcess> { process }.AsQueryable();

            dbSetMock.As<IQueryable<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<BinanceMonitoringCoinWalletBalanceObjectiveProcess>(data.Provider));
            dbSetMock.As<IQueryable<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>()
                .Setup(m => m.Expression)
                .Returns(data.Expression);
            dbSetMock.As<IQueryable<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>()
                .Setup(m => m.ElementType)
                .Returns(data.ElementType);
            dbSetMock.As<IQueryable<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>()
                .Setup(m => m.GetEnumerator())
                .Returns(data.GetEnumerator());

            dbSetMock.As<IAsyncEnumerable<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<BinanceMonitoringCoinWalletBalanceObjectiveProcess>(data.GetEnumerator()));

            _binanceContextMock.Setup(x => x.BinanceMonitoringCoinWalletBalanceObjectiveProcesses)
                .Returns(dbSetMock.Object);

            // Act
            var result = await _repository.GetIsIngProcessForObjective();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetIsIngProcessForObjective_ShouldReturnFalse_WhenNoActiveProcessExists()
        {
            // Arrange - Préparation des données de test
            var process = _fixture.Create<BinanceMonitoringCoinWalletBalanceObjectiveProcess>();
            process.EndDate = DateTime.Now;
            
            // Configuration du mock pour simuler une requête LINQ
            var dbSetMock = new Mock<DbSet<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>();
            var data = new List<BinanceMonitoringCoinWalletBalanceObjectiveProcess> { process }.AsQueryable();

            dbSetMock.As<IQueryable<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<BinanceMonitoringCoinWalletBalanceObjectiveProcess>(data.Provider));
            dbSetMock.As<IQueryable<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>()
                .Setup(m => m.Expression)
                .Returns(data.Expression);
            dbSetMock.As<IQueryable<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>()
                .Setup(m => m.ElementType)
                .Returns(data.ElementType);
            dbSetMock.As<IQueryable<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>()
                .Setup(m => m.GetEnumerator())
                .Returns(data.GetEnumerator());

            // Configuration para soportar FirstOrDefaultAsync
            dbSetMock.As<IAsyncEnumerable<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<BinanceMonitoringCoinWalletBalanceObjectiveProcess>(data.GetEnumerator()));

            _binanceContextMock.Setup(x => x.BinanceMonitoringCoinWalletBalanceObjectiveProcesses)
                .Returns(dbSetMock.Object);

            // Act - Exécution de la méthode à tester
            var result = await _repository.GetIsIngProcessForObjective();

            // Assert - Vérification des résultats
            Assert.False(result);
        }

        [Fact]
        public async Task GetLastIsIngProcessForObjective_ShouldReturnProcess_WhenActiveProcessExists()
        {
            // Arrange
            var idTelegrame = 12345L;
            var process = _fixture.Create<BinanceMonitoringCoinWalletBalanceObjectiveProcess>();
            process.IdTelegrame = idTelegrame;
            process.EndDate = null;
            
            var dbSetMock = new Mock<DbSet<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>();
            var data = new List<BinanceMonitoringCoinWalletBalanceObjectiveProcess> { process }.AsQueryable();

            dbSetMock.As<IQueryable<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<BinanceMonitoringCoinWalletBalanceObjectiveProcess>(data.Provider));
            dbSetMock.As<IQueryable<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>()
                .Setup(m => m.Expression)
                .Returns(data.Expression);
            dbSetMock.As<IQueryable<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>()
                .Setup(m => m.ElementType)
                .Returns(data.ElementType);
            dbSetMock.As<IQueryable<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>()
                .Setup(m => m.GetEnumerator())
                .Returns(data.GetEnumerator());

            dbSetMock.As<IAsyncEnumerable<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<BinanceMonitoringCoinWalletBalanceObjectiveProcess>(data.GetEnumerator()));

            _binanceContextMock.Setup(x => x.BinanceMonitoringCoinWalletBalanceObjectiveProcesses)
                .Returns(dbSetMock.Object);

            // Act
            var result = await _repository.GetLastIsIngProcessForObjective(idTelegrame);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(idTelegrame, result.IdTelegrame);
            Assert.Null(result.EndDate);
        }

        [Fact]
        public async Task Update_ShouldReturnTrue_WhenUpdateSuccessful()
        {
            // Arrange
            var process = _fixture.Create<BinanceMonitoringCoinWalletBalanceObjectiveProcess>();
            var dbSetMock = new Mock<DbSet<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>();
            
            _binanceContextMock.Setup(x => x.BinanceMonitoringCoinWalletBalanceObjectiveProcesses)
                .Returns(dbSetMock.Object);

            // Act
            var result = await _repository.Update(process);

            // Assert
            Assert.True(result);
            dbSetMock.Verify(x => x.Update(process), Times.Once);
            _binanceContextMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturnFalse_WhenExceptionOccurs()
        {
            // Arrange
            var process = _fixture.Create<BinanceMonitoringCoinWalletBalanceObjectiveProcess>();
            var dbSetMock = new Mock<DbSet<BinanceMonitoringCoinWalletBalanceObjectiveProcess>>();
            
            _binanceContextMock.Setup(x => x.BinanceMonitoringCoinWalletBalanceObjectiveProcesses)
                .Returns(dbSetMock.Object);
            _binanceContextMock.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _repository.Update(process);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Create_ShouldCreateProcess_WhenUsingInMemoryDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BinanceContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_3" + Guid.NewGuid().ToString())
                .Options;

            using (var context = new BinanceContext(options))
            {
                var repository = new BinanceMonitoringCoinWalletBalanceObjectiveProcessRepository(
                    _loggerMock.Object,
                    context);

                var process = _fixture.Build<BinanceMonitoringCoinWalletBalanceObjectiveProcess>()
                    .Without(x => x.IdTelegrameNavigation)
                    .Without(x => x.BinanceTraderUrlForUpdatePositionBinanceQueries)
                    .Create();

                // Act
                var result = await repository.Create(process);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(process.Id, result.Id);
                Assert.Equal(process.Equity, result.Equity);
                Assert.Equal(process.EquityObjective, result.EquityObjective);

                // Verify the process was actually saved to the in-memory database
                var savedProcess = await context.BinanceMonitoringCoinWalletBalanceObjectiveProcesses
                    .FirstOrDefaultAsync(p => p.Id == process.Id);
                Assert.NotNull(savedProcess);
                Assert.Equal(process.Equity, savedProcess.Equity);
                Assert.Equal(process.EquityObjective, savedProcess.EquityObjective);
            }
        }

        [Fact]
        public async Task GetIsIngProcessForObjective_ShouldReturnTrue_WhenUsingInMemoryDatabase_WithShouldly()
        {
            // Configuration de la base de données en mémoire avec un nom unique
            var databaseName = "TestDb_6" + Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<BinanceContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            using (var context = new BinanceContext(options))
            {
                // Nettoyage complet de la base de données pour s'assurer qu'elle est vide
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // Création du repository avec le logger mocké et le contexte en mémoire
                var repository = new BinanceMonitoringCoinWalletBalanceObjectiveProcessRepository(
                    _loggerMock.Object,
                    context);

                // Création d'un processus actif avec des valeurs de test spécifiques
                var activeProcess = _fixture.Build<BinanceMonitoringCoinWalletBalanceObjectiveProcess>()
                    .Without(x => x.IdTelegrameNavigation)  // Exclusion de la navigation pour éviter les références circulaires
                    .Without(x => x.BinanceTraderUrlForUpdatePositionBinanceQueries)  // Exclusion de la collection de navigation
                    .With(x => x.EndDate, (DateTime?)null)  // Configuration du processus comme actif (EndDate = null)
                    .With(x => x.Equity, 1000.0)  // Valeur de test pour Equity
                    .With(x => x.EquityObjective, 2000.0)  // Valeur de test pour EquityObjective
                    .Create();

                // Ajout du processus actif à la base de données en mémoire
                await context.BinanceMonitoringCoinWalletBalanceObjectiveProcesses.AddAsync(activeProcess);
                await context.SaveChangesAsync();  // Sauvegarde des changements

                // Act - Exécution de la méthode à tester
                var result = await repository.GetIsIngProcessForObjective();

                // Assert avec Shouldly - Vérification du résultat principal
                result.ShouldBeTrue("Le résultat devrait être true car il existe un processus actif");
                
                // Récupération du processus actif depuis la base de données pour vérification
                var savedProcess = await context.BinanceMonitoringCoinWalletBalanceObjectiveProcesses
                    .FirstOrDefaultAsync(p => p.EndDate == null);
                    
                // Vérifications détaillées des propriétés du processus
                savedProcess.ShouldNotBeNull("Un processus actif devrait exister dans la base de données");
                savedProcess.Equity.ShouldBe(1000.0, "La valeur Equity devrait être 1000.0");
                savedProcess.EquityObjective.ShouldBe(2000.0, "La valeur EquityObjective devrait être 2000.0");
                savedProcess.EndDate.ShouldBeNull("Le processus devrait être actif (EndDate = null)");
                
                // Vérification du nombre total de processus dans la base de données
                var totalProcesses = await context.BinanceMonitoringCoinWalletBalanceObjectiveProcesses.CountAsync();
                totalProcesses.ShouldBe(1, "Il devrait y avoir exactement un processus dans la base de données");
            }
        }

        [Fact]
        public async Task GetLastIsIngProcessForObjective_ShouldReturnCorrectProcess_WhenUsingInMemoryDatabase()
        {
            // Arrange - Configuration de la base de données en mémoire
            var options = new DbContextOptionsBuilder<BinanceContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_1" + Guid.NewGuid().ToString())  // Création d'un nom unique pour la base de données
                .Options;

            using (var context = new BinanceContext(options))  // Création du contexte avec la base de données en mémoire
            {
                // Création du repository avec le contexte en mémoire
                var repository = new BinanceMonitoringCoinWalletBalanceObjectiveProcessRepository(
                    _loggerMock.Object,
                    context);

                var idTelegrame = 12345L;  // ID Telegrame de test
                var now = DateTime.Now;    // Date de référence pour les tests

                // Création du premier processus (le plus ancien)
                var process1 = _fixture.Build<BinanceMonitoringCoinWalletBalanceObjectiveProcess>()
                    .Without(x => x.IdTelegrameNavigation)  // Exclusion de la navigation
                    .Without(x => x.BinanceTraderUrlForUpdatePositionBinanceQueries)  // Exclusion de la collection
                    .With(x => x.IdTelegrame, idTelegrame)  // Configuration de l'ID Telegrame
                    .With(x => x.CreatedOn, now.AddHours(-2))  // Création il y a 2 heures
                    .With(x => x.EndDate, (DateTime?)null)  // Processus actif
                    .Create();

                // Création du deuxième processus (le plus récent et actif)
                var process2 = _fixture.Build<BinanceMonitoringCoinWalletBalanceObjectiveProcess>()
                    .Without(x => x.IdTelegrameNavigation)
                    .Without(x => x.BinanceTraderUrlForUpdatePositionBinanceQueries)
                    .With(x => x.IdTelegrame, idTelegrame)
                    .With(x => x.CreatedOn, now.AddHours(-1))  // Création il y a 1 heure
                    .With(x => x.EndDate, (DateTime?)null)  // Processus actif
                    .Create();

                // Création du troisième processus (inactif)
                var process3 = _fixture.Build<BinanceMonitoringCoinWalletBalanceObjectiveProcess>()
                    .Without(x => x.IdTelegrameNavigation)
                    .Without(x => x.BinanceTraderUrlForUpdatePositionBinanceQueries)
                    .With(x => x.IdTelegrame, idTelegrame)
                    .With(x => x.CreatedOn, now)  // Création maintenant
                    .With(x => x.EndDate, DateTime.Now)  // Processus inactif
                    .Create();

                // Ajout des trois processus à la base de données en mémoire
                await context.BinanceMonitoringCoinWalletBalanceObjectiveProcesses.AddRangeAsync(process1, process2, process3);
                await context.SaveChangesAsync();  // Sauvegarde des changements

                // Act - Exécution de la méthode à tester
                var result = await repository.GetLastIsIngProcessForObjective(idTelegrame);

                // Assert - Vérifications des résultats
                Assert.NotNull(result);  // Vérifie que le résultat n'est pas null
                Assert.Equal(process2.Id, result.Id);  // Vérifie que c'est bien le processus 2 (le plus récent et actif)
                Assert.Equal(idTelegrame, result.IdTelegrame);  // Vérifie l'ID Telegrame
                Assert.Null(result.EndDate);  // Vérifie que c'est un processus actif (EndDate = null)
            }
        }
    }
}
