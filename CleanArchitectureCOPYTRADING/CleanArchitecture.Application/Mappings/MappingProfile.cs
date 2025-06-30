using AutoMapper;
using CleanArchitecture.Application.Features.Binance.Commands.CreatePosition;
using CleanArchitecture.Application.Features.Directors.Commands.CreateDirector;
using CleanArchitecture.Application.Features.Streamers.Commands;
using CleanArchitecture.Application.Features.Streamers.Commands.UpdateStreamer;
using CleanArchitecture.Application.Features.Videos.Queries.GetVideosList;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain;
using CleanArchitecture.Domain.Binance;
using static CleanArchitecture.Domain.Streamer;

namespace CleanArchitecture.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<CreatePositionCommand, Streamer>();
            CreateMap<Video, VideosVm>();
            CreateMap<CreateStreamerCommand, Streamer>();
            CreateMap<UpdateStreamerCommand, Streamer>();
            CreateMap<CreateDirectorCommand, Director>();

           // CreateMap<BinanceTrader, BinanceTraderFromJsonReponseDatum>()
            //    .ForMember(dest => dest.rank, act => act.MapFrom(src => src.RankTrader));
                //.ForMember(dest => dest.rank, act => act.MapFrom(src => src.RankTrader));
        }
    }
}
