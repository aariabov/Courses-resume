using System.ComponentModel.DataAnnotations;
using Devpull.Users;
using Microsoft.AspNetCore.Identity;

namespace Devpull.Controllers.Models;

public class RegisterModel
{
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddressCustom(ErrorMessage = "Некорректный формат Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подтвердите пароль")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class RegisterModelValidator : ValidatorBase<RegisterModel>
{
    private readonly IUserManagerService _userManager;

    public RegisterModelValidator(IUserManagerService userManager)
    {
        _userManager = userManager;
    }

    protected override async Task ValidateAsync(ValidationErrors res, RegisterModel model)
    {
        if (model.ConfirmPassword != model.Password)
        {
            res.Add(nameof(model.ConfirmPassword), "Пароли не совпадают");
            return;
        }

        // TODO: политики для пароля

        var user = await _userManager.FindByEmailMaybeAsync(model.Email);
        if (user is { EmailConfirmed: true })
        {
            res.Add(nameof(model.Email), "Пользователь уже существует, попробуйте Войти");
        }
    }
}
