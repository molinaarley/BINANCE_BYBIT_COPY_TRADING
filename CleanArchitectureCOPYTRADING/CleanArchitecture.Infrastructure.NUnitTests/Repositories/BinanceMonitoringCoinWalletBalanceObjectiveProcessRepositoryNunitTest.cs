using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Moq;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Repositories;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Infrastructure.NUnitTests.Repositories
{
    [TestFixture]
    public class BinanceMonitoringCoinWalletBalanceObjectiveProcessRepositoryNunitTest
    {
        //Cette méthode de test vérifie l'ajout d'un processus dans la base de données en mémoire
        [Test]
        public async Task Create_ShouldAddProcess_ToInMemoryDatabase()
        {
            // Arrange - Configuration de la base de données en mémoire
            var options = new DbContextOptionsBuilder<BinanceContext>()
                .UseInMemoryDatabase(databaseName: "NUnitTestDb_" + Guid.NewGuid().ToString())
                .Options;

            using (var context = new BinanceContext(options))
            {
                // Création du repository
                var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<BinanceMonitoringCoinWalletBalanceObjectiveProcessRepository>>();
                var repository = new BinanceMonitoringCoinWalletBalanceObjectiveProcessRepository(loggerMock.Object, context);

                // Création d'un processus de test
                var process = new BinanceMonitoringCoinWalletBalanceObjectiveProcess
                {
                    Id = 1,
                    Equity = 500.0,
                    EquityObjective = 1000.0,
                    CreatedOn = DateTime.Now
                };

                // Act - Ajout du processus
                var result = await repository.Create(process);

                // Assert - Vérification que le processus a bien été ajouté
                NUnit.Framework.Assert.IsNotNull(result, "Le résultat ne doit pas être null");
                NUnit.Framework.Assert.AreEqual(500.0, result.Equity, "La valeur Equity doit être 500.0");
                Assert.AreEqual(1000.0, result.EquityObjective, "La valeur EquityObjective doit être 1000.0");

                // Vérification dans la base de données en mémoire
                var saved = await context.BinanceMonitoringCoinWalletBalanceObjectiveProcesses.FirstOrDefaultAsync(p => p.Id == 1);
                Assert.IsNotNull(saved, "Le processus doit exister dans la base de données");
            }
        }

        // Cette méthode de test vérifie que la méthode retourne false s'il n'y a pas de processus actif
        [Test]
        public async Task GetIsIngProcessForObjective_ShouldReturnFalse_WhenNoActiveProcessExists()
        {
            // Arrange - Configuration de la base de données en mémoire
            var options = new DbContextOptionsBuilder<BinanceContext>()
                .UseInMemoryDatabase(databaseName: "NUnitTestDb_" + Guid.NewGuid().ToString())
                .Options;

            using (var context = new BinanceContext(options))
            {
                var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<BinanceMonitoringCoinWalletBalanceObjectiveProcessRepository>>();
                var repository = new BinanceMonitoringCoinWalletBalanceObjectiveProcessRepository(loggerMock.Object, context);

                // Ajout d'un processus inactif (EndDate non null)
                var inactiveProcess = new BinanceMonitoringCoinWalletBalanceObjectiveProcess
                {
                    Id = 2,
                    Equity = 300.0,
                    EquityObjective = 800.0,
                    CreatedOn = DateTime.Now,
                    EndDate = DateTime.Now
                };
                await context.BinanceMonitoringCoinWalletBalanceObjectiveProcesses.AddAsync(inactiveProcess);
                await context.SaveChangesAsync();

                // Act - Appel de la méthode à tester
                var result = await repository.GetIsIngProcessForObjective();

                // Assert - On attend false car il n'y a pas de processus actif
                Assert.IsFalse(result, "Le résultat doit être false car il n'y a pas de processus actif");
            }
        }
    }
}
