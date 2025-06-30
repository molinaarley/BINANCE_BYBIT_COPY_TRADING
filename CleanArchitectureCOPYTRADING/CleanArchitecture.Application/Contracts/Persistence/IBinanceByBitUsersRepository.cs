using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;

namespace CleanArchitecture.Application.Contracts.Persistence
{
    public interface IBinanceByBitUsersRepository 
    {
        Task<long> Create(BinanceByBitUser binanceByBitUser);
        Task<List<BinanceByBitUser>> GetAllIsactive();
        Task<List<BinanceByBitUser>> GetAll();
        Task<Dictionary<long, long>> GetAllInDictionary();
    }
}
