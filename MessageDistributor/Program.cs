using Google.Protobuf.WellKnownTypes;
using MessageDistributor.Services;
using MessageProtocol;
using Microsoft.AspNetCore.Hosting;
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

    .ConfigureWebHostDefaults(webBuilder =>
    {
    webBuilder.ConfigureServices(services =>
    {
        services.AddGrpc();
    });

    //webBuilder.Configure(app =>
    //{
    //    app.MapGrpcService<MessageServiceImpl>();
    //    app.MapGet("/", () => "gRPC server is running.");
    //})

    .Build()
    .Run();
