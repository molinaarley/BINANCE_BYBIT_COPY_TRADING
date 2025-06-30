using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IByBitApiV3Repository
    {
        Task<GetPositionInfoV3Root> GetPositionInfo(GetPositionInfoV3Query getPositionInfoV3Query);
        Task<GetBybitServerTimeRoot> GetBybitServerTime();
     }
}
