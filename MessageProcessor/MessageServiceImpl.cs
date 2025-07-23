using Grpc.Core;
using GrpcContracts;
using System.Text.RegularExpressions;

public class MessageServiceImpl : MessageService.MessageServiceBase
{



    // متد ثبت موتور
    public override Task<MessageResponse> RegisterEngine(EngineInfo request, ServerCallContext context)
    {
        Console.WriteLine($" Engine  registered ID: {request.Id}, Type: {request.Type}");

        return Task.FromResult(new MessageResponse
        {
            Id = 0,
            Engine = request.Type,
            MessageLength = 0,
            IsValid = true
        });
    }
    // متد پردازش پیام
    public override Task<MessageResponse> ProcessMessage(MessageRequest request, ServerCallContext context)
    {
        string msg = request.Message;
        int length = msg.Length;

        // فقط شامل حروف انگلیسی و فاصله؟
        var isOnlyEnglish = Regex.IsMatch(msg, @"^[a-zA-Z\s]+$");

        // تعداد کلمات
        int wordCount = msg.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

        // شرط معتبر بودن: فقط انگلیسی + حداقل ۳ کلمه
        bool isValid = isOnlyEnglish && wordCount >= 3;

        // نمایش اطلاعات در کنسول
        Console.WriteLine($"\n New message received \"{msg}\"");
        Console.WriteLine($"message length: {length}");
        Console.WriteLine($" Count: {wordCount}");
        Console.WriteLine($" Valid? {isValid}");

        // ساخت پاسخ
        var response = new MessageResponse
        {
            Id = request.Id,
            Engine = "RegexEngine",
            MessageLength = length,
            IsValid = isValid
        };

        return Task.FromResult(response);
    }

    //// متد پردازش پیام
    //public override Task<MessageResponse> ProcessMessage(MessageRequest request, ServerCallContext context)
    //{
    //    string msg = request.Message;
    //    int length = msg.Length;
    //    bool isValid = true;

    //    Console.WriteLine($"\n New message received:");
    //    Console.WriteLine($" ID: {request.Id}");
    //    Console.WriteLine($" Sender: {request.Sender}");
    //    Console.WriteLine($" Message: \"{msg}\"");

    //    // اجرای regexها
    //    foreach (var pattern in _regexPatterns)
    //    {
    //        bool matched = Regex.IsMatch(msg, pattern.Value, RegexOptions.IgnoreCase);
    //        Console.WriteLine($" {pattern.Key}: {matched}");

    //        if (!matched)
    //            isValid = false;
    //    }

    //    // ساخت پاسخ
    //    var response = new MessageResponse
    //    {
    //        Id = request.Id,
    //        Engine = "RegexEngine",
    //        MessageLength = length,
    //        IsValid = isValid
    //    };

    //    Console.WriteLine($"  Length: {length}, IsValid: {isValid}");

    //    return Task.FromResult(response);
}
