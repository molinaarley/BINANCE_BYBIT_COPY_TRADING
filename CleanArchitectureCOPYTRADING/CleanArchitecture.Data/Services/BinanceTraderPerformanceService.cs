using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Models;
using Microsoft.Extensions.Logging;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Binance;
using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;
using CleanArchitecture.Application.Extensions;
using CleanArchitecture.Application.Constants;

namespace CleanArchitecture.Infrastructure.Services
{

    public class BinanceTraderPerformanceService : IBinanceTraderPerformanceService
    {
        private readonly IBinanceTraderService _binanceTraderService;
        private readonly IBinanceTraderPerformanceRetListRepository _binanceTraderPerformanceRetListRepository;
        private readonly IBinanceTraderPerformanceRetListRepositoryAudit _binanceTraderPerformanceRetListRepositoryAudit;
        public ILogger<BinanceTraderPerformanceService> _logger { get; }
        public IConfiguration _configuration { get; }
        private readonly MLContext _mlContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly PredictionEnginePool<TraderDataPerformanceBinance, TraderDataPerformanceBinanceIsDailYROIIncreasingPrediction> _predictionIsDailYROIIncreasingEnginePool;
        private readonly PredictionEnginePool<TraderDataPerformanceBinance, TraderDataPerformanceBinanceIsMonthlY_ROI_IncreasingPrediction> _predictionIsMonthlYROIIncreasingEnginePool;
        private readonly PredictionEnginePool<TraderDataPerformanceBinance, TraderPerformancePrediction> _predictionIsTopTraderScoreEnginePool;


        

        public BinanceTraderPerformanceService() { }

        public BinanceTraderPerformanceService(PredictionEnginePool<TraderDataPerformanceBinance, TraderPerformancePrediction> predictionIsTopTraderScoreEnginePool,
            PredictionEnginePool<TraderDataPerformanceBinance, TraderDataPerformanceBinanceIsMonthlY_ROI_IncreasingPrediction> predictionIsMonthlYROIIncreasingEnginePool,
            PredictionEnginePool<TraderDataPerformanceBinance, TraderDataPerformanceBinanceIsDailYROIIncreasingPrediction> predictionIsDailYROIIncreasingEnginePool,
            IServiceProvider serviceProvider, ILogger<BinanceTraderPerformanceService> logger,
            IBinanceTraderPerformanceRetListRepository binanceTraderPerformanceRetListRepository,
             IBinanceTraderPerformanceRetListRepositoryAudit binanceTraderPerformanceRetListRepositoryAudit,
            IBinanceTraderService binanceTraderService, IConfiguration configuration,
             MLContext mlContext)
        {
            _predictionIsTopTraderScoreEnginePool = predictionIsTopTraderScoreEnginePool ?? throw new ArgumentException(nameof(predictionIsTopTraderScoreEnginePool));

            _predictionIsMonthlYROIIncreasingEnginePool = predictionIsMonthlYROIIncreasingEnginePool ?? throw new ArgumentException(nameof(predictionIsMonthlYROIIncreasingEnginePool));
            _predictionIsDailYROIIncreasingEnginePool = predictionIsDailYROIIncreasingEnginePool ?? throw new ArgumentException(nameof(predictionIsDailYROIIncreasingEnginePool));

            _serviceProvider = serviceProvider ?? throw new ArgumentException(nameof(serviceProvider));
            _logger = logger;
            _binanceTraderPerformanceRetListRepository = binanceTraderPerformanceRetListRepository ?? throw new ArgumentException(nameof(binanceTraderPerformanceRetListRepository));
            _binanceTraderPerformanceRetListRepositoryAudit = binanceTraderPerformanceRetListRepositoryAudit ?? throw new ArgumentException(nameof(binanceTraderPerformanceRetListRepositoryAudit));
            _binanceTraderService = binanceTraderService ?? throw new ArgumentException(nameof(binanceTraderService));
            _configuration = configuration;
            _mlContext = mlContext ?? throw new ArgumentException(nameof(mlContext));
        }

        public async Task<string> Create(BinanceTraderPerformanceRetList binanceTraderPerformanceRetList)
        {
            return await _binanceTraderPerformanceRetListRepository.Create(binanceTraderPerformanceRetList);
        }
        public async Task<Dictionary<string, string>> GetAllDictionary()
        {
            var result = await _binanceTraderPerformanceRetListRepository.GetAllDictionary();
            return result;
        }
        public async Task<Dictionary<string,List<BinanceTraderPerformanceRetList>>> GetAllDictionaryByEncryptedUid()
        {
            var result = (await _binanceTraderPerformanceRetListRepository.GetAll()).GroupBy(x => x.EncryptedUid).ToDictionary(p => p.Key, p => p.ToList());
            return result;
        }


        private async Task<Dictionary<string, List<BinanceTraderPerformanceRetListAudit>>> GetAllDictionaryByEncryptedUidAudit()
        {
            var result = (await _binanceTraderPerformanceRetListRepositoryAudit.GetAll()).GroupBy(x => x.EncryptedUid).ToDictionary(p => p.Key, p => p.ToList());
            return result;
        }

