using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class BinanceOrderRepository : IBinanceOrderRepository
    {
        private readonly IBinanceContext _binanceContext;
        public ILogger<BinanceOrderRepository> _logger { get; }
        public BinanceOrderRepository(ILogger<BinanceOrderRepository> logger, IBinanceContext binanceContext)
        {
            _logger = logger;
            _binanceContext = binanceContext ?? throw new ArgumentException(nameof(binanceContext));

        }
        public async Task<int> Create(BinanceOrder binanceOrder)
        {
            try
            {
                await _binanceContext.BinanceOrders.AddAsync(binanceOrder);
                await _binanceContext.SaveChangesAsync();
                return binanceOrder.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating BinanceOrderRepository {binanceOrder.Symbol}");
                // throw ex;
                return 1;
            }
        }

        public async Task<BinanceOrder> GetOrder(BinanceOrder binanceOrder, string EncryptedUid)
        {

            var result = _binanceContext.BinanceOrders.Where(p => p.Side.Equals(binanceOrder.Side) && p.Symbol.Equals(binanceOrder.Symbol)
                /*&& p.Amount == binanceOrder.Amount && p.EntryPrice == binanceOrder.EntryPrice*/
                && p.Leverage == binanceOrder.Leverage).Include(p => p.BinanceByBitOrders).AsNoTracking().AsQueryable();
            var resultData = await result.FirstOrDefaultAsync();
            return resultData;

        }

        public async Task<List<BinanceOrder>> GetAll()
        {
            var result = _binanceContext.BinanceOrders.Include(p => p.BinanceByBitOrders).
               AsNoTracking().AsQueryable();
            return result.ToList();

        }

        public async Task<List<BinanceOrder>> GetOpenOrdersByEncryptedUid(string EncryptedUid)
        {
            List<BinanceOrder> result = await _binanceContext.BinanceOrders.Where(p => p.EncryptedUid.Equals(EncryptedUid)
                    && !string.IsNullOrEmpty(p.Symbol)).Include(p => p.BinanceByBitOrders)
                  .GroupBy(p => p.Symbol).Where(p => p.Count() > 1)
                  .Select(g => g.OrderByDescending(e => e.CreatedOn).FirstOrDefault())
                  .ToListAsync();
            return result;
        }


        public async Task<BinanceOrder> GetOrderByUserIdTelegrame(BinanceOrder binanceOrder, long IdTelegrame, string EncryptedUid)
        {
            var orderResult = await GetOrder(binanceOrder, EncryptedUid);

            if (orderResult != null)
            {
                var resultBinanceByBitOrder = _binanceContext.BinanceByBitOrders.Where(p => p.BinanceOrderId == orderResult.Id
              && p.IdTelegrame == IdTelegrame).AsNoTracking().AsQueryable();
                var resultData = await resultBinanceByBitOrder.FirstOrDefaultAsync();
                if (resultData != null)
                {
                    return orderResult;
                }
            }
            return null;
        }

        public async Task<List<BinanceOrder>> GetBinanceByBitOrdersForByEncryptedUid(string EncryptedUid)
        {
            var orderForDeletedResult = await GetOpenOrdersByEncryptedUid(EncryptedUid);
            List<BinanceOrder> result = new List<BinanceOrder>();
            if (orderForDeletedResult.Any())
            {
                foreach (var item in orderForDeletedResult)
                {
                    var binanceOrdersForDeleted = await _binanceContext.BinanceOrders.Where(p => p.Symbol.Equals(item.Symbol)
                               && p.EncryptedUid.Equals(item.EncryptedUid)
                               && p.CreatedOn != item.CreatedOn).Include(p => p.BinanceByBitOrders).ToListAsync();
                    result.AddRange(binanceOrdersForDeleted);
                }
            }
            return result;
        }


        public async Task<List<BinanceOrder>> DeleteBinanceByBitOrdersForByEncryptedUid(string EncryptedUid)
        {
            var orderForDeletedResult = await GetOpenOrdersByEncryptedUid(EncryptedUid);
            List<BinanceOrder> result = new List<BinanceOrder>();
            if (orderForDeletedResult.Any())
            {
                foreach (var item in orderForDeletedResult)
                {
                    var binanceOrdersForDeleted = await _binanceContext.BinanceOrders.Where(p => p.Symbol.Equals(item.Symbol)
                               && p.EncryptedUid.Equals(item.EncryptedUid)
                               && p.CreatedOn != item.CreatedOn).Include(p => p.BinanceByBitOrders).ToListAsync();

                    foreach (var itemdeleted in binanceOrdersForDeleted)
                    {
                        _binanceContext.BinanceByBitOrders.RemoveRange(itemdeleted.BinanceByBitOrders);
                        await _binanceContext.SaveChangesAsync();
                    }
                    result.AddRange(binanceOrdersForDeleted);
                }
            }
            return result;
        }
        public async Task<bool> DeletedOrder(BinanceOrder binanceOrder)
        {


            try
            {
                _binanceContext.BinanceOrders.Remove(binanceOrder);
                await _binanceContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
               // throw new Exception(
               //     "DeletedOrder  EXEPTION.", e);
            }
            return true;
        }

        public async Task<bool> DeletedOrders(List<BinanceOrder> binanceOrders)
        {
            foreach (var item in binanceOrders)
            {
                await DeletedOrder(item);
            }
            return true;
        }

        public async Task<List<OtherPositionRetList>> GetOtherPositionRetList(List<PositionData> positionsData)
        {
            List<OtherPositionRetList> resultList = new List<OtherPositionRetList>();

            if (positionsData != null)
            {
                foreach (var item in positionsData)
                {
                    if (item.data != null && item.data.otherPositionRetList != null)
                    {
                        foreach (var itemPosition in item.data.otherPositionRetList)
                        {
                            if (itemPosition.amount<0) //item.LongShort.ContainsKey(itemPosition.symbol)
                            {
                                itemPosition.side = EnumConverter.GetString(OrderSide.Sell);//item.LongShort[itemPosition.symbol].Trim();
                            }
                            else
                            {
                                itemPosition.side = EnumConverter.GetString(OrderSide.Buy);
                            }

                            resultList.Add(itemPosition);
                        }
                    }
                }
            }
            return resultList;
        }


        public async Task<PlaceOrder> GetPlaceOrderForDeleted(GetPositionInfoResult positionBybitInfo,
             string apiKey,string secretKey, double qty)
        {
            PlaceOrder placeOrder = new PlaceOrder();
            placeOrder.symbol = positionBybitInfo.list.FirstOrDefault().symbol;//itemPosition.symbol;
            placeOrder.orderType = OrderType.Market.ToString();


            if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.Buy)))
            {
                placeOrder.side = EnumConverter.GetString(OrderSide.Sell);
            }
            if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.Sell)))
            {
                placeOrder.side = EnumConverter.GetString(OrderSide.Buy);
            }

            if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.None)))
            {
                placeOrder.side = EnumConverter.GetString(OrderSide.None);
            }
            placeOrder.category = EnumConverter.GetString(Category.Linear);
            //"linear";
            placeOrder.apiKey = apiKey;
            placeOrder.secretKey = secretKey;
            placeOrder.timeInForce = EnumConverter.GetString(TimeInForce.GoodTillCanceled);
            placeOrder.reduceOnly = true;
            placeOrder.closeOnTrigger = true;
            //placeOrder.qty = getClosedPnL.result.list.FirstOrDefault().qty.Value < 0 ? getClosedPnL.result.list.FirstOrDefault().qty.Value * -1 : getClosedPnL.result.list.FirstOrDefault().qty.Value;
            placeOrder.qty = qty;
            return placeOrder;

        }
    }
}
