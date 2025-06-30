using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Domain.Binance;
using MediatR;
using Microsoft.Extensions.Configuration;


namespace CleanArchitecture.Application.Features.BinanceTraderIA.Queries.CreateModelTrader
{
    public class TraderDataPerformanceBinancePrediction
    {
        public float WeeklY_ROI { get; set; }
        // Define las demás propiedades según sea necesario
    }

    public class CreateModelTraderHandler : IRequestHandler<CreateModelTraderQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITelegrameBotService _telegrameBotService;
        private readonly IDonetZipService _donetZipService;
        private readonly IFileService _fileService;
        private readonly IByBitApiService _byBitApiService;
        private readonly IBinanceTraderService _binanceTraderService;
        private readonly IBinanceMonitoringProcessService _binanceMonitoringProcessService;
        private readonly IBinanceTraderPerformanceService _loadTraderPerformanceService;
        public IConfiguration _configuration { get; }

        public CreateModelTraderHandler(IUnitOfWork unitOfWork, ITelegrameBotService telegrameBotService,
            IDonetZipService donetZipService, IFileService fileService, IByBitApiService byBitService,
            IBinanceTraderService binanceTraderService,
            IBinanceMonitoringProcessService binanceMonitoringProcessService,
            IBinanceTraderPerformanceService loadTraderPerformanceService,
            IConfiguration configuration)
        {

            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _telegrameBotService = telegrameBotService ?? throw new ArgumentException(nameof(telegrameBotService));
            _donetZipService = donetZipService ?? throw new ArgumentException(nameof(donetZipService));
            _fileService = fileService ?? throw new ArgumentException(nameof(fileService));
            _byBitApiService = byBitService ?? throw new ArgumentException(nameof(byBitService));
            _binanceTraderService = binanceTraderService ?? throw new ArgumentException(nameof(binanceTraderService));
            _binanceMonitoringProcessService = binanceMonitoringProcessService ?? throw new ArgumentException(nameof(binanceMonitoringProcessService));
            _loadTraderPerformanceService = loadTraderPerformanceService ?? throw new ArgumentException(nameof(loadTraderPerformanceService));
        }

        public async Task<bool> Handle(CreateModelTraderQuery request, CancellationToken cancellationToken)
        {

            Dictionary<string, BinanceTrader> dicBinanceTrader = (await _binanceTraderService.GetAll()).GroupBy(x => x.EncryptedUid).ToDictionary(p => p.Key, p => p.FirstOrDefault());

            var dataTraderDataPerformanceBinance = await _loadTraderPerformanceService.GetAllTraderDataPerformanceBinanceForModel(dicBinanceTrader);
            var dataTraderDataPerformanceBinanceAudit = await _loadTraderPerformanceService.GetAllTraderDataPerformanceBinanceForModelAudit(dicBinanceTrader);
            dataTraderDataPerformanceBinance = dataTraderDataPerformanceBinance.Union(dataTraderDataPerformanceBinanceAudit).ToList();

            int minTotalTopTrader = int.Parse(_configuration.GetSection("BinanceBybitSettings:MinTotalTopTrader").Value);
            var dictionaryData = dataTraderDataPerformanceBinance.Where(p=>!string.IsNullOrEmpty(p.EncryptedUid) )
             .GroupBy(data => data.EncryptedUid)  // Agrupamos por EncryptedUid
             .Where(group => group.Count() > minTotalTopTrader)  // Filtramos los grupos donde el count > 5 porque a estado en el top  de tradores binance mas de 5 veces
             .ToDictionary(
                 group => group.Key,              // La clave del diccionario es EncryptedUid
                 group => group.Count()           // El valor del diccionario es el conteo de elementos en el grupo
             );

            dataTraderDataPerformanceBinance = dataTraderDataPerformanceBinance.Where(p=>(dictionaryData.ContainsKey( p.EncryptedUid)  )).ToList();

            dataTraderDataPerformanceBinance = await _loadTraderPerformanceService.CalculateIncreasingROIs(dataTraderDataPerformanceBinance);
            await _loadTraderPerformanceService.ModelIsTopTraderScore(dataTraderDataPerformanceBinance);
            await _loadTraderPerformanceService.ReloadModelIsTopTraderScore();

            await _loadTraderPerformanceService.ModelIsMonthlY_ROI_Increasing(dataTraderDataPerformanceBinance);
            await _loadTraderPerformanceService.ReloadModelIsMonthlY_ROI_Increasing();

            await _loadTraderPerformanceService.ModelIsDailY_ROI_Increasing(dataTraderDataPerformanceBinance);
            await _loadTraderPerformanceService.ReloadModelIsDailY_ROI_Increasing();

            await _loadTraderPerformanceService.ModelIsYearlY_ROI_Increasing(dataTraderDataPerformanceBinance);
            return true;
        }
    }
}