        public async Task<List<TraderDataPerformanceBinance>> GetAllTraderDataPerformanceBinanceForModelAudit(Dictionary<string, BinanceTrader> dicBinanceTrader)
        {

            List<TraderDataPerformanceBinance> result = new List<TraderDataPerformanceBinance>();

            

            var dictionaryByEncryptedUid = await GetAllDictionaryByEncryptedUidAudit();

            foreach (var item in dictionaryByEncryptedUid.Values)
            {
                var groupByDate=  item.GroupBy(x => x.CreatedOn.Value.Date).ToDictionary(p => p.Key, p => p.ToList());
              
                foreach (var itemBinanceTraderPerformanceRetList in groupByDate)
                {
                    TraderDataPerformanceBinance newItem = new TraderDataPerformanceBinance();

                    foreach ( var itemValue in itemBinanceTraderPerformanceRetList.Value)
                    {

                        if (dicBinanceTrader.ContainsKey(itemValue.EncryptedUid))
                        {
                            newItem.RankTrader = dicBinanceTrader[itemValue.EncryptedUid].RankTrader.Value;
                            newItem.FollowerCount = dicBinanceTrader[itemValue.EncryptedUid].FollowerCount.Value;
                            newItem.EncryptedUid = itemValue.EncryptedUid;
                            newItem.CreatedOn = itemValue.CreatedOn.HasValue ? itemValue.CreatedOn.Value.ToString() : DateTime.Now.ToString();
                        }
                        switch (string.Concat(itemValue.PeriodType, "_", itemValue.StatisticsType))
                        {
                            case "DAILY_ROI":
                                // code block
                                newItem.DailY_ROI = Convert.ToSingle(itemValue.Value.Value * 100);
                                break;
                            case "DAILY_PNL":
                                newItem.DailY_PNL = Convert.ToSingle(itemValue.Value.Value);
                                break;
                            case "EXACT_YEARLY_ROI":
                                newItem.ExacT_YEARLY_ROI = Convert.ToSingle(itemValue.Value.Value * 100);
                                break;
                            case "EXACT_YEARLY_PNL":
                                newItem.ExacT_YEARLY_PNL = Convert.ToSingle(itemValue.Value.Value);
                                break;
                            case "WEEKLY_ROI":
                                newItem.WeeklY_ROI = Convert.ToSingle(itemValue.Value.Value * 100);
                                break;
                            case "WEEKLY_PNL":
                                newItem.WeeklY_PNL = Convert.ToSingle(itemValue.Value.Value);
                                break;
                            case "MONTHLY_ROI":
                                newItem.MonthlY_ROI = Convert.ToSingle(itemValue.Value.Value * 100);
                                break;
                            case "MONTHLY_PNL":
                                newItem.MonthlY_PNL = Convert.ToSingle(itemValue.Value.Value);
                                break;
                            case "YEARLY_ROI":
                                newItem.YearlY_ROI = Convert.ToSingle(itemValue.Value.Value * 100);
                                break;
                            case "YEARLY_PNL":
                                newItem.YearlY_PNL = Convert.ToSingle(itemValue.Value.Value);
                                break;
                            case "ALL_ROI":
                                newItem.AlL_ROI = Convert.ToSingle(itemValue.Value.Value * 100);
                                break;
                            case "ALL_PNL":
                                newItem.AlL_PNL = Convert.ToSingle(itemValue.Value.Value);
                                break;
                            default:
                                break;
                        }
                    }
                    newItem.IsTopTraderScore = Convert.ToSingle(CalculateIsTopTraderScore(newItem));
                    newItem.IAIsTopTraderScore = PredictIsTopTraderScore(newItem).PerformanceScore;
                    newItem.IsDailY_ROI_Increasing = PredictIsDailYROIIncreasingIncreasing(newItem);


                    newItem.IsMonthlY_ROI_Increasing = PredictBinanceIsMonthlYROIIncreasing(newItem);
                    newItem.IsYearlY_ROI_Increasing = PredictBinanceIsYearlYROIIncreasing(newItem);




                    result.Add(newItem);
                }
            }
            return result;
        }



        public async Task<List<TraderDataPerformanceBinance>> GetAllTraderDataPerformanceBinanceForModelAuditDataInit(Dictionary<string, BinanceTrader> dicBinanceTrader)
        {

            List<TraderDataPerformanceBinance> result = new List<TraderDataPerformanceBinance>();



            var dictionaryByEncryptedUid = await GetAllDictionaryByEncryptedUidAudit();

            foreach (var item in dictionaryByEncryptedUid.Values)
            {
                var groupByDate = item.GroupBy(x => x.CreatedOn.Value.Date).ToDictionary(p => p.Key, p => p.ToList());

                foreach (var itemBinanceTraderPerformanceRetList in groupByDate)
                {
                    TraderDataPerformanceBinance newItem = new TraderDataPerformanceBinance();

                    foreach (var itemValue in itemBinanceTraderPerformanceRetList.Value)
                    {

                        if (dicBinanceTrader.ContainsKey(itemValue.EncryptedUid))
                        {
                            newItem.RankTrader = dicBinanceTrader[itemValue.EncryptedUid].RankTrader.Value;
                            newItem.FollowerCount = dicBinanceTrader[itemValue.EncryptedUid].FollowerCount.Value;
                            newItem.EncryptedUid = itemValue.EncryptedUid;
                            newItem.CreatedOn = itemValue.CreatedOn.HasValue ? itemValue.CreatedOn.Value.ToString() : DateTime.Now.ToString();
                        }
                        switch (string.Concat(itemValue.PeriodType, "_", itemValue.StatisticsType))
                        {
                            case "DAILY_ROI":
                                // code block
                                newItem.DailY_ROI = Convert.ToSingle(itemValue.Value.Value * 100);
                                break;
                            case "DAILY_PNL":
                                newItem.DailY_PNL = Convert.ToSingle(itemValue.Value.Value);
                                break;
                            case "EXACT_YEARLY_ROI":
                                newItem.ExacT_YEARLY_ROI = Convert.ToSingle(itemValue.Value.Value * 100);
                                break;
                            case "EXACT_YEARLY_PNL":
                                newItem.ExacT_YEARLY_PNL = Convert.ToSingle(itemValue.Value.Value);
                                break;
                            case "WEEKLY_ROI":
                                newItem.WeeklY_ROI = Convert.ToSingle(itemValue.Value.Value * 100);
                                break;
                            case "WEEKLY_PNL":
                                newItem.WeeklY_PNL = Convert.ToSingle(itemValue.Value.Value);
                                break;
                            case "MONTHLY_ROI":
                                newItem.MonthlY_ROI = Convert.ToSingle(itemValue.Value.Value * 100);
                                break;
                            case "MONTHLY_PNL":
                                newItem.MonthlY_PNL = Convert.ToSingle(itemValue.Value.Value);
                                break;
                            case "YEARLY_ROI":
                                newItem.YearlY_ROI = Convert.ToSingle(itemValue.Value.Value * 100);
                                break;
                            case "YEARLY_PNL":
                                newItem.YearlY_PNL = Convert.ToSingle(itemValue.Value.Value);
                                break;
                            case "ALL_ROI":
                                newItem.AlL_ROI = Convert.ToSingle(itemValue.Value.Value * 100);
                                break;
                            case "ALL_PNL":
                                newItem.AlL_PNL = Convert.ToSingle(itemValue.Value.Value);
                                break;
                            default:
                                break;
                        }
                    }
                    newItem.IsTopTraderScore = 0;
                    newItem.IAIsTopTraderScore = 0;
                    newItem.IsDailY_ROI_Increasing = 0;
                    newItem.IsMonthlY_ROI_Increasing = 0;
                    newItem.IsYearlY_ROI_Increasing = 0;
                    result.Add(newItem);
                }
            }
            return result;
        }



