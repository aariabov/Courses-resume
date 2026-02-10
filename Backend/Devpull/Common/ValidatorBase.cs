using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Serialization;

namespace Devpull.Controllers;

public class ValidatorBase<T>
    where T : notnull
{
    protected virtual Task ValidateAsync(ValidationErrors res, T model)
    {
        return Task.CompletedTask;
    }

    public async Task Validate(T model)
    {
        var validationErrors = ValidateObject(model);
        ThrowIfHasErrors(validationErrors);

        await ValidateAsync(validationErrors, model);
        ThrowIfHasErrors(validationErrors);
    }

    private static void ThrowIfHasErrors(ValidationErrors validationErrors)
    {
        if (validationErrors.Any())
        {
            validationErrors = MapToCamelCase(validationErrors);
            throw new ValidationFailException(validationErrors);
        }
    }

    private static ValidationErrors ValidateObject(object model)
    {
        var validationResult = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResult, true);

        var errorList = new ValidationErrors();
        foreach (var result in validationResult)
        {
            if (result.ErrorMessage == null)
            {
                continue;
            }

            foreach (var fieldName in result.MemberNames)
            {
                errorList.Add(fieldName, result.ErrorMessage);
            }
        }

        return errorList;
    }

    private static ValidationErrors MapToCamelCase(ValidationErrors dict)
    {
        var res = new ValidationErrors();
        var resolver = new CamelCasePropertyNamesContractResolver();
        foreach (var item in dict)
        {
            var key = resolver.GetResolvedPropertyName(item.Key);
            res[key] = item.Value;
        }

        return res;
    }
}

public class ValidationErrors : Dictionary<string, List<string>>
{
    public void Add(string fieldName, string error)
    {
        if (this.TryGetValue(fieldName, out var errors))
        {
            errors.Add(error);
        }
        else
        {
            this[fieldName] = [error];
        }
    }
}
