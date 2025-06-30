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

namespace CleanArchitecture.Application.Features.Binance.Queries.LoadTraderPerformanceZip
{
    public class LoadTraderPerformanceZipHandler : IRequestHandler<LoadTraderPerformanceZipQuery, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITelegrameBotService _telegrameBotService;
        private string _pathDataJson = @"C:\worck\BINANCE_DATA_TRADER_INFO\";
        private string _pathDataJsonZip = @"C:\worck\BINANCE_DATA_TRADER_INFO_ZIP\";
        private readonly IDonetZipService _donetZipService;
        private readonly IFileService _fileService;

      
        private readonly IBinanceTraderPerformanceService _loadTraderPerformanceService;
        public LoadTraderPerformanceZipHandler(IUnitOfWork unitOfWork,  ITelegrameBotService telegrameBotService,
            IDonetZipService donetZipService, IFileService fileService,
            IBinanceTraderPerformanceService loadTraderPerformanceService)
        {

            _unitOfWork = unitOfWork;
            _telegrameBotService = telegrameBotService ?? throw new ArgumentException(nameof(telegrameBotService));
            _donetZipService = donetZipService ?? throw new ArgumentException(nameof(donetZipService));
            _fileService = fileService ?? throw new ArgumentException(nameof(fileService));
            _loadTraderPerformanceService = loadTraderPerformanceService ?? throw new ArgumentException(nameof(loadTraderPerformanceService));
        }

        public async Task<int> Handle(LoadTraderPerformanceZipQuery request, CancellationToken cancellationToken)
        {
            List<string> fileJson = Directory.GetFiles(_pathDataJson, "*.*").ToList();

            if (fileJson.Any())
            {
                await _loadTraderPerformanceService.DeletedOldPerformanceItem(request.Guid);

                await _donetZipService.ZipFolderPosition(_pathDataJson, _pathDataJsonZip);
                await _fileService.DeleteAllFiles(_pathDataJson);
            }
            return fileJson.Count;

        }
    }
}
