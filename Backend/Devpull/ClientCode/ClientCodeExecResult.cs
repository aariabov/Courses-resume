using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis;

namespace Devpull.ClientCode;

public class ClientCodeExecResult
{
    public string? Result { get; }

    public string? CompileError { get; }

    public string? RuntimeError { get; }

    /// <summary>Ошибки, не связанные с клиентским кодом, например, Docker не запущен</summary>
    public string? Error { get; }

    public bool InProgress { get; }

    [JsonIgnore]
    public int ExitCode { get; }

    [JsonIgnore]
    public bool IsNormalCode => ExitCode is >= 0 and < 100;

    [JsonIgnore]
    public bool IsExpectedError => ExitCode is 101 or 102 or 125 or 127 or 137;

    public static ClientCodeExecResult Create(
        int exitCode,
        string output,
        string error,
        int ramLimit
    )
    {
        return exitCode switch
        {
            0 => CreateSuccess(exitCode, output, inProgress: false),
            1 => CreateSuccess(exitCode, output, inProgress: true),
            2 => CreateCompileError(exitCode, error),
            101 => CreateRuntimeError(exitCode, output, error),
            102 => CreateError(exitCode, output, error), // OperationCanceledException
            125 => CreateError(exitCode, output, "Ошибка при запуске Docker"),
            127 => CreateError(exitCode, output, "Docker не запущен"),
            137 => CreateError(exitCode, output, $"Превышен лимит памяти {ramLimit}Mb"),
            _ => CreateError(exitCode, output, "Непредвиденная ошибка")
        };
    }

    private static ClientCodeExecResult CreateSuccess(int exitCode, string result, bool inProgress)
    {
        return new ClientCodeExecResult(exitCode, result, null, null, null, inProgress);
    }

    private static ClientCodeExecResult CreateCompileError(int exitCode, string error)
    {
        return new ClientCodeExecResult(exitCode, null, error, null, null, false);
    }

    private static ClientCodeExecResult CreateRuntimeError(
        int exitCode,
        string result,
        string runtimeError
    )
    {
        return new ClientCodeExecResult(exitCode, result, null, runtimeError, null, false);
    }

    private static ClientCodeExecResult CreateError(int exitCode, string result, string error)
    {
        return new ClientCodeExecResult(exitCode, result, null, null, error, false);
    }

    private ClientCodeExecResult(
        int exitCode,
        string? result,
        string? compileError,
        string? runtimeError,
        string? error,
        bool inProgress
    )
    {
        Result = result;
        CompileError = compileError;
        RuntimeError = runtimeError;
        InProgress = inProgress;
        Error = error;
        ExitCode = exitCode;
    }
}
