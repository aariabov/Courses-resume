using Devpull.Controllers;
using Devpull.Exercises.Models;
using Microsoft.AspNetCore.Mvc;

namespace Devpull.Exercises;

[ApiController]
[Route("api/exercise")]
public class ExerciseController : ControllerBase
{
    private readonly ExerciseService _exerciseService;
    private readonly ExecutionService _executionService;

    public ExerciseController(ExerciseService exerciseService, ExecutionService executionService)
    {
        _exerciseService = exerciseService;
        _executionService = executionService;
    }

    [HttpPost("get-exercises")]
    public Task<OperationResult<ExerciseRegistryRecord[]>> GetExercises()
    {
        return _executionService.TryExecute(
            () => _exerciseService.GetAll(HttpContext.RequestAborted)
        );
    }

    [HttpPost("get-exercise-by-url")]
    public Task<OperationResult<ExerciseDto>> GetExerciseByUrl([FromBody] string url)
    {
        return _executionService.TryExecute(
            () => _exerciseService.GetByUrl(url, HttpContext.RequestAborted)
        );
    }

    [HttpPost("get-run-exercise-history")]
    public Task<OperationResult<RunExerciseHistoryDto[]>> GetRunExerciseHistory(
        [FromBody] Guid exerciseId
    )
    {
        return _executionService.TryExecute(
            () => _exerciseService.GetRunExerciseHistory(exerciseId, HttpContext.RequestAborted)
        );
    }
}
