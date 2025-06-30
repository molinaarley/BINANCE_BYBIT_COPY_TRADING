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








        public LoadBinanceTraderFromJsonHandler(IUnitOfWork unitOfWork, ITelegrameBotService telegrameBotService,
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

        private List<TraderFilesNames> getNewFilesName()
        {
            List<string> result = new List<string>();

            List<TraderFilesNames> objResult = new List<TraderFilesNames>();

            List<string> fileJson = Directory.GetFiles(_pathDataJson, "*.json").ToList();

            foreach ( var item in  fileJson)
            {
                FileInfo fileinfo = new FileInfo(item);
                TraderFilesNames newData = new TraderFilesNames();
                newData.Name = item;
                string nameFile = fileinfo.Name.Split("_")[1].Split(".")[0];
                newData.CreateDate = new DateTime(int.Parse(nameFile.Split("-")[0]), int.Parse(nameFile.Split("-")[1]), int.Parse(nameFile.Split("-")[2]), int.Parse(nameFile.Split("-")[3]), int.Parse(nameFile.Split("-")[4]),
                    int.Parse(nameFile.Split("-")[5]) );
                objResult.Add(newData);
            }
            objResult = objResult.OrderBy(p=>p.CreateDate).ToList();

         
            objResult[0].TypeData = "total";
            objResult[1].TypeData = "total";
            objResult[2].TypeData = "mensuel";
            objResult[3].TypeData = "hebdomadaire";
            objResult[4].TypeData = "quotidien";
            objResult[5].TypeData = "total";
            objResult[6].TypeData = "total";
            objResult[7].TypeData = "mensuel";
            objResult[8].TypeData = "hebdomadaire";
            objResult[9].TypeData = "quotidien";
            return objResult;

        }

        public async Task<int> Handle(LoadBinanceTraderFromJsonQuery request, CancellationToken cancellationToken)
        {
            List<string> fileJson = Directory.GetFiles(_pathDataJson, "*.json").ToList();

            List<TraderFilesNames> objResultFileType = getNewFilesName();
            Dictionary<string, string> dicEncryptedUid = new Dictionary<string, string>();

            if (objResultFileType.Any())
            {

                await _binanceTraderTypeDataService.DeletedAll();

                foreach (var item in objResultFileType)
                {
                    string typeData = string.Empty;

                    if (!string.IsNullOrEmpty(item.TypeData))
                    {
                        if (item.TypeData.Contains("quotidien"))
                        {
                            typeData = "quotidien";
                        }
                        if (item.TypeData.Contains("hebdomadaire"))
                        {
                            typeData = "hebdomadaire";
                        }
                        if (item.TypeData.Contains("mensuel"))
                        {
                            typeData = "mensuel";
                        }
                        if (item.TypeData.Contains("total"))
                        {
                            typeData = "total";
                        }
                    }
                    else
                    {
                        typeData = "total";

                    }

                    BinanceTraderFromJsonReponseRoot binanceTraderFromJsonReponseRoot = new BinanceTraderFromJsonReponseRoot();

                    try
                    {
                        binanceTraderFromJsonReponseRoot = await _binanceTraderService.LoadBinanceTraderFromJson(item.Name);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    

                   // var allTraderDic = await _binanceTraderService.GetAllDictionary();
                    foreach (var itemTrader in binanceTraderFromJsonReponseRoot.data)
                    {

                        if (!string.IsNullOrEmpty(itemTrader.encryptedUid) && !dicEncryptedUid.ContainsKey(itemTrader.encryptedUid))
                        {
                            dicEncryptedUid.Add(itemTrader.encryptedUid,itemTrader.encryptedUid);
                            BinanceTrader currentTrader = await _binanceTraderService.GetByUid(itemTrader.encryptedUid);

                            if (currentTrader!=null && !string.IsNullOrEmpty(currentTrader.EncryptedUid))
                            {
                            await _binanceTraderTypeDataService.Create(new BinanceTraderTypeData()
                                    {
                                        CreatedOn = DateTime.Now,
                                        EncryptedUid = itemTrader.encryptedUid,
                                        Pnl = itemTrader.pnl,
                                        Roi = itemTrader.roi,
                                        TypeData = typeData
                                    });

                                if (!currentTrader.CreatedOn.HasValue)
                                {
                                    currentTrader.CreatedOn = DateTime.Now;
                                }
                                currentTrader.FollowerCount = itemTrader.followerCount;
                                currentTrader.NickName = itemTrader.nickName;
                                currentTrader.PositionShared = itemTrader.positionShared;
                                currentTrader.RankTrader = itemTrader.rank;
                                currentTrader.UpdateTime = itemTrader.updateTime;
                                await _binanceTraderService.Update(currentTrader);


                            }
                            else
                            {
                               
                                    string EncryptedUid = await _binanceTraderService.Create(new BinanceTrader()
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

                }

              
                //zip trader
                await _donetZipService.ZipFolderPosition(_pathDataJson, _pathDataJsonZip);
                await _fileService.DeleteAllFiles(_pathDataJson);
            }
            return fileJson.Count;
        }
    }


     class TraderFilesNames
    {
        public string Name { get; set; }
        public string TypeData { get; set; }        
        public DateTime CreateDate { get; set; }

    }
}
