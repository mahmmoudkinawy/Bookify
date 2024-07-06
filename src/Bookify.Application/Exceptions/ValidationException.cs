namespace Bookify.Application.Exceptions;
public sealed class ValidationException(List<ValidationError> validationErrors) : Exception
{
    public IEnumerable<ValidationError> ValidationErrors { get; private set; } = validationErrors;
}
