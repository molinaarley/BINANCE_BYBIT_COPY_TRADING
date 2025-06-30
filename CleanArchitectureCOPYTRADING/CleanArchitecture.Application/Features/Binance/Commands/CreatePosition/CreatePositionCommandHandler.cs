using AutoMapper;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Features.Binance.Commands.CreatePosition
{
    public class CreatePositionCommandHandler : IRequestHandler<CreatePositionCommand, bool>
    {
        //private readonly IStreamerRepository _streamerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailservice;
        private readonly ILogger<CreatePositionCommandHandler> _logger;
        private readonly ITelegrameBotService _telegrameBotService;
        

        public CreatePositionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailservice,
            ITelegrameBotService telegrameBotService, ILogger<CreatePositionCommandHandler> logger)
        {
            //_streamerRepository = streamerRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailservice = emailservice;
            _telegrameBotService = telegrameBotService ?? throw new ArgumentException(nameof(telegrameBotService));
            _logger = logger;
        }

        public async Task<bool> Handle(CreatePositionCommand request, CancellationToken cancellationToken)
        {
            _telegrameBotService.InitBotClient();
            //var streamerEntity = _mapper.Map<Streamer>(request);
            return true;
        }
    }
}
