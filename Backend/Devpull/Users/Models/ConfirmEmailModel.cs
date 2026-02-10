using System.ComponentModel.DataAnnotations;
using Devpull.Users;
using Microsoft.AspNetCore.Identity;

namespace Devpull.Controllers.Models;

public class ConfirmEmailModel
{
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Код обязателен")]
    [StringLength(maximumLength: 6, MinimumLength = 6, ErrorMessage = "Введите 6-ти значный код")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Device Fingerprint обязателен")]
    public string DeviceFingerprint { get; set; } = string.Empty;
}

public class ConfirmEmailModelValidator : ValidatorBase<ConfirmEmailModel>
{
    private readonly IUserManagerService _userManager;

    public ConfirmEmailModelValidator(IUserManagerService userManager)
    {
        _userManager = userManager;
    }

    protected override async Task ValidateAsync(ValidationErrors res, ConfirmEmailModel model)
    {
        var user = await _userManager.FindByEmailMaybeAsync(model.Email);
        if (user is null)
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
