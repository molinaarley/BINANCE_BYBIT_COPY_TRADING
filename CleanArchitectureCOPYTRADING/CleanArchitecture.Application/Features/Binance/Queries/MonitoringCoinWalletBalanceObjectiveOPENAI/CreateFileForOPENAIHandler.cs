using System.ClientModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using AutoMapper;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Application.Models.Identity;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.ML;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Files;
#pragma warning disable OPENAI001

namespace CleanArchitecture.Application.Features.Binance.Queries.MonitoringCoinWalletBalanceObjectiveOPENAI
{
    public class CreateFileForOPENAIHandler : IRequestHandler<CreateFileForOPENAIQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITelegrameBotService _telegrameBotService;
        private string _pathDataJson = @"C:\worck\BINANCE_DATA\";
        private string _pathDataJsonZip = @"C:\worck\BINANCE_DATA_ZIP\";
        private readonly IDonetZipService _donetZipService;
        private readonly IFileService _fileService;
        private readonly IByBitApiService _byBitApiService;
        private readonly IBinanceTraderService _binanceTraderService;
        private readonly IBinanceOrderService _binanceOrderService;
        private readonly IBinanceByBitUsersService _binanceByBitUsersService;
        private readonly IBinanceByBitOrderService _binanceByBitOrderService;
        private readonly IBinanceMonitoringProcessService _binanceMonitoringProcessService;
        private readonly IBinanceTraderPerformanceService _loadTraderPerformanceService;
        private readonly IBinanceMonitoringCoinWalletBalanceObjectiveProcessService _monitoringCoinWalletBalanceObjectiveProcessService;
        private readonly IBinanceTraderUrlForUpdatePositionBinanceQueryService _binanceTraderUrlForUpdatePositionBinanceQueryService;
        public readonly BinanceBybitSettings _binanceBybitSettings;
        private readonly JwtSettings _jwtSettings;
        public IConfiguration _configuration { get; }
        private readonly OpenAIClient _openAIClient;


        public CreateFileForOPENAIHandler(IOptions<JwtSettings> jwtSettings, IOptions<BinanceBybitSettings> binanceBybitSettings, IUnitOfWork unitOfWork,  ITelegrameBotService telegrameBotService,
            IDonetZipService donetZipService, IFileService fileService, IByBitApiService byBitService,
            IBinanceTraderService binanceTraderService, IBinanceOrderService binanceOrderService,
            IBinanceByBitUsersService binanceByBitUsersService,
            IBinanceByBitOrderService binanceByBitOrderService,
            IBinanceMonitoringProcessService binanceMonitoringProcessService,
            IBinanceTraderPerformanceService loadTraderPerformanceService,
            IConfiguration configuration, IBinanceMonitoringCoinWalletBalanceObjectiveProcessService monitoringCoinWalletBalanceObjectiveProcessService,
            IBinanceTraderUrlForUpdatePositionBinanceQueryService binanceTraderUrlForUpdatePositionBinanceQueryService, OpenAIClient openAIClient)
        {
            _binanceTraderUrlForUpdatePositionBinanceQueryService = binanceTraderUrlForUpdatePositionBinanceQueryService ?? throw new ArgumentException(nameof(binanceTraderUrlForUpdatePositionBinanceQueryService));
            _monitoringCoinWalletBalanceObjectiveProcessService = monitoringCoinWalletBalanceObjectiveProcessService ?? throw new ArgumentException(nameof(monitoringCoinWalletBalanceObjectiveProcessService));
            _donetZipService = donetZipService ?? throw new ArgumentException(nameof(donetZipService));
            _unitOfWork = unitOfWork;
            _binanceBybitSettings = binanceBybitSettings.Value;
            _jwtSettings = jwtSettings.Value;

            _telegrameBotService = telegrameBotService ?? throw new ArgumentException(nameof(telegrameBotService));
            _donetZipService = donetZipService ?? throw new ArgumentException(nameof(donetZipService));
            _fileService = fileService ?? throw new ArgumentException(nameof(fileService));
            _byBitApiService = byBitService ?? throw new ArgumentException(nameof(byBitService));
            _binanceTraderService = binanceTraderService ?? throw new ArgumentException(nameof(binanceTraderService));
            _binanceOrderService = binanceOrderService ?? throw new ArgumentException(nameof(binanceOrderService));
            _binanceByBitUsersService = binanceByBitUsersService ?? throw new ArgumentException(nameof(binanceByBitUsersService));
            _binanceByBitOrderService = binanceByBitOrderService ?? throw new ArgumentException(nameof(binanceByBitOrderService));
            _binanceMonitoringProcessService = binanceMonitoringProcessService ?? throw new ArgumentException(nameof(binanceMonitoringProcessService));
            _loadTraderPerformanceService = loadTraderPerformanceService ?? throw new ArgumentException(nameof(loadTraderPerformanceService));
            _configuration = configuration ?? throw new ArgumentException(nameof(configuration));
            _openAIClient = openAIClient;
        }

