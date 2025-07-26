using SharedModels.Models;
using System.Net.Http;
using System.Net.Http.Json;
using Polly;
using Polly.Retry;

namespace MessageDistributor.Services;

public class HealthCheckService
{
    private readonly HttpClient _httpClient;
    private readonly string _url = "http://localhost:5062/api/module/health";

    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy = Policy
        .Handle<HttpRequestException>()
        .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: attempt => TimeSpan.FromSeconds(2 * attempt),
            onRetry: (result, time, retryCount, context) =>
            {
                Console.WriteLine($" تلاش مجدد HealthCheck #{retryCount} بعد از {time.TotalSeconds} ثانیه");
            });

    // Constructor با DI
    public HealthCheckService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

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
            var response = await _retryPolicy.ExecuteAsync(() =>
                _httpClient.PostAsJsonAsync(_url, request)
            );

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<HealthCheckResponse>();
                return result;
            }
            else
            {
                Console.WriteLine($" وضعیت ناموفق: {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($" خطا در HealthCheck: {ex.Message}");
            return null;
        }
    }
}
