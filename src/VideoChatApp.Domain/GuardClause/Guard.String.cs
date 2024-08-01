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
            ErrorList.Add(Error.Validation($"{valueExpression} não pode ser null nem vazio", 
                "ERR_IS_NULL_OR_EMPTY", valueExpression));
        }

        return this;
    }

    public IGuardInternal MaxLength(string value, int max,
      [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {

        if (value.Length > max)
        {
            ErrorList.Add(Error.Validation($"{valueExpression} deve possuir no máximo {max} caracteres",
                "ERR_TOO_LOOG", valueExpression));
        }

        return this;
    }

    public IGuardInternal MinLength(string value, int min,
        [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {

        if (value.Length < min)
        {
            ErrorList.Add(Error.Validation($"{valueExpression} deve possuir no mínimo {min} caracteres",
                "ERR_TOO_SHORT", valueExpression));
        }

        return this;
    }

    public IGuardInternal InRange(string value, int min, int max,
        [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {

        if (value.Length < min || value.Length > max)
        {
            ErrorList.Add(Error.Validation($"{valueExpression} deve possuir no mínimo {min} caracteres e no máximo {max}",
                "ERR_LENGTH_OUT_OF_RANGE", valueExpression));
        }

        return this;
    }

    public IGuardInternal AllNumeric(string value, [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {
        if (!NumericRegex.IsMatch(value))
        {
            ErrorList.Add(Error.Validation($"{valueExpression} deve conter apena números",
                "ERR_NOT_NUMERIC", valueExpression));
        }

        return this;
    }

    public IGuardInternal IsAlphanumeric(string value, [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {
        if (!AlphanumericRegex.IsMatch(value))
        {
            ErrorList.Add(Error.Validation($"{valueExpression} deve conter apenas letras e números",
                "ERR_NOT_ALPHANUMERIC", valueExpression));
        }

        return this;
    }
}
