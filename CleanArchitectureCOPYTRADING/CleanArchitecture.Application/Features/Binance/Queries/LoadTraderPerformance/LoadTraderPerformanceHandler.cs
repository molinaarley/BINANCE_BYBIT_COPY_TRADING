using System.Globalization;
using System.IO;
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

namespace CleanArchitecture.Application.Features.Binance.Queries.LoadTraderPerformance
{
    public class LoadTraderPerformanceHandler : IRequestHandler<LoadTraderPerformanceQuery, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITelegrameBotService _telegrameBotService;
        private string _pathDataJson = @"C:\worck\BINANCE_DATA_TRADER_INFO\";
        private string _pathDataJsonZip = @"C:\worck\BINANCE_DATA_TRADER_INFO_ZIP\";
        private readonly IDonetZipService _donetZipService;
        private readonly IFileService _fileService;
        private readonly IBinanceTraderService _binanceTraderService;
        private readonly IBinanceTraderTypeDataService _binanceTraderTypeDataService;
        private readonly IMapper _mapper;
        private readonly IBinanceTraderPerformanceService _loadTraderPerformanceService;
        public LoadTraderPerformanceHandler(IUnitOfWork unitOfWork,  ITelegrameBotService telegrameBotService,
            IDonetZipService donetZipService, IFileService fileService,
            IBinanceTraderService binanceTraderService,
            IBinanceTraderTypeDataService binanceTraderTypeDataService,
            IBinanceTraderPerformanceService loadTraderPerformanceService,
            IMapper mapper)
        {

            _unitOfWork = unitOfWork;
            _telegrameBotService = telegrameBotService ?? throw new ArgumentException(nameof(telegrameBotService));
            _donetZipService = donetZipService ?? throw new ArgumentException(nameof(donetZipService));
            _fileService = fileService ?? throw new ArgumentException(nameof(fileService));
            _binanceTraderService = binanceTraderService ?? throw new ArgumentException(nameof(binanceTraderService));
            _binanceTraderTypeDataService = binanceTraderTypeDataService ?? throw new ArgumentException(nameof(binanceTraderTypeDataService));
            _loadTraderPerformanceService = loadTraderPerformanceService ?? throw new ArgumentException(nameof(loadTraderPerformanceService));
            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));

        }

        public async Task<int> Handle(LoadTraderPerformanceQuery request, CancellationToken cancellationToken)
        {
            List<string> fileJson = Directory.GetFiles(_pathDataJson, "*.*").ToList();
            fileJson =fileJson.Where(p => p.Contains(request.EncryptedUid)).ToList();
           // await _loadTraderPerformanceService.DeletedAll();

            Dictionary<string, string> dicEncryptedUidStatisticsType = new Dictionary<string, string>();

          var allTraderDic = await _binanceTraderService.GetAllDictionary();
          var dicloadTraderPerformanceService= await _loadTraderPerformanceService.GetAllDictionary();

          bool existeInfo=  fileJson.Select(p=> new FileInfo(p)).Where(p=> dicloadTraderPerformanceService.ContainsKey(p.Name.Split('_')[0]) ).Any();

            if (existeInfo)
            {
                string name = fileJson.Select(p => new FileInfo(p)).Select(p => p.Name.Split('_')[0]).FirstOrDefault();
                await _loadTraderPerformanceService.Deleted(name);
            }

            foreach (var item in fileJson)
            {
                FileInfo fileinfo = new FileInfo(item);
                
                string typeData = string.Empty;
                RootBinancePerformanceRetList binanceTraderFromJsonReponseRoot = await _loadTraderPerformanceService.LoadBinanceTraderPerformanceFromJson(item);
               

              

                if (binanceTraderFromJsonReponseRoot.positionsOpen && !dicEncryptedUidStatisticsType.ContainsKey(fileinfo.Name.Split('_')[0]))
                {
                    dicEncryptedUidStatisticsType.Add(fileinfo.Name.Split('_')[0], fileinfo.Name.Split('_')[0]);


                    foreach (var itemPerformanceTrader in binanceTraderFromJsonReponseRoot.data.performanceRetList)
                    {
                        string keyAdd = string.Concat(fileinfo.Name.Split('_')[0], "_",
                            itemPerformanceTrader.statisticsType, "_", itemPerformanceTrader.periodType);
                        if (!dicEncryptedUidStatisticsType.ContainsKey(keyAdd))
                        {

                            dicEncryptedUidStatisticsType.Add(keyAdd, keyAdd);

                            string EncryptedUid = await _loadTraderPerformanceService.Create(new BinanceTraderPerformanceRetList()
                            {
                                CreatedOn = DateTime.Now,
                                EncryptedUid = fileinfo.Name.Split('_')[0],
                                PeriodType = itemPerformanceTrader.periodType,
                                StatisticsType = itemPerformanceTrader.statisticsType,
                                Value = itemPerformanceTrader.value,
                                Rank = itemPerformanceTrader.rank,
                                GuidNewItem=request.Guid
                            });
                        }
                    }
                }
            }
            //zip trader
           // await _donetZipService.ZipFolderPosition(_pathDataJson, _pathDataJsonZip);
           // await _fileService.DeleteAllFiles(_pathDataJson);
            return fileJson.Count;

        }
    }
}
