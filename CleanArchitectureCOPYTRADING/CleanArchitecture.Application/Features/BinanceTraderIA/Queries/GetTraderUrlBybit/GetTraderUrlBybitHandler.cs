using System.Globalization;
using System.Reflection;
using AutoMapper;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using MediatR;
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Features.Binance.Queries.GetTraderUrlBybit
{
    public class GetTraderUrlBybitHandler : IRequestHandler<GetTraderUrlBybitQuery, List<ByBitGuidKeyResult> >
    {
        private readonly IBinanceTraderTypeDataService _binanceTraderTypeDataService;
        private readonly IBinanceTraderService _binanceTraderService;

        public GetTraderUrlBybitHandler(IBinanceTraderTypeDataService binanceTraderTypeDataService,
            IBinanceTraderService binanceTraderService)
        {

            _binanceTraderTypeDataService = binanceTraderTypeDataService ?? throw new ArgumentException(nameof(binanceTraderTypeDataService));
            _binanceTraderService = binanceTraderService ?? throw new ArgumentException(nameof(binanceTraderService));
        }

        public async  Task<List<ByBitGuidKeyResult> > Handle(GetTraderUrlBybitQuery request, CancellationToken cancellationToken)
        {
          
            var dataBinanceTrader =await _binanceTraderService.GetAll();
            var dataDicBinanceTrader = dataBinanceTrader.GroupBy(p=>p.EncryptedUid).ToDictionary(p => p.Key, p => p.FirstOrDefault());

       
            List<ByBitGuidKeyResult> resultData = new List<ByBitGuidKeyResult>();

            resultData.Add(new ByBitGuidKeyResult()
            {
                guidByBit = "1177F0D8-7092-40F0-A26F-B9184641B9BB",
                urlApiKeyByBit = "https://api2.bybit.com/fapi/beehive/public/v1/common/position/list?leaderMark=Vo4kz1H2YQ0hVKKYyl1/Pg=="
            });
           return resultData;
        }
    }
}
