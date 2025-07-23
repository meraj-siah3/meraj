using SharedModels.Models;
using System.Net.Http.Json;

namespace MessageDistributor.Services;

public class HealthCheckService
{
    //HttpClient ساخته شده برای فرستادن درخواست
    private readonly HttpClient _httpClient = new();
    
    private readonly string _url = "http://localhost:5062/api/module/health";

    public async Task<HealthCheckResponse?> CheckHealthAsync(string id, int connectedClients)
    {
        var request = new HealthCheckRequest
        {
            Id = id,
            SystemTime = DateTime.UtcNow.ToString("o"),
            NumberofConnectedClients = connectedClients
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(_url, request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<HealthCheckResponse>();
                return result;
            } 
            else
            {
                Console.WriteLine($" error in  HealthCheck: StatusCode = {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception  in  HealthCheck: {ex.Message}");
            return null;
        }
    }
}
