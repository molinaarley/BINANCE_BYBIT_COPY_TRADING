using System;
using AutoMapper;
using CleanArchitecture.Application.Features.Binance.Commands.CreatePosition;
using CleanArchitecture.Application.Features.Binance.Queries.CleanPosition;
using CleanArchitecture.Application.Features.Binance.Queries.InitBotClient;
using CleanArchitecture.Application.Features.Binance.Queries.LoadPosition;
using CleanArchitecture.Application.Features.Videos.Queries.GetVideosList;
using CleanArchitecture.Domain;
using CleanArchitecture.WEB.Controllers.Binance.CreatePosition;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using CleanArchitecture.Application.Features.Binance.Queries.BotIsIngProcess;
using CleanArchitecture.Application.Features.Binance.Queries.ZipDataBinance;
using CleanArchitecture.Application.Features.Binance.Queries.GetTraderUrl;
using CleanArchitecture.Application.Features.Binance.Queries.LoadTraderPerformance;
using CleanArchitecture.Application.Features.Binance.Queries.LoadTraderPerformanceZip;
using CleanArchitecture.Application.Features.Binance.Queries.GetTraderUrlForUpdatePositionBinance;
using CleanArchitecture.Application.Features.Binance.Queries.GetTraderUrlBybit;
using CleanArchitecture.Application.Features.Binance.Queries.GetTraderUrlForUpdatePositionBinanceInfo;
using CleanArchitecture.Application.Features.Binance.Queries.GetAddIndBinanceTraderPerformanceRetListAudit;
using CleanArchitecture.Application.Features.Binance.Queries.SetTradingStop;
using CleanArchitecture.Application.Features.Binance.Queries.MonitoringCoinWalletBalanceObjective;
using CleanArchitecture.Application.Features.Binance.Queries.IsInProcessForObjective;
using CleanArchitecture.Application.Features.Binance.Queries.GetMonitoringCoinWalletBalance;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Application.Features.Binance.Queries.ObjectiveProcessForUpdatePositions;
using CleanArchitecture.Application.Features.Binance.Queries.ObjectiveForUpdatePosition;
using CleanArchitecture.Application.Features.Binance.Queries.ClearCachePositions;
using CleanArchitecture.Application.Features.Binance.Queries.ObjectiveForUpdatePositionWithTrailingStop;
using CleanArchitecture.Application.Features.Binance.Queries.SymbolInBDNotInBybitCleanPosition;
using CleanArchitecture.Application.Features.Binance.Queries.MonitoringCoinWalletBalanceObjectiveOPENAI;

namespace CleanArchitecture.WEB.Controllers.Binance;
[ApiController]
[Route("[controller]")]
public class BinanceController : AbstractController
{
    public BinanceController(IMediator mediator, IMapper mapper) : base(mediator, mapper)
    {
        //Init Telegrame BotClient
        //var result = _mediator.Send(new InitBotClientQuery() { }).Result;
    }