        public async Task<List<string>> TraderUrlForUpdatePositionBinance()
        {
            List<string> listUrl = new List<string>();
            Dictionary<string, BinanceTrader> dicBinanceTrader = (await _binanceTraderService.GetAll()).GroupBy(x => x.EncryptedUid).ToDictionary(p => p.Key, p => p.FirstOrDefault());

            var dataTraderDataPerformanceBinance = await GetAllTraderDataPerformanceBinanceForModel(dicBinanceTrader);

           int  minTotalTopTrader = int.Parse(_configuration.GetSection("BinanceBybitSettings:MinTotalTopTrader").Value);

            var dataTraderDataPerformanceBinanceAudit = await GetAllTraderDataPerformanceBinanceForModelAudit(dicBinanceTrader);
            var dictionaryData = dataTraderDataPerformanceBinanceAudit.Where(p => !string.IsNullOrEmpty(p.EncryptedUid))
          .GroupBy(data => data.EncryptedUid)  // Agrupamos por EncryptedUid
          .Where(group => group.Count() > minTotalTopTrader)  // Filtramos los grupos donde el count > 5 porque a estado en el top  de tradores binance mas de 10 veces
          .ToDictionary(
              group => group.Key,              // La clave del diccionario es EncryptedUid
              group => group.Count()           // El valor del diccionario es el conteo de elementos en el grupo
          );


            var data = (dataTraderDataPerformanceBinance).Where(p => (dictionaryData.ContainsKey(p.EncryptedUid))).OrderByDescending(p => p.IAIsTopTraderScore).ToList();

            int minFollowerCount = int.Parse(_configuration.GetSection("BinanceBybitSettings:MinFollowerCount").Value);
            var allIAIsTopTraderScoreTraderDataPerformanceBinanceForModel = data.Where(p => p.FollowerCount > minFollowerCount).OrderByDescending(p => p.IAIsTopTraderScore).GroupBy(p => p.EncryptedUid).Select(p => new { p.FirstOrDefault().EncryptedUid, p.OrderByDescending(p => p.IAIsTopTraderScore).FirstOrDefault().IAIsTopTraderScore }).Distinct().ToList();
            var dicByEncryptedUidInStock = allIAIsTopTraderScoreTraderDataPerformanceBinanceForModel.GroupBy(p => p.EncryptedUid).ToDictionary(p => p.Key, p => p.FirstOrDefault().EncryptedUid);

            allIAIsTopTraderScoreTraderDataPerformanceBinanceForModel = allIAIsTopTraderScoreTraderDataPerformanceBinanceForModel.GroupBy(p => p.EncryptedUid).
                Select(p => new { p.FirstOrDefault().EncryptedUid, p.OrderByDescending(p => p.IAIsTopTraderScore).FirstOrDefault().IAIsTopTraderScore }).OrderByDescending(p => p.IAIsTopTraderScore).ToList();

            listUrl = allIAIsTopTraderScoreTraderDataPerformanceBinanceForModel.Where(p=> !NotIncludeGuidTrader.NotIncludeGuid.ContainsKey(p.EncryptedUid) ).Take(int.Parse(_configuration.GetSection("BinanceBybitSettings:TotalTraderUrlForUpdatePositionBinanceQuery").Value)).Select(p => p.EncryptedUid).Distinct().ToList();
           
            /*if (!listUrl.Contains("6408AAEEEBF0C76A3D5F0E39C64AAABA"))
            {
                listUrl=listUrl.Prepend("6408AAEEEBF0C76A3D5F0E39C64AAABA").Take(int.Parse(_configuration.GetSection("BinanceBybitSettings:TotalTraderUrlForUpdatePositionBinanceQuery").Value)).ToList();
            }
            */
            return listUrl.Distinct().ToList();
        }


