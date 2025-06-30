using AutoMapper;
using CleanArchitecture.Application.Features.Binance.Commands.CreatePosition;
using CleanArchitecture.Application.Features.Videos.Queries.GetVideosList;
using CleanArchitecture.WEB.Controllers.Binance.CreatePosition;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using CleanArchitecture.Infrastructure;
using Microsoft.Extensions.ML;
using Telegram.Bot.Types;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Application.Features.BinanceTraderIA.Queries.CreateModelTrader;
using CleanArchitecture.Application.Features.BinanceTraderIA.Queries.GetTraderUrlForUpdatePositionBinanceIA;
using CleanArchitecture.Application.Features.BinanceTraderIA.Queries.GetAllTraderDataPerformanceBinanceForModel;

namespace CleanArchitecture.WEB.Controllers.Binance;
[ApiController]
[Route("[controller]")]
public class BinanceTraderIAController : AbstractController
{
    public BinanceTraderIAController(IMediator mediator, IMapper mapper) : base(mediator, mapper)
    {
    
        //Init Telegrame BotClient
        //var result = _mediator.Send(new InitBotClientQuery() { }).Result;
    }

    [HttpGet("CreateModelTrader")]
    //[Authorize]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> CreateModelTrader()
    {
        var result = await _mediator.Send(new CreateModelTraderQuery() { });
        return Ok(result);
    }

    /// <summary>
    /// GET GUIDS TO RETRIEVE EACH TRADER'S DATA FROM BINANCE leaderboard from model ML.NET
    /// </summary>
    /// <returns></returns>
    [HttpGet("trader-url-for-update-position-from-binance-ia")]
    [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<List<string>>> GetTraderUrlForUpdatePositionBinanceIA()
    {
        var result = await _mediator.Send(new GetTraderUrlForUpdatePositionBinanceIAQuery() { });
        return Ok(result);
    }


    [HttpGet("get-all-trader-data-performance-binance-for-model-query")]
    [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<List<TraderDataPerformanceBinance>>> GetAllTraderDataPerformanceBinanceForModelQuery()
    {
        var result = await _mediator.Send(new GetAllTraderDataPerformanceBinanceForModelQuery() { });
        return Ok(result);
    }




    























    [HttpPost("PostVideosByUsername")]
    //[Authorize]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<VideosVm>>> PostVideosByUsername([FromBody] ModelInput request)
    {
      //var result=  _traderPredictionEngine.Predict<TraderDataPerformanceBinance, ModelTraderDataOutput>( new TraderDataPerformanceBinance() { });

       // Console.WriteLine(result.IsTopTrader.ToString(),Color.YellowColor);
        //*** var prediction = _traderPredictionEngine.Predict(request);

        // async (PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool, ModelInput input) =>
        // await Task.FromResult(_traderPredictionEngine.Predict(modelName: "SentimentAnalysisModel", input));
        /*
          [HttpPost("CreatePosition")]
    //[Authorize(Roles = "Administrator")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> CreatePosition([FromBody] CreatePositionRequest request)
    {
         */

        /*
         var predictionHandler =
    async (PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool, ModelInput input) =>
        await Task.FromResult(predictionEnginePool.Predict(modelName: "SentimentAnalysisModel", input));

         */
        // var query = new GetVideosListQuery(username);
        // var videos = await _mediator.Send(query);
        //return Ok(videos);
        return Ok("yesok");

    }

    

    [HttpPost("CreatePosition")]
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

