using System.Globalization;
using System.Linq;
using System.Reflection;
using AutoMapper;
using CleanArchitecture.Application.Constants;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Features.Binance.Queries.GetTraderUrlForUpdatePositionBinance;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.ML;
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Features.Binance.Queries.GetTraderUrlForUpdatePositionBinanceInfo
{

   
    public class GetTraderUrlForUpdatePositionBinanceInfoHandler : IRequestHandler<GetTraderUrlForUpdatePositionBinanceInfoQuery, List<TraderUrlForUpdatePositionBinanceInfoResult>>
    {
        private readonly IBinanceTraderPerformanceService _loadTraderPerformanceService;
        private readonly IBinanceTraderService _binanceTraderService;
        private readonly PredictionEnginePool<TraderDataPerformanceBinance, ModelTraderDataOutput> _traderPredictionEngine;
        public IConfiguration _configuration { get; }
        public GetTraderUrlForUpdatePositionBinanceInfoHandler(IBinanceTraderPerformanceService loadTraderPerformanceService,IBinanceTraderService binanceTraderService,
            PredictionEnginePool<TraderDataPerformanceBinance, ModelTraderDataOutput> traderPredictionEngine, IConfiguration configuration)
        {
            _loadTraderPerformanceService = loadTraderPerformanceService ?? throw new ArgumentException(nameof(loadTraderPerformanceService));
            _binanceTraderService = binanceTraderService ?? throw new ArgumentException(nameof(binanceTraderService));
            _traderPredictionEngine = traderPredictionEngine ?? throw new ArgumentException(nameof(traderPredictionEngine));
            _configuration = configuration ?? throw new ArgumentException(nameof(configuration));


        }

        public async Task<List<TraderUrlForUpdatePositionBinanceInfoResult>> Handle(GetTraderUrlForUpdatePositionBinanceInfoQuery request, CancellationToken cancellationToken)
        {


            List<string> tesmp = (await _loadTraderPerformanceService.TraderUrlForUpdatePositionBinanceWithCompositeScore());
            tesmp.Insert(0, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff"));
            File.AppendAllLines("TraderUrlForUpdatePositionBinanceWithCompositeScoreBacktesting.txt", tesmp);


            List<string> listUrl = new List<string>();
            Dictionary<string, BinanceTrader> dicBinanceTrader = (await _binanceTraderService.GetAll()).GroupBy(x => x.EncryptedUid).ToDictionary(p => p.Key, p => p.FirstOrDefault());

            var data = (await _loadTraderPerformanceService.GetAllTraderDataPerformanceBinanceForModel(dicBinanceTrader)).OrderByDescending(p => p.IAIsTopTraderScore).ToList();
            //var dataAudit =( await _loadTraderPerformanceService.GetAllTraderDataPerformanceBinanceForModelAudit(dicBinanceTrader)).Where(p => !NotIncludeGuidTrader.NotIncludeGuid.ContainsKey(p.EncryptedUid)).OrderByDescending(p => p.IAIsTopTraderScore).ToList();


            int minFollowerCount = int.Parse(_configuration.GetSection("BinanceBybitSettings:MinFollowerCount").Value);
            var allIAIsTopTraderScoreTraderDataPerformanceBinanceForModel = data.Where(p => p.FollowerCount > minFollowerCount).OrderByDescending(p => p.IAIsTopTraderScore).GroupBy(p => p.EncryptedUid).Select(p => new { p.FirstOrDefault().EncryptedUid, p.OrderByDescending(p => p.IAIsTopTraderScore).FirstOrDefault().IAIsTopTraderScore }).Distinct().ToList();
            var dicByEncryptedUidInStock = allIAIsTopTraderScoreTraderDataPerformanceBinanceForModel.GroupBy(p => p.EncryptedUid).ToDictionary(p => p.Key, p => p.FirstOrDefault().EncryptedUid);

            //hystorique
            //var allIAIsTopTraderScoreTraderDataPerformanceBinanceForModelHystorique = dataAudit.Where(p => p.FollowerCount > minFollowerCount && dicByEncryptedUidInStock.ContainsKey(p.EncryptedUid)/* DateTime.Parse(p.CreatedOn) > DateTime.Now.AddDays(-20)*/ ).OrderByDescending(p => p.IAIsTopTraderScore).
            //    Select(p => new { p.EncryptedUid, p.IAIsTopTraderScore }).Distinct().ToList();

            //allIAIsTopTraderScoreTraderDataPerformanceBinanceForModel.AddRange(allIAIsTopTraderScoreTraderDataPerformanceBinanceForModelHystorique);
            allIAIsTopTraderScoreTraderDataPerformanceBinanceForModel = allIAIsTopTraderScoreTraderDataPerformanceBinanceForModel.GroupBy(p => p.EncryptedUid).
                Select(p => new { p.FirstOrDefault().EncryptedUid, p.OrderByDescending(p => p.IAIsTopTraderScore).FirstOrDefault().IAIsTopTraderScore }).OrderByDescending(p => p.IAIsTopTraderScore).ToList();




            // var allIsDailY_ROI_IncreasingTraderDataPerformanceBinanceForModel = data.Where(p=>p.FollowerCount>minFollowerCount).OrderByDescending(p => p.IsDailY_ROI_Increasing).GroupBy(p => p.EncryptedUid).Select(p => new { p.FirstOrDefault().EncryptedUid, p.OrderByDescending(p=>p.IsDailY_ROI_Increasing).FirstOrDefault().IsDailY_ROI_Increasing }).Distinct().ToList();

            //  var allIsDailY_ROI_IncreasingTraderDataPerformanceBinanceForModelHystorique = dataAudit.Where(p => p.FollowerCount > minFollowerCount).Where(p => //DateTime.Parse(p.CreatedOn) > DateTime.Now.AddDays(-20)
            // dicByEncryptedUidInStock.ContainsKey(p.EncryptedUid) ).OrderByDescending(p => p.IsDailY_ROI_Increasing).
            //     Select(p => new { p.EncryptedUid, p.IsDailY_ROI_Increasing }).Distinct().ToList();

            // allIsDailY_ROI_IncreasingTraderDataPerformanceBinanceForModel.AddRange(allIsDailY_ROI_IncreasingTraderDataPerformanceBinanceForModelHystorique);
            //var allIsIsMonthlY_ROI_IncreasingTraderDataPerformanceBinanceForModel = data.OrderByDescending(p => p.IsMonthlY_ROI_Increasing).ToList();
           

            listUrl = allIAIsTopTraderScoreTraderDataPerformanceBinanceForModel.Where(p => !NotIncludeGuidTrader.NotIncludeGuid.ContainsKey(p.EncryptedUid)).Select(p => p.EncryptedUid).Distinct().ToList();//Take( 50/*int.Parse(_configuration.GetSection("BinanceBybitSettings:TotalTraderUrlForUpdatePositionBinanceQuery").Value) */).

            // el IsDailY_ROI_Increasing no tuvo buenos resultados
            //  listUrl.AddRange(allIsDailY_ROI_IncreasingTraderDataPerformanceBinanceForModel.OrderByDescending(p => p.IsDailY_ROI_Increasing).Take(2).Select(p => p.EncryptedUid).ToList());
            // return listUrl.Distinct().ToList();

            var dataPerformanceBinanceByEncryptedUid =  data.GroupBy(x => x.EncryptedUid).ToDictionary(p => p.Key, p => p.FirstOrDefault());
            

            return listUrl.Distinct().Where(p=> dicBinanceTrader.ContainsKey(p)).Select(p=> new TraderUrlForUpdatePositionBinanceInfoResult()
            {
                CreatedOn = dicBinanceTrader[p].CreatedOn.Value,
                EncryptedUid = dicBinanceTrader[p].EncryptedUid,
                FollowerCount = dicBinanceTrader[p].FollowerCount.Value,
                NickName =string.Concat( dicBinanceTrader[p].NickName," | ", dicBinanceTrader[p].EncryptedUid," | ", dataPerformanceBinanceByEncryptedUid.ContainsKey(dicBinanceTrader[p].EncryptedUid) ? dataPerformanceBinanceByEncryptedUid[dicBinanceTrader[p].EncryptedUid].IAIsTopTraderScore : string.Empty),
                RankTrader = dicBinanceTrader[p].RankTrader.Value,
                PositionShared = dicBinanceTrader[p].PositionShared
            } ).ToList();
        }
    }

   
}
