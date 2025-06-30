using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using AutoMapper.Execution;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CleanArchitecture.Infrastructure.Services
{

    public class TelegrameBotService : ITelegrameBotService
    {
        public ILogger<TelegrameBotService> _logger { get; }
        //private  ITelegramBotClient _telegramBotClient;
        private readonly ITelegramBotClientProvider _telegramBotClientProvider;
        private readonly IBinanceByBitUsersRepository _binanceByBitUsersRepository;
        
        private readonly IBinanceContext _binanceContext;


        private ConcurrentDictionary<long, long> _chatIdBotClient;
        
        private TelegramBotClient _telegramBotClient;
        private string _pathDataJson = @"C:\worck\BINANCE_DATA\";


        

        public TelegrameBotService(ILogger<TelegrameBotService> logger,
            ITelegramBotClientProvider telegramBotClientProvider,
            IBinanceByBitUsersRepository binanceByBitUsersRepository)
        {
            _logger = logger;
          //  _binanceByBitUsersRepository = binanceByBitUsersRepository ?? throw new ArgumentException(nameof(binanceByBitUsersRepository));
            // _telegramBotClient = telegramBotClient;

            _telegramBotClientProvider = telegramBotClientProvider;
            _telegramBotClient = _telegramBotClientProvider.GetTelegramBotClient();
            _chatIdBotClient = new ConcurrentDictionary<long, long>();
            
            _binanceByBitUsersRepository = binanceByBitUsersRepository ?? throw new ArgumentException(nameof(binanceByBitUsersRepository));

            var allByBitUsers=  _binanceByBitUsersRepository.GetAllInDictionary().Result;
            foreach (var item in allByBitUsers)
            {
                _chatIdBotClient.TryAdd(item.Key,item.Key);
            }

            //_chatIdBotClient.TryAdd(5273492949, 5273492949);
            //_chatIdBotClient.TryAdd(1476318624, 1476318624);


        }

        public async Task<List<PositionData>> LoadPosition(string pathDataJson)
        {
                List<PositionData> listPositionData = new List<PositionData>();
                List<string> fileEntries = Directory.GetFiles(pathDataJson, "*.txt").Where(p => !p.Contains("encryptedUid")).ToList();
                
            // var dicFileEntriesType = fileEntriesType.
            //   ToDictionary(p => p.Split('_')[0]);

            if (!fileEntries.Any())
            {
                return listPositionData;
            }
          
               
                RootInfoUser rootInfoUser = new RootInfoUser();

            foreach (var item in fileEntries)
            {
                var fileinfo = new FileInfo(item);
                string codeUser = fileinfo.Name.Split('_')[0];
            

                List<string> listPositionDataJson = System.IO.File.ReadAllLines(item).ToList();
                string positionsstr = listPositionDataJson.Where(p => p.Contains("otherPositionRetList")).FirstOrDefault();
                string rootInfoUserJson = listPositionDataJson.Where(p => p.Contains("nickName")).FirstOrDefault();

                if (!string.IsNullOrEmpty(positionsstr))
                {
                    if (!string.IsNullOrEmpty(rootInfoUserJson))
                    {
                        rootInfoUser = JsonConvert.DeserializeObject<RootInfoUser>(rootInfoUserJson);
                    }
                    PositionData positionData = JsonConvert.DeserializeObject<PositionData>(positionsstr);

                   // positionData.LongShort = new Dictionary<string, string>();
                   // positionData.LongShort = longShort;

                    if (rootInfoUser != null && rootInfoUser.data != null)
                    {
                        positionData.nickName = rootInfoUser.data.nickName;
                    }
                    positionData.code = codeUser;

                    if (positionData.data != null && positionData.data.otherPositionRetList != null
                        && positionData.data.otherPositionRetList.Any())
                    {
                        positionData.data.otherPositionRetList = positionData.data.otherPositionRetList./*.
                            Where(p=>p.pnl>0  && new DateTime(p.updateTime[0], p.updateTime[1],
                                   p.updateTime[2], p.updateTime[3], p.updateTime[4], p.updateTime[5])>DateTime.Now.AddDays(-21)

                            )*/
                            Select(p => new OtherPositionRetList()
                            {
                                symbol = p.symbol,
                                entryPrice = p.entryPrice,
                                markPrice = p.markPrice,
                                pnl = p.pnl,
                                roe = p.roe,
                                updateTime = p.updateTime,
                                amount = p.amount,
                                updateTimeStamp = p.updateTimeStamp,
                                yellow = p.yellow,
                                tradeBefore = p.tradeBefore,
                                leverage = p.leverage,
                                side = p.amount < 0 ? EnumConverter.GetString(OrderSide.Sell) : EnumConverter.GetString(OrderSide.Buy) //positionData.LongShort.ContainsKey(p.symbol) ? positionData.LongShort[p.symbol] : string.Empty
                            }).ToList();



                        listPositionData.Add(positionData);
                    }
                    else
                    {
                        listPositionData.Add(positionData);
                    }
                }
            }
            return listPositionData;
        }

        public async Task<bool> SendDataPositions(PositionData positionData,long Id)
        {
            if (positionData != null && positionData.data != null && positionData.data.otherPositionRetList != null
                && positionData.data.otherPositionRetList.Any())
            {
                try
                {
                    using CancellationTokenSource cts = new();
                    CancellationToken cancellationToken = new CancellationToken();

                    if (_chatIdBotClient.ContainsKey(Id))
                    {
                        // string html = "Positions : \n\n";
                        StringBuilder htmlTelegram = new StringBuilder();
                        htmlTelegram.Append("New Positions : \n\n");
                        htmlTelegram.Append(string.Concat("-------**", positionData.nickName, "***********---\n"));
                        foreach (var item in positionData.data.otherPositionRetList)
                        {

                            htmlTelegram.Append(string.Concat("----------******", DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"), " * ******----------\n"));
                            htmlTelegram.Append(string.Concat("symbol : ", item.symbol, "\n"));
                            htmlTelegram.Append(string.Concat("entryPrice : ", item.entryPrice, "\n"));
                            htmlTelegram.Append(string.Concat("markPrice : ", item.markPrice, "\n"));
                            htmlTelegram.Append(string.Concat("pnl : ", item.pnl, "\n"));
                            htmlTelegram.Append(string.Concat("roe : ", item.roe, "\n"));
                            htmlTelegram.Append(string.Concat("amount : ", item.amount, "\n"));
                            // htmlTelegram.Append(string.Concat("yellow : ", item.yellow, "\n"));
                            // htmlTelegram.Append(string.Concat("tradeBefore : ", item.tradeBefore, "\n"));
                            htmlTelegram.Append(string.Concat("leverage : ", item.leverage, "\n"));
                            htmlTelegram.Append(string.Concat("Long/Short : ", item.amount < 0 ? EnumConverter.GetString(OrderSide.Sell) : EnumConverter.GetString(OrderSide.Buy), "\n"));

                           
                            // Echo received message text
                            Message sentMessage = await _telegramBotClient.SendTextMessageAsync(
                            chatId: _chatIdBotClient[Id],
                            text: htmlTelegram.ToString(),
                             cancellationToken: cts.Token);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

        }
            return true;

        }



        public async Task<bool> SendOtherPositionRetList(OtherPositionRetList OtherPosition, long Id
            , Dictionary<string, string> LongShort, string nickName)
        {
            if (OtherPosition != null )
            {
                using CancellationTokenSource cts = new();
                CancellationToken cancellationToken = new CancellationToken();

                if (_chatIdBotClient.ContainsKey(Id))
                {
                    // string html = "Positions : \n\n";
                    StringBuilder htmlTelegram = new StringBuilder();
                    htmlTelegram.Append("New Positions : \n\n");
                    htmlTelegram.Append(string.Concat("-------**", nickName, "***********---\n"));

                        htmlTelegram.Append(string.Concat("----------******", DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"), " * ******----------\n"));
                        htmlTelegram.Append(string.Concat("symbol : ", OtherPosition.symbol, "\n"));
                        htmlTelegram.Append(string.Concat("entryPrice : ", OtherPosition.entryPrice, "\n"));
                        htmlTelegram.Append(string.Concat("markPrice : ", OtherPosition.markPrice, "\n"));
                        htmlTelegram.Append(string.Concat("pnl : ", OtherPosition.pnl, "\n"));
                        htmlTelegram.Append(string.Concat("roe : ", OtherPosition.roe, "\n"));
                        htmlTelegram.Append(string.Concat("amount : ", OtherPosition.amount, "\n"));
                        // htmlTelegram.Append(string.Concat("yellow : ", item.yellow, "\n"));
                        // htmlTelegram.Append(string.Concat("tradeBefore : ", item.tradeBefore, "\n"));
                        htmlTelegram.Append(string.Concat("leverage : ", OtherPosition.leverage, "\n"));
                        htmlTelegram.Append(string.Concat("Long/Short : ", OtherPosition.amount < 0 ? EnumConverter.GetString(OrderSide.Sell) : EnumConverter.GetString(OrderSide.Buy), "\n"));
                        
                        // Echo received message text
                        Message sentMessage = await _telegramBotClient.SendTextMessageAsync(
                        chatId: _chatIdBotClient[Id],
                        text: htmlTelegram.ToString(),
                         cancellationToken: cts.Token);
                }
            }
            return true;
        }



        public async Task<bool> SendDataPositions(PositionData positionData)
        {
            if (positionData != null && positionData.data != null && positionData.data.otherPositionRetList != null
                && positionData.data.otherPositionRetList.Any())
            {
               


                try
                {
                    using CancellationTokenSource cts = new();
                    CancellationToken cancellationToken = new CancellationToken();

                    foreach (var itemChatId in _chatIdBotClient)
                    {
                        // string html = "Positions : \n\n";

                        foreach (var item in positionData.data.otherPositionRetList)
                        {
                            StringBuilder htmlTelegram = new StringBuilder();
                            htmlTelegram.Append("New Positions : \n\n");
                            htmlTelegram.Append(string.Concat("-------**", positionData.nickName, "***********---\n"));

                            htmlTelegram.Append(string.Concat("----------******", DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"), " * ******----------\n"));
                            htmlTelegram.Append(string.Concat("symbol : ", item.symbol, "\n"));
                            htmlTelegram.Append(string.Concat("entryPrice : ", item.entryPrice, "\n"));
                            htmlTelegram.Append(string.Concat("markPrice : ", item.markPrice, "\n"));
                            htmlTelegram.Append(string.Concat("pnl : ", item.pnl, "\n"));
                            htmlTelegram.Append(string.Concat("roe : ", item.roe, "\n"));
                            htmlTelegram.Append(string.Concat("amount : ", item.amount, "\n"));
                            // htmlTelegram.Append(string.Concat("yellow : ", item.yellow, "\n"));
                            // htmlTelegram.Append(string.Concat("tradeBefore : ", item.tradeBefore, "\n"));
                            htmlTelegram.Append(string.Concat("leverage : ", item.leverage, "\n"));
                            htmlTelegram.Append(string.Concat("Long/Short : ", item.amount < 0 ? EnumConverter.GetString(OrderSide.Sell) : EnumConverter.GetString(OrderSide.Long), "\n"));
                           

                            // Echo received message text
                            Message sentMessage = await _telegramBotClient.SendTextMessageAsync(
                            chatId: itemChatId.Key,
                            text: htmlTelegram.ToString(),
                             cancellationToken: cts.Token);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return true;
        }
        public async Task<bool> InitBotClient()
        {
            var me = _telegramBotClient.GetMeAsync();
            string idlog = $"my id {me.Id} and my name  ";

            using CancellationTokenSource cts = new();
            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
            };

            _telegramBotClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token);
            return true;
        }



        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;
            var chatId = message.Chat.Id;
           
            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            switch (messageText.ToLower())
            {
                case "/position":
                    // code block
                    var allPosition= await LoadPosition(_pathDataJson);

                    foreach (var itemdata in allPosition)
                    {
                        await this.SendDataPositions(itemdata);
                    }
                    break;
                case "/start":
                    //_chatIdBotClient.TryAdd(chatId, chatId);
                    // Echo received message text
                    if (!_chatIdBotClient.ContainsKey(chatId))
                    {
                        await _binanceByBitUsersRepository.Create(new Domain.Binance.BinanceByBitUser()
                        {
                            IdTelegrame = message.Chat.Id,
                            Name = message.Chat.FirstName,
                            Isactive=true
                        });
                        _chatIdBotClient.TryAdd(chatId, chatId);
                    }


                    Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: @"Pour mettre à jour les nouvelles positions disponibles /position , sinon les positions seront envoyées automatiquement toutes les 1h",
                    cancellationToken: cancellationToken, parseMode: ParseMode.Html);//parseMode:ParseMode.Html
                    break;
                default:
                    // code block
                    break;
            }


        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

    }      
}

/*
agregar miembro a un grupo
var botToken = "TuTokenDeAcceso";
var botClient = new TelegramBotClient(botToken);

var chatId = "ID_del_grupo";
var userId = "ID_del_usuario_a_agregar";

await botClient.AddChatMemberAsync(chatId, userId);
 */