        public async Task<List<string>> TraderUrlForUpdatePositionBinanceWithCompositeScore()
        {
            List<string> listUrl = new List<string>();
            Dictionary<string, BinanceTrader> dicBinanceTrader = (await _binanceTraderService.GetAll())
                .GroupBy(x => x.EncryptedUid)
                .ToDictionary(p => p.Key, p => p.FirstOrDefault());

            var dataTraderDataPerformanceBinance = await GetAllTraderDataPerformanceBinanceForModel(dicBinanceTrader);

            int minTotalTopTrader = int.Parse(_configuration.GetSection("BinanceBybitSettings:MinTotalTopTrader").Value);

            var dataTraderDataPerformanceBinanceAudit = await GetAllTraderDataPerformanceBinanceForModelAudit(dicBinanceTrader);
            var dictionaryData = dataTraderDataPerformanceBinanceAudit
                .Where(p => !string.IsNullOrEmpty(p.EncryptedUid))
                .GroupBy(data => data.EncryptedUid)
                .Where(group => group.Count() > minTotalTopTrader)
                .ToDictionary(group => group.Key, group => group.Count());

            var data = dataTraderDataPerformanceBinance
                .Where(p => dictionaryData.ContainsKey(p.EncryptedUid))
                .ToList();

            // Calcular el nuevo CompositeScore
            foreach (var item in data)
            {
                item.CompositeScore = (float)(
                    (0.6 * item.IAIsTopTraderScore) +
                    (0.2 * item.IsMonthlY_ROI_Increasing) +
                    (0.1 * item.IsDailY_ROI_Increasing) +
                    (0.1 * item.IsYearlY_ROI_Increasing)
                );
            }

            data = data.OrderByDescending(p => p.CompositeScore).ToList();

            int minFollowerCount = int.Parse(_configuration.GetSection("BinanceBybitSettings:MinFollowerCount").Value);
            var filtered = data
                .Where(p => p.FollowerCount > minFollowerCount)
                .GroupBy(p => p.EncryptedUid)
                .Select(p => new
                {
                    p.FirstOrDefault().EncryptedUid,
                    Score = p.OrderByDescending(x => x.CompositeScore).FirstOrDefault().CompositeScore
                })
                .OrderByDescending(x => x.Score)
                .ToList();

            listUrl = filtered
                .Where(p => !NotIncludeGuidTrader.NotIncludeGuid.ContainsKey(p.EncryptedUid))
                .Take(int.Parse(_configuration.GetSection("BinanceBybitSettings:TotalTraderUrlForUpdatePositionBinanceQuery").Value))
                .Select(p => p.EncryptedUid)
                .Distinct()
                .ToList();

            return listUrl;
        }



        public async Task<List<TraderDataPerformanceBinance>> GetAlldataTraderDataPerformanceBinanceAndTraderDataPerformanceBinanceAudit()
        {
            List<string> listUrl = new List<string>();
            Dictionary<string, BinanceTrader> dicBinanceTrader = (await _binanceTraderService.GetAll()).GroupBy(x => x.EncryptedUid).ToDictionary(p => p.Key, p => p.FirstOrDefault());

            var dataTraderDataPerformanceBinance = await GetAllTraderDataPerformanceBinanceForModelDataInit(dicBinanceTrader);

            int minTotalTopTrader = int.Parse(_configuration.GetSection("BinanceBybitSettings:MinTotalTopTrader").Value);

            var dataTraderDataPerformanceBinanceAudit = await GetAllTraderDataPerformanceBinanceForModelAuditDataInit(dicBinanceTrader);
            var dictionaryData = dataTraderDataPerformanceBinanceAudit.Where(p => !string.IsNullOrEmpty(p.EncryptedUid))
          .GroupBy(data => data.EncryptedUid)  // Agrupamos por EncryptedUid
          .Where(group => group.Count() > 0)  // Filtramos los grupos donde el count > 5 porque a estado en el top  de tradores binance mas de 10 veces
          .ToDictionary(
              group => group.Key,              // La clave del diccionario es EncryptedUid
              group => group.Count()           // El valor del diccionario es el conteo de elementos en el grupo
          );
         var data = (dataTraderDataPerformanceBinance).Where(p => (dictionaryData.ContainsKey(p.EncryptedUid))).Where(p=> !NotIncludeGuidTrader.NotIncludeGuid.ContainsKey(p.EncryptedUid)).OrderByDescending(p => p.IAIsTopTraderScore).ToList();
         //data.AddRange(dataTraderDataPerformanceBinanceAudit.Where(p => dictionaryData.ContainsKey(p.EncryptedUid)).ToList());
            return data;
        }

