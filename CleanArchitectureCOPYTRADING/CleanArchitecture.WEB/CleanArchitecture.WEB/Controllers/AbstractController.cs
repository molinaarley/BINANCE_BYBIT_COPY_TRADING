using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WEB.Controllers;
/*
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender _mediator = null!;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}*/




[ApiController]
[Route("api/[controller]")]
public abstract class AbstractController : ControllerBase
{
    protected readonly IMediator _mediator;
    protected readonly IMapper _mapper;

    public AbstractController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }
}
