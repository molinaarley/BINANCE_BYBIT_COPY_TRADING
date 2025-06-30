using System.Globalization;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;  // Importante para usar Task y Parallel
using System.Collections.Generic;
using System.Threading;  // Importante para usar SemaphoreSlim

namespace CleanArchitecture.Application.Features.Binance.Queries.ObjectiveForUpdatePosition
{
    public class ObjectiveForUpdatePositionHandler : IRequestHandler<ObjectiveForUpdatePositionQuery, bool>
    {
        private readonly ITelegrameBotService _telegrameBotService;
        private readonly IByBitApiService _byBitApiService;
        private readonly IBinanceOrderService _binanceOrderService;
        private readonly IBinanceByBitUsersService _binanceByBitUsersService;
        private readonly IBinanceByBitOrderService _binanceByBitOrderService;
        private readonly IBinanceMonitoringCoinWalletBalanceObjectiveProcessService _monitoringCoinWalletBalanceObjectiveProcessService;
        private readonly IBinanceCacheByBitSymbolService _binanceCacheByBitSymbolService;
        public IConfiguration _configuration { get; }

        public ObjectiveForUpdatePositionHandler(ITelegrameBotService telegrameBotService,
             IByBitApiService byBitService,
            IBinanceOrderService binanceOrderService,
            IBinanceByBitUsersService binanceByBitUsersService,
            IBinanceByBitOrderService binanceByBitOrderService, IBinanceMonitoringCoinWalletBalanceObjectiveProcessService monitoringCoinWalletBalanceObjectiveProcessService,
            IConfiguration configuration, IBinanceCacheByBitSymbolService binanceCacheByBitSymbolService)
        {
            _telegrameBotService = telegrameBotService ?? throw new ArgumentException(nameof(telegrameBotService));
            _byBitApiService = byBitService ?? throw new ArgumentException(nameof(byBitService));
            _binanceOrderService = binanceOrderService ?? throw new ArgumentException(nameof(binanceOrderService));
            _binanceByBitUsersService = binanceByBitUsersService ?? throw new ArgumentException(nameof(binanceByBitUsersService));
            _binanceByBitOrderService = binanceByBitOrderService ?? throw new ArgumentException(nameof(binanceByBitOrderService));
            _monitoringCoinWalletBalanceObjectiveProcessService = monitoringCoinWalletBalanceObjectiveProcessService ?? throw new ArgumentException(nameof(monitoringCoinWalletBalanceObjectiveProcessService));
            _configuration = configuration ?? throw new ArgumentException(nameof(configuration));
            _binanceCacheByBitSymbolService = binanceCacheByBitSymbolService ?? throw new ArgumentException(nameof(binanceCacheByBitSymbolService));
        }