        public async Task<bool> Handle(CreateFileForOPENAIQuery request, CancellationToken cancellationToken)
        {
            int BatchSize = 50;

            var selectedTraders = await _loadTraderPerformanceService.GetAlldataTraderDataPerformanceBinanceAndTraderDataPerformanceBinanceAudit();
           
            string jsonLista = JsonConvert.SerializeObject(selectedTraders, Formatting.Indented);
            File.WriteAllText(string.Concat( @"C:\worck\BINANCE_MODEL_OPEN_AI\", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), "_selectedTraders.json"), jsonLista) ;

            // OpenAIClient openAIClient = new("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");
           // OpenAIFileClient fileClient = _openAIClient.GetOpenAIFileClient();
            // AssistantClient assistantClient = _openAIClient.GetAssistantClient();
          //  AssistantClient assistantClient = _openAIClient.GetAssistantClient();

            /*
            // 🔄 **Iteramos hasta que queden solo 5 traders**
            while (selectedTraders.Count > 5)
            {
                List<TraderDataPerformanceBinance> nextSelection = new List<TraderDataPerformanceBinance>();

                for (int i = 0; i < selectedTraders.Count; i += BatchSize)
                {
                    var batch = selectedTraders.GetRange(i, Math.Min(BatchSize, selectedTraders.Count - i));

                    Console.WriteLine($"🔄 Procesando lote {i / BatchSize + 1} con {batch.Count} traders");

                    // Convertir lote a JSON
                    string jsonData = JsonConvert.SerializeObject(new { description = "Trader performance data for Binance.", traders = batch });
                    using Stream document = BinaryData.FromBytes(System.Text.Encoding.UTF8.GetBytes(jsonData)).ToStream();

                    OpenAIFile tradersFile = fileClient.UploadFile(document, "trader_performance.json", FileUploadPurpose.Assistants);
                    Console.WriteLine($"✅ Archivo subido con éxito. ID: {tradersFile.Id}");

                    // 🔹 Crear asistente con instrucciones mejoradas
                    AssistantCreationOptions assistantOptions = new()
                    {
                        Name = "Trader Performance Analyzer",
                        Instructions =
                            "You are a financial analyst specializing in copy trading. The uploaded file contains a JSON object with key 'traders', an array of trader performance data. "
                            + "Analyze the traders and return the top 5 EncryptedUid values with the best probability of making a profit today, based on daily, weekly, and monthly ROI and PNL trends. "
                            + "Return only their EncryptedUid values separated by '#', without any explanation.",
                        Tools = { new FileSearchToolDefinition() },
                        ToolResources = new()
                        {
                            FileSearch = new()
                            {
                                NewVectorStores = { new VectorStoreCreationHelper([tradersFile.Id]) }
                            }
                        },
                    };

                    Assistant assistant = assistantClient.CreateAssistant("gpt-4o", assistantOptions);

                    // 🔹 Crear consulta para seleccionar los mejores del lote
                    ThreadCreationOptions threadOptions = new()
                    {
                        InitialMessages = {
                        "Analyze the uploaded trader performance data and return the top 5 traders expected to be profitable today. "
                        + "Return only their EncryptedUid values separated by '#', without any explanation."
                    }
                    };

                    // 🔹 Ejecutar consulta al asistente
                    ThreadRun threadRun = assistantClient.CreateThreadAndRun(assistant.Id, threadOptions);

                    // 🔄 Esperar respuesta del asistente
                    do
                    {
                        Console.WriteLine($"⏳ Esperando respuesta... Estado actual: {threadRun.Status}");
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        threadRun = assistantClient.GetRun(threadRun.ThreadId, threadRun.Id);
                    } while (!threadRun.Status.IsTerminal);

                    Console.WriteLine($"✅ Respuesta lista. Estado final: {threadRun.Status}");

                    // 📩 Obtener la respuesta del asistente
                    CollectionResult<ThreadMessage> messages =
                        assistantClient.GetMessages(threadRun.ThreadId, new MessageCollectionOptions() { Order = MessageCollectionOrder.Ascending });

                    foreach (ThreadMessage message in messages)
                    {
                        if (message.Role == MessageRole.Assistant) // Filtrar solo la respuesta del asistente
                        {
                            foreach (MessageContent contentItem in message.Content)
                            {
                                if (!string.IsNullOrEmpty(contentItem.Text))
                                {
                                    Console.WriteLine($"🔹 Respuesta del Asistente: {contentItem.Text}");
                                    var tradersFromResponse = contentItem.Text.Split('#');
                                    foreach (var trader in tradersFromResponse)
                                    {
                                        string traderId = trader.Trim();
                                        var foundTrader = batch.FirstOrDefault(t => t.EncryptedUid == traderId);
                                        if (foundTrader != null)
                                        {
                                            nextSelection.Add(foundTrader);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // 🧹 Borrar archivos para evitar sobrecarga en OpenAI
                    _ = fileClient.DeleteFileAsync(tradersFile.Id);
                }

                // Si la selección de la iteración es menor a 5, terminamos con los que tenemos
                if (nextSelection.Count <= 5)
                {
                    selectedTraders = nextSelection;
                    break;
                }

                // Guardar los mejores traders para la siguiente iteración
                selectedTraders = new List<TraderDataPerformanceBinance>(nextSelection);
            }

            */

            


            return true;
            var allUserActive = await _binanceByBitUsersService.GetAllIsactive();
           

            foreach (var itemUser in allUserActive)
            {
                var lastIsIngProcessForObjective = await _monitoringCoinWalletBalanceObjectiveProcessService.GetLastIsIngProcessForObjective(itemUser.IdTelegrame);

                //NOUS OBTENONS LE MONTANT D'ARGENT QUE VOUS AVEZ SUR VOTRE COMPTE BYBIT
                var walletBalance = await _byBitApiService.WalletBalance(itemUser.ApiKey, itemUser.SecretKey);
                while (walletBalance == null)
                {
                    Thread.Sleep(5000);
                    walletBalance = await _byBitApiService.WalletBalance(itemUser.ApiKey, itemUser.SecretKey);

                    // code block to be executed
                }
                var coinWalletBalance = await _byBitApiService.GetCoinFromWalletBalance(walletBalance);

                if (lastIsIngProcessForObjective == null || lastIsIngProcessForObjective.EndDate.HasValue)
                {
                
                var result=   await _monitoringCoinWalletBalanceObjectiveProcessService.Create(new BinanceMonitoringCoinWalletBalanceObjectiveProcess()
                   {
                       CreatedOn = DateTime.Now,
                       Equity = coinWalletBalance.equity.Value,
                       EquityObjective = coinWalletBalance.equity.Value + int.Parse(_configuration.GetSection("BinanceBybitSettings:BinanceObjectiveDailY").Value),
                       IdTelegrame = itemUser.IdTelegrame,
                       UnrealisedPnl = coinWalletBalance.unrealisedPnl.Value,
                       WalletBalance = coinWalletBalance.walletBalance.Value,
                       EndDate=DateTime.Now
                   } );
                    await _binanceTraderUrlForUpdatePositionBinanceQueryService.AddEncryptedUidList(await _loadTraderPerformanceService.TraderUrlForUpdatePositionBinance(),result.Id);
                    result.EndDate = (DateTime?)null;
                    await _monitoringCoinWalletBalanceObjectiveProcessService.Update(result);
                }
                else
                {
                    if (lastIsIngProcessForObjective != null && !lastIsIngProcessForObjective.EndDate.HasValue)
                    {
                       

                        var resultProcess = await _monitoringCoinWalletBalanceObjectiveProcessService.Create(new BinanceMonitoringCoinWalletBalanceObjectiveProcess()
                        {
                            CreatedOn = DateTime.Now,
                            Equity = coinWalletBalance.equity.Value,
                            EquityObjective = coinWalletBalance.equity.Value + int.Parse(_configuration.GetSection("BinanceBybitSettings:BinanceObjectiveDailY").Value),
                            IdTelegrame = itemUser.IdTelegrame,
                            UnrealisedPnl = coinWalletBalance.unrealisedPnl.Value,
                            WalletBalance = coinWalletBalance.walletBalance.Value,
                            EndDate=DateTime.Now
                        });
                        await _binanceTraderUrlForUpdatePositionBinanceQueryService.AddEncryptedUidList(await _loadTraderPerformanceService.TraderUrlForUpdatePositionBinance(), resultProcess.Id);
                        resultProcess.EndDate = (DateTime?)null;
                        await _monitoringCoinWalletBalanceObjectiveProcessService.Update(resultProcess);

                        lastIsIngProcessForObjective.EndDate = DateTime.Now;
                        await _monitoringCoinWalletBalanceObjectiveProcessService.Update(lastIsIngProcessForObjective);
                    }

                   
                }
            }
            return true;
        }
    }
}
