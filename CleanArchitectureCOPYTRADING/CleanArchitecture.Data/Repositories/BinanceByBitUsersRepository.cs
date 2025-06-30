using System.Collections.Specialized;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Web;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System.Linq;
using Telegram.Bot.Types;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class BinanceByBitUsersRepository : IBinanceByBitUsersRepository
    {
       private readonly IBinanceContext _binanceContext;
        public ILogger<BinanceByBitUsersRepository> _logger { get; }
        public BinanceByBitUsersRepository(ILogger<BinanceByBitUsersRepository> logger, IBinanceContext binanceContext) 
        {
            _logger = logger;
            _binanceContext = binanceContext ?? throw new ArgumentException(nameof(binanceContext));
        }

        public async Task<long> Create(BinanceByBitUser binanceByBitUser)
        {
             try
             {
                await  _binanceContext.BinanceByBitUsers.AddAsync(binanceByBitUser);
                await _binanceContext.SaveChangesAsync();
                return binanceByBitUser.IdTelegrame;
             }
            catch (System.ObjectDisposedException ex)
            {

                DbContextOptionsBuilder<BinanceContext> _optionsBuilder = new DbContextOptionsBuilder<BinanceContext>();
                _optionsBuilder.UseSqlServer(@"Password=xiomaraA1;Persist Security Info=True;User ID=linagma32046com26487_artelcom;Initial Catalog=Binance;Data Source=DESKTOP-H61MD4F;TrustServerCertificate=true");
                BinanceContext _db = new BinanceContext(_optionsBuilder.Options);
                _db.BinanceByBitUsers.Add(binanceByBitUser);
                await _db.SaveChangesAsync();

                _logger.LogError(ex, $"Error creating BinanceByBitUsers {binanceByBitUser.IdTelegrame}");
                return binanceByBitUser.IdTelegrame;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating BinanceByBitUsers {binanceByBitUser.IdTelegrame}");
                throw ex;
            }


            //
        }

        public async Task<List<BinanceByBitUser>> GetAllIsactive()
        {
            var result = _binanceContext.BinanceByBitUsers.Where(p => p.Isactive.HasValue
            && p.Isactive.Value &&  !string.IsNullOrEmpty(p.ApiKey) && !string.IsNullOrEmpty(p.SecretKey)).
               AsNoTracking().AsQueryable();
            return result.ToList();
        }

       
        public async Task<List<BinanceByBitUser>> GetAll()
        {
            var result = _binanceContext.BinanceByBitUsers.
               AsNoTracking().AsQueryable();
            return result.ToList();

        }

        public async Task<Dictionary<long, long>> GetAllInDictionary()
        {
            var result = (await GetAll())
           .ToDictionary(p => p.IdTelegrame, p => p.IdTelegrame);
            return result;
        }
    }
}
