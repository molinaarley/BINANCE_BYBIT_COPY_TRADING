using System.Globalization;
using System.Reflection;
using AutoMapper;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Converters;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Domain.Enum;
using MediatR;
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Features.Binance.Queries.GetTraderUrl
{
    public class GetTraderUrlHandler : IRequestHandler<GetTraderUrlQuery, List<string>>
    {
        private readonly IBinanceTraderTypeDataService _binanceTraderTypeDataService;
        private readonly IBinanceTraderService _binanceTraderService;

        public GetTraderUrlHandler(IBinanceTraderTypeDataService binanceTraderTypeDataService,
            IBinanceTraderService binanceTraderService)
        {

            _binanceTraderTypeDataService = binanceTraderTypeDataService ?? throw new ArgumentException(nameof(binanceTraderTypeDataService));
            _binanceTraderService = binanceTraderService ?? throw new ArgumentException(nameof(binanceTraderService));
        }

        public async Task<List<string>> Handle(GetTraderUrlQuery request, CancellationToken cancellationToken)
        {
            //PeanutPark  for test no se perdio pero no se gano
            // allUrl.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=5018838FFE413B7A80D2529393DB1D7A");

            //LongNShort
            // allUrl.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=AB995C0BACF7B0DF83AAAA61CAD3AD11");

            //FUI RENTABLE Anocoyn NO INCLUIRLO -102 DE EL TOTAL 
            //allUrl.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=B86ACF50FDB361E97A7F06BC8C5B5CD3");


            // allUrl.Add("https://www.binance.com/fr/futures-activity/leaderboard/user?encryptedUid=CCF3E0CB0AAD54D9D6B4CEC5E3E741D2");//CnTraderT

            var dataBinanceTrader =await _binanceTraderService.GetAll();
            var dataDicBinanceTrader = dataBinanceTrader.GroupBy(p=>p.EncryptedUid).ToDictionary(p => p.Key, p => p.FirstOrDefault());

            List<string> result = new List<string>();
            List<string> hebdomadaire = (await _binanceTraderTypeDataService.GetAllByTypeData("hebdomadaire")).
                Where(p => dataDicBinanceTrader.ContainsKey(p.EncryptedUid) && dataDicBinanceTrader[p.EncryptedUid].FollowerCount > 150
                && p.Pnl > 0 && p.Roi > 0).
                 Select(p => p.EncryptedUid).ToList();

            List<string> mensuel = (await _binanceTraderTypeDataService.GetAllByTypeData("mensuel")).
                Where(p=> dataDicBinanceTrader.ContainsKey( p.EncryptedUid) && dataDicBinanceTrader[p.EncryptedUid].FollowerCount>150
                && p.Pnl>0 && p.Roi>0).
                Select(p => p.EncryptedUid).ToList();

            List<string> quotidien = (await _binanceTraderTypeDataService.GetAllByTypeData("quotidien")).
                Where(p => dataDicBinanceTrader.ContainsKey(p.EncryptedUid) && dataDicBinanceTrader[p.EncryptedUid].FollowerCount > 150
                && p.Pnl > 0 && p.Roi > 0).
                Select(p => p.EncryptedUid).OrderBy(p=>p).ToList();

            List<string> total = (await _binanceTraderTypeDataService.GetAllByTypeData("total")).
                Where(p => dataDicBinanceTrader.ContainsKey(p.EncryptedUid) && dataDicBinanceTrader[p.EncryptedUid].FollowerCount > 150
                && p.Pnl > 0 && p.Roi > 0).
                Select(p => p.EncryptedUid).OrderBy(p => p).ToList();


            hebdomadaire.AddRange(mensuel);
            hebdomadaire.AddRange(quotidien);
            hebdomadaire.AddRange(total);
            // int coundev = hebdomadaire.Distinct().ToList().Count;
           return hebdomadaire.Distinct().ToList();

           // result.AddRange(resultHebMen.Intersect(quotidien).ToList().OrderBy(p=>p).Take(5));
            var dataResult= quotidien.Select(p => string.Concat("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=", p)).ToList();
       
    
        //dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=FB23E1A8B7E2944FAAEC6219BBDF8243");

      dataResult = new List<string>();
      dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=A086AC7B587E11941378E95DD6C872C6");
      dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=DA200CE4A90667D0E59FDF8E6B68E599");
      dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=87FFB710AC2792DE3145272BCBA05EBE");
      dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=D3AFE978B3F0CD58489BC27B35906769");
      dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=7FB0AA335FD2D5C5C5967F700E0D8C1D");
      dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=538E78E33A3B0363FC37E393EB334103");
      dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=C20E7A8966C0014A4AF5774DD709DC42");
         


            // dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=FB23E1A8B7E2944FAAEC6219BBDF8243");
            //-10 EN UNA OPERACION ADIOS PeanutPark  dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=5018838FFE413B7A80D2529393DB1D7A");
            //se puso rojo todo  dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=A086AC7B587E11941378E95DD6C872C6");
            //***dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=CC41F779886196E25A22B41B261A86EB");
            //*** dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=B2E4D88D1E5633B2584F87EB5E2A6D6A");

            //***  dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=7FB0AA335FD2D5C5C5967F700E0D8C1D");
            //*** dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=4B555864FA5F5FD1E22DF764535131DA");
            //*** dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=D3AFE978B3F0CD58489BC27B35906769");


            //dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=9745A111F31F836D6D2E9F758DA3A07B");
            // dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=AB995C0BACF7B0DF83AAAA61CAD3AD11");


            //  dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=475ED7A7F7887E694E7828A61D1DFEAF");
            // dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=7BA46BBB007C9254462E4B5CDC14C907");
            // dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=AFAF232C088DD4BDD61F9165341485F3");


            //dataResult.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=2BA9DFB3A9F5378293991D3085155EAD");


            return dataResult;
       // result = new List<string>();


            /*manuelment
        result.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=FB23E1A8B7E2944FAAEC6219BBDF8243");//triofalfa
       //EL REI MENSUAL ES NEGATIVO result.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=CCF3E0CB0AAD54D9D6B4CEC5E3E741D2");//CnTraderT
       //REI MENSUAL NEGATIVO     result.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=A086AC7B587E11941378E95DD6C872C6");
        result.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=538E78E33A3B0363FC37E393EB334103");
        result.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=B2E4D88D1E5633B2584F87EB5E2A6D6A");
        result.Add("https://www.binance.com/fr/futures-activity/leaderboard/user/um?encryptedUid=2154D02AD930F6C6E65C507DD73CB3E7");*/
        
        

        return result;
        }
    }
}