        public async Task<bool> Handle(ObjectiveForUpdatePositionQuery request, CancellationToken cancellationToken)
        {
            // OBTENIR DES UTILISATEURS QUI ONT PAYÉ LA REDEVANCE D'ABONNEMENT
            var allUserActive = await _binanceByBitUsersService.GetAllIsactive();
            List<BinanceOrder> binanceOrderBD = await _binanceOrderService.GetAll();

            // Inicializamos el semáforo para permitir 3 tareas concurrentes
            SemaphoreSlim semaphore = new SemaphoreSlim(int.Parse(_configuration.GetSection("BinanceBybitSettings:TotalParallelProcessesObjectiveForUpdatePositionHandler").Value) );

            // Lista para almacenar las tareas
            List<Task> tasks = new List<Task>();

            foreach (var itemUser in allUserActive)
            {


                // NOUS OBTENONS LE MONTANT D'ARGENT QUE VOUS AVEZ SUR VOTRE COMPTE BYBIT
                var walletBalance = await _byBitApiService.WalletBalance(itemUser.ApiKey, itemUser.SecretKey);
                var coinWalletBalance = await _byBitApiService.GetCoinFromWalletBalance(walletBalance);

                // verificamos si estamos buscando el objetivo
                bool IsIngProcessForObjective = (await _monitoringCoinWalletBalanceObjectiveProcessService.IsIngProcessForObjective(long.Parse(_configuration.GetSection("BinanceBybitSettings:IdTelegrameMain").Value), coinWalletBalance));
                if (!IsIngProcessForObjective)
                {
                    return false;
                }


                var getFeeRateInfo = await _byBitApiService.GetFeeRate(new GetFeeRateQuery()
                {
                    category = "linear",
                    apiKey = itemUser.ApiKey,
                    secretKey = itemUser.SecretKey
                });

                var getInstrumentsInfo = (await _byBitApiService.GetInstrumentsInfo(new GetInstrumentsInfoQuery()
                {
                    apiKey = itemUser.ApiKey,
                    secretKey = itemUser.SecretKey,
                    category = EnumConverter.GetString(Category.Linear)
                })).result.list.GroupBy(x => x.symbol).ToDictionary(p => p.Key, p => p.FirstOrDefault());

               

                // Crear una tarea para cada operación en paralelo, con control de concurrencia
                foreach (var itembinanceOrder in binanceOrderBD)
                {
                    if (!string.IsNullOrEmpty(itembinanceOrder.Symbol))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            await semaphore.WaitAsync();  // Adquirir semáforo

                            try
                            {
                                var tickersCategorySymbol = await _byBitApiService.GetTickersCategorySymbol(new GetTickers() { category = "inverse", symbol = itembinanceOrder.Symbol });

                                var positionBybitInfo = await _byBitApiService.GetPositionInfo(new GetPositionInfo()
                                {
                                    category = EnumConverter.GetString(Category.Linear),
                                    symbol = itembinanceOrder.Symbol,
                                    apiKey = itemUser.ApiKey,
                                    secretKey = itemUser.SecretKey
                                });

                                if (positionBybitInfo != null && positionBybitInfo.list != null && positionBybitInfo.list.Any() &&
                                    !positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.None)))
                                {
                                    if (positionBybitInfo.list.FirstOrDefault().unrealisedPnl > 5)
                                    {
                                        PlaceOrder placeOrder = new PlaceOrder();
                                        placeOrder = await _binanceOrderService.GetPlaceOrderForDeleted(positionBybitInfo, itemUser.ApiKey, itemUser.SecretKey, 0);
                                        placeOrder.orderType = OrderType.Limit.ToString();

                                        if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.Buy)))
                                        {
                                            placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice - (positionBybitInfo.list.FirstOrDefault().markPrice * getInstrumentsInfo[placeOrder.symbol].priceFilter.tickSize);
                                        }

                                        if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.Sell)))
                                        {
                                            placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice + (positionBybitInfo.list.FirstOrDefault().markPrice * getInstrumentsInfo[placeOrder.symbol].priceFilter.tickSize);
                                        }

                                        if (positionBybitInfo.list.FirstOrDefault().side.Equals(EnumConverter.GetString(OrderSide.None)))
                                        {
                                            placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice;
                                        }

                                        if (string.IsNullOrEmpty(positionBybitInfo.list.FirstOrDefault().side))
                                        {
                                            placeOrder.price = positionBybitInfo.list.FirstOrDefault().markPrice;
                                            placeOrder.side = EnumConverter.GetString(OrderSide.None);
                                        }

                                        if (tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value.ToString(CultureInfo.InvariantCulture).Contains("."))
                                        {
                                            placeOrder.price = Math.Round(placeOrder.price.Value, tickersCategorySymbol.result.list.FirstOrDefault().lastPrice.Value.ToString(CultureInfo.InvariantCulture).Split('.')[1].Length, MidpointRounding.ToEven);
                                        }
                                        else
                                        {
                                            placeOrder.price = Math.Round(placeOrder.price.Value, 0, MidpointRounding.ToEven);
                                        }

                                        var placeOrderByBit = await _byBitApiService.PlaceOrder(placeOrder);

                                        // itembinanceOrder.BinanceByBitOrders
                                        await _binanceByBitOrderService.DeletedRange(itembinanceOrder.BinanceByBitOrders.ToList());
                                        await _binanceOrderService.DeletedOrder(itembinanceOrder);

                                        await _binanceCacheByBitSymbolService.Add(itembinanceOrder.Symbol);

                                        File.AppendAllLines("Deleted_ObjectiveForUpdatePositionQuery.txt", new[] { string.Concat(" Deleted_ObjectiveForUpdatePositionQuery (Handle) =>date: ", DateTime.Now.ToString(), " guid:  ", itembinanceOrder.EncryptedUid) });
                                    }
                                }
                            }
                            finally
                            {
                                semaphore.Release();  // Liberar el semáforo
                            }
                        }));
                    }
                }
            }

            // Esperar a que todas las tareas se completen
            await Task.WhenAll(tasks);

            return true;
        }
    }
}
