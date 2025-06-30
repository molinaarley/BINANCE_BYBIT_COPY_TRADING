using CleanArchitecture.Application.Models;
using Telegram.Bot;

namespace CleanArchitecture.Application.Contracts.Infrastructure
{
    public interface ITelegramBotClientProvider
    {
        TelegramBotClient GetTelegramBotClient();
    }

}
