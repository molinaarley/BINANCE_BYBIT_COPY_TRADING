using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Infrastructure
{
    public interface IByBitApiV3Service
    {
        Task<GetPositionInfoV3Root> GetPositionInfo(GetPositionInfoV3Query getPositionInfoV3Query);
    }
}
