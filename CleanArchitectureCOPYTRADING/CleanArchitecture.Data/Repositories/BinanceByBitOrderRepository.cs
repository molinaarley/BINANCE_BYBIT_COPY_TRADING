using System.Collections.Specialized;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Web;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System.Linq;
using Telegram.Bot.Types;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class BinanceByBitOrderRepository : IBinanceByBitOrderRepository
    {
       private readonly IBinanceContext _binanceContext;
        public ILogger<BinanceByBitOrderRepository> _logger { get; }
        public BinanceByBitOrderRepository(ILogger<BinanceByBitOrderRepository> logger, IBinanceContext binanceContext) 
        {
            _logger = logger;
            _binanceContext = binanceContext ?? throw new ArgumentException(nameof(binanceContext));
        }

        public async Task<BinanceByBitOrder> Create(BinanceByBitOrder binanceByBitOrder)
        {
            try
            {
                await _binanceContext.BinanceByBitOrders.AddAsync(binanceByBitOrder);
                await _binanceContext.SaveChangesAsync();
                return binanceByBitOrder;
            }
            catch (Exception e)
            {
                throw new Exception(
                    "Create  BinanceByBitOrder.", e);
            }

        }
        public async Task<List<BinanceByBitOrder>> GetAll()
        {
            var result = _binanceContext.BinanceByBitOrders.Include(p=>p.BinanceOrder).
               AsNoTracking().AsQueryable();
            return result.ToList();
        }

        public async Task<double> GetTotalAmount()
        {
            var result =await _binanceContext.BinanceByBitOrders.SumAsync(p=>p.Amount);
            return result.Value;
        }

        public async Task<bool> Deleted(BinanceByBitOrder binanceByBitOrder)
        {
            try
            {
                
                _binanceContext.BinanceByBitOrders.Remove(binanceByBitOrder);
                await _binanceContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                 System.IO.File.AppendAllLines("WriteLinesLogerrorBinanceByBitOrders.txt", new[] { e.Message  });
                                      

               // throw new Exception(
                //    "DeletedOrder  EXEPTION.", e);
            }
            return true;
        }

        public async Task<bool> DeletedRange(List<BinanceByBitOrder>  binanceByBitOrder)
        {
            try
            {

                _binanceContext.BinanceByBitOrders.RemoveRange(binanceByBitOrder);
                await _binanceContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(
                    "DeletedOrder  EXEPTION.", e);
            }
            return true;
        }

        public async Task<BinanceByBitOrder> GetOrder(BinanceByBitOrder binanceByBitOrder)
        {
            var result = _binanceContext.BinanceByBitOrders.Where(p => p.Id == binanceByBitOrder.Id
            ).AsNoTracking().AsQueryable();
            var resultData = await result.FirstOrDefaultAsync();
            return resultData;
        }




        public async Task<Dictionary<long, List<BinanceByBitOrder>>> GetAllInDictionary()
        {
            var result = (await GetAll()).GroupBy(p => p.IdTelegrame)
           .ToDictionary(p => p.Key, p => p.ToList());
            return result;
        }


    }
}
