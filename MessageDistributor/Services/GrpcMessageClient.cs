using Grpc.Net.Client;
using GrpcContracts;


namespace MessageDistributor.Services;

public class GrpcMessageClient
{
    private readonly MessageService.MessageServiceClient _client;

    public GrpcMessageClient(string grpcAddress)
    {
        
        var channel = GrpcChannel.ForAddress(grpcAddress);
        _client = new MessageService.MessageServiceClient(channel);
    }

    public async Task<bool> RegisterEngineAsync(string id, string type)
    {
        var response = await _client.RegisterEngineAsync(new EngineInfo
        {
            Id = id,
            Type = type
        });

        Console.WriteLine($" RegisterEngine  Engine: {response.Engine}, Valid: {response.IsValid}");
        return response.IsValid;
    }

    public async Task<MessageResponse?> ProcessMessageAsync(MessageRequest request)
    {
        try
        {
            
            var response = await _client.ProcessMessageAsync(request);
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Error in ProcessMessage: {ex.Message}");
            return null;
        }
    }
}