    [HttpGet("{username}", Name = "GetVideo")]
    //[Authorize]
    [ProducesResponseType(typeof(IEnumerable<VideosVm>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<VideosVm>>> GetVideosByUsername(string username)
    {
        var query = new GetVideosListQuery(username);
        var videos = await _mediator.Send(query);
        return Ok(videos);
    }

    /// <summary>
    /// (CleanPositionQuery)=>ELIMINATE THE POSITIONS THAT ARE PRESENT ON BYBIT BUT ARE NOT PRESENT ON BINANCE('BINANCE_DATA'), WHICH MEANS THEY WERE ELIMINATED BY THE TREDORES 
    ///(LoadPositionQuery)=>LOAD POSITIONS FROM 'BINANCE_DATA' FOLDER TO UPDATE BYBIT
    /// </summary>
    /// <returns></returns>
    [HttpGet("load-position")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> LoadPosition()
    {

        var resultBinanceMonitoringProcess = await _mediator.Send(new CleanPositionQuery() { });
        var query = new LoadPositionQuery() {
            DateBeginForLoad = DateTime.Now.AddHours(-24)

        };
        query.BinanceMonitoringProces = new BinanceMonitoringProcess() { };
        var result = await _mediator.Send(query);
        return Ok(result);

    }

    /// <summary>
    /// Monitor the wallet and create an algorithm by setting a daily target.
    /// If we reach the target, we stop the update for the current day.
    /// CREATE ET item IN THE TABLE Binance_MonitoringCoinWalletBalanceObjectiveProcess
    /// </summary>
    /// <returns></returns>
    [HttpGet("monitoring-coin-wallet-balance-objective")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> MonitoringCoinWalletBalanceObjective()
    {
        var resultMonitoringCoinWalletBalanceObjectiveQuery = await _mediator.Send(new MonitoringCoinWalletBalanceObjectiveQuery() { });
        return Ok(resultMonitoringCoinWalletBalanceObjectiveQuery);
    }

    [HttpGet("is-in-process-for-objective")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> IsInProcessForObjective(long IdTelegrame)
    {
        var resultIsInProcessForObjective = await _mediator.Send(new IsInProcessForObjectiveQuery() { IdTelegrame=IdTelegrame });
        return Ok(resultIsInProcessForObjective);
    }




    /// <summary>
    ///  DISABLED FOR THE MOMENT
    /// obserbo las posiciones si hay alguna posicion con mas de 5000 de unrealisedPnl
    /// o sea aganancia la ciero,esta accion no es interesante tampoco 
    /// </summary>
    /// <returns></returns>
    [HttpGet("regulation-position")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> RegulationPosition()
    {
        var result = await _mediator.Send(new RegulationPositionQuery() { });
        return Ok(result);
    }

        /// <summary>
        /// THIS CALL IS DISABLED FOR THE MOMENT, I DO NOT SEE IT NECESSARY
        /// In the event that I have profits greater than 3000 dollars, I close all the positions
        /// and transfer the profits unrealisedPnl into spot, this must be changed and not close the positions
        /// but rather modify the stop ploss to avoid losing  
        /// </summary>
        /// <returns></returns>
        [HttpGet("validation-unrealised-pnl")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> ValidationUnrealisedPnl()
    {

        //var result = await _mediator.Send(query);
        //return Ok(result);  
        var resultBinanceMonitoringProcess = await _mediator.Send(new ValidationUnrealisedPnlQuery() { });
        return Ok(resultBinanceMonitoringProcess);
       
    }



    //[Authorize]
    [HttpGet("validation-clean-position")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> ValidationCleanPosition()
    {

        //var result = await _mediator.Send(query);
        //return Ok(result);  
        var resultBinanceMonitoringProcess = await _mediator.Send(new ValidationCleanPositionQuery() { });
        return Ok(resultBinanceMonitoringProcess);

    }

    /// <summary>
    /// WE DELETE POSITIONS CREATED IN THE DATABASE BUT NOT PRESENT IN BYBIT.
    /// BECAUSE A POSITION CREATED IN THE DATABASE MUST BE PRESENT IN BYBIT,
    /// OTHERWISE, IT IS A POSITION THAT WAS LIQUIDATED AND HAS NOT BEEN REFLECTED IN THE DATABASE.
    /// </summary>
    /// <returns></returns>
    //[Authorize]
    [HttpGet("symbol-in-bd-not-in-bybit-clean-position")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> SymbolInBDNotInBybitCleanPosition()
    {

        //var result = await _mediator.Send(query);
        //return Ok(result);  
        var resultBinanceMonitoringProcess = await _mediator.Send(new SymbolInBDNotInBybitCleanPositionQuery() { });
        return Ok(resultBinanceMonitoringProcess);

    }

    

    /// <summary>
    /// VERIFICAMOS CADA 5 SEGUNDOS SI CUMPLIMOS EL OBJETIVO DE EL DIA
    /// EN EL CASO QUE EL OBJETIVO SEA ALCANZASO CERRAMOS TODAS LAS POSICIONES AVIERTAS
    /// </summary>
    /// <returns></returns>
    [HttpGet("objective-process-for-update-positions")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> ObjectiveProcessForUpdatePositions()
    {

        var resultBinanceMonitoringProcess = await _mediator.Send(new ObjectiveProcessForUpdatePositionsQuery() { });
        return Ok(resultBinanceMonitoringProcess);

    }

    /// <summary>
    /// VERIFICAMOS CADA 1 SEGUNDOS SI CUMPLIMOS EL OBJETIVO DELA POSICION
    /// EN EL CASO QUE EL OBJETIVO DE LA POSICION  SEA ALCANZASO CERRAMOS LA  POSICION
    /// </summary>
    /// <returns></returns>
    [HttpGet("objective-for-update-position")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> ObjectiveForUpdatePosition()
    {

        var resultBinanceMonitoringProcess = await _mediator.Send(new ObjectiveForUpdatePositionQuery() { });
        return Ok(resultBinanceMonitoringProcess);

    }


    /// <summary>
    /// VERIFICAMOS CADA 1 SEGUNDOS SI CUMPLIMOS EL OBJETIVO DELA POSICION
    /// EN EL CASO QUE EL OBJETIVO DE LA POSICION  SEA ALCANZASO MOVEMOS EL STOP PLOS
    /// EN DIRECCION DE LA POSICION 0.04375 %
    /// </summary>
    /// <returns></returns>
    [HttpGet("objective-for-update-position-with-trailing-stop")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> ObjectiveForUpdatePositionWithTrailingStop()
    {

        var resultBinanceMonitoringProcess = await _mediator.Send(new ObjectiveForUpdatePositionWithTrailingStopQuery() { });
        return Ok(resultBinanceMonitoringProcess);

    }




    /// <summary>
    /// suprimimos el cache de las posiciones
    /// </summary>
    /// <returns></returns>
    [HttpGet("clear-cache-positions")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> ClearCachePositions()
    {

        //var result = await _mediator.Send(query);
        //return Ok(result);  
        var resultBinanceMonitoringProcess = await _mediator.Send(new ClearCachePositionsQuery() { });
        return Ok(resultBinanceMonitoringProcess);

    }

    //[Authorize]
    [HttpGet("init-bot")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> InitBot()
    {
        var result = _mediator.Send(new InitBotClientQuery() { }).Result;
        return Ok(result);
    }

    //[Authorize]
    [HttpGet("bot-is-ing-process")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> BotIsIngProcess()
    {
        var result = _mediator.Send(new BotIsIngProcessQuery() { }).Result;
        return Ok(result);
    }

    /// <summary>
    /// WE LOAD THE URLS THAT COME FROM THE Binance_Trader_TypeData TABLE, WE ONLY TAKE THE LINES (TRADEUR) WITH TYPES => (weekly, monthly, daily, total, FollowerCount > 300, Pnl > 0 && Roi > 0)
    /// </summary>
    /// <returns></returns>
    [HttpGet("trader-url")]
    [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> GetTraderUrl()
    {
        
        var result = await _mediator.Send(new GetTraderUrlQuery() { });
        return Ok(result);

    }

    /// <summary>
    /// WE LOAD THE URLS THAT COME FROM THE Binance_Trader_TypeData TABLE, WE ONLY TAKE THE LINES (TRADEUR) WITH TYPES => (weekly, monthly, daily, total, FollowerCount > 300, Pnl > 0 && Roi > 0)
    /// </summary>
    /// <returns></returns>
    [HttpGet("trader-url-bybit")]
    [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> GetTraderUrlBybit()
    {

        var result = await _mediator.Send(new GetTraderUrlBybitQuery() { });
        return Ok(result);

    }

    /// <summary>
    /// MONITORIAR TODOS LOS WALLET DE CADA UTILIZAR EN CADA ACTUALIZACION , SE GUARDA LA TRAZA EN LA TABLA Binance_MonitoringCoinWalletBalance
    /// </summary>
    /// <returns></returns>
    [HttpGet("monitoring-coin-wallet-balance")]
    [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> MonitoringCoinWalletBalance()
    {

        var result = await _mediator.Send(new GetMonitoringCoinWalletBalanceQuery() { });
        return Ok(result);

    }



    //Binance_MonitoringCoinWalletBalance


    /// <summary>
    /// GET GUIDS TO RETRIEVE EACH TRADER'S DATA FROM BINANCE leaderboard
    /// </summary>
    /// <returns></returns>
    [HttpGet("trader-url-for-update-position-from-binance")]
    [ProducesResponseType(typeof(List<ByBitGuidKeyResult> ), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> GetTraderUrlForUpdatePositionBinance()
    {
        

        //var devTEs=new GetTraderUrlForUpdatePositionBinanceQuery() { };
       // Console.WriteLine("********ARM****GetTraderUrlForUpdatePositionBinanceQuery********");


      //  var devTEs1 = new GetTraderUrlForUpdatePositionBinanceQuery() { };
      //  Console.WriteLine("********ARM****GetTraderUrlForUpdatePositionBinanceQuery1********");


        var result = await _mediator.Send(new GetTraderUrlForUpdatePositionBinanceQuery() { });
        //result = new List<string>();
        //result.Add("A086AC7B587E11941378E95DD6C872C6");//verificar esta negativo
        //result.Add("5018838FFE413B7A80D2529393DB1D7A"); super negativo
        //result.Add("9745A111F31F836D6D2E9F758DA3A07B");
       // result.Add("D3AFE978B3F0CD58489BC27B35906769");

        
        return Ok(result);
    }

    /// <summary>
    /// We check if all positions have a Stop-Loss. If not, we add the Stop-Loss.
    /// </summary>
    /// <returns></returns>
    [HttpGet("set-trading-stop")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> SetTradingStop()
    {
        var result = await _mediator.Send(new SetTradingStopQuery() { });
        return Ok(result);
    }



    /// <summary>
    /// GET GUIDS TO RETRIEVE EACH TRADER'S DATA FROM BINANCE leaderboard
    /// </summary>
    /// <returns></returns>
    [HttpGet("trader-url-for-update-position-from-binance-info")]
    [ProducesResponseType(typeof(List<TraderUrlForUpdatePositionBinanceInfoResult>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<TraderUrlForUpdatePositionBinanceInfoResult>> GetTraderUrlForUpdatePositionBinanceInfo()
    {
        var result = await _mediator.Send(new GetTraderUrlForUpdatePositionBinanceInfoQuery() { });
        return Ok(result);
    }


    [HttpGet("add-in-binance-trader-performanceret-list-audit")]
    [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> GetAddIndBinanceTraderPerformanceRetListAudit()
    {
        var result = await _mediator.Send(new GetAddIndBinanceTraderPerformanceRetListAuditQuery() { });
        return Ok(result);
    }




    [HttpGet("create-file-for-openai")]
    [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> CreateFileForOPENAI()
    {
        var result = await _mediator.Send(new CreateFileForOPENAIQuery() { });
        return Ok(result);
    }

    /// <summary>
    /// RETRIEVE DATA FROM THE /BINANCE_DATA_TRADER_INFO FOLDER
    /// INSERTS THE DATA INTO THE TABLE AFTER EMPTYING THE TABLE BinanceTraderPerformanceRetList
    /// </summary>
    /// <returns></returns>
    [HttpGet("load-trader-performance")]
    [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> LoadTraderPerformance(string guid,string encryptedUid)
    {

        var result = await _mediator.Send(new LoadTraderPerformanceQuery() { Guid=guid, EncryptedUid= encryptedUid });
        return Ok(result);

    }

    [HttpGet("load-trader-performance-zip")]
    [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> LoadTraderPerformanceZip(string guid)
    {

        var result = await _mediator.Send(new LoadTraderPerformanceZipQuery() { Guid = guid });
        return Ok(result);

        // return Ok(allUrl);
    }

   




    [HttpGet("zip-data-binance")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> ZipDataBinance()
    {
        var result = await _mediator.Send(new ZipDataBinanceQuery() { });
        return Ok(result);

    }

    /// <summary>
    /// RECOVER THE DATA FROM THE FOLDER /BINANCE_DATA_TRADER
    /// THE RECOVERED FILES ARE  data_hebdomadaire.jso,data_mensuel.json,data_quotidien.json,data_total.json
    /// DATA IS AGGREGATED IN THE BinanceTraderTypeData table
    /// </summary>
    /// <returns></returns>
    [HttpGet("load-binance-trader-from-json")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> LoadBinanceTraderFromJson()
    {
        var result = await _mediator.Send(new LoadBinanceTraderFromJsonQuery() { });
        return Ok(result);
    }

    [HttpPost(Name = "CreatePosition")]
    //[Authorize(Roles = "Administrator")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> CreatePosition([FromBody] CreatePositionRequest request)
    {
        CreatePositionCommand command = new CreatePositionCommand() {Amount=request.Amount,EncryptedUid=request.EncryptedUid };

        //var streamerEntity = _mapper.Map<CreatePositionCommand>(request);
        //
        return await _mediator.Send(command);
    }

}

