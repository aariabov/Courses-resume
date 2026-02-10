using System.Security.Cryptography;
using System.Text;

namespace Devpull.Payments;

public interface IIdempotenceKeyGenerator
{
    string Generate(string userId, decimal amount);
}

public class FakeIdempotenceKeyGenerator : IIdempotenceKeyGenerator
{
    public string Generate(string userId, decimal amount)
    {
        return Guid.NewGuid().ToString();
    }
}

public class IdempotenceKeyGenerator : IIdempotenceKeyGenerator
{
    public string Generate(string userId, decimal amount)
    {
        // Формируем строку, от которой будет зависеть ключ
        var input = $"{amount}:{userId}";

        // Хэшируем с помощью SHA256 и конвертируем в hex
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder();
        foreach (var b in hashBytes)
        {
            sb.Append(b.ToString("x2")); // hex
        }

        // Можно использовать первые 64 символа, чтобы ключ был короче
        return sb.ToString().Substring(0, 64);
    }
}
