using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using VideoChatApp.Common.Utils.ResultError;

namespace VideoChatApp.Domain.GuardClause;

public partial class Guard
{
    private static readonly Regex NumericRegex = new Regex("^[0-9]*$", RegexOptions.Compiled);
    private static readonly Regex AlphanumericRegex = new Regex("^[a-zA-Z0-9]*$", RegexOptions.Compiled);

    public IGuardInternal IsNullOrWhiteSpace(string value,
    [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ErrorList.Add(Error.Validation($"{valueExpression} cannot be null or empty",
                "ERR_IS_NULL_OR_EMPTY", valueExpression));
        }

        return this;
    }

    public IGuardInternal MaxLength(string value, int max,
        [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {
        if (value.Length > max)
        {
            ErrorList.Add(Error.Validation($"{valueExpression} must have a maximum of {max} characters",
                "ERR_TOO_LONG", valueExpression));
        }

        return this;
    }

    public IGuardInternal MinLength(string value, int min,
        [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {
        if (value.Length < min)
        {
            ErrorList.Add(Error.Validation($"{valueExpression} must have at least {min} characters",
                "ERR_TOO_SHORT", valueExpression));
        }

        return this;
    }

    public IGuardInternal InRange(string value, int min, int max,
        [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {
        if (value.Length < min || value.Length > max)
        {
            ErrorList.Add(Error.Validation($"{valueExpression} must be between {min} and {max} characters",
                "ERR_LENGTH_OUT_OF_RANGE", valueExpression));
        }

        return this;
    }

    public IGuardInternal AllNumeric(string value, [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {
        if (!NumericRegex.IsMatch(value))
        {
            ErrorList.Add(Error.Validation($"{valueExpression} must contain only numbers",
                "ERR_NOT_NUMERIC", valueExpression));
        }

        return this;
    }

    public IGuardInternal IsAlphanumeric(string value, [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {
        if (!AlphanumericRegex.IsMatch(value))
        {
            ErrorList.Add(Error.Validation($"{valueExpression} must contain only letters and numbers",
                "ERR_NOT_ALPHANUMERIC", valueExpression));
        }

        return this;
    }


    public IGuardInternal MatchesPattern(
        string value, string pattern, string errorMessage, string errorCode,
        [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {
        if (!Regex.IsMatch(value, pattern))
        {
            ErrorList.Add(Error.Validation(errorMessage, errorCode, valueExpression));
        }

        return this;
    }

    public IGuardInternal FailIf(bool condition, string message, string code, string field)
    {
        if (condition)
        {
            ErrorList.Add(Error.Validation(message, code, field));
        }

        return this;
    }
}
