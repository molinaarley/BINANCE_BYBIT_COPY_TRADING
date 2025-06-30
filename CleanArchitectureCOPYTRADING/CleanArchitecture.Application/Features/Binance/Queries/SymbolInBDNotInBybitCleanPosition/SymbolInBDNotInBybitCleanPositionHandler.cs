using System.Text;
using System;
using System.Globalization;
using System.Reflection;
using AutoMapper;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using MediatR;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace CleanArchitecture.Application.Features.Binance.Queries.SymbolInBDNotInBybitCleanPosition
{
    /// <summary>
    /// WE DELETE POSITIONS CREATED IN THE DATABASE BUT NOT PRESENT IN BYBIT.
    /// BECAUSE A POSITION CREATED IN THE DATABASE MUST BE PRESENT IN BYBIT,
    /// OTHERWISE, IT IS A POSITION THAT WAS LIQUIDATED AND HAS NOT BEEN REFLECTED IN THE DATABASE.
    /// </summary>
    public class SymbolInBDNotInBybitCleanPositionHandler : IRequestHandler<SymbolInBDNotInBybitCleanPositionQuery, int>
    {
        private readonly ITelegrameBotService _telegrameBotService;
        private string _pathDataJson = @"C:\worck\BINANCE_DATA\";
        private readonly IByBitApiService _byBitApiService;
        private readonly IBinanceOrderService _binanceOrderService;
        private readonly IBinanceByBitUsersService _binanceByBitUsersService;
        private readonly IBinanceByBitOrderService _binanceByBitOrderService;
        public IConfiguration _configuration { get; }



        public SymbolInBDNotInBybitCleanPositionHandler(ITelegrameBotService telegrameBotService,
             IByBitApiService byBitService,
            IBinanceOrderService binanceOrderService,
            IBinanceByBitUsersService binanceByBitUsersService,
            IBinanceByBitOrderService binanceByBitOrderService, 
            IConfiguration configuration)
        {
            //_videoRepository = videoRepository;
            _telegrameBotService = telegrameBotService ?? throw new ArgumentException(nameof(telegrameBotService));
            _byBitApiService = byBitService ?? throw new ArgumentException(nameof(byBitService));
            _binanceOrderService = binanceOrderService ?? throw new ArgumentException(nameof(binanceOrderService));
            _binanceByBitUsersService = binanceByBitUsersService ?? throw new ArgumentException(nameof(binanceByBitUsersService));
            _binanceByBitOrderService = binanceByBitOrderService ?? throw new ArgumentException(nameof(binanceByBitOrderService));
            
         
            _configuration = configuration ?? throw new ArgumentException(nameof(configuration));
        }

        public async Task<int> Handle(SymbolInBDNotInBybitCleanPositionQuery request, CancellationToken cancellationToken)
        {
            int totalDeleted = 0;
            List<PositionData> positionsData = await _telegrameBotService.LoadPosition(_pathDataJson);
            Dictionary<string, string> keyFromFile  = positionsData.GroupBy(p => p.code).ToDictionary(p => p.Key, p => p.FirstOrDefault().code);

            List<OtherPositionRetList> otherPositionRetList = await _binanceOrderService.GetOtherPositionRetList(positionsData);

            //OBTENIR DES UTILISATEURS QUI ONT PAYÉ LA REDEVANCE D'ABONNEMENT
            var allUserActive = await _binanceByBitUsersService.GetAllIsactive();
            List<BinanceOrder> binanceOrderBD = await _binanceOrderService.GetAll();

            foreach (var itemUser in allUserActive)
            {
                // WE GET THE AMOUNT OF MONEY YOU HAVE IN YOUR BYBIT ACCOUNT
                var walletBalance = await _byBitApiService.WalletBalance(itemUser.ApiKey, itemUser.SecretKey);
                var coinWalletBalance = await _byBitApiService.GetCoinFromWalletBalance(walletBalance);


                var getFeeRateInfo = await _byBitApiService.GetFeeRate(new GetFeeRateQuery()
                {
                    category = "linear",
                    apiKey = itemUser.ApiKey,
                    secretKey = itemUser.SecretKey
                });

                Dictionary<string, string> result = new Dictionary<string, string>();
                result = otherPositionRetList.GroupBy(p => p.symbol).ToDictionary(p => p.Key, p => p.FirstOrDefault().symbol);

                var getInstrumentsInfo = (await _byBitApiService.GetInstrumentsInfo(new GetInstrumentsInfoQuery()
                {
                    apiKey = itemUser.ApiKey,
                    secretKey = itemUser.SecretKey,
                    category = EnumConverter.GetString(Category.Linear)
                })).result.list.GroupBy(x => x.symbol).ToDictionary(p => p.Key, p => p.FirstOrDefault());

                foreach (var itembinanceOrder in binanceOrderBD)
                {
                        var tickersCategorySymbol = await _byBitApiService.GetTickersCategorySymbol(new GetTickers() { category = "inverse", symbol = itembinanceOrder.Symbol });

                        var positionBybitInfo = await _byBitApiService.GetPositionInfo(new GetPositionInfo()
                        {
                            category = EnumConverter.GetString(Category.Linear),//"linear",
                            symbol = itembinanceOrder.Symbol,
                            apiKey = itemUser.ApiKey,
                            secretKey = itemUser.SecretKey
                        });
                        if (positionBybitInfo != null && positionBybitInfo.list != null && positionBybitInfo.list.Any() &&
                                string.IsNullOrEmpty(positionBybitInfo.list.FirstOrDefault().side) )
                        {
                            if (positionBybitInfo != null && positionBybitInfo.list != null && positionBybitInfo.list.Any())
                            {

                                totalDeleted++;
                                await _binanceByBitOrderService.DeletedRange(itembinanceOrder.BinanceByBitOrders.ToList());
                                await _binanceOrderService.DeletedOrder(itembinanceOrder);
                                File.AppendAllLines("SymbolInBDNotInBybitCleanPositionHandlerlog.txt", new[] { string.Concat("deted from bd symbol (Handle) :", itembinanceOrder.Symbol, "*******position deleted*********", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff")) });
                                File.AppendAllLines("SymbolInBDNotInBybitCleanPositionHandlerlog.txt", new[] { string.Concat(" itembinanceOrder.Symbole (Handle) => Empty") });
                            }
                        }
 
                }
            }
            File.AppendAllLines("WriteLinesLog.txt", new[] { "*******total Deleted position un plus SymbolInBDNotInBybitCleanPositionHandler************" });
            File.AppendAllLines("WriteLinesLog.txt", new[] { string.Concat("total => ", totalDeleted.ToString()) });
            return totalDeleted;
        }


        
    }
}
