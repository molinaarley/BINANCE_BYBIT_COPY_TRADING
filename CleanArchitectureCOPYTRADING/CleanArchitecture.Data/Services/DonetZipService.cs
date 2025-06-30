using System.Collections.Concurrent;
using CleanArchitecture.Application.Contracts.Infrastructure;
using Ionic.Zip;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace CleanArchitecture.Infrastructure.Services
{

    public class DonetZipService : IDonetZipService
    {
        public ILogger<TelegrameBotService> _logger { get; }
        //private  ITelegramBotClient _telegramBotClient;
        private readonly ITelegramBotClientProvider _telegramBotClientProvider;

        private ConcurrentDictionary<long, long> _chatIdBotClient;
        private TelegramBotClient _telegramBotClient;

        public DonetZipService() { }

        public DonetZipService(ILogger<TelegrameBotService> logger, ITelegramBotClientProvider telegramBotClientProvider/*ITelegramBotClient telegramBotClient*/)
        {
            _logger = logger;
            // _telegramBotClient = telegramBotClient;

            _telegramBotClientProvider = telegramBotClientProvider;
            _telegramBotClient = _telegramBotClientProvider.GetTelegramBotClient();
            _chatIdBotClient = new ConcurrentDictionary<long, long>();
        }
        public async Task<bool> ZipFolderPosition(string filepath,string destinationZip)
        {
            using(ZipFile zip = new ZipFile())
                {
                  zip.AddSelectedFiles("*.txt", filepath, "");
                  zip.AddSelectedFiles("*.json", filepath, "");
                  zip.Save( string.Concat(destinationZip, string.Concat(DateTime.Now.ToString("yyyy-MM-dd-hh-mm"), "_Position.zip")));
               
                }
            return true;
        }
    }      
}
