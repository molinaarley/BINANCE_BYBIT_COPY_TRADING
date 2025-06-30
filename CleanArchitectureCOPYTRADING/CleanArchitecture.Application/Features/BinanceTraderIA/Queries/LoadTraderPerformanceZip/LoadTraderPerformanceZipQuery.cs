using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using MediatR;

namespace CleanArchitecture.Application.Features.Binance.Queries.LoadTraderPerformanceZip
{
    public class LoadTraderPerformanceZipQuery : IRequest<int>
    {
        public string Guid { get; set; }
    }
}
