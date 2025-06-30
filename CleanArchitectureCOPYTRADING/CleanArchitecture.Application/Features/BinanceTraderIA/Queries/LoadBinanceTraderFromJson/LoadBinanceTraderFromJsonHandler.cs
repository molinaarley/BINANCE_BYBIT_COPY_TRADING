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

namespace CleanArchitecture.Application.Features.Binance.Queries.LoadPosition
{
    public class LoadBinanceTraderFromJsonHandler : IRequestHandler<LoadBinanceTraderFromJsonQuery, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITelegrameBotService _telegrameBotService;
        private string _pathDataJson = @"C:\worck\BINANCE_DATA_TRADER\";
        private string _pathDataJsonZip = @"C:\worck\BINANCE_DATA_TRADER_ZIP\";
        private readonly IDonetZipService _donetZipService;
        private readonly IFileService _fileService;
        private readonly IByBitApiService _byBitApiService;
        private readonly IBinanceTraderService _binanceTraderService;
        private readonly IBinanceOrderService _binanceOrderService;
        private readonly IBinanceByBitUsersService _binanceByBitUsersService;
        private readonly IBinanceByBitOrderService _binanceByBitOrderService;
        private readonly IBinanceMonitoringProcessService _binanceMonitoringProcessService;
        private readonly IBinanceTraderTypeDataService _binanceTraderTypeDataService;
        private readonly IMapper _mapper;








        public LoadBinanceTraderFromJsonHandler(IUnitOfWork unitOfWork,  ITelegrameBotService telegrameBotService,
            IDonetZipService donetZipService, IFileService fileService, IByBitApiService byBitService,
            IBinanceTraderService binanceTraderService, IBinanceOrderService binanceOrderService,
            IBinanceByBitUsersService binanceByBitUsersService,
            IBinanceByBitOrderService binanceByBitOrderService,
            IBinanceMonitoringProcessService binanceMonitoringProcessService,
            IBinanceTraderTypeDataService binanceTraderTypeDataService,
            IMapper mapper)
        {

            _unitOfWork = unitOfWork;
            _telegrameBotService = telegrameBotService ?? throw new ArgumentException(nameof(telegrameBotService));
            _donetZipService = donetZipService ?? throw new ArgumentException(nameof(donetZipService));
            _fileService = fileService ?? throw new ArgumentException(nameof(fileService));
            _byBitApiService = byBitService ?? throw new ArgumentException(nameof(byBitService));
            _binanceTraderService = binanceTraderService ?? throw new ArgumentException(nameof(binanceTraderService));
            _binanceOrderService = binanceOrderService ?? throw new ArgumentException(nameof(binanceOrderService));
            _binanceByBitUsersService = binanceByBitUsersService ?? throw new ArgumentException(nameof(binanceByBitUsersService));
            _binanceByBitOrderService = binanceByBitOrderService ?? throw new ArgumentException(nameof(binanceByBitOrderService));
            _binanceMonitoringProcessService = binanceMonitoringProcessService ?? throw new ArgumentException(nameof(binanceMonitoringProcessService));
            _binanceTraderTypeDataService = binanceTraderTypeDataService ?? throw new ArgumentException(nameof(binanceTraderTypeDataService));

            

            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));

        }

        public async Task<int> Handle(LoadBinanceTraderFromJsonQuery request, CancellationToken cancellationToken)
        {
            List<string> fileJson = Directory.GetFiles(_pathDataJson, "*.json").ToList();
            await _binanceTraderTypeDataService.DeletedAll();

            foreach (var item in fileJson)
            {
                string typeData = string.Empty;
                if (item.Contains("quotidien"))
                {
                    typeData = "quotidien";
                }
                if (item.Contains("hebdomadaire"))
                {
                    typeData = "hebdomadaire";
                }
                if (item.Contains("mensuel"))
                {
                    typeData = "mensuel";
                }
                if (item.Contains("total"))
                {
                    typeData = "total";
                }

                BinanceTraderFromJsonReponseRoot binanceTraderFromJsonReponseRoot = await _binanceTraderService.LoadBinanceTraderFromJson(item);

                var allTraderDic = await _binanceTraderService.GetAllDictionary();
                foreach (var itemTrader in binanceTraderFromJsonReponseRoot.data)
                {
                    if (allTraderDic.ContainsKey(itemTrader.encryptedUid))
                    {
                        /*await _binanceTraderService.Update(new BinanceTrader()
                        {
                            CreatedOn = DateTime.Now,
                            EncryptedUid = itemTrader.encryptedUid,
                            FollowerCount = itemTrader.followerCount,
                            NickName = itemTrader.nickName,
                            PositionShared = itemTrader.positionShared,
                            RankTrader = itemTrader.rank,
                            UpdateTime = itemTrader.updateTime,
                        });*/

                        await  _binanceTraderTypeDataService.Create(new BinanceTraderTypeData()
                        {
                            CreatedOn = DateTime.Now,
                            EncryptedUid = itemTrader.encryptedUid,
                            Pnl = itemTrader.pnl,
                            Roi = itemTrader.roi,
                            TypeData = typeData
                        });
                    }
                    else
                    {
                       string EncryptedUid= await _binanceTraderService.Create(new BinanceTrader()
                       {
                           CreatedOn = DateTime.Now,
                           EncryptedUid = itemTrader.encryptedUid,
                           FollowerCount = itemTrader.followerCount,
                           NickName = itemTrader.nickName,
                           PositionShared = itemTrader.positionShared,
                           RankTrader = itemTrader.rank,
                           UpdateTime = itemTrader.updateTime,
                       });

                        await _binanceTraderTypeDataService.Create(new BinanceTraderTypeData()
                        {
                            CreatedOn = DateTime.Now,
                            EncryptedUid = EncryptedUid,
                            Pnl = itemTrader.pnl,
                            Roi = itemTrader.roi,
                            TypeData = typeData
                        });
                    }
                }
            }
            //zip trader
            await _donetZipService.ZipFolderPosition(_pathDataJson, _pathDataJsonZip);
            await _fileService.DeleteAllFiles(_pathDataJson);
            return fileJson.Count;

        }
    }
}
