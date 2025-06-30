using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using MediatR;

namespace CleanArchitecture.Application.Features.Binance.Queries.LoadPosition
{
    public class LoadPositionQuery : IRequest<int>
    {
        public BinanceMonitoringProcess BinanceMonitoringProces { get; set; }
        public DateTime DateBeginForLoad { get; set; }

    }
}
