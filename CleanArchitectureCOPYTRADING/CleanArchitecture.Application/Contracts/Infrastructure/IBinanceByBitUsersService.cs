using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Infrastructure
{
    public interface IBinanceByBitUsersService
    {
        Task<long> Create(BinanceByBitUser binanceByBitUser);
        Task<List<BinanceByBitUser>> GetAllIsactive();


    }
}
