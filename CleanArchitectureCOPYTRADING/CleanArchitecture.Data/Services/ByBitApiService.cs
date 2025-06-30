using System.Globalization;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Models;
using Microsoft.Extensions.Logging;
using RestSharp;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using CleanArchitecture.Application.Converters;
using System.Drawing;

namespace CleanArchitecture.Infrastructure.Services
{

    public class ByBitApiService : IByBitApiService
    {
        //*********copiado de bybit.net**************
        protected byte[] _sBytes;
        //**********************
        private readonly HttpClient _httpClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly int _recvWindow = 5000;

        private readonly IAuthenticationByBitService _authenticationByBitService;
        private readonly IByBitApiRepository _byBitApiRepository;


        private IRestClient _restClient;

        public ILogger<ByBitApiService> _logger { get; }

        public ByBitApiService() { }

        public ByBitApiService(ILogger<ByBitApiService> logger, HttpClient httpClient, IAuthenticationByBitService authenticationByBitService
            , IByBitApiRepository byBitApiRepository)
        {
            _restClient = new RestClient(httpClient);
            _logger = logger;
            _httpClient = httpClient ?? throw new ArgumentException(nameof(httpClient));
            _byBitApiRepository = byBitApiRepository ?? throw new ArgumentException(nameof(byBitApiRepository));


            //_sBytes = Encoding.UTF8.GetBytes("ltEHhA91lPHnPTUpMYiNb15AVFjvkLLXV1mc");
            _authenticationByBitService = authenticationByBitService ?? throw new ArgumentException(nameof(authenticationByBitService));
            _httpClient.DefaultRequestHeaders.Add("X-BAPI-RECV-WINDOW", _recvWindow.ToString());


        }

        public async Task<RootOrderReponse> CreatePosition(OtherPositionRetList data, string apiKey, string secretKey, Coin coin, ResultTickersCategory resultTickersCategory)
        {
            return await _byBitApiRepository.CreatePosition(data, apiKey, secretKey, coin, resultTickersCategory);
        }



        public async Task<bool> GetPosition(string apiKey, string secretKey)
        {

            var timestamp = (await _authenticationByBitService.ConvertToMilliseconds((await _authenticationByBitService.GetTimestamp()).AddMilliseconds(-1000))).Value.ToString(CultureInfo.InvariantCulture);
            DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
            _httpClient.DefaultRequestHeaders.Add("X-BAPI-API-KEY", "wMTFl1uqruwAVyvac8");
            _httpClient.DefaultRequestHeaders.Add("X-BAPI-TIMESTAMP", timestamp/*now.ToUnixTimeMilliseconds().ToString()*/);
            _httpClient.DefaultRequestHeaders.Add("X-BAPI-RECV-WINDOW", "20000");
            _httpClient.DefaultRequestHeaders.Add("X-BAPI-SIGN", "ltEHhA91lPHnPTUpMYiNb15AVFjvkLLXV1mc");
            var responseString = await _httpClient.GetAsync("/v5/position/list");
            string jsonReponse = await responseString.Content.ReadAsStringAsync();
            //var responseString = await _httpClient.GetStringAsync("/private/position/create");
            //var catalog = JsonConvert.DeserializeObject<Position>(responseString);
            return true;
        }

        public async Task<bool> SetLeverage(SetLeverage setLeverage)
        {
            var result = await _byBitApiRepository.SetLeverage(setLeverage);
            return true;
        }

        public async Task<bool> APIKeyInformation(string apiKey, string secretKey)
        {

            var result = await _byBitApiRepository.APIKeyInformation(apiKey, secretKey);
            return true;
        }

        public async Task<List<WalletBalance>> WalletBalance(string apiKey, string secretKey)
        {

            return await _byBitApiRepository.WalletBalance(apiKey, secretKey);
        }



        public async Task<RootTickersCategorySymbol> GetTickersCategorySymbol(GetTickers getTickersRequest)
        {

            return await _byBitApiRepository.GetTickersCategorySymbol(getTickersRequest);
        }

        public async Task<Coin> GetCoinFromWalletBalance(List<WalletBalance> walletBalanceList)
        {

            if (walletBalanceList != null && walletBalanceList.Any() && walletBalanceList.FirstOrDefault().coin.Any())
            {

                return walletBalanceList.FirstOrDefault().coin.Where(p => !string.IsNullOrEmpty(p.coin) && p.coin.ToLower().Equals("USDT".ToLower())).FirstOrDefault();

            }
            return new Coin() { coin = string.Empty };
        }




