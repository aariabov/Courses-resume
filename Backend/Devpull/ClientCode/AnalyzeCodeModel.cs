using System.ComponentModel.DataAnnotations;
using Devpull.Controllers;

namespace Devpull.ClientCode;

public class AnalyzeCodeModel
{
    [Required(ErrorMessage = "Code обязателен")]
    public string Code { get; set; } = string.Empty;
}

public class AnalyzeCodeModelValidator : ValidatorBase<AnalyzeCodeModel> { }
