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
    "urgent action required",
    "this is a test message",
    "error occurred during process",
    " hi this is a test",
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

Console.WriteLine(" MessageDistributor   Strat.....");

//  بررسی سلامت سیستم هر 30 ثانیه
_ = Task.Run(async () =>
{
    while (true)
    {
        var result = await healthCheckService.CheckHealthAsync(engineId, 5);
        isEnabled = result?.IsEnabled ?? false;

        Console.WriteLine($" HealthCheck  Status: IsEnabled = {isEnabled}");

        // معرفی اولیه اگر فعال بود
        if (isEnabled && !isRegistered)
        {
            isRegistered = await grpcClient.RegisterEngineAsync(engineId, engineType);
        }

        await Task.Delay(TimeSpan.FromSeconds(30));
    }
});

//  ارسال پیام‌های تصادفی هر 2 ثانیه
_ = Task.Run(async () =>
{
    var rnd = new Random();

    while (true)
    {
        if (isEnabled && isRegistered)
        {
        //    اینجا پیام ساخته میشه و در خط بعد پیام ارسال میشه
            var message = new MessageRequest
            {
                Id = rnd.Next(1000, 9999),
                Sender = senders[rnd.Next(senders.Length)],
                Message = messages[rnd.Next(messages.Length)]
            };
            //اینجا می‌ره به کلاس GrpcMessageClient و متد ProcessMessageAsync رو صدا می‌زنه
            var response = await grpcClient.ProcessMessageAsync(message);
            if (response != null)
            {
                Console.WriteLine($"The message is processed : {JsonSerializer.Serialize(response)}");
            }
        }

        await Task.Delay(TimeSpan.FromSeconds(2));
    }
});

Console.WriteLine(" Click button to exit ");
Console.ReadLine();
