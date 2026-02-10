using System.ComponentModel.DataAnnotations;
using Devpull.Users;
using Microsoft.AspNetCore.Identity;

namespace Devpull.Controllers.Models;

public class LoginModel
{
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Device Fingerprint обязателен")]
    public string DeviceFingerprint { get; set; } = string.Empty;

    [Required(ErrorMessage = "Флаг Запомнить меня обязательный")]
    public bool? RememberMe { get; set; }
}

public class TokensModel
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
}

public class LoginModelValidator : ValidatorBase<LoginModel>
{
    private readonly IUserManagerService _userManager;

    public LoginModelValidator(IUserManagerService userManager)
    {
        _userManager = userManager;
    }

    protected override async Task ValidateAsync(ValidationErrors res, LoginModel model)
    {
        var user = await _userManager.FindByEmailMaybeAsync(model.Email);
        if (user is null || !user.EmailConfirmed)
        {
            res.Add(nameof(model.Email), "Пользователя с таким Email не существует");
            return;
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!isPasswordValid)
        {
            res.Add(nameof(model.Password), "Неправильный пароль");
        }
    }
}
