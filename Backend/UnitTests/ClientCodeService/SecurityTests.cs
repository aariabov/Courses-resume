using System.Diagnostics;
using System.Globalization;
using Devpull.ClientCode;
using Devpull.Common;
using Microsoft.Extensions.Configuration;

namespace UnitTests.ClientCodeService;

[TestClass]
public class SecurityTests
{
    private const int TimeoutInSeconds = 14;
    private const int RamLimitInMb = 256;
    private const int CpuLimitInPercent = 50;

    [TestMethod]
    public async Task no_timeout()
    {
        var expected = "Привет!";
        var code =
            $@"
await Task.Delay(500);
Console.Write(""{expected}"");";

        var sut = CreateSut();
        var result = await sut.Execute(new ClientCodeModel { Code = code }, CancellationToken.None);
        Assert.AreEqual(expected, result.Result);
        Assert.IsNull(result.RuntimeError);
        Assert.IsNull(result.CompileError);
        Assert.IsNull(result.Error);
        Assert.IsFalse(result.InProgress);
    }

    [TestMethod]
    [Ignore("Запускать отдельно, т.к сильно кушает ресурсы и мешает другим тестам")]
    public async Task parallel()
    {
        var expected = "Привет!";
        var code =
            $@"
await Task.Delay(500);
Console.Write(""{expected}"");";

        var sut = CreateSut(timeoutInSeconds: TimeoutInSeconds * 100); // много процессов и докер не запускает их сразу, чтоб не падало по таймауту

        const int parallelCount = 100;

        var tasks = Enumerable
            .Range(0, parallelCount)
            .Select(i =>
            {
                var model = new ClientCodeModel { Code = code };

                return Task.Run(async () =>
                {
                    var result = await sut.Execute(model, CancellationToken.None);

                    Assert.IsNull(result.Error, result.Error);
                    Assert.AreEqual(expected, result.Result);
                    Assert.IsNull(result.RuntimeError);
                    Assert.IsNull(result.CompileError);
                    Assert.IsFalse(result.InProgress);
                });
            });

        await Task.WhenAll(tasks);
    }

    [TestMethod]
    public async Task timeout()
    {
        var code =
            $@"
await Task.Delay({TimeoutInSeconds * 1000 + 100});
Console.Write(""Привет!"");";

        var sut = CreateSut();
        var result = await sut.Execute(new ClientCodeModel { Code = code }, CancellationToken.None);
        Assert.AreEqual(string.Empty, result.Result);
        Assert.IsNull(result.RuntimeError);
        Assert.IsNull(result.CompileError);
        Assert.AreEqual(
            $"Выполнение программы было отменено по таймауту {TimeoutInSeconds} сек.",
            result.Error
        );
        Assert.IsFalse(result.InProgress);
    }

    [TestMethod]
    public async Task timeout_while_true()
    {
        var code = $@"while(true);";
        var containerName = $"csharp-runner_{Guid.NewGuid()}";

        var sut = CreateSut();
        var result = await sut.Execute(
            new ClientCodeModel { Code = code },
            CancellationToken.None,
            containerName
        );
        Assert.AreEqual(string.Empty, result.Result);
        Assert.IsNull(result.RuntimeError);
        Assert.IsNull(result.CompileError);
        Assert.AreEqual(
            $"Выполнение программы было отменено по таймауту {TimeoutInSeconds} сек.",
            result.Error
        );
        Assert.IsFalse(result.InProgress);
        Assert.IsFalse(await IsContainerExists(containerName));
    }

    [TestMethod]
    public async Task timeout_with_output()
    {
        var expected = "Start";
        var code =
            $@"
Console.Write(""{expected}"");
await Task.Delay({TimeoutInSeconds * 1000 + 100});
Console.Write(""Привет!"");";

        var sut = CreateSut();
        var result = await sut.Execute(new ClientCodeModel { Code = code }, CancellationToken.None);
        Assert.AreEqual(expected, result.Result);
        Assert.IsNull(result.RuntimeError);
        Assert.IsNull(result.CompileError);
        Assert.AreEqual(
            $"Выполнение программы было отменено по таймауту {TimeoutInSeconds} сек.",
            result.Error
        );
        Assert.IsFalse(result.InProgress);
    }

    [TestMethod]
    public async Task too_small_memory()
    {
        var expected = "Привет!";
        var code =
            $@"
await Task.Delay(500);
Console.Write(""{expected}"");";
        var ramLimit = 32;

        var sut = CreateSut(ramLimit);
        var result = await sut.Execute(new ClientCodeModel { Code = code }, CancellationToken.None);

        Assert.AreEqual(string.Empty, result.Result);
        Assert.IsNull(result.RuntimeError);
        Assert.IsNull(result.CompileError);
        Assert.AreEqual($"Превышен лимит памяти {ramLimit}Mb", result.Error);
        Assert.IsFalse(result.InProgress);
    }

