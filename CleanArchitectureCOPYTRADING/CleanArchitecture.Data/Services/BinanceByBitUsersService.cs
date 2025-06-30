using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Enum;
using EllipticCurve.Utils;
using Ionic.Zip;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using Telegram.Bot;
using System.Linq;
using System.Collections.Specialized;
using System.Web;
using System.Reflection.Metadata;
using System.Data.Common;
using System.Reflection.PortableExecutable;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Infrastructure.Repositories;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Infrastructure.Persistence;

namespace CleanArchitecture.Infrastructure.Services
{

    public class BinanceByBitUsersService : IBinanceByBitUsersService
    {
        private readonly IBinanceByBitUsersRepository _binanceByBitUsersRepository;
        public ILogger<BinanceByBitUsersService> _logger { get; }
     
        public BinanceByBitUsersService() { }

        public BinanceByBitUsersService(ILogger<BinanceByBitUsersService> logger,
            IBinanceByBitUsersRepository binanceByBitUsersRepository)
        {
            _logger = logger;
            _binanceByBitUsersRepository = binanceByBitUsersRepository ?? throw new ArgumentException(nameof(binanceByBitUsersRepository));
        }
       
        public async Task<long> Create(BinanceByBitUser binanceByBitUser)
        {
           return await _binanceByBitUsersRepository.Create(binanceByBitUser);
        }

        public async Task<List<BinanceByBitUser>> GetAllIsactive()
        {
            return await _binanceByBitUsersRepository.GetAllIsactive();
        }

    }
}
