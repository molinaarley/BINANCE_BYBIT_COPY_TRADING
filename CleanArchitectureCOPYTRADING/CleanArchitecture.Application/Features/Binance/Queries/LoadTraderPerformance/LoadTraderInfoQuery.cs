using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using MediatR;

namespace CleanArchitecture.Application.Features.Binance.Queries.LoadTraderPerformance
{
    public class LoadTraderPerformanceQuery : IRequest<int>
    {
        public string Guid { get; set; }
        public string EncryptedUid { get; set; }
        

    }
}
