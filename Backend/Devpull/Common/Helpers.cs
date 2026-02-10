using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Devpull.Common;

public static partial class Helpers
{
    public static string CreateTempFile(string resultCode)
    {
        // 1. Создаём временный файл, т.к при передаче кода аргументом команды есть проблемы с экранированием
        var tempFile = Path.Combine(Path.GetTempPath(), $"script_{Guid.NewGuid()}.csx");
        File.WriteAllText(tempFile, resultCode, Encoding.UTF8);
        return tempFile;
    }

    public static string GetFunctionName(string template)
    {
        var regex = FunctionNameRegex();
        var match = regex.Match(template);
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    public static Process CreateProcess(
        string image,
        string containerName,
        double cpuLimit,
        int ramLimit,
        string tempFile,
        string exerciseId = ""
    )
    {
        var containerPath = "/app/script.csx";

        // 2. Запускаем Docker-контейнер с монтированным скриптом
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments =
                    $"run --rm --name {containerName} --read-only --cpus={cpuLimit.ToString(CultureInfo.InvariantCulture)} --memory={ramLimit}m --memory-swap={ramLimit}m -v \"{tempFile}:{containerPath}:ro\" {image} \"{containerPath}\" {exerciseId}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            }
        };
        return process;
    }

    public static async Task DeleteContainer(string containerName)
    {
        try
        {
            var stopInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = $"rm -f {containerName}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            using var stopProcess = new Process { StartInfo = stopInfo };
            stopProcess.Start();
            await stopProcess.WaitForExitAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении контейнера: {ex.Message}");
        }
    }

    [GeneratedRegex(@"\b\w+\s+(\w+)\s*\(")]
    private static partial Regex FunctionNameRegex();
}
