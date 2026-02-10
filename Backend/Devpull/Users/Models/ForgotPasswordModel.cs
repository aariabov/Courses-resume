using System.ComponentModel.DataAnnotations;
using Devpull.Users;

namespace Devpull.Controllers.Models;

public class ForgotPasswordModel
{
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат Email")]
    public string Email { get; set; } = string.Empty;
}

public class ForgotPasswordModelValidator : ValidatorBase<ForgotPasswordModel>
{
    private readonly IUserManagerService _userManager;

    public ForgotPasswordModelValidator(IUserManagerService userManager)
    {
        _userManager = userManager;
    }

    protected override async Task ValidateAsync(ValidationErrors res, ForgotPasswordModel model)
    {
        var user = await _userManager.FindByEmailMaybeAsync(model.Email);
        if (user is null || !user.EmailConfirmed)
        {
            res.Add(nameof(model.Email), "Пользователя с таким Email не существует");
            return;
        }
    }
}
