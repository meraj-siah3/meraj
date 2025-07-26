using GrpcContracts;
using MessageDistributor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;


Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        //  اضافه کردن HttpClient با DI برای HealthCheckService
        services.AddHttpClient<HealthCheckService>();

        //  ثبت Worker برای اجرای 
        services.AddHostedService<Worker>();
    })
    .Build()
    .Run();
