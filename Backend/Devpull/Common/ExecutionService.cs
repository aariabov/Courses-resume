using System.Diagnostics;
using Devpull.Course;
using Serilog;
using Serilog.Context;

namespace Devpull.Controllers;

public class ExecutionService
{
    private readonly AuthService _authService;

    public ExecutionService(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<OperationResult<TRes>> TryExecute<TRes>(Func<Task<TRes>> callback)
    {
        var sw = Stopwatch.StartNew();
        var requestId = _authService.GetRequestId();
        var userId = _authService.GetUserId();

        using (LogContext.PushProperty("RequestId", requestId))
        using (LogContext.PushProperty("UserId", userId))
        {
            try
            {
                var result = await callback();
                return OperationResult.Ok(result);
            }
            catch (AuthenticationException e)
            {
                LogException(e);
                return OperationResult.FailAuthentication<TRes>(e.Message);
            }
            catch (ValidationFailException e)
            {
                LogException(e);
                return OperationResult.FailValidation<TRes>(e.ErrorsList);
            }
            catch (NotFoundException e)
            {
                LogException(e);
                return OperationResult.NotFound<TRes>(e.Message);
            }
            catch (Exception e)
            {
                LogException(e);
                return OperationResult.Fail<TRes>(e.Message);
            }
            finally
            {
                sw.Stop();

                var elapsedMs = sw.ElapsedMilliseconds;
                Log.Information(
                    "Request {RequestId} completed in {ElapsedMs} ms",
                    requestId,
                    elapsedMs
                );
            }
        }
    }

    private static void LogException(Exception e)
    {
        Log.Error(e, "Ошибка: {e.Message}", e.Message);
    }
}

public class ValidationFailException : Exception
{
    public ValidationErrors ErrorsList { get; }

    public ValidationFailException(ValidationErrors errors)
        : base("Validation exception")
    {
        ErrorsList = errors;
    }
}
