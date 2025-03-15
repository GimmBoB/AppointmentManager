using AppointmentManager.Shared;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AppointmentManager.Web.Validation;

public class BaseValidator<T> : AbstractValidator<T>, IBaseValidator<T>
    where T : class
{
    private readonly IStringLocalizer _localizer;
    protected BaseValidator(IStringLocalizer localizer)
    {
        _localizer = localizer;
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue() => async (model, propertyName) => await ValidateAsync(model, propertyName);

    public async Task<IEnumerable<string>> ValidateValue(object model, string propertyName)
    {
        return await ValidateAsync(model, propertyName);
    }

    public async Task<(ValidationContext<T>, IEnumerable<string>)> ValidateValueAndGetContext(object model,
        string propertyName)
    {
        var validationContext = ValidationContext<T>.CreateWithOptions((T)model,
            x => x.IncludeProperties(propertyName.SubstringFromChar('.'))); 
        
        var result = 
            await ValidateAsync(validationContext);

        return result.IsValid
            ? (validationContext, Enumerable.Empty<string>())
            : (validationContext, result.Errors.Select(e => e.ErrorMessage));
    }
    
    protected string CreateLocalizedErrorMessage(string errorMessage)
    {
        return CreateLocalizedErrorMessage(errorMessage, Array.Empty<object>());
    }

    private string CreateLocalizedErrorMessage(string errorMessage, params object[] args)
    {
        return _localizer?[errorMessage, args] ?? string.Format(errorMessage, args);
    }

    private async Task<IEnumerable<string>> ValidateAsync(object model, string propertyName)
    {
        var validationContext = ValidationContext<T>.CreateWithOptions((T)model,
            x => x.IncludeProperties(propertyName.SubstringFromChar('.'))); 
        
        var result = 
            await ValidateAsync(validationContext);

        return result.IsValid
            ? Enumerable.Empty<string>()
            : result.Errors.Select(e => e.ErrorMessage);
    }
}