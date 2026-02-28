using FluentValidation.Results;

namespace SwiggyClone.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when one or more FluentValidation rules fail during
/// the MediatR validation pipeline.
/// </summary>
public sealed class ValidationException : Exception
{
    private const string DefaultMessage = "One or more validation failures have occurred.";

    /// <summary>
    /// A dictionary mapping property names to their associated validation error messages.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Creates a new <see cref="ValidationException"/> with an empty error dictionary.
    /// </summary>
    public ValidationException()
        : base(DefaultMessage)
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Creates a new <see cref="ValidationException"/> from a collection of
    /// <see cref="ValidationFailure"/> instances, grouping error messages by property name.
    /// </summary>
    /// <param name="failures">The validation failures to include.</param>
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base(DefaultMessage)
    {
        Errors = failures
            .GroupBy(f => f.PropertyName, StringComparer.Ordinal)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray(),
                StringComparer.Ordinal);
    }
}
