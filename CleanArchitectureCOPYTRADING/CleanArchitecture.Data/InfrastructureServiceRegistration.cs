using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Contracts.Persistence;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Binance;
using CleanArchitecture.Infrastructure.Email;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Repositories;
using CleanArchitecture.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;
using Microsoft.ML;
using Microsoft.ML.Data;
using OpenAI;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure
{

    public class ModelInput
    {
        public string SentimentText;
    }

    public class ModelOutput
    {
        [ColumnName("PredictedLabel")]
        public bool Sentiment { get; set; }

        public float Probability { get; set; }

        public float Score { get; set; }
    }

    public class ModelInput2
    {
        public string SentimentText;
    }

    public class ModelOutput2
    {
        [ColumnName("PredictedLabel")]
        public bool Sentiment { get; set; }

        public float Probability { get; set; }

        public float Score { get; set; }
    }

   
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {


            services.AddDbContext<StreamerDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ConnectionString"))
            );
            services.AddDbContext<BinanceContext>(options => options.UseSqlServer(configuration.GetConnectionString("BinanceByBiteDataBaseConnection")));



            /*services.AddDbContext<BinanceContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("BinanceByBiteDataBaseConnection"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); // Configuración de NoTracking
            });*/


            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;


                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<BinanceContext>()
              .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
   opt.TokenLifespan = TimeSpan.FromHours(24));


            services.AddScoped<IBinanceContext, BinanceContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            
            services.AddScoped<IVideoRepository, VideoRepository>();
            services.AddScoped<IStreamerRepository, StreamerRepository>();

            services.AddScoped<IBinanceTraderRepository, BinanceTraderRepository>();
            services.AddScoped<IBinanceOrderRepository, BinanceOrderRepository>();
            services.AddScoped<IBinanceOrderAuditRepository, BinanceOrderAuditRepository>();
            
            services.AddScoped<IBinanceByBitUsersRepository, BinanceByBitUsersRepository>();
            services.AddScoped<IBinanceByBitOrderRepository, BinanceByBitOrderRepository>();
            services.AddScoped<IBinanceMonitoringProcessRepository, BinanceMonitoringProcessRepository>();
            services.AddScoped<IBinanceMonitoringCoinWalletBalanceObjectiveProcessRepository, BinanceMonitoringCoinWalletBalanceObjectiveProcessRepository>();
            services.AddScoped<IBinanceMonitoringCoinWalletBalanceRepository, BinanceMonitoringCoinWalletBalanceRepository>();

            services.AddScoped<IBinanceTraderTypeDataRepository, BinanceTraderTypeDataRepository>();
            services.AddScoped<IBinanceTraderPerformanceRetListRepository, BinanceTraderPerformanceRetListRepository>();
            services.AddScoped<IBinanceTraderPerformanceRetListRepositoryAudit, BinanceTraderPerformanceRetListRepositoryAudit>();

            services.AddScoped<IBinanceTraderUrlForUpdatePositionBinanceQueryRepository, BinanceTraderUrlForUpdatePositionBinanceQueryRepository>();


            




            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IDonetZipService, DonetZipService>();

           
            services.AddScoped<ITelegrameBotService, TelegrameBotService>();

            services.AddScoped<ITelegramBotClientProvider>(provider =>
            {
                var botKey = "6393575706:AAF7LjBJOUKSBQwvMZGdt0mhvHPB15CgzIs"; // Tu clave del BotFather
                return new TelegramBotClientProvider(botKey);
            });

           

            
            services.Configure<ByBitSettings>(c => configuration.GetSection("ByBitSettings"));
            services.Configure<EmailSettings>(c => configuration.GetSection("EmailSettings"));
            services.Configure<BinanceBybitSettings>(c => configuration.GetSection("BinanceBybitSettings"));

            
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuthenticationByBitService, AuthenticationByBitService>();
            services.AddScoped<IBinanceTraderService, BinanceTraderService>();
            services.AddScoped<IBinanceOrderService, BinanceOrderService>();
            services.AddScoped<IBinanceByBitUsersService, BinanceByBitUsersService>();
            services.AddScoped<IBinanceByBitOrderService, BinanceByBitOrderService>();
            services.AddScoped<IBinanceMonitoringProcessService, BinanceMonitoringProcessService>();
            services.AddScoped<IBinanceMonitoringCoinWalletBalanceObjectiveProcessService, BinanceMonitoringCoinWalletBalanceObjectiveProcessService>();
            services.AddScoped<IBinanceTraderTypeDataService, BinanceTraderTypeDataService>();
            services.AddScoped<IBinanceTraderPerformanceService, BinanceTraderPerformanceService>();
            services.AddScoped<IBinanceMonitoringCoinWalletBalanceService, BinanceMonitoringCoinWalletBalanceService>();
            services.AddScoped<IBinanceTraderUrlForUpdatePositionBinanceQueryService, BinanceTraderUrlForUpdatePositionBinanceQueryService>();
            services.AddScoped<IBinanceCacheByBitSymbolService, BinanceCacheByBitSymbolService>();
            services.AddMemoryCache();

            //Add All Model IA
            // services.AddPredictionEnginePool<ModelInput, ModelOutput>()
            // .FromFile(modelName: "SentimentAnalysisModel", filePath: "sentiment_model.zip", watchForChanges: true);

            services.AddSingleton<MLContext>();

            // Configura la inyección de dependencias para MLContext
            /*  if (System.IO.File.Exists(configuration.GetSection("BinanceBybitSettings:BinanceWeeklY_ROI").Value))
              {
                  //not user
                  services.AddPredictionEnginePool<TraderDataPerformanceBinance, ModelTraderDataOutput>()
                  .FromFile(configuration.GetSection("BinanceBybitSettings:BinanceWeeklY_ROI").Value); // Nombre del modelo 1
              }

              if (System.IO.File.Exists(configuration.GetSection("BinanceBybitSettings:Binance_IsDailY_ROI_Increasing").Value))
              {
                  services.AddPredictionEnginePool<TraderDataPerformanceBinance, TraderDataPerformanceBinanceIsDailYROIIncreasingPrediction>()
                  .FromFile(configuration.GetSection("BinanceBybitSettings:Binance_IsDailY_ROI_Increasing").Value); // Nombre del modelo 1
              }


              if (System.IO.File.Exists(configuration.GetSection("BinanceBybitSettings:Binance_IsMonthlY_ROI_Increasing").Value))
              {
                  services.AddPredictionEnginePool<TraderDataPerformanceBinance, TraderDataPerformanceBinanceIsMonthlY_ROI_IncreasingPrediction>()
                  .FromFile(configuration.GetSection("BinanceBybitSettings:Binance_IsMonthlY_ROI_Increasing").Value);
              }

              if (System.IO.File.Exists(configuration.GetSection("BinanceBybitSettings:Binance_IsTopTraderScore").Value))
              {
                  services.AddPredictionEnginePool<TraderDataPerformanceBinance, TraderPerformancePrediction>()
                  .FromFile(configuration.GetSection("BinanceBybitSettings:Binance_IsTopTraderScore").Value);
              }*/


            string[] modelKeys = new[]
            {
                "BinanceWeeklY_ROI",
                "Binance_IsDailY_ROI_Increasing",
                "Binance_IsMonthlY_ROI_Increasing",
                "Binance_IsTopTraderScore"
            };

            foreach (var key in modelKeys)
            {
                var path = configuration[$"BinanceBybitSettings:{key}"];

                if (File.Exists(path))
                {
                    Console.WriteLine($"✅ Modelo {key} encontrado en: {path}");

                    switch (key)
                    {
                        case "BinanceWeeklY_ROI":
                            services.AddPredictionEnginePool<TraderDataPerformanceBinance, ModelTraderDataOutput>()
                                .FromFile(path);
                            break;

                        case "Binance_IsDailY_ROI_Increasing":
                            services.AddPredictionEnginePool<TraderDataPerformanceBinance, TraderDataPerformanceBinanceIsDailYROIIncreasingPrediction>()
                                .FromFile(path);
                            break;

                        case "Binance_IsMonthlY_ROI_Increasing":
                            services.AddPredictionEnginePool<TraderDataPerformanceBinance, TraderDataPerformanceBinanceIsMonthlY_ROI_IncreasingPrediction>()
                                .FromFile(path);
                            break;

                        case "Binance_IsTopTraderScore":
                            services.AddPredictionEnginePool<TraderDataPerformanceBinance, TraderPerformancePrediction>()
                                .FromFile(path);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine($"❌ Modelo {key} NO encontrado en: {path}");
                }
            }



                services.AddHttpClient<IByBitApiService, ByBitApiService>(client =>
            {
                client.BaseAddress = new Uri(configuration.GetSection("ByBitSettings:BaseUrl").Value);
            });

            services.AddHttpClient<IByBitApiRepository, ByBitApiRepository>(client =>
            {
                client.BaseAddress = new Uri(configuration.GetSection("ByBitSettings:BaseUrl").Value);
            });

            services.AddHttpClient<IByBitApiV3Service, ByBitApiV3Service>(client =>
            {
                client.BaseAddress = new Uri(configuration.GetSection("ByBitSettings:BaseUrlv3").Value);
            });

            

            services.AddHttpClient<IByBitApiV3Repository, ByBitApiV3Repository>(client =>
            {
                client.BaseAddress = new Uri(configuration.GetSection("ByBitSettings:BaseUrlv3").Value);
            });



          
            services.AddSingleton(sp =>
            {
                var apiKey = configuration["ByBitSettings:OpenAiApiKey"];
                return new OpenAIClient(apiKey);
            });


            return services;
        }


        
    }
}
