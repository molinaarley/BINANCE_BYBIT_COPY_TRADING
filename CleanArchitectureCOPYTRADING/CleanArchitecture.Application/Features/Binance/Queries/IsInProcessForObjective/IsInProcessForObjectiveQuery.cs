using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using MediatR;

namespace CleanArchitecture.Application.Features.Binance.Queries.IsInProcessForObjective
{
    public class IsInProcessForObjectiveQuery : IRequest<bool>
    {
        public long IdTelegrame { get; set; }

    }
}
