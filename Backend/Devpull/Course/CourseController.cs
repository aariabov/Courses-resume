using Common;
using Devpull.Course;
using Devpull.DbModels;
using Devpull.Exercises;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Devpull.Controllers;

[ApiController]
[Route("api/course")]
public class CourseController : ControllerBase
{
    private readonly ExerciseService _exerciseService;
    private readonly ExecutionService _executionService;
    private readonly CourseService _courseService;

    public CourseController(
        ExerciseService exerciseService,
        ExecutionService executionService,
        CourseService courseService
    )
    {
        _exerciseService = exerciseService;
        _executionService = executionService;
        _courseService = courseService;
    }

    [HttpPost("get-courses")]
    public Task<OperationResult<CourseRegistryRecord[]>> GetArticles()
    {
        return _executionService.TryExecute(
            () => _courseService.GetAll(HttpContext.RequestAborted)
        );
    }

    [HttpPost("get-course-by-url")]
    public Task<OperationResult<CourseDto>> Get([FromBody] CourseParam courseParam)
    {
        return _executionService.TryExecute(
            () => _courseService.GetByUrl(courseParam, HttpContext.RequestAborted)
        );
    }

    [HttpPost("test-add")]
    public Task<OperationResult<FunctionTestingResultView>> TestAdd(
        CancellationToken cancellationToken
    )
    {
        string code1 =
            @"
            int Add(int a, int b)
    {
        if (b == 0)
        {
            var i = a / b;
        }
        return a + b;
    }
        ";
        return _executionService.TryExecute(
            () =>
                _exerciseService.Test(
                    new FunctionTestModel
                    {
                        ExerciseId = new Guid(ExerciseConst.AddExerciseId),
                        Code = code1
                    },
                    cancellationToken
                )
        );
    }

    [HttpPost("test")]
    public Task<OperationResult<FunctionTestingResultView>> Test(
        FunctionTestModel model,
        CancellationToken cancellationToken
    )
    {
        return _executionService.TryExecute(
            () => _exerciseService.Test(model, HttpContext.RequestAborted)
        );
    }

    [HttpPost("test1")]
    public Task<OperationResult<bool>> Test1()
    {
        return _executionService.TryExecute(() => _exerciseService.Test1());
    }
}
