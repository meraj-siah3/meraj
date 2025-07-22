using Grpc.Core;
using GrpcContracts;
using Microsoft.AspNetCore.Hosting.Server;

const int Port = 5002;

var server = new Server
{
    Services = { MessageService.BindService(new MessageServiceImpl()) },
    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
};

server.Start();
Console.WriteLine($"🚀 MessageProcessor gRPC Server اجرا شد روی پورت {Port}");
Console.WriteLine("برای توقف کلید بزنید...");
Console.ReadKey();
await server.ShutdownAsync();
