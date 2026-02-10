using System.ComponentModel.DataAnnotations;
using Devpull.Controllers;
using Devpull.Users;

namespace Devpull.Course;

public class FunctionTestModel
{
    [Required(ErrorMessage = "ExerciseId обязателен")]
    public Guid ExerciseId { get; set; }

    [Required(ErrorMessage = "Code обязателен")]
    public string Code { get; set; } = string.Empty;
}

public class FunctionTestModelValidator : ValidatorBase<FunctionTestModel>
{
    private readonly AuthService _authService;
    private readonly IUserManagerService _userManagerService;
    private readonly Devpull.Common.AppConfig _config;

    public FunctionTestModelValidator(
        AuthService authService,
        IUserManagerService userManagerService,
        Devpull.Common.AppConfig config
    )
    {
        _authService = authService;
        _userManagerService = userManagerService;
        _config = config;
    }

    protected override async Task ValidateAsync(ValidationErrors res, FunctionTestModel model)
    {
    var freeRunsPerDay = _config.User.FreeRunsPerDay;

        var userId = _authService.GetUserIdOrThrow();
        var user = await _userManagerService.FindByIdAsync(userId);
        var (canRun, fromDate) = user.CanUserRunExercise(freeRunsPerDay);
        if (!canRun)
        {
            throw new AuthorizationException(
                $"Количество бесплатных запусков закончилось. Оформите подписку. Бесплатные запуски будут доступны {fromDate:dd.MM.yyyy HH:mm}"
            );
        }
    }
}
