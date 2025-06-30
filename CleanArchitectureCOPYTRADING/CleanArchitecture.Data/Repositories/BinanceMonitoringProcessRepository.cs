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
using SendGrid.Helpers.Mail;
using System;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class BinanceMonitoringProcessRepository : IBinanceMonitoringProcessRepository
    {
       private readonly IBinanceContext _binanceContext;
        public ILogger<BinanceMonitoringProcessRepository> _logger { get; }
        public BinanceMonitoringProcessRepository(ILogger<BinanceMonitoringProcessRepository> logger, IBinanceContext binanceContext) 
        {
            _logger = logger;
            _binanceContext = binanceContext ?? throw new ArgumentException(nameof(binanceContext));
        }
        public async Task<BinanceMonitoringProcess> Create(BinanceMonitoringProcess binanceMonitoringProcess)
        {
            try
            {
                await _binanceContext.BinanceMonitoringProcess.AddAsync(binanceMonitoringProcess);
                await _binanceContext.SaveChangesAsync();
                return binanceMonitoringProcess;
            }
            catch (Exception e)
            {
                throw new Exception(
                    "Create  BinanceMonitoringProcess.", e);
            }
        }
        public async Task<bool> Update(BinanceMonitoringProcess binanceMonitoringProcess)
        {
            try
            {

                _binanceContext.BinanceMonitoringProcess.Update(binanceMonitoringProcess);
                
                await _binanceContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
               // throw new Exception(
               //     "Create  BinanceMonitoringProcess.", e);
               return false;
            }
        }

        public async Task<bool> GetIsIngProcess()
        {
            var result = _binanceContext.BinanceMonitoringProcess.OrderByDescending(p=>p.CreatedOn).
               AsNoTracking().AsQueryable();
             var resultData =await result.FirstOrDefaultAsync();
            if (resultData == null|| resultData.EndDate.HasValue)
            {
                return false;
            }
            return true;
        }


        public async Task<BinanceMonitoringProcess> GetLatBinanceMonitoringProcess()
        {
            var result = _binanceContext.BinanceMonitoringProcess.OrderByDescending(p => p.CreatedOn).
               AsNoTracking().AsQueryable();
            var resultData = await result.FirstOrDefaultAsync();
            return resultData;
          
        }

    }
}
