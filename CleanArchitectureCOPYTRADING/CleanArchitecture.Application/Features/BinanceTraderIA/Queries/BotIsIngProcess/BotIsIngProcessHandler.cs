using AutoMapper;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Features.Binance.Commands.CreatePosition;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Features.Binance.Queries.BotIsIngProcess
{
    public class BotIsIngProcessHandler : IRequestHandler<BotIsIngProcessQuery, bool>
    {
        private readonly IMapper _mapper;

        private readonly ILogger<BotIsIngProcessHandler> _logger;
        private readonly IBinanceMonitoringProcessService _binanceMonitoringProcessService;

        public BotIsIngProcessHandler(IMapper mapper, ILogger<BotIsIngProcessHandler> logger, IBinanceMonitoringProcessService binanceMonitoringProcessService)
        {
            _mapper = mapper;
            _logger = logger;
            _binanceMonitoringProcessService = binanceMonitoringProcessService ?? throw new ArgumentException(nameof(binanceMonitoringProcessService));
        }

        public async Task<bool> Handle(BotIsIngProcessQuery request, CancellationToken cancellationToken)
        {
            return false;
           return await _binanceMonitoringProcessService.GetIsIngProcess();
        }
    }
}