    [TestMethod]
    public async Task memory_limit_exceed()
    {
        var arrSize = 200 * 1024 * 1024; // 200Mb
        var code = $"var buffer = new byte[{arrSize}];";
        var ramLimit = 110;
        var expectedRuntimeError =
            "System.OutOfMemoryException: Exception of type 'System.OutOfMemoryException' was thrown.";

        var sut = CreateSut(ramLimit);
        var result = await sut.Execute(new ClientCodeModel { Code = code }, CancellationToken.None);

        Assert.AreEqual(string.Empty, result.Result);
        Assert.AreEqual(expectedRuntimeError, result.RuntimeError!.Trim());
        Assert.IsNull(result.CompileError);
        Assert.IsNull(result.Error);
        Assert.IsFalse(result.InProgress);
    }

    [TestMethod]
    public async Task cpu_restriction()
    {
        var code = $@"while(true);";
        var containerName = $"csharp-runner_{Guid.NewGuid()}";

        var sut = CreateSut();
        var executeTask = sut.Execute(
            new ClientCodeModel { Code = code },
            CancellationToken.None,
            containerName
        );
        await Task.Delay(1000); // дать время на запуск

        // Проверка CPU через stats
        var statsInfo = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = $"stats {containerName} --no-stream --format \"{{{{.CPUPerc}}}}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false
        };

        var statsProcess = Process.Start(statsInfo);
        if (statsProcess != null)
        {
            var output = await statsProcess.StandardOutput.ReadToEndAsync();
            await statsProcess.WaitForExitAsync();

            if (
                double.TryParse(
                    output.Trim().Replace("%", ""),
                    CultureInfo.InvariantCulture,
                    out double cpuUsage
                )
            )
            {
                Assert.IsTrue(cpuUsage <= 55, $"CPU usage too high: {cpuUsage}%");
            }
            else
            {
                Assert.IsFalse(true, "Could not parse CPU usage");
            }
        }

        var result = await executeTask;

        Assert.AreEqual(string.Empty, result.Result);
        Assert.IsNull(result.RuntimeError);
        Assert.IsNull(result.CompileError);
        Assert.AreEqual(
            $"Выполнение программы было отменено по таймауту {TimeoutInSeconds} сек.",
            result.Error
        );
        Assert.IsFalse(result.InProgress);
        Assert.IsFalse(await IsContainerExists(containerName));
    }

    [TestMethod]
    public async Task write_file_forbidden()
    {
        var sut = CreateSut();
        var result = await sut.Execute(
            new ClientCodeModel
            {
                Code =
                    @"using System.IO;
string fileName = ""output.txt"";
string currentDir = Directory.GetCurrentDirectory();
string filePath = Path.Combine(currentDir, fileName);
FileStream fs = new FileStream(filePath, FileMode.CreateNew);
fs.Seek(6 * 1024 * 1024, SeekOrigin.Begin);
fs.WriteByte(0);
fs.Close();
Console.WriteLine($""Файл записан в: {currentDir}"");
while(true){}"
            },
            CancellationToken.None
        );

        Assert.IsNull(result.Error, result.Error);
        Assert.IsNull(result.CompileError, result.CompileError);
        Assert.AreEqual(string.Empty, result.Result);
        Assert.AreEqual(
            "System.IO.IOException: Read-only file system : '/app/output.txt'",
            result.RuntimeError!.Trim()
        );
        Assert.IsFalse(result.InProgress);
    }

    private static async Task<bool> IsContainerExists(string containerName)
    {
        // Проверка: существует ли контейнер
        var checkInfo = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = $"ps -a -q -f name=^{containerName}$",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var checkProcess = new Process { StartInfo = checkInfo };
        checkProcess.Start();
        var output = await checkProcess.StandardOutput.ReadToEndAsync();
        await checkProcess.WaitForExitAsync();
        return !string.IsNullOrWhiteSpace(output);
    }

    private static Devpull.ClientCode.ClientCodeService CreateSut(
        int ramLimit = RamLimitInMb,
        int timeoutInSeconds = TimeoutInSeconds
    )
    {
        var appConfig = new AppConfig
        {
            ClientCode =
            {
                TimeoutInSeconds = timeoutInSeconds,
                RamLimitInMb = ramLimit,
                CpuInPercent = CpuLimitInPercent
            }
        };

        return new Devpull.ClientCode.ClientCodeService(
            new ClientCodeModelValidator(),
            new AnalyzeCodeModelValidator(),
            appConfig
        );
    }
}
