using System.Runtime.CompilerServices;

namespace UtilityKit.DX;

/// <summary>
/// Represents the outcome of an <see cref="Ensure"/> guard clause check.
/// </summary>
/// <remarks>
/// Provides a consistent way to handle guard clause results without exceptions.
/// </remarks>
public class EnsureResult
{
    /// <summary>
    /// Indicates whether the guard check passed.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Provides a descriptive message if the guard check failed; empty when successful.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Creates a new <see cref="EnsureResult"/>.
    /// </summary>
    /// <param name="isValid">True if validation passed; otherwise false.</param>
    /// <param name="message">Optional failure message.</param>
    public EnsureResult(bool isValid, string message = "")
    {
        IsValid = isValid;
        Message = string.IsNullOrWhiteSpace(message) && !isValid
            ? "Validation failed."
            : message;
    }

    /// <summary>
    /// A reusable success result instance.
    /// </summary>
    public static EnsureResult Success => new(true);

    /// <summary>
    /// Creates a failure result with a message.
    /// </summary>
    /// <param name="message">Failure reason.</param>
    public static EnsureResult Fail(string message = "") =>
        new(false, string.IsNullOrWhiteSpace(message) ? "Validation failed." : message);
}

/// <summary>
/// Provides fluent guard clause builder for chaining validations.
/// </summary>
/// <typeparam name="T">The type of the value being validated.</typeparam>
public class EnsureBuilder<T>
{
    private readonly T _value;
    private readonly string _paramName;
    private EnsureResult _result = EnsureResult.Success;

    public EnsureBuilder(T value, string paramName)
    {
        _value = value;
        _paramName = paramName;
    }

    /// <summary>
    /// Ensures the value is not null.
    /// </summary>
    /// <example>
    /// <code>
    /// var result = Ensure.That(customer)
    ///     .NotNull()
    ///     .Result;
    /// </code>
    /// </example>
    public EnsureBuilder<T> NotNull(string? message = null)
    {
        if (_result.IsValid && _value is null)
        {
            _result = EnsureResult.Fail($"{_paramName} cannot be null. {message}".Trim());
        }
        return this;
    }

    /// <summary>
    /// Ensures the value is a non-empty string.
    /// </summary>
    /// <example>
    /// <code>
    /// var result = Ensure.That(orderId)
    ///     .NotEmpty()
    ///     .Result;
    /// </code>
    /// </example>
    public EnsureBuilder<T> NotEmpty(string? message = null)
    {
        if (_result.IsValid && _value is string s && string.IsNullOrWhiteSpace(s))
        {
            _result = EnsureResult.Fail($"{_paramName} cannot be empty. {message}".Trim());
        }
        return this;
    }

    /// <summary>
    /// Ensures the value is within a numeric range.
    /// </summary>
    /// <example>
    /// <code>
    /// var result = Ensure.That(age)
    ///     .InRange(18, 65)
    ///     .Result;
    /// </code>
    /// </example>
    public EnsureBuilder<T> InRange(int min, int max, string? message = null)
    {
        if (_result.IsValid && _value is int v && (v < min || v > max))
        {
            _result = EnsureResult.Fail($"{_paramName} must be between {min} and {max}. {message}".Trim());
        }
        return this;
    }

    /// <summary>
    /// Ensures the collection is not null and contains at least one element.
    /// </summary>
    /// <example>
    /// <code>
    /// var result = Ensure.That(items)
    ///     .Any()
    ///     .Result;
    /// </code>
    /// </example>
    public EnsureBuilder<T> Any(string? message = null)
    {
        if (_result.IsValid && _value is IEnumerable<object> col && !col.Cast<object>().Any())
        {
            _result = EnsureResult.Fail($"{_paramName} must contain at least one item. {message}".Trim());
        }
        return this;
    }

    /// <summary>
    /// Ensures the value satisfies a custom predicate.
    /// </summary>
    /// <example>
    /// <code>
    /// var result = Ensure.That(order)
    ///     .Satisfies(o => o.OrderId.StartsWith("UK-") && o.TotalAmount > 0 && o.Items.Any(),
    ///                "Order must have a valid ID, positive total, and at least one item.")
    ///     .Result;
    /// </code>
    /// </example>
    public EnsureBuilder<T> Satisfies(Func<T, bool> predicate, string message = "Validation failed.")
    {
        if (_result.IsValid && !predicate(_value))
        {
            _result = EnsureResult.Fail($"{_paramName}: {message}");
        }
        return this;
    }

    /// <summary>
    /// Ensures a condition is true.
    /// </summary>
    /// <example>
    /// <code>
    /// var result = Ensure.That(total)
    ///     .True(t => t >= 0, "Total cannot be negative.")
    ///     .Result;
    /// </code>
    /// </example>
    public EnsureBuilder<T> True(Func<T, bool> condition, string message)
    {
        if (_result.IsValid && !condition(_value))
        {
            _result = EnsureResult.Fail(message);
        }
        return this;
    }

    /// <summary>
    /// Final result of the chained validations.
    /// </summary>
    public EnsureResult Result => _result;
}

/// <summary>
/// Provides guard clause methods to validate arguments and enforce preconditions,
/// returning <see cref="EnsureResult"/> instead of throwing exceptions.
/// </summary>
/// <remarks>
/// Use these guards to keep method bodies clean and avoid widespread try/catch blocks.
/// </remarks>
/// <example>
/// <code>
/// // Example 1: Null check
/// var result = Ensure.That(customer)
///     .NotNull()
///     .Result;
///
/// if (!result.IsValid) Console.WriteLine(result.Message);
///
/// // Example 2: String not empty
/// var result = Ensure.That(orderId)
///     .NotEmpty()
///     .Result;
///
/// if (!result.IsValid) Console.WriteLine(result.Message);
///
/// // Example 3: Numeric range
/// var result = Ensure.That(age)
///     .InRange(18, 65)
///     .Result;
///
/// if (!result.IsValid) Console.WriteLine(result.Message);
///
/// // Example 4: Collection must contain items
/// var result = Ensure.That(items)
///     .Any()
///     .Result;
///
/// if (!result.IsValid) Console.WriteLine(result.Message);
///
/// // Example 5: Custom predicate for business rule
/// var result = Ensure.That(order)
///     .Satisfies(o => o.OrderId.StartsWith("UK-") && o.TotalAmount > 0 && o.Items.Any(),
///                "Order must have a valid ID, positive total, and at least one item.")
///     .Result;
///
/// if (!result.IsValid) Console.WriteLine(result.Message);
/// </code>
/// </example>
public static class Ensure
{
    /// <summary>
    /// Starts a fluent validation chain for the given value.
    /// </summary>
    /// <typeparam name="T">The type of the value being validated.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">Automatically inferred parameter name.</param>
    /// <returns>An <see cref="EnsureBuilder{T}"/> for chaining validations.</returns>
    public static EnsureBuilder<T> That<T>(
        T value,
        [CallerArgumentExpression("value")] string paramName = ""
    ) => new(value, paramName);
}
