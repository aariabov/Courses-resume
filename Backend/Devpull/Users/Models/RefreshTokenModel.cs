using System.ComponentModel.DataAnnotations;

namespace Devpull.Controllers.Models;

public class RefreshTokenModel
{
    [Required(ErrorMessage = "Refresh Token обязателен")]
    public required string RefreshToken { get; set; }
}

public class RefreshTokenModelValidator : ValidatorBase<RefreshTokenModel> { }