        public async Task<List<TraderDataPerformanceBinance>> GetAllTraderDataPerformanceBinanceForModel(Dictionary<string, BinanceTrader> dicBinanceTrader)
        {

            List<TraderDataPerformanceBinance> result = new List<TraderDataPerformanceBinance>();

            

            var dictionaryByEncryptedUid = await GetAllDictionaryByEncryptedUid();

            foreach (var item in dictionaryByEncryptedUid.Values)
            {
                TraderDataPerformanceBinance newItem = new TraderDataPerformanceBinance();

                bool setRankFollowerCount = false;
                foreach (var itemBinanceTraderPerformanceRetList in item)
                {
                    if (dicBinanceTrader.ContainsKey(itemBinanceTraderPerformanceRetList.EncryptedUid) && !setRankFollowerCount)
                    {
                        newItem.RankTrader = dicBinanceTrader[itemBinanceTraderPerformanceRetList.EncryptedUid].RankTrader.Value;
                        newItem.FollowerCount = dicBinanceTrader[itemBinanceTraderPerformanceRetList.EncryptedUid].FollowerCount.Value;
                        newItem.EncryptedUid = itemBinanceTraderPerformanceRetList.EncryptedUid;
                        newItem.CreatedOn = itemBinanceTraderPerformanceRetList.CreatedOn.HasValue ? itemBinanceTraderPerformanceRetList.CreatedOn.Value.ToString() :DateTime.Now.ToString() ;
                        setRankFollowerCount = true;
                    }
                    switch (string.Concat( itemBinanceTraderPerformanceRetList.PeriodType,"_" ,itemBinanceTraderPerformanceRetList.StatisticsType))
                    {
                        case "DAILY_ROI":
                            // code block
                            newItem.DailY_ROI = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value *100);
                            break;
                        case "DAILY_PNL":
                            newItem.DailY_PNL = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value);
                            break;
                        case "EXACT_YEARLY_ROI":
                            newItem.ExacT_YEARLY_ROI = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value * 100);
                            break;
                        case "EXACT_YEARLY_PNL":
                            newItem.ExacT_YEARLY_PNL = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value);
                            break;
                        case "WEEKLY_ROI":
                            newItem.WeeklY_ROI = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value * 100);
                            break;
                        case "WEEKLY_PNL":
                            newItem.WeeklY_PNL = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value);
                            break;
                        case "MONTHLY_ROI":
                            newItem.MonthlY_ROI = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value * 100);
                            break;
                        case "MONTHLY_PNL":
                            newItem.MonthlY_PNL = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value);
                            break;
                        case "YEARLY_ROI":
                            newItem.YearlY_ROI = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value * 100);
                            break;
                        case "YEARLY_PNL":
                            newItem.YearlY_PNL = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value);
                            break;
                        case "ALL_ROI":
                            newItem.AlL_ROI = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value * 100);
                            break;
                        case "ALL_PNL":
                            newItem.AlL_PNL = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value);
                            break;
                        default:
                            break;
                    }
                }

                newItem.IsTopTraderScore = Convert.ToSingle(CalculateIsTopTraderScore(newItem));
                newItem.IAIsTopTraderScore = PredictIsTopTraderScore(newItem).PerformanceScore;
                newItem.IsDailY_ROI_Increasing = PredictIsDailYROIIncreasingIncreasing(newItem);


                newItem.IsMonthlY_ROI_Increasing= PredictBinanceIsMonthlYROIIncreasing(newItem);
                newItem.IsYearlY_ROI_Increasing = PredictBinanceIsYearlYROIIncreasing(newItem);


               


                if (!string.IsNullOrEmpty(newItem.EncryptedUid))
                {
                    result.Add(newItem);
                }
                

            } 
            return result;
        }


        public async Task<List<TraderDataPerformanceBinance>> GetAllTraderDataPerformanceBinanceForModelDataInit(Dictionary<string, BinanceTrader> dicBinanceTrader)
        {

            List<TraderDataPerformanceBinance> result = new List<TraderDataPerformanceBinance>();



            var dictionaryByEncryptedUid = await GetAllDictionaryByEncryptedUid();

            foreach (var item in dictionaryByEncryptedUid.Values)
            {
                TraderDataPerformanceBinance newItem = new TraderDataPerformanceBinance();

                bool setRankFollowerCount = false;
                foreach (var itemBinanceTraderPerformanceRetList in item)
                {
                    if (dicBinanceTrader.ContainsKey(itemBinanceTraderPerformanceRetList.EncryptedUid) && !setRankFollowerCount)
                    {
                        newItem.RankTrader = dicBinanceTrader[itemBinanceTraderPerformanceRetList.EncryptedUid].RankTrader.Value;
                        newItem.FollowerCount = dicBinanceTrader[itemBinanceTraderPerformanceRetList.EncryptedUid].FollowerCount.Value;
                        newItem.EncryptedUid = itemBinanceTraderPerformanceRetList.EncryptedUid;
                        newItem.CreatedOn = itemBinanceTraderPerformanceRetList.CreatedOn.HasValue ? itemBinanceTraderPerformanceRetList.CreatedOn.Value.ToString() : DateTime.Now.ToString();
                        setRankFollowerCount = true;
                    }
                    switch (string.Concat(itemBinanceTraderPerformanceRetList.PeriodType, "_", itemBinanceTraderPerformanceRetList.StatisticsType))
                    {
                        case "DAILY_ROI":
                            // code block
                            newItem.DailY_ROI = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value * 100);
                            break;
                        case "DAILY_PNL":
                            newItem.DailY_PNL = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value);
                            break;
                        case "EXACT_YEARLY_ROI":
                            newItem.ExacT_YEARLY_ROI = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value * 100);
                            break;
                        case "EXACT_YEARLY_PNL":
                            newItem.ExacT_YEARLY_PNL = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value);
                            break;
                        case "WEEKLY_ROI":
                            newItem.WeeklY_ROI = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value * 100);
                            break;
                        case "WEEKLY_PNL":
                            newItem.WeeklY_PNL = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value);
                            break;
                        case "MONTHLY_ROI":
                            newItem.MonthlY_ROI = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value * 100);
                            break;
                        case "MONTHLY_PNL":
                            newItem.MonthlY_PNL = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value);
                            break;
                        case "YEARLY_ROI":
                            newItem.YearlY_ROI = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value * 100);
                            break;
                        case "YEARLY_PNL":
                            newItem.YearlY_PNL = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value);
                            break;
                        case "ALL_ROI":
                            newItem.AlL_ROI = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value * 100);
                            break;
                        case "ALL_PNL":
                            newItem.AlL_PNL = Convert.ToSingle(itemBinanceTraderPerformanceRetList.Value.Value);
                            break;
                        default:
                            break;
                    }
                }

                newItem.IsTopTraderScore = 0;
                newItem.IAIsTopTraderScore = 0;
                newItem.IsDailY_ROI_Increasing = 0;


                newItem.IsMonthlY_ROI_Increasing = 0;
                newItem.IsYearlY_ROI_Increasing = 0;





                if (!string.IsNullOrEmpty(newItem.EncryptedUid))
                {
                    result.Add(newItem);
                }


            }
            return result;
        }




        public TraderPerformancePrediction PredictIsTopTraderScore( TraderDataPerformanceBinance newDataPredict)
        {
           

            try
            {
                var prediction = _predictionIsTopTraderScoreEnginePool.Predict<TraderDataPerformanceBinance, TraderPerformancePrediction>(newDataPredict);
                return prediction;
            }
            catch (Exception e)
            {
                
                    Console.WriteLine(e.Message);
                //  Block of code to handle errors
            }
            return new TraderPerformancePrediction() { PerformanceScore = 0 };
        }

      
        public float PredictIsDailYROIIncreasingIncreasing(TraderDataPerformanceBinance newDataPredict)
        {

            try
            {
                var prediction = _predictionIsDailYROIIncreasingEnginePool.Predict<TraderDataPerformanceBinance, TraderDataPerformanceBinanceIsDailYROIIncreasingPrediction>(newDataPredict);
                return prediction.IsDailY_ROI_Increasing;
            }
            catch (Exception e)
            {
                //  Block of code to handle errors
                Console.WriteLine(e.Message);
            }
            return 0;
        }

        public float PredictBinanceIsMonthlYROIIncreasing(TraderDataPerformanceBinance newDataPredict)
        {
           

            try
            {
   
                var prediction = _predictionIsMonthlYROIIncreasingEnginePool.Predict<TraderDataPerformanceBinance, TraderDataPerformanceBinanceIsMonthlY_ROI_IncreasingPrediction>(newDataPredict);
                return prediction.IsMonthlY_ROI_Increasing;
            }
            catch (Exception e)
            {
                //  Block of code to handle errors
                Console.WriteLine(e.Message);
            }
            return 0;


        }



        public float PredictBinanceIsYearlYROIIncreasing(TraderDataPerformanceBinance newDataPredict)
        {
            try
            {
                DataViewSchema modelSchema;
                ITransformer trainedModel = _mlContext.Model.Load(_configuration.GetSection("BinanceBybitSettings:Binance_IsYearlY_ROI_Increasing").Value, out modelSchema);
                var predictionEngine = _mlContext.Model.CreatePredictionEngine<TraderDataPerformanceBinance, TraderDataPerformanceBinanceIsYearlYROIIncreasingPrediction>(trainedModel);
                var prediction = predictionEngine.Predict(newDataPredict);
                return prediction.IsYearlY_ROI_Increasing;
            }
            catch (Exception e)
            {
                //  Block of code to handle errors
                Console.WriteLine(e.Message);
            }
            return 0;
        }

        public double CalculateIsTopTraderScore(TraderDataPerformanceBinance traderData)
        {
            // Verifica si hay pérdidas significativas
            if (traderData.DailY_ROI < 0 && traderData.DailY_ROI < -5)//Math.Abs(traderData.DailY_ROI) * 0.06
            {
                return 0;
            }

            if (traderData.WeeklY_ROI < 0 && traderData.WeeklY_ROI < -3)//Math.Abs(traderData.WeeklY_ROI) * 0.03
            {
                return 0;
            }

            if (traderData.MonthlY_ROI < 0 && traderData.MonthlY_ROI < -2)//Math.Abs(traderData.MonthlY_ROI) * 0.02
            {
                return 0;
            }

            if (traderData.YearlY_ROI < 0 && traderData.YearlY_ROI < -2)//Math.Abs(traderData.YearlY_ROI) * 0.02
            {
                return 0;
            }

            if (traderData.AlL_ROI < 0 && traderData.AlL_ROI < -2)//Math.Abs(traderData.AlL_ROI) * 0.02
            {
                return 0;
            }

            // Verifica si el trader tiene un rango y seguidores
            if (traderData.RankTrader > 0 && traderData.FollowerCount > 1000)
            {
                // Calcula el puntaje para ROI y PNL de diferentes periodos
                double dailyScore = CalculateScore(traderData.DailY_ROI, traderData.DailY_PNL);
                double weeklyScore = CalculateScore(traderData.WeeklY_ROI, traderData.WeeklY_PNL);
                double monthlyScore = CalculateScore(traderData.MonthlY_ROI, traderData.MonthlY_PNL);
                double yearlyScore = CalculateScore(traderData.ExacT_YEARLY_ROI, traderData.ExacT_YEARLY_PNL);
                double allScore = CalculateScore(traderData.AlL_ROI, traderData.AlL_PNL);

                // Promedio de los puntajes
                double performanceScore = (dailyScore + weeklyScore + monthlyScore + yearlyScore + allScore) / 5.0;

                // Fórmula final ajustada para obtener un valor entre 0 y 1
                double finalScore = Math.Max(0, Math.Min(performanceScore, 1.0));

                return finalScore;
            }

            return 0; // Si falta información crucial, devolver 0
        }

        private double CalculateScore(double roi, double pnl)
        {
            // Puedes ajustar la lógica según tus preferencias
            // Verifica si el ROI es positivo o si la pérdida es menor al 4%
            if (roi >= 0 || pnl >= Math.Abs(roi) * 0.02)
            {
                return Math.Max(0, Math.Min(roi + pnl, 1.0));
            }
            else
            {
                return 0;
            }
        }

    
        /// <summary>
        /// Calcular si el ROI VA A  aumentado
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<List<TraderDataPerformanceBinance>> CalculateIncreasingROIs(List<TraderDataPerformanceBinance> data)
        {
            // Ordenar los datos por EncryptedUid y CreatedOn
            var sortedData = data.OrderBy(d => d.EncryptedUid).ThenBy(d => DateTime.Parse(d.CreatedOn)).ToList();

            for (int i = 0; i < sortedData.Count - 1; i++)
            {
                // Asegurarse de que estamos comparando el mismo trader
                if (sortedData[i].EncryptedUid == sortedData[i + 1].EncryptedUid)
                {
                    // Asignar el valor del próximo día
                    sortedData[i].IsDailY_ROI_Increasing = sortedData[i + 1].DailY_ROI;
                    sortedData[i].IsMonthlY_ROI_Increasing = sortedData[i + 1].MonthlY_ROI;
                    sortedData[i].IsYearlY_ROI_Increasing = sortedData[i + 1].YearlY_ROI;
                }
            }

            // Eliminar el último elemento de cada trader porque no tendrá un valor siguiente
            var cleanedData = sortedData.Where((data, index) => index < sortedData.Count - 1 && sortedData[index].EncryptedUid == sortedData[index + 1].EncryptedUid).ToList();

            return cleanedData;
        }


        public async Task<bool> ModelIsTopTraderScore(List<TraderDataPerformanceBinance> dataTraderDataPerformanceBinance)
        {

            var dataView = _mlContext.Data.LoadFromEnumerable(dataTraderDataPerformanceBinance);
            var trainTestSplit = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

            var dataProcessPipeline = _mlContext.Transforms.Concatenate("Features",
               "dailY_ROI",
               "dailY_PNL",
                "exacT_YEARLY_ROI",
                "exacT_YEARLY_PNL",
               "weeklY_ROI",
                "weeklY_PNL",
                "monthlY_ROI",
                "monthlY_PNL",
                "yearlY_ROI",
                "yearlY_PNL",
                "alL_ROI",
                "alL_PNL")
                .Append(_mlContext.Transforms.NormalizeMinMax("Features"));

            var trainer = _mlContext.Regression.Trainers.Sdca(labelColumnName: "isTopTraderScore", featureColumnName: "Features");

            var trainingPipeline = dataProcessPipeline.Append(trainer);
            var crossValidationResults = _mlContext.Regression.CrossValidate(dataView, trainingPipeline, numberOfFolds: 5, labelColumnName: "isTopTraderScore");

            var model = trainingPipeline.Fit(dataView);
            _mlContext.Model.Save(model, trainTestSplit.TrainSet.Schema, _configuration.GetSection("BinanceBybitSettings:Binance_IsTopTraderScore").Value);

            return true;
        }

        public async Task<bool> ReloadModelIsTopTraderScore()
        {
            // Recargar el modelo en PredictionEnginePool
            using (var scope = _serviceProvider.CreateScope())
            {
                var modelEnginePool = scope.ServiceProvider.GetRequiredService<PredictionEnginePool<TraderDataPerformanceBinance, TraderPerformancePrediction>>();
                DataViewSchema modelSchema;
                ITransformer updatedModel = _mlContext.Model.Load(_configuration.GetSection("BinanceBybitSettings:Binance_IsTopTraderScore").Value, out modelSchema);
                modelEnginePool.UpdateModel(updatedModel, modelSchema);
            }
            return true;
        }



        public async Task<bool> ModelIsDailY_ROI_Increasing(List<TraderDataPerformanceBinance> dataTraderDataPerformanceBinance)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(dataTraderDataPerformanceBinance);

            var dataProcessPipeline = _mlContext.Transforms.Concatenate("Features",
                "dailY_ROI",
                "dailY_PNL",
                "exacT_YEARLY_ROI",
                "exacT_YEARLY_PNL",
                "weeklY_ROI",
                "weeklY_PNL",
                "monthlY_ROI",
                "monthlY_PNL",
                "yearlY_ROI",
                "yearlY_PNL",
                "alL_ROI",
                "alL_PNL")
                .Append(_mlContext.Transforms.NormalizeMinMax("Features"));

            var trainer = _mlContext.Regression.Trainers.Sdca(labelColumnName: "IsDailY_ROI_Increasing", featureColumnName: "Features");


            var trainingPipeline = dataProcessPipeline.Append(trainer);

            // Validación cruzada con 5 pliegues
            var crossValidationResults = _mlContext.Regression.CrossValidate(dataView, trainingPipeline, numberOfFolds: 5, labelColumnName: "IsDailY_ROI_Increasing");

            // Entrenamiento del modelo final en todo el conjunto de datos
            var model = trainingPipeline.Fit(dataView);

            // Guardar el modelo
            _mlContext.Model.Save(model, dataView.Schema, _configuration.GetSection("BinanceBybitSettings:Binance_IsDailY_ROI_Increasing").Value);

            return true;
        }

        public async Task<bool> ReloadModelIsDailY_ROI_Increasing()
        {
            // Recargar el modelo en PredictionEnginePool
            using (var scope = _serviceProvider.CreateScope())
            {
                var modelEnginePool = scope.ServiceProvider.GetRequiredService<PredictionEnginePool<TraderDataPerformanceBinance, TraderDataPerformanceBinanceIsDailYROIIncreasingPrediction>>();
                DataViewSchema modelSchema;
                ITransformer updatedModel = _mlContext.Model.Load(_configuration.GetSection("BinanceBybitSettings:Binance_IsDailY_ROI_Increasing").Value, out modelSchema);
                modelEnginePool.UpdateModel(updatedModel, modelSchema);
            }
            return true;
        }


    public async Task<bool> ModelIsMonthlY_ROI_Increasing(List<TraderDataPerformanceBinance> dataTraderDataPerformanceBinance)
        {
          
      
            var dataView = _mlContext.Data.LoadFromEnumerable(dataTraderDataPerformanceBinance);
            var trainTestSplit = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

            var dataProcessPipeline = _mlContext.Transforms.Concatenate("Features",
               "dailY_ROI",
               "dailY_PNL",
               "exacT_YEARLY_ROI",
               "exacT_YEARLY_PNL",
               "weeklY_ROI",
               "weeklY_PNL",
               "monthlY_ROI",
               "monthlY_PNL",
               "yearlY_ROI",
               "yearlY_PNL",
               "alL_ROI",
               "alL_PNL")
               .Append(_mlContext.Transforms.NormalizeMinMax("Features"));

            var trainer = _mlContext.Regression.Trainers.Sdca(labelColumnName: "IsMonthlY_ROI_Increasing", featureColumnName: "Features");

            var trainingPipeline = dataProcessPipeline.Append(trainer);

            var crossValidationResults = _mlContext.Regression.CrossValidate(dataView, trainingPipeline, numberOfFolds: 5, labelColumnName: "IsMonthlY_ROI_Increasing");
            var model = trainingPipeline.Fit(trainTestSplit.TrainSet);
            _mlContext.Model.Save(model, trainTestSplit.TrainSet.Schema, _configuration.GetSection("BinanceBybitSettings:Binance_IsMonthlY_ROI_Increasing").Value);
            return true;
        }


        public async Task<bool> ReloadModelIsMonthlY_ROI_Increasing()
        {
            // Recargar el modelo en PredictionEnginePool
            using (var scope = _serviceProvider.CreateScope())
            {
                var modelEnginePool = scope.ServiceProvider.GetRequiredService<PredictionEnginePool<TraderDataPerformanceBinance, TraderDataPerformanceBinanceIsMonthlY_ROI_IncreasingPrediction>>();
                DataViewSchema modelSchema;
                ITransformer updatedModel = _mlContext.Model.Load(_configuration.GetSection("BinanceBybitSettings:Binance_IsMonthlY_ROI_Increasing").Value, out modelSchema);
                modelEnginePool.UpdateModel(updatedModel, modelSchema);
            }
            return true;
        }



        public async Task<bool> ModelIsYearlY_ROI_Increasing(List<TraderDataPerformanceBinance> dataTraderDataPerformanceBinance)
        {
      
            var dataView = _mlContext.Data.LoadFromEnumerable(dataTraderDataPerformanceBinance);
            var trainTestSplit = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

            var dataProcessPipeline = _mlContext.Transforms.Concatenate("Features",
               "dailY_ROI",
               "dailY_PNL",
               "exacT_YEARLY_ROI",
               "exacT_YEARLY_PNL",
               "weeklY_ROI",
               "weeklY_PNL",
               "monthlY_ROI",
               "monthlY_PNL",
               "yearlY_ROI",
               "yearlY_PNL",
               "alL_ROI",
               "alL_PNL")
               .Append(_mlContext.Transforms.NormalizeMinMax("Features"));

            //var trainer = _mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "IsYearlY_ROI_Increasing", featureColumnName: "Features");
            var trainer = _mlContext.Regression.Trainers.Sdca(labelColumnName: "IsYearlY_ROI_Increasing", featureColumnName: "Features");
            var trainingPipeline = dataProcessPipeline.Append(trainer);

            var crossValidationResults = _mlContext.Regression.CrossValidate(dataView, trainingPipeline, numberOfFolds: 5, labelColumnName: "IsYearlY_ROI_Increasing");

            var model = trainingPipeline.Fit(trainTestSplit.TrainSet);
            _mlContext.Model.Save(model, trainTestSplit.TrainSet.Schema, _configuration.GetSection("BinanceBybitSettings:Binance_IsYearlY_ROI_Increasing").Value);

            return true;
        }

        public async Task<int> AddIndBinanceTraderPerformanceRetListAudit()
        {
            var result = await _binanceTraderPerformanceRetListRepository.AddIndBinanceTraderPerformanceRetListAudit();
            return result;
        }


        public async Task<BinanceTraderPerformanceRetList> Update(BinanceTraderPerformanceRetList binanceTraderPerformanceRetList)
        {
            var result = await _binanceTraderPerformanceRetListRepository.Update(binanceTraderPerformanceRetList);
            return result;
        }

        public async Task<bool> DeletedAll()
        {
            var result = await _binanceTraderPerformanceRetListRepository.DeletedAll();
            return true;
        }

        public async Task<bool> Deleted(string encryptedUid)
        {
            var result = await _binanceTraderPerformanceRetListRepository.Deleted(encryptedUid);
            return true;
        }
        public async Task<Dictionary<string, int>> GetTraderForUpdatePositionFromBinance()
        {
            var result = await _binanceTraderPerformanceRetListRepository.GetTraderForUpdatePositionFromBinance();
            return result;

        }

            public async Task<bool> DeletedOldPerformanceItem(string guidNewItem)
        {
            var result = await _binanceTraderPerformanceRetListRepository.DeletedOldPerformanceItem(guidNewItem);
            return true;

        }
            public async Task<bool> UpdateRange(List<BinanceTraderPerformanceRetList> binanceTraderPerformanceRetList)
        {
            var result = await _binanceTraderPerformanceRetListRepository.UpdateRange(binanceTraderPerformanceRetList);
            return result;

        }
        public async Task<RootBinancePerformanceRetList> LoadBinanceTraderPerformanceFromJson(string filePath)
        {
            var result = await _binanceTraderPerformanceRetListRepository.LoadBinanceTraderPerformanceFromJson(filePath);
            return result;
        }

        
    }
}
