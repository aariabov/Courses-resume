namespace Common;

#pragma warning disable CA1027
public enum TestStatus
#pragma warning restore CA1027
{
    Success = 1,
    CompileError = 2,
    TimeoutError = 4,
    CommonError = 5,
    FailPublicTests = 6,
    FailPrivateTests = 7,
}

public class FunctionTestingResult
{
    public TestStatus Status { get; set; }

    /// <summary>Все результаты тестов</summary>
    public TestResult[] Results { get; set; } = Array.Empty<TestResult>();

    /// <summary>Тесты, которые не прошли</summary>
    public TestResult[] ErrorTests { get; set; } = Array.Empty<TestResult>();

    /// <summary>Тесты, которые прошли</summary>
    public TestResult[] SuccessTests { get; set; } = Array.Empty<TestResult>();

    public string? CompileError { get; set; }

    public string? RuntimeError { get; set; }

    /// <summary>Ошибки, не связанные с клиентским кодом, например, Docker не запущен</summary>
    public string? Error { get; set; }

    public static FunctionTestingResult CreateCompileError(string compileError)
    {
        return new FunctionTestingResult(TestStatus.CompileError, testResults: null, compileError, runtimeError: null, error: null);
    }

    public static FunctionTestingResult CreateTimeoutError(string error)
    {
        return new FunctionTestingResult(TestStatus.TimeoutError, null, null, null, error);
    }

    public static FunctionTestingResult CreateError(string error)
    {
        return new FunctionTestingResult(TestStatus.CommonError, null, null, null, error);
    }

    public static FunctionTestingResult CreateError(int exitCode, int ramLimit)
    {
        return exitCode switch
        {
            125 => CreateError("Ошибка при запуске Docker"),
            127 => CreateError("Docker не запущен"),
            137 => CreateError($"Превышен лимит памяти {ramLimit}Mb"),
            _ => CreateError("Непредвиденная ошибка")
        };
    }

    public static FunctionTestingResult CreateTestResult(TestResult[] testResults)
    {
        TestStatus status;
        if (testResults.All(r => r.IsSuccess))
        {
            status = TestStatus.Success;
        }
        else if(testResults.Any(r => r is { IsSuccess: false, IsPublic: true }))
        {
            status = TestStatus.FailPublicTests;
        }
        else
        {
            status = TestStatus.FailPrivateTests;
        }
        return new FunctionTestingResult(status, testResults, null, null, null);
    }

    private FunctionTestingResult(TestStatus status, TestResult[]? testResults, string? compileError, string? runtimeError, string? error)
    {
        CompileError = compileError;
        RuntimeError = runtimeError;
        Error = error;
        Results = testResults ?? [];
        Status = status;
        ErrorTests = testResults?.Where(r => !r.IsSuccess).ToArray() ?? [];
        SuccessTests = testResults?.Where(r => r.IsSuccess).ToArray() ?? [];
    }

    public FunctionTestingResult()
    {
    }
}

public class TestResult
{
    public string? Error { get; set; }
    public string? FailMsg { get; set; }
    public required string[] Parameters { get; set; }
    public bool IsPublic { get; set; }
    public bool IsSuccess => Error is null && FailMsg is null;

    public static TestResult CreateSuccess(string[] parameters, bool isPublic)
    {
        return new TestResult
        {
            Parameters = parameters,
            IsPublic = isPublic,
            Error = null,
            FailMsg = null
        };
    }

    public static TestResult CreateFailed(string[] parameters, bool isPublic, string failMsg)
    {
        return new TestResult
        {
            Parameters = parameters,
            IsPublic = isPublic,
            Error = null,
            FailMsg = failMsg
        };
    }

    public static TestResult CreateError(string[] parameters, bool isPublic, string error)
    {
        return new TestResult
        {
            Parameters = parameters,
            IsPublic = isPublic,
            Error = error,
            FailMsg = null
        };
    }
}
