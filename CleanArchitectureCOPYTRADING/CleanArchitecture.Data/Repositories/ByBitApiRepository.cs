using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class ByBitApiRepository : IByBitApiRepository
    {
        //*********copiado de bybit.net**************
        protected byte[] _sBytes;
        //**********************
        private readonly HttpClient _httpClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly int _recvWindow = 5000;
        private readonly IAuthenticationByBitService _authenticationByBitService;
        private IRestClient _restClient;
        public IConfiguration _configuration { get; }

        public ILogger<ByBitApiRepository> _logger { get; }
        public ByBitApiRepository(ILogger<ByBitApiRepository> logger, HttpClient httpClient, IAuthenticationByBitService authenticationByBitService, IConfiguration configuration) 
        {

            _restClient = new RestClient(httpClient);
            _logger = logger;
            _httpClient = httpClient ?? throw new ArgumentException(nameof(httpClient));
            _authenticationByBitService = authenticationByBitService ?? throw new ArgumentException(nameof(authenticationByBitService));
            //_httpClient.DefaultRequestHeaders.Add("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            _configuration = configuration ?? throw new ArgumentException(nameof(configuration));

        }

        public async Task<bool> APIKeyInformation(string apiKey, string secretKey)
        {
            //var time = (await _authenticationByBitService.GetTimestamp()).AddMinutes(1).AddSeconds(-6).AddMilliseconds(-1500);
            //var timestamp = (await _authenticationByBitService.ConvertToMilliseconds(time)).Value.ToString(CultureInfo.InvariantCulture);
            var timestamp = await GetBybitServerTimeValue();
           
            _sBytes = Encoding.UTF8.GetBytes(secretKey);
            var payload = timestamp + apiKey + _recvWindow.ToString();

            string sign = await _authenticationByBitService.SignHMACSHA256(payload, secretKey);
            _httpClient.DefaultRequestHeaders.Add("X-BAPI-API-KEY", apiKey);
            _httpClient.DefaultRequestHeaders.Add("X-BAPI-TIMESTAMP", timestamp);
            _httpClient.DefaultRequestHeaders.Add("X-BAPI-SIGN", sign);
            _httpClient.DefaultRequestHeaders.Add("X-BAPI-RECV-WINDOW", _recvWindow.ToString());

            var responseString = await _httpClient.GetAsync("/v5/user/query-api");
            string jsonReponse = await responseString.Content.ReadAsStringAsync();
            return true;
        }

        public async Task<List<WalletBalance>> WalletBalance(string apiKey, string secretKey)
        {
            
          
            _sBytes = Encoding.UTF8.GetBytes(secretKey);
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection.Add("accountType", _configuration.GetSection("BinanceBybitSettings:AccountType").Value );


            var signPayload = nameValueCollection.ToString();

            var timestamp = await GetBybitServerTimeValue();
   
            var payload = timestamp + apiKey + _recvWindow.ToString() + signPayload;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload, secretKey);

            var request = new RestRequest("/account/wallet-balance", Method.Get);
            request.AddParameter("accountType", _configuration.GetSection("BinanceBybitSettings:AccountType").Value);

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", apiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.GetAsync(request);

            var resultDada = JsonConvert.DeserializeObject<Root>(response.Content.ToString());
            return resultDada.result.list;
        }


        public async Task<CreateInternalTransferRoot> CreateInternalTransfer(CreateInternalTransferQuery createInternalTransferQuery)
        {
            _sBytes = Encoding.UTF8.GetBytes(createInternalTransferQuery.SecretKey);
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection.Add("transferId", createInternalTransferQuery.TransferId);
            nameValueCollection.Add("coin", createInternalTransferQuery.Coin/*OrderType.Market.ToString()*/);
            nameValueCollection.Add("amount", createInternalTransferQuery.Amount/*OrderType.Market.ToString()*/);
            nameValueCollection.Add("fromAccountType", createInternalTransferQuery.FromAccountType/*placeOrder.qty.Value.ToString(CultureInfo.InvariantCulture)*/);
            nameValueCollection.Add("toAccountType", createInternalTransferQuery.ToAccountType);//data.side

            var signPayload = nameValueCollection.ToString();
            var timestamp = await GetBybitServerTimeValue();

            var payload = timestamp + createInternalTransferQuery.ApiKey + _recvWindow.ToString() + signPayload;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload, createInternalTransferQuery.SecretKey);

            var request = new RestRequest("/asset/transfer/inter-transfer", Method.Post);
            request.AddParameter("transferId", createInternalTransferQuery.TransferId);
            request.AddParameter("coin", createInternalTransferQuery.Coin);
            request.AddParameter("amount", createInternalTransferQuery.Amount);
            request.AddParameter("fromAccountType", createInternalTransferQuery.FromAccountType);
            request.AddParameter("toAccountType", createInternalTransferQuery.ToAccountType);

            

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", createInternalTransferQuery.ApiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.PostAsync(request);

            var resultDada = JsonConvert.DeserializeObject<CreateInternalTransferRoot>(response.Content.ToString());
            return resultDada;
        }

        /// <summary>
        /// Query for the latest price snapshot, best bid/ask price, and trading volume in the last 24 hours.
        /// </summary>
        /// <param name="getTickersRequest"></param>
        /// <returns></returns>
        public async Task<RootTickersCategorySymbol> GetTickersCategorySymbol(GetTickers getTickersRequest)
        {
           
            var request = new RestRequest("/market/tickers", Method.Get);
            request.AddParameter("category", getTickersRequest.category);
            request.AddParameter("symbol", getTickersRequest.symbol);

            _restClient = new RestClient(_httpClient);
            var response = await _restClient.GetAsync(request);

            var resultDada = JsonConvert.DeserializeObject<RootTickersCategorySymbol>(response.Content.ToString());
            return resultDada;
        }


        public async Task<GetBybitServerTimeRoot> GetBybitServerTime()
        {
            var request = new RestRequest("/market/time", Method.Get);

            _restClient = new RestClient(_httpClient);
            var response = await _restClient.GetAsync(request);
            var resultDada = JsonConvert.DeserializeObject<GetBybitServerTimeRoot>(response.Content.ToString());
            return resultDada;
        }
        public async Task<string> GetBybitServerTimeValue()
        {
            var resultDada =  await GetBybitServerTime();
            return resultDada.time.ToString(CultureInfo.InvariantCulture);
        }

        public async Task<RootOrderReponse> PlaceOrder_FOR_TELSNET_NOT_UNIOFIED(PlaceOrder placeOrder)
        {

            //  var lastPrice = resultTickersCategory.list.FirstOrDefault().lastPrice.Value;
            // double qty = Math.Round((((coin.equity * 30) / 100).Value) / lastPrice, 3, MidpointRounding.ToEven);
            //placeOrder.qty = qty;
            double leverage = 0;

            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection.Add("symbol", placeOrder.symbol);// data.symbol
            nameValueCollection.Add("orderType", placeOrder.orderType);//OrderType.Limit.ToString()
            nameValueCollection.Add("qty", placeOrder.qty.Value.ToString(CultureInfo.InvariantCulture));
            nameValueCollection.Add("side", placeOrder.side.Trim());//data.side
            nameValueCollection.Add("category", placeOrder.category);//linear
            if(placeOrder.price.HasValue)
            nameValueCollection.Add("price", placeOrder.price.Value.ToString(CultureInfo.InvariantCulture));//linear

            if (!string.IsNullOrEmpty(placeOrder.timeInForce))
                nameValueCollection.Add("timeInForce", placeOrder.timeInForce);
            if (placeOrder.reduceOnly != null)
                nameValueCollection.Add("reduceOnly", placeOrder.reduceOnly.ToString().ToLowerInvariant());
            if (placeOrder.closeOnTrigger != null)
                nameValueCollection.Add("closeOnTrigger", placeOrder.closeOnTrigger.ToString().ToLowerInvariant());
            
                 if (placeOrder.stopLoss.HasValue)
                nameValueCollection.Add("stopLoss", placeOrder.stopLoss.Value.ToString(CultureInfo.InvariantCulture));

            //var timestamp = await _authenticationByBitService.ConvertToMillisecondsString();
            var timestamp = await GetBybitServerTimeValue();
            var signPayload = nameValueCollection.ToString();
            _sBytes = Encoding.UTF8.GetBytes(placeOrder.secretKey);
            var payload = timestamp + placeOrder.apiKey + _recvWindow.ToString() + signPayload;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload, placeOrder.secretKey);
            var request = new RestRequest("/order/create");

            request.AddParameter("symbol", placeOrder.symbol);
            request.AddParameter("orderType", placeOrder.orderType);
            request.AddParameter("qty", placeOrder.qty.Value.ToString(CultureInfo.InvariantCulture));
            request.AddParameter("side", placeOrder.side.Trim());//data.side

            request.AddParameter("category", placeOrder.category);//linear
            if (placeOrder.price.HasValue)
                request.AddParameter("price", placeOrder.price.Value.ToString(CultureInfo.InvariantCulture));

            if (!string.IsNullOrEmpty(placeOrder.timeInForce))
                request.AddParameter("timeInForce", placeOrder.timeInForce);
            if (placeOrder.reduceOnly != null)
                request.AddParameter("reduceOnly", placeOrder.reduceOnly.ToString().ToLowerInvariant());
            if (placeOrder.closeOnTrigger != null)
                request.AddParameter("closeOnTrigger", placeOrder.closeOnTrigger.ToString().ToLowerInvariant());

            if (placeOrder.stopLoss.HasValue)
                request.AddParameter("stopLoss", placeOrder.stopLoss.Value.ToString(CultureInfo.InvariantCulture));

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", placeOrder.apiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.PostAsync(request);

            var resultDada = JsonConvert.DeserializeObject<RootOrderReponse>(response.Content.ToString());
            return resultDada;
        }

        public async Task<RootOrderReponse> PlaceOrder(PlaceOrder placeOrder)
        {
            //if (placeOrder == null)
            //    throw new ArgumentNullException(nameof(placeOrder));

            var timestamp = await GetBybitServerTimeValue();
            var payload = new
            {
                symbol = placeOrder.symbol,
                orderType = placeOrder.orderType,
                qty = placeOrder.qty.Value.ToString(CultureInfo.InvariantCulture),
                side = placeOrder.side.Trim(),
                category = placeOrder.category,
                price = placeOrder.price?.ToString(CultureInfo.InvariantCulture),
                timeInForce = placeOrder.timeInForce,
                reduceOnly = placeOrder.reduceOnly?.ToString().ToLowerInvariant(),
                closeOnTrigger = placeOrder.closeOnTrigger?.ToString().ToLowerInvariant(),
                stopLoss = placeOrder.stopLoss?.ToString(CultureInfo.InvariantCulture)
            };

            var signPayload = JsonConvert.SerializeObject(payload);
            _sBytes = Encoding.UTF8.GetBytes(placeOrder.secretKey);
            var sign = await _authenticationByBitService.SignHMACSHA256(timestamp + placeOrder.apiKey + _recvWindow + signPayload, placeOrder.secretKey);

            var request = new RestRequest("/order/create", Method.Post);
            request.AddJsonBody(payload);

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", placeOrder.apiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());

            var response = await _restClient.ExecuteAsync(request);
          
            var resultData = JsonConvert.DeserializeObject<RootOrderReponse>(response.Content);
            return resultData;
        }

        public async Task<RootOrderReponse> CreatePosition(OtherPositionRetList data, string apiKey, string secretKey,
                Coin coin, ResultTickersCategory resultTickersCategory)
        {

            var timestamp = await GetBybitServerTimeValue();

            var lastPrice = resultTickersCategory.list.FirstOrDefault().lastPrice.Value;
            double qty = Math.Round((((coin.equity * 5) / 100).Value) / lastPrice, 3, MidpointRounding.ToEven);

            double leverage = 0;
            //if (data.leverage != null)
            //    parameters.AddOptionalParameter("isLeverage", isLeverage.Value ? 1 : 0);

            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection.Add("symbol", data.symbol);
            nameValueCollection.Add("orderType", OrderType.Market.ToString());
            nameValueCollection.Add("qty", qty.ToString(CultureInfo.InvariantCulture));
            nameValueCollection.Add("side", data.side);
            nameValueCollection.Add("category", "linear");

            var signPayload = nameValueCollection.ToString();

            _sBytes = Encoding.UTF8.GetBytes(secretKey);
            var payload = timestamp + apiKey + _recvWindow.ToString() + signPayload;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload,secretKey);
            var request = new RestRequest("/order/create");

            request.AddParameter("symbol", data.symbol);
            request.AddParameter("orderType", OrderType.Market.ToString());
            request.AddParameter("qty", qty.ToString(CultureInfo.InvariantCulture));
            request.AddParameter("side", data.side);
            request.AddParameter("category", "linear");

            /*if (data.leverage.HasValue)
            {
                request.AddParameter("isLeverage", "1");
            }
            else
            {
                request.AddParameter("isLeverage", "0");
            }*/

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", apiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.PostAsync(request);

            var resultDada = JsonConvert.DeserializeObject<RootOrderReponse>(response.Content.ToString());

            return resultDada;
        }
        public async Task<bool> SetLeverage(SetLeverage setLeverage)
        {
            var timestamp = await GetBybitServerTimeValue();

            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection.Add("symbol", setLeverage.Symbol);
            nameValueCollection.Add("category", setLeverage.Category);
            nameValueCollection.Add("buyLeverage", setLeverage.BuyLeverage);
            nameValueCollection.Add("sellLeverage", setLeverage.SellLeverage);

            var signPayload = nameValueCollection.ToString();
            _sBytes = Encoding.UTF8.GetBytes(setLeverage.SecretKey);
            var payload = timestamp + setLeverage.ApiKey + _recvWindow.ToString() + signPayload;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload, setLeverage.SecretKey);
            var request = new RestRequest("/position/set-leverage");

           request.AddParameter("symbol", setLeverage.Symbol);
           request.AddParameter("category", setLeverage.Category);
           request.AddParameter("buyLeverage", setLeverage.BuyLeverage);
           request.AddParameter("sellLeverage", setLeverage.SellLeverage);

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", setLeverage.ApiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.PostAsync(request);
            return true;
        }


        public async Task<bool> SwitchCrossIsolatedMargin_OBSOLET_FOR_TESLNET(SwitchIsolated switchIsolated)
        {
            

            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection.Add("symbol", switchIsolated.Symbol);
            nameValueCollection.Add("category", switchIsolated.Category);
            nameValueCollection.Add("tradeMode", switchIsolated.TradeMode.ToString());
            nameValueCollection.Add("buyLeverage", switchIsolated.BuyLeverage);
            nameValueCollection.Add("sellLeverage", switchIsolated.SellLeverage);

            var timestamp = await GetBybitServerTimeValue();
            var signPayload = nameValueCollection.ToString();
            _sBytes = Encoding.UTF8.GetBytes(switchIsolated.SecretKey);
            var payload = timestamp + switchIsolated.ApiKey + _recvWindow.ToString() + signPayload;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload, switchIsolated.SecretKey);
            var request = new RestRequest("/position/switch-isolated");

            request.AddParameter("symbol", switchIsolated.Symbol);
            request.AddParameter("category", switchIsolated.Category);
            request.AddParameter("tradeMode", switchIsolated.TradeMode.ToString());
            request.AddParameter("buyLeverage", switchIsolated.BuyLeverage);
            request.AddParameter("sellLeverage", switchIsolated.SellLeverage);

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", switchIsolated.ApiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.PostAsync(request);
            return true;
        }

        public async Task<bool> SwitchCrossIsolatedMargin(SwitchIsolated switchIsolated)
        {
            var payload = new
            {
                symbol = switchIsolated.Symbol,
                category = switchIsolated.Category,
                tradeMode = switchIsolated.TradeMode.ToString(),
                buyLeverage = switchIsolated.BuyLeverage,
                sellLeverage = switchIsolated.SellLeverage
            };

            var timestamp = await GetBybitServerTimeValue();
            var signPayload = JsonConvert.SerializeObject(payload);
            _sBytes = Encoding.UTF8.GetBytes(switchIsolated.SecretKey);
            var sign = await _authenticationByBitService.SignHMACSHA256(timestamp + switchIsolated.ApiKey + _recvWindow + signPayload, switchIsolated.SecretKey);

            var request = new RestRequest("/position/switch-isolated", Method.Post);
            request.AddJsonBody(payload);

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", switchIsolated.ApiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());

            var response = await _restClient.ExecuteAsync(request);
            //if (!response.IsSuccessful)
            //{
            //    // Handle error
            //    throw new ApplicationException($"API error: {response.Content}");
            //}

            return true;
        }


        //does not work
        public async Task<bool> SetTradingStop(SetTradingStopRequest setTradingStop)
        {


            var timestamp = await GetBybitServerTimeValue();

            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection.Add("symbol", setTradingStop.symbol);
            nameValueCollection.Add("category", setTradingStop.category);
            nameValueCollection.Add("positionIdx", setTradingStop.positionIdx.ToString());

            if (setTradingStop.stopLoss.HasValue)
                nameValueCollection.Add("stopLoss", setTradingStop.stopLoss.Value.ToString(CultureInfo.InvariantCulture));


            if (!string.IsNullOrEmpty(setTradingStop.slOrderType))
                nameValueCollection.Add("slOrderType", setTradingStop.slOrderType);


            //if (setTradingStop.slLimitPrice.HasValue)
            //    nameValueCollection.Add("slLimitPrice", setTradingStop.slLimitPrice.Value.ToString(CultureInfo.InvariantCulture));
            // Solo agrega slLimitPrice si el tipo de orden NO es "market"
            if (!string.IsNullOrEmpty(setTradingStop.slOrderType) && setTradingStop.slOrderType.Equals(OrderType.Limit.ToString()) )
            {
                if (setTradingStop.slLimitPrice.HasValue)
                {
                    nameValueCollection.Add("slLimitPrice", setTradingStop.slLimitPrice.Value.ToString(CultureInfo.InvariantCulture));
                }
            }


            if (!string.IsNullOrEmpty(setTradingStop.tpslMode))
                nameValueCollection.Add("tpslMode", setTradingStop.tpslMode);

            if (setTradingStop.slSize!=0)
                nameValueCollection.Add("slSize", setTradingStop.slSize.ToString(CultureInfo.InvariantCulture));



            /*  if (setTradingStop.takeProfit.HasValue)
                  nameValueCollection.Add("takeProfit", setTradingStop.takeProfit.Value.ToString(CultureInfo.InvariantCulture));



              */



            var signPayload = nameValueCollection.ToString();
            _sBytes = Encoding.UTF8.GetBytes(setTradingStop.secretKey);
            var payload = timestamp + setTradingStop.apiKey + _recvWindow.ToString() + signPayload;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload, setTradingStop.secretKey);
            var request = new RestRequest("/position/trading-stop");

            request.AddParameter("symbol", setTradingStop.symbol);
            request.AddParameter("category", setTradingStop.category);
            request.AddParameter("positionIdx", setTradingStop.positionIdx.ToString());

            if (setTradingStop.stopLoss.HasValue)
                request.AddParameter("stopLoss", setTradingStop.stopLoss.Value.ToString(CultureInfo.InvariantCulture));

            if (!string.IsNullOrEmpty(setTradingStop.slOrderType))
               request.AddParameter("slOrderType", setTradingStop.slOrderType);

            //if (setTradingStop.slLimitPrice.HasValue)
            //    request.AddParameter("slLimitPrice", setTradingStop.slLimitPrice.Value.ToString(CultureInfo.InvariantCulture));
            if (!string.IsNullOrEmpty(setTradingStop.slOrderType) && setTradingStop.slOrderType.Equals(OrderType.Limit.ToString()))
            {
                if (setTradingStop.slLimitPrice.HasValue)
                {
                    request.AddParameter("slLimitPrice", setTradingStop.slLimitPrice.Value.ToString(CultureInfo.InvariantCulture));
                }
            }



            if (!string.IsNullOrEmpty(setTradingStop.tpslMode))
                request.AddParameter("tpslMode", setTradingStop.tpslMode);


            if (setTradingStop.slSize != 0)
                request.AddParameter("slSize", setTradingStop.slSize.ToString(CultureInfo.InvariantCulture));

            /*  if (setTradingStop.takeProfit.HasValue)
                  request.AddParameter("takeProfit", setTradingStop.takeProfit.Value.ToString(CultureInfo.InvariantCulture));

             

              */



            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", setTradingStop.apiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.PostAsync(request);
          
            return true;
        }

        public async Task<GetPositionInfoResult> GetPositionInfo(GetPositionInfo getPositionInfo)
        {
            var timestamp = await GetBybitServerTimeValue();

            _sBytes = Encoding.UTF8.GetBytes(getPositionInfo.secretKey);
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection.Add("category", getPositionInfo.category);
            //nameValueCollection.Add("positionIdx", "0");
            if ( !string.IsNullOrEmpty(getPositionInfo.symbol) )
            nameValueCollection.Add("symbol", getPositionInfo.symbol);


            var signPayload = nameValueCollection.ToString();

            var payload = timestamp + getPositionInfo.apiKey + _recvWindow.ToString() + signPayload;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload, getPositionInfo.secretKey);

            var request = new RestRequest("/position/list", Method.Get);
            request.AddParameter("category", getPositionInfo.category);
           // request.AddParameter("positionIdx","0");
            
            if (!string.IsNullOrEmpty(getPositionInfo.symbol))
                request.AddParameter("symbol", getPositionInfo.symbol);

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", getPositionInfo.apiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.GetAsync(request);

            var resultDada = JsonConvert.DeserializeObject<GetPositionInfoResultRoot>(response.Content.ToString());
            return resultDada.result;
        }

        public async Task<GetPositionInfoResult> GetTradeHistory(GetTradeHistory getTradeHistoryRequest)
        {
            var timestamp = await GetBybitServerTimeValue();

            _sBytes = Encoding.UTF8.GetBytes(getTradeHistoryRequest.secretKey);
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection.Add("category", getTradeHistoryRequest.category);

            if (!string.IsNullOrEmpty(getTradeHistoryRequest.symbol))
                nameValueCollection.Add("symbol", getTradeHistoryRequest.symbol);

            if (getTradeHistoryRequest.startTime>0)
                nameValueCollection.Add("startTime", getTradeHistoryRequest.startTime.ToString());

            if (getTradeHistoryRequest.endTime > 0)
                nameValueCollection.Add("endTime", getTradeHistoryRequest.endTime.ToString());


            var signPayload = nameValueCollection.ToString();

            var payload = timestamp + getTradeHistoryRequest.apiKey + _recvWindow.ToString() + signPayload;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload, getTradeHistoryRequest.secretKey);

            var request = new RestRequest("/execution/list", Method.Get);
            request.AddParameter("category", getTradeHistoryRequest.category);
            // request.AddParameter("positionIdx","0");

            if (!string.IsNullOrEmpty(getTradeHistoryRequest.symbol))
                request.AddParameter("symbol", getTradeHistoryRequest.symbol);

            if (getTradeHistoryRequest.startTime>0)
                request.AddParameter("startTime", getTradeHistoryRequest.startTime);

            if (getTradeHistoryRequest.endTime > 0)
                request.AddParameter("endTime", getTradeHistoryRequest.endTime);

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", getTradeHistoryRequest.apiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.GetAsync(request);

            var resultDada = JsonConvert.DeserializeObject<GetPositionInfoResultRoot>(response.Content.ToString());
            return resultDada.result;
        }

        public async Task<GetInstrumentsInfoRoot> GetInstrumentsInfo(GetInstrumentsInfoQuery getInstrumentsInfo)
        {
            var timestamp = await GetBybitServerTimeValue();

            _sBytes = Encoding.UTF8.GetBytes(getInstrumentsInfo.secretKey);
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection.Add("category", getInstrumentsInfo.category);
            if (!string.IsNullOrEmpty(getInstrumentsInfo.symbol))
                nameValueCollection.Add("symbol", getInstrumentsInfo.symbol);


            var signPayload = nameValueCollection.ToString();

            var payload = timestamp + getInstrumentsInfo.apiKey + _recvWindow.ToString() + signPayload;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload, getInstrumentsInfo.secretKey);
     
            var request = new RestRequest("/market/instruments-info", Method.Get);
            request.AddParameter("category", getInstrumentsInfo.category);
           
            if (!string.IsNullOrEmpty(getInstrumentsInfo.symbol))
                request.AddParameter("symbol", getInstrumentsInfo.symbol);

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", getInstrumentsInfo.apiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.GetAsync(request);

            var resultDada = JsonConvert.DeserializeObject<GetInstrumentsInfoRoot>(response.Content.ToString());
            return resultDada;
        }

        /// <summary>
        /// Query unfilled or partially filled orders in real-time
        /// </summary>
        /// <param name="getOpenOrders"></param>
        /// <returns></returns>
        public async Task<GetOpenOrdersResult> GetOpenOrders(GetOpenOrders getOpenOrders)
        {

            var timestamp = await GetBybitServerTimeValue();

            _sBytes = Encoding.UTF8.GetBytes(getOpenOrders.secretKey);
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection.Add("category", getOpenOrders.category);
            if (!string.IsNullOrEmpty(getOpenOrders.symbol))
                nameValueCollection.Add("symbol", getOpenOrders.symbol);


            var signPayload = nameValueCollection.ToString();

            var payload = timestamp + getOpenOrders.apiKey + _recvWindow.ToString() + signPayload;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload, getOpenOrders.secretKey);

            var request = new RestRequest("/order/realtime", Method.Get);
            request.AddParameter("category", getOpenOrders.category);

            if (!string.IsNullOrEmpty(getOpenOrders.symbol))
                request.AddParameter("symbol", getOpenOrders.symbol);

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", getOpenOrders.apiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.GetAsync(request);

            var resultDada = JsonConvert.DeserializeObject<GetOpenOrdersRoot>(response.Content.ToString());
            return resultDada.result;
        }

        public async Task<bool> CancelOrder(GetOpenOrdersList getOpenOrdersList, string ApiKey, string SecretKey, string category = "inverse")
        {
            var timestamp = await GetBybitServerTimeValue();

            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection.Add("symbol", getOpenOrdersList.symbol);
            nameValueCollection.Add("category", category);
            nameValueCollection.Add("orderId", getOpenOrdersList.orderId);

            var signPayload = nameValueCollection.ToString();
            _sBytes = Encoding.UTF8.GetBytes(SecretKey);
            var payload = timestamp + ApiKey + _recvWindow.ToString() + signPayload;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload, SecretKey);
            var request = new RestRequest("/order/cancel");

            request.AddParameter("symbol", getOpenOrdersList.symbol);
            request.AddParameter("category", category);
            request.AddParameter("orderId", getOpenOrdersList.orderId);

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", ApiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.PostAsync(request);
            return true;
        }


        public async Task<GetFeeRateReponseRoot> GetFeeRate(GetFeeRateQuery getFeeRate)
        {
            var timestamp = await GetBybitServerTimeValue();

            _sBytes = Encoding.UTF8.GetBytes(getFeeRate.secretKey);
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection.Add("category", getFeeRate.category);
            if (!string.IsNullOrEmpty(getFeeRate.symbol))
                nameValueCollection.Add("symbol", getFeeRate.symbol);
            if (!string.IsNullOrEmpty(getFeeRate.baseCoin))
                nameValueCollection.Add("baseCoin", getFeeRate.baseCoin);


            var signPayload = nameValueCollection.ToString();

            var payload = timestamp + getFeeRate.apiKey + _recvWindow.ToString() + signPayload;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload, getFeeRate.secretKey);

            var request = new RestRequest("/account/fee-rate", Method.Get);
            request.AddParameter("category", getFeeRate.category);

            if (!string.IsNullOrEmpty(getFeeRate.symbol))
                request.AddParameter("symbol", getFeeRate.symbol);
            if (!string.IsNullOrEmpty(getFeeRate.baseCoin))
                request.AddParameter("baseCoin", getFeeRate.baseCoin);

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", getFeeRate.apiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.GetAsync(request);

            var resultDada = JsonConvert.DeserializeObject<GetFeeRateReponseRoot>(response.Content.ToString());
            return resultDada;
        }


        /// <summary>
        /// Query user's closed profit and loss records. The results are sorted by createdTime in descending order.
        /// </summary>
        /// <param name="getFeeRate"></param>
        /// <returns></returns>
        public async Task<GetClosedPnLReponseRoot> GetClosedPnL(GetClosedPnL getFeeRate)
        {
            var timestamp = await GetBybitServerTimeValue();

            _sBytes = Encoding.UTF8.GetBytes(getFeeRate.secretKey);
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection.Add("category", getFeeRate.category);
            if (!string.IsNullOrEmpty(getFeeRate.symbol))
                nameValueCollection.Add("symbol", getFeeRate.symbol);
   
            var signPayload = nameValueCollection.ToString();

            var payload = timestamp + getFeeRate.apiKey + _recvWindow.ToString() + signPayload;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload, getFeeRate.secretKey);

            var request = new RestRequest("/position/closed-pnl", Method.Get);
            request.AddParameter("category", getFeeRate.category);

            if (!string.IsNullOrEmpty(getFeeRate.symbol))
                request.AddParameter("symbol", getFeeRate.symbol);
           

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", getFeeRate.apiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.GetAsync(request);

            var resultDada = JsonConvert.DeserializeObject<GetClosedPnLReponseRoot>(response.Content.ToString());
            return resultDada;
        }

        public async Task<GetPositionInfoResult> GetAllCoinsBalance(GetAllCoinsBalance getPositionInfo)
        {
           

            _sBytes = Encoding.UTF8.GetBytes(getPositionInfo.secretKey);
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection.Add("accountType", getPositionInfo.accountType);


            var timestamp = await GetBybitServerTimeValue();
            var signPayload = nameValueCollection.ToString();

            var payload = timestamp + getPositionInfo.apiKey + _recvWindow.ToString() + signPayload;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload, getPositionInfo.secretKey);

            var request = new RestRequest("/asset/transfer/query-account-coins-balance", Method.Get);
            request.AddParameter("accountType", getPositionInfo.accountType);

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", getPositionInfo.apiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.GetAsync(request);

            var resultDada = JsonConvert.DeserializeObject<GetPositionInfoResultRoot>(response.Content.ToString());
            return resultDada.result;
        }

        public async Task<bool> SetDepositAccount(string ApiKey,string SecretKey, string accountType = "UNIFIED")
        {
            var timestamp = await GetBybitServerTimeValue();

            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection.Add("accountType", accountType);
           

            var signPayload = nameValueCollection.ToString();
            _sBytes = Encoding.UTF8.GetBytes(SecretKey);
            var payload = timestamp + ApiKey + _recvWindow.ToString() + signPayload;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload, SecretKey);
            var request = new RestRequest("/asset/deposit/deposit-to-account");
          

            request.AddParameter("accountType", accountType);
           

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", ApiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.PostAsync(request);
            return true;
        }
        
        public async Task<bool> GetCoinInfo(string ApiKey, string SecretKey)
        {
            var timestamp = await GetBybitServerTimeValue();
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
           
            var signPayload = nameValueCollection.ToString();
            _sBytes = Encoding.UTF8.GetBytes(SecretKey);
            var payload = timestamp + ApiKey + _recvWindow.ToString() ;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload, SecretKey);
            var request = new RestRequest("/asset/coin/query-info");

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", ApiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.GetAsync(request);
            return true;
        }
    }
}
