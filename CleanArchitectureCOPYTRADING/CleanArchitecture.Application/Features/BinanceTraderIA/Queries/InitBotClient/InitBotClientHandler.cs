using AutoMapper;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Features.Binance.Commands.CreatePosition;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Features.Binance.Queries.InitBotClient
{
    public class LoadPositionHandler : IRequestHandler<InitBotClientQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        private readonly ILogger<LoadPositionHandler> _logger;
        private readonly ITelegrameBotService _telegrameBotService;

        public LoadPositionHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<LoadPositionHandler> logger, ITelegrameBotService telegrameBotService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _telegrameBotService = telegrameBotService ?? throw new ArgumentException(nameof(telegrameBotService));
        }

        public async Task<bool> Handle(InitBotClientQuery request, CancellationToken cancellationToken)
        {
            await _telegrameBotService.InitBotClient();
            return true;
   ;
        }
    }
}
