using AutoMapper;
using CleanArchitecture.Application.Contracts.Persistence;
using MediatR;

namespace CleanArchitecture.Application.Features.Videos.Queries.GetVideosList
{
    public class GetVideosListQueryHandler : IRequestHandler<GetVideosListQuery, List<VideosVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IVideoRepository _videoRepository;
        private readonly IMapper _mapper;

        public GetVideosListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            //_videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<VideosVm>> Handle(GetVideosListQuery request, CancellationToken cancellationToken)
        {

            var listvideo = new List<VideosVm>();

            listvideo.Add(new VideosVm() {Nombre="nm1",StreamerId=1 });
            listvideo.Add(new VideosVm() { Nombre = "nm2", StreamerId = 2 });

            return listvideo;

            var videoList = await  _unitOfWork.VideoRepository.GetVideoByUsername(request._Username);

            return _mapper.Map<List<VideosVm>>(videoList);
        }
    }
}
