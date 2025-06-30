using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CleanArchitecture.Infrastructure.Services
{

    public class TelegramBotClientProvider : ITelegramBotClientProvider
    {
        private readonly string _botKey;

        public TelegramBotClientProvider(string botKey)
        {
            _botKey = botKey;
        }

        public TelegramBotClient GetTelegramBotClient()
        {
            return new TelegramBotClient(_botKey);
        }
    }
}
