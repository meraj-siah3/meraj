using Grpc.Core;
using GrpcContracts;
using System.Text.RegularExpressions;

public class MessageServiceImpl : MessageService.MessageServiceBase
{
    // پیاده‌سازی متد ارتباط دوطرفه
    public override async Task StreamMessages(
        IAsyncStreamReader<MessageRequest> requestStream,
        IServerStreamWriter<MessageResponse> responseStream,
        ServerCallContext context)
    {
        // ارسال پیام معرفی از سمت Processor
        var registerResponse = new MessageResponse
        {
            Id = 0,
            Engine = "RegexEngine",
            MessageLength = 0,
            IsValid = true
        };

        await responseStream.WriteAsync(registerResponse);
        Console.WriteLine("✅ پیام معرفی به کلاینت ارسال شد");

        // دریافت و پردازش پیام‌ها از کلاینت
        await foreach (var request in requestStream.ReadAllAsync())
        {
            string msg = request.Message;
            int length = msg.Length;
            bool isOnlyEnglish = Regex.IsMatch(msg, @"^[a-zA-Z\s]+$");
            int wordCount = msg.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            bool isValid = isOnlyEnglish && wordCount >= 3;

            Console.WriteLine($"\n📥 پیام استریم دریافت شد: \"{msg}\" از {request.Sender}");
            Console.WriteLine($"📏 طول: {length}, 🧮 کلمات: {wordCount}, ✅ معتبر؟ {isValid}");

            var response = new MessageResponse
            {
                Id = request.Id,
                Engine = "RegexEngine",
                MessageLength = length,
                IsValid = isValid
            };

            await responseStream.WriteAsync(response);
        }

        Console.WriteLine("📴 ارتباط stream بسته شد.");
    }
}
