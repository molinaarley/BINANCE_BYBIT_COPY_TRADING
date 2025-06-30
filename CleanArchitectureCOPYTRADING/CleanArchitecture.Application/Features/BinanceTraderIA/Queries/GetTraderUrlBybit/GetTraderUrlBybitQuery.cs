using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using MediatR;

namespace CleanArchitecture.Application.Features.Binance.Queries.GetTraderUrlBybit
{
    public class GetTraderUrlBybitQuery : IRequest<List<ByBitGuidKeyResult>>
    {
 
    }
}
