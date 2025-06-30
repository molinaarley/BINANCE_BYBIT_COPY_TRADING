using CleanArchitecture.Application.Behaviours;
using CleanArchitecture.Application.Features.Binance.Queries.GetTraderUrlForUpdatePositionBinance;
using CleanArchitecture.Application.Models;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CleanArchitecture.Application
{
    public static class ApplicationServiceRegistration
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {

            //Console.WriteLine("*****arm***AddApplicationServices**");
            services.AddAutoMapper(typeof(GetTraderUrlForUpdatePositionBinanceQuery).Assembly);
            //services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddValidatorsFromAssembly(typeof(GetTraderUrlForUpdatePositionBinanceQuery).Assembly);

            //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            //services.AddMediatR(Assembly.GetExecutingAssembly());

            Console.WriteLine("*****arm***add in aplication add handle;cs**");
            Console.WriteLine(typeof(GetTraderUrlForUpdatePositionBinanceQuery).Assembly);
            services.AddMediatR(typeof(GetTraderUrlForUpdatePositionBinanceQuery).Assembly);
            Console.WriteLine("*****arm***fin add in aplication add handle;cs**");

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.Configure<BinanceBybitSettings>(c => configuration.GetSection("BinanceBybitSettings"));

            return services;

           
        }

    }
}