        public async Task<bool> SetDepositAccount(string ApiKey, string SecretKey, string accountType = "UNIFIED")
        {
            return await _byBitApiRepository.SetDepositAccount(ApiKey, SecretKey, accountType);
        }

        public async Task<bool> GetCoinInfo(string ApiKey, string SecretKey)
        {
            return await _byBitApiRepository.GetCoinInfo(ApiKey, SecretKey);
        }

        public async Task<bool> SwitchCrossIsolatedMargin(SwitchIsolated switchIsolated)
        {
            return await _byBitApiRepository.SwitchCrossIsolatedMargin(switchIsolated);
        }

        public async Task<GetPositionInfoResult> GetPositionInfo(GetPositionInfo getOpenOrders)
        {
            return await _byBitApiRepository.GetPositionInfo(getOpenOrders);
        }

        public async Task<GetOpenOrdersResult> GetOpenOrders(GetOpenOrders getOpenOrders)
        {
            return await _byBitApiRepository.GetOpenOrders(getOpenOrders);
        }


        public async Task<bool> GethasOrderOpenForThisSymbolAndCancel(GetOpenOrdersResult getOpenOrdersResult, string ApiKey, string SecretKey)
        {
            bool hasOrderOpen = false;

            /*
             
               DateTime updateTime = new DateTime(itemPosition.updateTime[0], itemPosition.updateTime[1],
                                            itemPosition.updateTime[2], itemPosition.updateTime[3], itemPosition.updateTime[4], itemPosition.updateTime[5]);
                                
                                        DateTime dateTimeParis = DatetimeConvert.GetDateParisTimeZone(updateTime); 
             */

            if (getOpenOrdersResult.list!=null && getOpenOrdersResult.list.Any())
                {
                    DateTime dateTimeOrderOpenForSymbol = DateTimeOffset.FromUnixTimeMilliseconds(getOpenOrdersResult.list.FirstOrDefault().createdTime).DateTime.AddHours(1);
                    int totalMinute = (int)(DateTime.Now - dateTimeOrderOpenForSymbol).TotalMinutes;
                    if (totalMinute > 10)
                    {
                        var resultCancel = await CancelOrder(getOpenOrdersResult.list.FirstOrDefault(), ApiKey, SecretKey, getOpenOrdersResult.category);

                        return false;
                    }
                    else
                    {
                        hasOrderOpen = true;
                    }
                }
            return hasOrderOpen;
        }

        public async Task<bool> CancelOrder(GetOpenOrdersList getOpenOrdersList, string ApiKey, string SecretKey, string category = "inverse")
        {
            return await _byBitApiRepository.CancelOrder(getOpenOrdersList, ApiKey, SecretKey, category);
        }

        public async Task<bool> SetTradingStop(SetTradingStopRequest setTradingStop)
        {
            return await _byBitApiRepository.SetTradingStop(setTradingStop);
        }

