using System.Globalization;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Models;
using Microsoft.Extensions.Logging;
using RestSharp;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Infrastructure.Services
{

    public class ByBitApiV3Service : IByBitApiV3Service
    {
        //*********copiado de bybit.net**************
        protected byte[] _sBytes;
        //**********************
        private readonly HttpClient _httpClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly int _recvWindow = 5000;

        private readonly IAuthenticationByBitService _authenticationByBitService;
        private readonly IByBitApiV3Repository _byBitApiRepository;


        private IRestClient _restClient;

        public ILogger<ByBitApiV3Service> _logger { get; }

        public ByBitApiV3Service() { }

        public ByBitApiV3Service(ILogger<ByBitApiV3Service> logger, HttpClient httpClient, IAuthenticationByBitService authenticationByBitService
            , IByBitApiV3Repository byBitApiRepository)
        {
            _restClient = new RestClient(httpClient);
            _logger = logger;
            _httpClient = httpClient ?? throw new ArgumentException(nameof(httpClient));
            _byBitApiRepository = byBitApiRepository ?? throw new ArgumentException(nameof(byBitApiRepository));


            //_sBytes = Encoding.UTF8.GetBytes("ltEHhA91lPHnPTUpMYiNb15AVFjvkLLXV1mc");
            _authenticationByBitService = authenticationByBitService ?? throw new ArgumentException(nameof(authenticationByBitService));
            _httpClient.DefaultRequestHeaders.Add("X-BAPI-RECV-WINDOW", _recvWindow.ToString());


        }

        public async Task<GetPositionInfoV3Root> GetPositionInfo(GetPositionInfoV3Query getPositionInfoV3Query)
        {
            return await _byBitApiRepository.GetPositionInfo(getPositionInfoV3Query);
        }

       
    }
}
