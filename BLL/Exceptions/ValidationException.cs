using FluentValidation.Results;

namespace BLL.Exceptions;

public class ValidationException : Exception
{
    public List<ValidationFailure> Errors { get; } = new();

    public ValidationException(string message) : base(message) { }

    public ValidationException(List<ValidationFailure> errors)
        : base(string.Join("; ", errors.Select(e => e.ErrorMessage)))
    {
        Errors = errors;
    }
}
