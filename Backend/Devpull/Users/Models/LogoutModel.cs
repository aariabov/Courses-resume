using System.ComponentModel.DataAnnotations;

namespace Devpull.Controllers.Models;

public class LogoutModel
{
    [Required(ErrorMessage = "Refresh Token обязателен")]
    public required string RefreshToken { get; set; }
}

public class LogoutModelValidator : ValidatorBase<LogoutModel> { }
