using System.ComponentModel.DataAnnotations;
using Devpull.Users;

namespace Devpull.Controllers.Models;

public class ResetPasswordModel
{
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddressCustom(ErrorMessage = "Некорректный формат Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Код обязателен")]
    [StringLength(maximumLength: 6, MinimumLength = 6, ErrorMessage = "Введите 6-ти значный код")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подтвердите пароль")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Device Fingerprint обязателен")]
    public string DeviceFingerprint { get; set; } = string.Empty;
}

public class ResetPasswordModelValidator : ValidatorBase<ResetPasswordModel>
{
    private readonly IUserManagerService _userManager;

    public ResetPasswordModelValidator(IUserManagerService userManager)
    {
        _userManager = userManager;
    }

    protected override async Task ValidateAsync(ValidationErrors res, ResetPasswordModel model)
    {
        if (model.ConfirmPassword != model.Password)
        {
            res.Add(nameof(model.ConfirmPassword), "Пароли не совпадают");
            return;
        }

        // TODO: политики для пароля

        var user = await _userManager.FindByEmailMaybeAsync(model.Email);
        if (user is null || !user.EmailConfirmed)
        {
            res.Add(nameof(model.Email), "Пользователя с таким Email не существует");
            return;
        }

        if (
            user.EmailConfirmationCodeExpiry is null
            || user.EmailConfirmationCodeExpiry < DateTime.UtcNow
        )
        {
            res.Add(nameof(model.Code), "Истек срок действия кода");
            return;
        }

        if (
            string.IsNullOrWhiteSpace(user.EmailConfirmationCode)
            || user.EmailConfirmationCode != model.Code
        )
        {
            res.Add(nameof(model.Code), "Неправильный код");
        }
    }
}