        public async Task<bool> SetTradingStopForPosition(GetPositionInfoResultList positionInfoResult, GetInstrumentsInfoList getInstrumentsInfoList, BinanceByBitUser binanceByBitUser)
        {
            var tickersCategorySymbol =await GetTickersCategorySymbol(new GetTickers() { category = "inverse", symbol = positionInfoResult.symbol });


            var orderOpenForThisSymbol = await GetOpenOrders(new GetOpenOrders()
            {
                category = EnumConverter.GetString(Category.Linear),
                apiKey = binanceByBitUser.ApiKey,
                secretKey = binanceByBitUser.SecretKey,
                symbol = positionInfoResult.symbol
            });

            /*
              nameValueCollection.Add("symbol", getOpenOrdersList.symbol);
            nameValueCollection.Add("category", category);
            nameValueCollection.Add("orderId", getOpenOrdersList.orderId);
             */

            if (orderOpenForThisSymbol.list.Count>0)
            {
                await CancelOrder(new GetOpenOrdersList() { orderId = orderOpenForThisSymbol.list.FirstOrDefault().orderId, symbol = positionInfoResult.symbol },
               binanceByBitUser.ApiKey, binanceByBitUser.SecretKey, EnumConverter.GetString(Category.Linear));
            }
           
            if (positionInfoResult.side.Equals(EnumConverter.GetString(OrderSide.Buy)))
            {
                double? stopLoss = positionInfoResult.markPrice.Value - (getInstrumentsInfoList.priceFilter.tickSize * positionInfoResult.markPrice);
                double? slLimitPrice = positionInfoResult.markPrice.Value - (getInstrumentsInfoList.priceFilter.tickSize * positionInfoResult.markPrice);

                if (tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value.ToString(CultureInfo.InvariantCulture).Contains("."))
                {

                    stopLoss = Math.Round(stopLoss.Value, tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value.ToString(CultureInfo.InvariantCulture).Split('.')[1].Length, MidpointRounding.ToEven);
                    slLimitPrice = Math.Round(slLimitPrice.Value, tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value.ToString(CultureInfo.InvariantCulture).Split('.')[1].Length, MidpointRounding.ToEven);
                }
                else
                {
                    stopLoss = Math.Round(stopLoss.Value, 0, MidpointRounding.ToEven);
                    slLimitPrice = Math.Round(slLimitPrice.Value, 0, MidpointRounding.ToEven);
                }




                await SetTradingStop(new SetTradingStopRequest()
                {
                    apiKey = binanceByBitUser.ApiKey,
                    secretKey = binanceByBitUser.SecretKey,
                    symbol = positionInfoResult.symbol,
                    positionIdx = 0,
                    stopLoss = stopLoss,
                    category = EnumConverter.GetString(Category.Linear),
                    slOrderType = OrderType.Limit.ToString(),
                    slLimitPrice = slLimitPrice,
                    tpslMode = EnumConverter.GetString(TpslMode.Partial),
                    slSize=100
                });

                Console.WriteLine("tesok");
            }
            if (positionInfoResult.side.Equals(EnumConverter.GetString(OrderSide.Sell)))
            {
                double? stopLoss = positionInfoResult.markPrice.Value + getInstrumentsInfoList.priceFilter.tickSize;

                if (tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value.ToString(CultureInfo.InvariantCulture).Contains("."))
                {

                    stopLoss = Math.Round(stopLoss.Value, tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value.ToString(CultureInfo.InvariantCulture).Split('.')[1].Length, MidpointRounding.ToEven);
                }
                else
                {
                    stopLoss = Math.Round(stopLoss.Value, 0, MidpointRounding.ToEven);
                }
                await SetTradingStop(new SetTradingStopRequest()
                {
                    apiKey = binanceByBitUser.ApiKey,
                    secretKey = binanceByBitUser.SecretKey,
                    symbol = positionInfoResult.symbol,
                    positionIdx = 0,
                    stopLoss = stopLoss,
                    category = EnumConverter.GetString(Category.Linear),
                    slOrderType = OrderType.Limit.ToString(),
                    slLimitPrice = stopLoss,
                    tpslMode = EnumConverter.GetString(TpslMode.Partial)
                });
            }

            return true;
        }

        public async Task<RootOrderReponse> PlaceOrder(PlaceOrder placeOrder)
        {
            return await _byBitApiRepository.PlaceOrder(placeOrder);
        }

        public async Task<GetPositionInfoResult> GetAllCoinsBalance(GetAllCoinsBalance getAllCoinsBalance)
        {
            return await _byBitApiRepository.GetAllCoinsBalance(getAllCoinsBalance);
        }

        public async Task<GetBybitServerTimeRoot> GetBybitServerTime()
        {
            return await _byBitApiRepository.GetBybitServerTime();

        }
        public async Task<CreateInternalTransferRoot> CreateInternalTransfer(CreateInternalTransferQuery createInternalTransferQuery)
        {
            return await _byBitApiRepository.CreateInternalTransfer(createInternalTransferQuery);
        }

        public async Task<GetFeeRateReponseRoot> GetFeeRate(GetFeeRateQuery getFeeRate)
        {
            return await _byBitApiRepository.GetFeeRate(getFeeRate);
        }
        /// <summary>
        /// Query user's closed profit and loss records. The results are sorted by createdTime in descending order.
        /// </summary>
        /// <param name="getFeeRate"></param>
        /// <returns></returns>
        public async Task<GetClosedPnLReponseRoot> GetClosedPnL(GetClosedPnL getFeeRate)
        {
            return await _byBitApiRepository.GetClosedPnL(getFeeRate);
        }

        public async Task<GetInstrumentsInfoRoot> GetInstrumentsInfo(GetInstrumentsInfoQuery getInstrumentsInfo)
        {
            return await _byBitApiRepository.GetInstrumentsInfo(getInstrumentsInfo);

        }
    }
}
