using Grpc.Core;
using GrpcContracts;
using System.Text.RegularExpressions;

public class MessageServiceImpl : MessageService.MessageServiceBase
{
    // لیست regexهای تحلیل
    private readonly Dictionary<string, string> _regexPatterns = new()
    {
        { "ContainsLorem", "lorem" },
        { "ContainsError", "error" },
        { "PersianChars", "[آ-ی]" },
        { "EndsWithDot", @"\.$" }
    };

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
        bool isValid = true;

        Console.WriteLine($"\n New message received:");
        Console.WriteLine($" ID: {request.Id}");
        Console.WriteLine($" Sender: {request.Sender}");
        Console.WriteLine($" Message: \"{msg}\"");

        // اجرای regexها
        foreach (var pattern in _regexPatterns)
        {
            bool matched = Regex.IsMatch(msg, pattern.Value, RegexOptions.IgnoreCase);
            Console.WriteLine($" {pattern.Key}: {matched}");

            if (!matched)
                isValid = false;
        }

        // ساخت پاسخ
        var response = new MessageResponse
        {
            Id = request.Id,
            Engine = "RegexEngine",
            MessageLength = length,
            IsValid = isValid
        };

        Console.WriteLine($"  Length: {length}, IsValid: {isValid}");

        return Task.FromResult(response);
    }
}
