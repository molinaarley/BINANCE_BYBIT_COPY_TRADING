using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using MediatR;

namespace CleanArchitecture.Application.Features.Binance.Queries.SetTradingStop
{
    public class SetTradingStopQuery : IRequest<bool>
    {
        public BinanceMonitoringProcess BinanceMonitoringProces { get; set; }
        public DateTime DateBeginForLoad { get; set; }

    }
}
