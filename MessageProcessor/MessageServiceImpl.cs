using Grpc.Core;
using GrpcContracts;
using System.Text.RegularExpressions;

public class MessageServiceImpl : MessageService.MessageServiceBase
{
    //// متد ثبت موتور
    //public override Task<MessageResponse> RegisterEngine(EngineInfo request, ServerCallContext context)
    //{
    //    Console.WriteLine($" Engine  registered ID: {request.Id}, Type: {request.Type}");

    //    return Task.FromResult(new MessageResponse
    //    {
    //        Id = 0,
    //        Engine = request.Type,
    //        MessageLength = 0,
    //        IsValid = true
    //    });
    //}

    // متد پردازش پیام (نسخه ساده تکی)
    public override Task<MessageResponse> ProcessMessage(MessageRequest request, ServerCallContext context)
    {
        string msg = request.Message;
        int length = msg.Length;

        var isOnlyEnglish = Regex.IsMatch(msg, @"^[a-zA-Z\s]+$");
        int wordCount = msg.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        bool isValid = isOnlyEnglish && wordCount >= 3;

        Console.WriteLine($"\n New message received \"{msg}\"");
        Console.WriteLine($"message length: {length}");
        Console.WriteLine($" Count: {wordCount}");
        Console.WriteLine($" Valid? {isValid}");

        var response = new MessageResponse
        {
            Id = request.Id,
            Engine = "RegexEngine",
            MessageLength = length,
            IsValid = isValid
        };

        return Task.FromResult(response);
    }

    // 🔥 متد جدید: ارتباط استریم دوطرفه (طبق PDF)
    public override async Task StreamMessages(
        IAsyncStreamReader<MessageRequest> requestStream,
        IServerStreamWriter<MessageResponse> responseStream,
        ServerCallContext context)
    {
        // مرحله اول: ارسال پیام معرفی به کلاینت
        var registerResponse = new MessageResponse
        {
            Id = 0,
            Engine = "RegexEngine",
            MessageLength = 0,
            IsValid = true
        };

        await responseStream.WriteAsync(registerResponse);
        Console.WriteLine("✅ پیام معرفی به کلاینت ارسال شد");

        // مرحله دوم: دریافت و پردازش پیام‌ها
        await foreach (var request in requestStream.ReadAllAsync())
        {
            string msg = request.Message;
            int length = msg.Length;
            bool isOnlyEnglish = Regex.IsMatch(msg, @"^[a-zA-Z\s]+$");
            int wordCount = msg.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            bool isValid = isOnlyEnglish && wordCount >= 3;
        }
    }
}
