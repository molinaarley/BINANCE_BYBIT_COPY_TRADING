using System.Collections.Specialized;
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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class ByBitApiV3Repository : IByBitApiV3Repository
    {
        //*********copiado de bybit.net**************
        protected byte[] _sBytes;
        //**********************
        private readonly HttpClient _httpClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly int _recvWindow = 5000;
        private readonly IAuthenticationByBitService _authenticationByBitService;
        private IRestClient _restClient;

        public ILogger<ByBitApiV3Repository> _logger { get; }
        public ByBitApiV3Repository(ILogger<ByBitApiV3Repository> logger, HttpClient httpClient, IAuthenticationByBitService authenticationByBitService) 
        {

            _restClient = new RestClient(httpClient);
            _logger = logger;
            _httpClient = httpClient ?? throw new ArgumentException(nameof(httpClient));
            _authenticationByBitService = authenticationByBitService ?? throw new ArgumentException(nameof(authenticationByBitService));
            //_httpClient.DefaultRequestHeaders.Add("X-BAPI-RECV-WINDOW", _recvWindow.ToString());

        }

        public async Task<GetPositionInfoV3Root> GetPositionInfo(GetPositionInfoV3Query getPositionInfo)
        {
            var timestamp = await GetBybitServerTimeValue();

            _sBytes = Encoding.UTF8.GetBytes(getPositionInfo.secretKey);
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
           
            if (!string.IsNullOrEmpty(getPositionInfo.symbol))
                nameValueCollection.Add("symbol", getPositionInfo.symbol);
            if (!string.IsNullOrEmpty(getPositionInfo.settleCoin))
                nameValueCollection.Add("settleCoin", getPositionInfo.settleCoin);


            var signPayload = nameValueCollection.ToString();

            var payload = timestamp + getPositionInfo.apiKey + _recvWindow.ToString() + signPayload;
            string sign = await _authenticationByBitService.SignHMACSHA256(payload, getPositionInfo.secretKey);

            var request = new RestRequest("/contract/v3/private/position/list", Method.Get);
          

            if (!string.IsNullOrEmpty(getPositionInfo.symbol))
                request.AddParameter("symbol", getPositionInfo.symbol);

            if (!string.IsNullOrEmpty(getPositionInfo.settleCoin))
                request.AddParameter("settleCoin", getPositionInfo.settleCoin);

            _restClient = new RestClient(_httpClient);
            _restClient.AddDefaultHeader("X-BAPI-API-KEY", getPositionInfo.apiKey);
            _restClient.AddDefaultHeader("X-BAPI-SIGN", sign);
            _restClient.AddDefaultHeader("X-BAPI-SIGN-TYPE", "2");
            _restClient.AddDefaultHeader("X-BAPI-TIMESTAMP", timestamp);
            _restClient.AddDefaultHeader("X-BAPI-RECV-WINDOW", _recvWindow.ToString());
            var response = await _restClient.GetAsync(request);

            var resultDada = JsonConvert.DeserializeObject<GetPositionInfoV3Root>(response.Content.ToString());
            return resultDada;
        }

        public async Task<string> GetBybitServerTimeValue()
        {
            var resultDada = await GetBybitServerTime();
            return resultDada.time.ToString(CultureInfo.InvariantCulture);
        }

        public async Task<GetBybitServerTimeRoot> GetBybitServerTime()
        {
            var request = new RestRequest("/v3/public/time", Method.Get);

            _restClient = new RestClient(_httpClient);
            var response = await _restClient.GetAsync(request);
            var resultDada = JsonConvert.DeserializeObject<GetBybitServerTimeRoot>(response.Content.ToString());
            return resultDada;
        }
    }
}
