using GrpcContracts;
using MessageDistributor.Services;
using System.Text.Json;

var grpcAddress = "http://localhost:5002"; // آدرس gRPC سرور (MessageProcessor)
var engineId = Guid.NewGuid().ToString();
var engineType = "RegexEngine";
var isRegistered = false;
var isEnabled = false;

var healthCheckService = new HealthCheckService();
var grpcClient = new GrpcMessageClient(grpcAddress);

// لیست پیام‌ها و فرستنده‌های تصادفی
string[] messages = new[]
{
    "lorem ipsum dolor sit amet",
    "hello world from gRPC client",
    "urgent: action required",
    "this is a test message",
    "error occurred during process",
    "سلام، این یک پیام تستی است",
    "please confirm your identity",
    "regex matching is powerful",
    "custom engine processing now",
    "report generated successfully"
};

string[] senders = new[]
{
    "Legal",
    "Finance",
    "Support",
    "Admin",
    "System",
    "ClientA",
    "ClientB"
};

Console.WriteLine(" MessageDistributor   Strat...");

// 🔁 بررسی سلامت سیستم هر 30 ثانیه
_ = Task.Run(async () =>
{
    while (true)
    {
        var result = await healthCheckService.CheckHealthAsync(engineId, 5);
        isEnabled = result?.IsEnabled ?? false;

        Console.WriteLine($"[HealthCheck] وضعیت: IsEnabled = {isEnabled}");

        // معرفی اولیه موتور اگر فعال بود
        if (isEnabled && !isRegistered)
        {
            isRegistered = await grpcClient.RegisterEngineAsync(engineId, engineType);
        }

        await Task.Delay(TimeSpan.FromSeconds(30));
    }
});

// 🔁 ارسال پیام‌های تصادفی هر 2 ثانیه
_ = Task.Run(async () =>
{
    var rnd = new Random();

    while (true)
    {
        if (isEnabled && isRegistered)
        {
            var message = new MessageRequest
            {
                Id = rnd.Next(1000, 9999),
                Sender = senders[rnd.Next(senders.Length)],
                Message = messages[rnd.Next(messages.Length)]
            };

            var response = await grpcClient.ProcessMessageAsync(message);
            if (response != null)
            {
                Console.WriteLine($"📨 پیام پردازش شد: {JsonSerializer.Serialize(response)}");
            }
        }

        await Task.Delay(TimeSpan.FromSeconds(2));
    }
});

Console.WriteLine("✅ سیستم فعال است. برای خروج Enter بزنید.");
Console.ReadLine();
