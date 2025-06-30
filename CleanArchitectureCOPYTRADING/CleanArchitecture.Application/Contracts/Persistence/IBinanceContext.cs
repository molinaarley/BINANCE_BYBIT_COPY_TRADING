using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Binance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IBinanceContext
    {
       DbSet<BinanceByBitOrder> BinanceByBitOrders { get; set; }
        DbSet<BinanceByBitUser> BinanceByBitUsers { get; set; } 
         DbSet<BinanceOrder> BinanceOrders { get; set; }
        DbSet<BinanceOrderAudit> BinanceOrderAudits { get; set; }
        DbSet<BinanceTrader> BinanceTraders { get; set; }
        DbSet<BinanceTraderTypeData> BinanceTraderTypeData { get; set; }
        DbSet<BinanceTraderPerformanceRetList> BinanceTraderPerformanceRetList { get; set; }
        DbSet<BinanceTraderPerformanceRetListAudit> BinanceTraderPerformanceRetListAudit { get; set; }

        DbSet<BinanceMonitoringProcess> BinanceMonitoringProcess { get; set; }

         DbSet<BinanceMonitoringCoinWalletBalance> BinanceMonitoringCoinWalletBalances { get; set; }
        DbSet<BinanceMonitoringCoinWalletBalanceObjectiveProcess> BinanceMonitoringCoinWalletBalanceObjectiveProcesses { get; set; }
        DbSet<BinanceTraderUrlForUpdatePositionBinanceQuery> BinanceTraderUrlForUpdatePositionBinanceQueries { get; set; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        void RemoveRange(IEnumerable<object> entities);

        EntityEntry Update(object entity);

    }

}
