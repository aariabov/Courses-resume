using System.ComponentModel.DataAnnotations;
using Devpull.Controllers;
using Devpull.Course;

namespace Devpull.ClientCode;

public class ClientCodeModel
{
    [Required(ErrorMessage = "Code обязателен")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Inputs обязателен")]
    public string[] Inputs { get; set; } = [];
}

public class ClientCodeModelValidator : ValidatorBase<ClientCodeModel>
{
    
}