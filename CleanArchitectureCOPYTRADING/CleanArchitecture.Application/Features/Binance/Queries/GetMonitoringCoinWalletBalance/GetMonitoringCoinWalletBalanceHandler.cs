using System.Globalization;
using System.Reflection;
using AutoMapper;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Features.Binance.Queries.GetMonitoringCoinWalletBalance
{
    public class GetMonitoringCoinWalletBalanceHandler : IRequestHandler<GetMonitoringCoinWalletBalanceQuery,bool >
    {

        private readonly IBinanceTraderService _binanceTraderService;
        private readonly IBinanceMonitoringCoinWalletBalanceService _binanceMonitoringCoinWalletBalanceService;
        private readonly IBinanceByBitUsersService _binanceByBitUsersService;
        private readonly IByBitApiService _byBitApiService;
        private readonly IBinanceByBitOrderService _binanceByBitOrderService;

        public GetMonitoringCoinWalletBalanceHandler(
            IBinanceTraderService binanceTraderService
            ,IBinanceMonitoringCoinWalletBalanceService binanceMonitoringCoinWalletBalanceService,
               IBinanceByBitUsersService binanceByBitUsersService,
               IByBitApiService byBitService,
               IBinanceByBitOrderService binanceByBitOrderService
            )
        {

            _binanceTraderService = binanceTraderService ?? throw new ArgumentException(nameof(binanceTraderService));
            _binanceMonitoringCoinWalletBalanceService = binanceMonitoringCoinWalletBalanceService ?? throw new ArgumentException(nameof(binanceMonitoringCoinWalletBalanceService));
            _binanceByBitUsersService = binanceByBitUsersService ?? throw new ArgumentException(nameof(binanceByBitUsersService));
            _byBitApiService = byBitService ?? throw new ArgumentException(nameof(byBitService));
            _binanceByBitOrderService = binanceByBitOrderService ?? throw new ArgumentException(nameof(binanceByBitOrderService));
        }

        public async  Task<bool > Handle(GetMonitoringCoinWalletBalanceQuery request, CancellationToken cancellationToken)
        {
            var allUserActive = await _binanceByBitUsersService.GetAllIsactive();



            foreach (var itemUser in allUserActive)
            {
            
                //NOUS OBTENONS LE MONTANT D'ARGENT QUE VOUS AVEZ SUR VOTRE COMPTE BYBIT
                var walletBalance = await _byBitApiService.WalletBalance(itemUser.ApiKey, itemUser.SecretKey);
                while (walletBalance == null)
                {
                    Thread.Sleep(5000);
                    walletBalance = await _byBitApiService.WalletBalance(itemUser.ApiKey, itemUser.SecretKey);

                    // code block to be executed
                }
                var coinWalletBalance = await _byBitApiService.GetCoinFromWalletBalance(walletBalance);
                double? totalTotalAmount = await _binanceByBitOrderService.GetTotalAmount();
                await _binanceMonitoringCoinWalletBalanceService.Create(new BinanceMonitoringCoinWalletBalance()
                {
                    CreatedOn = DateTime.Now,
                    Equity = coinWalletBalance.equity.Value,
                    UnrealisedPnl = coinWalletBalance.unrealisedPnl.Value,
                    WalletBalance = coinWalletBalance.walletBalance.Value,
                    IdTelegrame = itemUser.IdTelegrame,
                    Amount= totalTotalAmount
                });
            }
            return true;
        }
    }
}
