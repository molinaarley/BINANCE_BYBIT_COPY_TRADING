using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using MediatR;
using Microsoft.Extensions.Configuration;


namespace CleanArchitecture.Application.Features.Binance.Queries.ObjectiveForUpdatePositionWithTrailingStop
{
    public class ObjectiveForUpdatePositionWithTrailingStopHandler : IRequestHandler<ObjectiveForUpdatePositionWithTrailingStopQuery, bool>
    {
        private readonly ITelegrameBotService _telegrameBotService;
        private readonly IByBitApiService _byBitApiService;
        private readonly IBinanceOrderService _binanceOrderService;
        private readonly IBinanceByBitUsersService _binanceByBitUsersService;
        private readonly IBinanceByBitOrderService _binanceByBitOrderService;
        private readonly IBinanceMonitoringCoinWalletBalanceObjectiveProcessService _monitoringCoinWalletBalanceObjectiveProcessService;
        private readonly IBinanceCacheByBitSymbolService _binanceCacheByBitSymbolService;
        public IConfiguration _configuration { get; }

        public ObjectiveForUpdatePositionWithTrailingStopHandler(ITelegrameBotService telegrameBotService,
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

        public async Task<bool> Handle(ObjectiveForUpdatePositionWithTrailingStopQuery request, CancellationToken cancellationToken)
        {
            // OBTENIR DES UTILISATEURS QUI ONT PAYÉ LA REDEVANCE D'ABONNEMENT
            var allUserActive = await _binanceByBitUsersService.GetAllIsactive();
            List<BinanceOrder> binanceOrderBD = await _binanceOrderService.GetAll();

            // Inicializamos el semáforo para permitir 3 tareas concurrentes
            SemaphoreSlim semaphore = new SemaphoreSlim(int.Parse(_configuration.GetSection("BinanceBybitSettings:TotalParallelProcessesObjectiveForUpdatePositionHandler").Value));

            // Lista para almacenar las tareas
            List<Task> tasks = new List<Task>();

            foreach (var itemUser in allUserActive)
            {
                var walletBalance = await _byBitApiService.WalletBalance(itemUser.ApiKey, itemUser.SecretKey);
                var coinWalletBalance = await _byBitApiService.GetCoinFromWalletBalance(walletBalance);

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

                bool IsIngProcessForObjective = (await _monitoringCoinWalletBalanceObjectiveProcessService.IsIngProcessForObjective(
                    long.Parse(_configuration.GetSection("BinanceBybitSettings:IdTelegrameMain").Value), coinWalletBalance));

                if (!IsIngProcessForObjective)
                {
                    return false;
                }

                foreach (var itembinanceOrder in binanceOrderBD)
                {
                    if (!string.IsNullOrEmpty(itembinanceOrder.Symbol))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            await semaphore.WaitAsync();  // Adquirir semáforo

                            try
                            {
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
                                    var currentPosition = positionBybitInfo.list.FirstOrDefault();

                                    if (currentPosition.unrealisedPnl.HasValue)
                                    {
                                        // Calcula el porcentaje correspondiente posicion de 160 euros (4.165% de la posición=7 EURO)(6.25% de la posición=10 EURO)(2.0825% de la posición=3.332 EURO)
                                        double percentageProfitThreshold = 4.165;//2.0825

                                        // Calcula el valor equivalente al porcentaje del tamaño de la posición
                                        double unrealisedPnl = currentPosition.unrealisedPnl.Value * 100 / currentPosition.positionValue.Value;

                                        if (unrealisedPnl > percentageProfitThreshold)
                                        {
                                            double trailingStopDistance = 0.0005; // Distancia del trailing stop del 0.05% para mantener el stop loss más cercano al precio de mercado


                                            // Calcular el precio del trailing stop
                                            double? stopPrice = null;
                                            if (currentPosition.side.Equals(EnumConverter.GetString(OrderSide.Buy)))
                                            {
                                                double proposedStopPrice = currentPosition.markPrice.Value - (currentPosition.markPrice.Value * trailingStopDistance);

                                                // Solo mover el stop loss si el nuevo precio es mayor al anterior
                                                if (!currentPosition.stopLoss.HasValue || proposedStopPrice > currentPosition.stopLoss.Value)
                                                {
                                                    stopPrice = proposedStopPrice;
                                                  
                                                }
                                            }
                                            else if (currentPosition.side.Equals(EnumConverter.GetString(OrderSide.Sell)))
                                            {
                                                double proposedStopPrice = currentPosition.markPrice.Value + (currentPosition.markPrice.Value * trailingStopDistance);

                                                // Solo mover el stop loss si el nuevo precio es menor al anterior
                                                if (!currentPosition.stopLoss.HasValue || proposedStopPrice < currentPosition.stopLoss.Value)
                                                {
                                                    stopPrice = proposedStopPrice;
                                                }
                                            }

                                            // Actualizar el Stop Loss en Bybit si hay un precio válido
                                            if (stopPrice.HasValue)
                                            {
                                                var setStopLossResult = await _byBitApiService.SetTradingStop(new SetTradingStopRequest()
                                                {
                                                    apiKey = itemUser.ApiKey,
                                                    secretKey = itemUser.SecretKey,
                                                    symbol = currentPosition.symbol,
                                                    stopLoss = stopPrice,
                                                    category = EnumConverter.GetString(Category.Linear),
                                                    positionIdx = 0,
                                                    slOrderType = OrderType.Market.ToString(), // Orden de mercado para ejecutar el stop loss
                                                    slLimitPrice = null, // Null para ordenes de tipo "market"
                                                    tpslMode = EnumConverter.GetString(TpslMode.Full) // Cerrar toda la posición cuando se toque el trailing stop
                                                });

                                                if (setStopLossResult)
                                                {
                                                    await _binanceCacheByBitSymbolService.Add(itembinanceOrder.Symbol);
                                                    Console.WriteLine($"Trailing Stop Loss ajustado para {currentPosition.symbol} a {stopPrice}");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //en caso que encontrastes la posicion en la BD y no en bybit entonces hay que eliminarla de la BD
                                        await _binanceByBitOrderService.DeletedRange(itembinanceOrder.BinanceByBitOrders.ToList());
                                        await _binanceOrderService.DeletedOrder(itembinanceOrder);
                                        File.AppendAllLines("Deleted_ObjectiveForUpdatePositionWithTrailingStopQuery.txt", new[] { string.Concat(" Deleted_ObjectiveForUpdatePositionWithTrailingStopQuery (Handle) =>date: ", DateTime.Now.ToString(), " guid:  ", itembinanceOrder.EncryptedUid) });
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

        /* public async Task<bool> Handle(ObjectiveForUpdatePositionWithTrailingStopQuery request, CancellationToken cancellationToken)
         {
             // OBTENIR DES UTILISATEURS QUI ONT PAYÉ LA REDEVANCE D'ABONNEMENT
             var allUserActive = await _binanceByBitUsersService.GetAllIsactive();
             List<BinanceOrder> binanceOrderBD = await _binanceOrderService.GetAll();

             // Inicializamos el semáforo para permitir 3 tareas concurrentes
             SemaphoreSlim semaphore = new SemaphoreSlim(int.Parse(_configuration.GetSection("BinanceBybitSettings:TotalParallelProcessesObjectiveForUpdatePositionHandler").Value));

             // Lista para almacenar las tareas
             List<Task> tasks = new List<Task>();

             foreach (var itemUser in allUserActive)
             {
                 var walletBalance = await _byBitApiService.WalletBalance(itemUser.ApiKey, itemUser.SecretKey);
                 var coinWalletBalance = await _byBitApiService.GetCoinFromWalletBalance(walletBalance);

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

                 bool IsIngProcessForObjective = (await _monitoringCoinWalletBalanceObjectiveProcessService.IsIngProcessForObjective(
                     long.Parse(_configuration.GetSection("BinanceBybitSettings:IdTelegrameMain").Value), coinWalletBalance));

                 if (!IsIngProcessForObjective)
                 {
                     return false;
                 }

                 foreach (var itembinanceOrder in binanceOrderBD)
                 {
                     if (!string.IsNullOrEmpty(itembinanceOrder.Symbol))
                     {
                         tasks.Add(Task.Run(async () =>
                         {
                             await semaphore.WaitAsync();  // Adquirir semáforo

                             try
                             {
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
                                     var currentPosition = positionBybitInfo.list.FirstOrDefault();

                                     // Calcula el porcentaje correspondiente (4.375% de la posición)
                                     double percentageProfitThreshold = 4.165;

                                     // Calcula el valor equivalente al porcentaje del tamaño de la posición
                                     double unrealisedPnl =  currentPosition.unrealisedPnl.Value *100 / currentPosition.positionValue.Value ;




                                     if (unrealisedPnl > percentageProfitThreshold)
                                     {


                                         double trailingStopDistance = 0.04375; // Distancia del trailing stop para 7 euros de ganancia (4.375% para 160 euros)

                                         // Calcular el precio del trailing stop
                                         double? stopPrice = null;
                                         if (currentPosition.side.Equals(EnumConverter.GetString(OrderSide.Buy)))
                                         {
                                             stopPrice = currentPosition.markPrice - (currentPosition.markPrice * trailingStopDistance);
                                         }
                                         else if (currentPosition.side.Equals(EnumConverter.GetString(OrderSide.Sell)))
                                         {
                                             stopPrice = currentPosition.markPrice + (currentPosition.markPrice * trailingStopDistance);
                                         }

                                         // Actualizar el Stop Loss en Bybit
                                         if (stopPrice.HasValue)
                                         {
                                             var setStopLossResult = await _byBitApiService.SetTradingStop(new SetTradingStopRequest()
                                             {
                                                 apiKey = itemUser.ApiKey,
                                                 secretKey = itemUser.SecretKey,
                                                 symbol = currentPosition.symbol,
                                                 stopLoss = stopPrice,
                                                 category = EnumConverter.GetString(Category.Linear),
                                                 positionIdx = 0,
                                                 slOrderType = OrderType.Market.ToString(), // Orden de mercado para ejecutar el stop loss
                                                 slLimitPrice = stopPrice,
                                                 tpslMode = EnumConverter.GetString(TpslMode.Full) // Cerrar toda la posición cuando se toque el trailing stop
                                             });

                                             if (setStopLossResult)
                                             {
                                                 await _binanceCacheByBitSymbolService.Add(itembinanceOrder.Symbol);
                                                 Console.WriteLine($"Trailing Stop Loss ajustado para {currentPosition.symbol} a {stopPrice}");
                                             }
                                         }
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
         }*/
    }
}
