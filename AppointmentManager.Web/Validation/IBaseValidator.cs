using FluentValidation;

namespace AppointmentManager.Web.Validation;

public interface IBaseValidator<T> : IValidator<T>
    where T : class
{
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue();
    public Task<IEnumerable<string>> ValidateValue(object model, string propertyName);

    public Task<(ValidationContext<T>, IEnumerable<string>)> ValidateValueAndGetContext(object model,
        string propertyName);
}