using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IByBitApiRepository
    {
        Task<bool> APIKeyInformation(string apiKey, string secretKey);
        Task<List<WalletBalance>> WalletBalance(string apiKey, string secretKey);
        Task<RootOrderReponse> CreatePosition(OtherPositionRetList data, string apiKey, string secretKey, Coin coin, ResultTickersCategory resultTickersCategory);
        Task<bool> SetLeverage(SetLeverage setLeverage);
         Task<bool> CancelOrder(GetOpenOrdersList getOpenOrdersList, string ApiKey, string SecretKey, string category = "inverse");
        Task<bool> SetDepositAccount(string ApiKey, string SecretKey, string accountType = "UNIFIED");
        Task<bool> GetCoinInfo(string ApiKey, string SecretKey);
        Task<bool> SwitchCrossIsolatedMargin(SwitchIsolated switchIsolated);
        Task<GetPositionInfoResult> GetPositionInfo(GetPositionInfo getOpenOrders);
        Task<GetOpenOrdersResult> GetOpenOrders(GetOpenOrders getOpenOrders);
        Task<bool> SetTradingStop(SetTradingStopRequest setTradingStop);
        Task<RootOrderReponse> PlaceOrder(PlaceOrder placeOrder);
        Task<GetPositionInfoResult> GetAllCoinsBalance(GetAllCoinsBalance getPositionInfo);
        Task<GetBybitServerTimeRoot> GetBybitServerTime();
        Task<CreateInternalTransferRoot> CreateInternalTransfer(CreateInternalTransferQuery createInternalTransferQuery);
        Task<GetFeeRateReponseRoot> GetFeeRate(GetFeeRateQuery getFeeRate);
        Task<GetClosedPnLReponseRoot> GetClosedPnL(GetClosedPnL getFeeRate);

        Task<GetInstrumentsInfoRoot> GetInstrumentsInfo(GetInstrumentsInfoQuery getInstrumentsInfo);
        Task<RootTickersCategorySymbol> GetTickersCategorySymbol(GetTickers getTickersRequest);
        Task<GetPositionInfoResult> GetTradeHistory(GetTradeHistory getTradeHistoryRequest);
     }
}
