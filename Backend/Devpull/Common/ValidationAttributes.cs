using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Devpull.Controllers;

public partial class EmailAddressCustomAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return false;
        }

        var email = value.ToString();

        if (email != null && !EmailRegex().IsMatch(email))
        {
            return false;
        }

        return true;
    }

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    private static partial Regex EmailRegex();
}
