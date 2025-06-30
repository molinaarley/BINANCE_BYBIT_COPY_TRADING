using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class BinanceTraderPerformanceRetListRepository : IBinanceTraderPerformanceRetListRepository
    {
       private readonly IBinanceContext _binanceContext;
        public ILogger<BinanceTraderPerformanceRetListRepository> _logger { get; }
        public BinanceTraderPerformanceRetListRepository(ILogger<BinanceTraderPerformanceRetListRepository> logger, IBinanceContext binanceContext) 
        {
            _logger = logger;
            _binanceContext = binanceContext ?? throw new ArgumentException(nameof(binanceContext));
        }

        public async Task<string> Create(BinanceTraderPerformanceRetList binanceTraderPerformanceRetList)
        {
             try
             {
                await  _binanceContext.BinanceTraderPerformanceRetList.AddAsync(binanceTraderPerformanceRetList);
                await _binanceContext.SaveChangesAsync();
                return binanceTraderPerformanceRetList.EncryptedUid;
             }
             catch (Exception ex)
             {
                _logger.LogError(ex, $"Error creating BinanceTraderPerformanceRetList {binanceTraderPerformanceRetList.EncryptedUid}");
                throw ex;
             }
        }

        public async Task<BinanceTraderPerformanceRetList> Update(BinanceTraderPerformanceRetList binanceTraderPerformanceRetList)
        {
            try
            {
                var updateEntity = _binanceContext.BinanceTraderPerformanceRetList.Update(binanceTraderPerformanceRetList);
                await _binanceContext.SaveChangesAsync();
                return updateEntity.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating BinanceTraderPerformanceRetList {binanceTraderPerformanceRetList.EncryptedUid}");
                throw ex;
            }
        }
        public async Task<bool> UpdateRange(List<BinanceTraderPerformanceRetList> binanceTraderPerformanceRetList)
        {
            try
            {
               _binanceContext.BinanceTraderPerformanceRetList.UpdateRange(binanceTraderPerformanceRetList);
                await _binanceContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error UpdateRange");
                throw ex;
            }
        }
        public async Task<bool> DeletedAll()
        {
            try
            {
               var allItem= _binanceContext.BinanceTraderPerformanceRetList.ToList();
               _binanceContext.BinanceTraderPerformanceRetList.RemoveRange(allItem);
                await _binanceContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error DeletedAll BinanceTraderPerformanceRetList");
                throw ex;
            }
            return true;
        }

        public async Task<bool> Deleted(string encryptedUid)
        {
            try
            {
                var allItem = _binanceContext.BinanceTraderPerformanceRetList.Where(p=>p.EncryptedUid.ToLower().Equals(encryptedUid.ToLower())).ToList();
                _binanceContext.BinanceTraderPerformanceRetList.RemoveRange(allItem);
                await _binanceContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error DeletedAll BinanceTraderPerformanceRetList");
                throw ex;
            }
            return true;
        }
        /// <summary>
        /// We removed all preformance of traders that were not currently calculated because they are not the best
        /// </summary>
        /// <param name="guidNewItem"></param>
        /// <returns></returns>
        public async Task<bool> DeletedOldPerformanceItem(string guidNewItem)
        {
            try
            {
                var allItem = _binanceContext.BinanceTraderPerformanceRetList.Where(p => !p.GuidNewItem.ToLower().Equals(guidNewItem.ToLower())).ToList();
                _binanceContext.BinanceTraderPerformanceRetList.RemoveRange(allItem);
                await _binanceContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error DeletedAll BinanceTraderPerformanceRetList");
                throw ex;
            }
            return true;
        }
        public async Task<RootBinancePerformanceRetList> LoadBinanceTraderPerformanceFromJson(string filePath)
        {
            try
            {
               List<string> listPositionperformanceDataJson = System.IO.File.ReadAllLines(filePath).ToList();
               string performance = listPositionperformanceDataJson.Where(p => p.Contains("performanceRetList")).FirstOrDefault();
                RootBinancePerformanceRetList resultDadaPerformance = new RootBinancePerformanceRetList();
                if (!string.IsNullOrEmpty(performance))
                {
                    resultDadaPerformance = JsonConvert.DeserializeObject<RootBinancePerformanceRetList>(performance);

                }
                string positionStr = listPositionperformanceDataJson.Where(p => p.Contains("otherPositionRetList")).FirstOrDefault();
                PositionData resultDadapositionStr = new PositionData();
                if (!string.IsNullOrEmpty(positionStr))
                {
                    resultDadapositionStr = JsonConvert.DeserializeObject<PositionData>(positionStr);

                }
                if (!string.IsNullOrEmpty(performance) &&  !string.IsNullOrEmpty(positionStr)  && resultDadapositionStr.data.otherPositionRetList != null &&
                    resultDadapositionStr.data.otherPositionRetList.Count() > 0)
                {
                    resultDadaPerformance.positionsOpen = true;
                }
                else
                {
                    resultDadaPerformance.positionsOpen = false;
                }
                return resultDadaPerformance;
            }
            catch (Exception ex)
            {
           
                throw ex;
            }
         
        }

        public async Task< BinanceTraderPerformanceRetList> GetByUidPeriodType(string uid,string periodType)
        {
            var result = _binanceContext.BinanceTraderPerformanceRetList.Where(p => p.EncryptedUid.Equals(uid) && p.PeriodType.ToLower().Equals(periodType.ToLower())).
               AsNoTracking().AsQueryable();
            var resultData = await  result.FirstOrDefaultAsync();
            return resultData;
           
        }

        public async Task<Dictionary<string, int>> GetTraderForUpdatePositionFromBinance()
        {
            var allItem = _binanceContext.BinanceTraderPerformanceRetList.
                 Where(p => !string.IsNullOrEmpty(p.PeriodType) && (p.PeriodType.Equals("ALL") || p.PeriodType.Equals("DAILY") ||
                 p.PeriodType.Equals("MONTHLY") || p.PeriodType.Equals("WEEKLY"))
                 && !string.IsNullOrEmpty(p.StatisticsType) && p.StatisticsType.Equals("ROI") && p.Value.HasValue && p.Value>0
                 ).AsNoTracking().ToList();
            var result = allItem.GroupBy(p => p.EncryptedUid).ToDictionary(p => p.Key, p => p.ToList().Count());
            return result;
        }

        public async Task<List<BinanceTraderPerformanceRetList>> GetAll()
        {
            var result =await  _binanceContext.BinanceTraderPerformanceRetList.AsNoTracking().ToListAsync();
            return result;
        }

        /// <summary>
        /// save the history of the greatest TRADER  to have the BI
        /// </summary>
        /// <returns></returns>
        public async Task<int> AddIndBinanceTraderPerformanceRetListAudit()
        {
            var param = 3;
            var result = await _binanceContext.Database.ExecuteSqlRawAsync(@$" insert into [dbo].[Binance_Trader_Performance_RetList_Audit]  ( [EncryptedUid]
              ,[CreatedOn],[PeriodType],[StatisticsType],[Value],[rank],[GuidNewItem])
                 select  [EncryptedUid],[CreatedOn],[PeriodType],[StatisticsType],[Value]
              ,[rank],[GuidNewItem] from [Binance].[dbo].[Binance_Trader_Performance_RetList]", param);
            return result;
        }


        public async Task<Dictionary<string,string>> GetAllDictionary()
        {
            var result = (await GetAll()).GroupBy(x => x.EncryptedUid)
           .ToDictionary(p => p.Key, p => p.FirstOrDefault().EncryptedUid );
            return result;
        }
    }
}
