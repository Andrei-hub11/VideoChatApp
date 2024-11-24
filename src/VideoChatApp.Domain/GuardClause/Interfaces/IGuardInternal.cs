using System.Runtime.CompilerServices;

namespace VideoChatApp.Domain.GuardClause;

public interface IGuardInternal
{
    void DoNotThrowOnError();
    IGuardInternal IsNullOrWhiteSpace(string value,
        [CallerArgumentExpression(nameof(value))] string valueExpression = "");
    IGuardInternal MaxLength(string value, int max,
      [CallerArgumentExpression(nameof(value))] string valueExpression = "");
    IGuardInternal MinLength(string value, int min,
        [CallerArgumentExpression(nameof(value))] string valueExpression = "");
    IGuardInternal InRange(string value, int min, int max,
        [CallerArgumentExpression(nameof(value))] string valueExpression = "");
    IGuardInternal AllNumeric(string value, [CallerArgumentExpression(nameof(value))] string valueExpression = "");
    IGuardInternal IsAlphanumeric(string value, [CallerArgumentExpression(nameof(value))] string valueExpression = "");
    IGuardInternal MatchesPattern(string value, string pattern, string errorMessage, string errorCode,
    [CallerArgumentExpression(nameof(value))] string valueExpression = "");
    IGuardInternal FailIf(bool condition, string message, string code, string field);
    IGuardInternal Contains<T>(IEnumerable<T> collection, Func<T, bool> isValidItem,
        [CallerArgumentExpression(nameof(isValidItem))] string itemExpression = "");
    IGuardInternal MaxSize(byte[] value, int maxSizeInBytes, string errorMessage, string errorCode,
    [CallerArgumentExpression(nameof(value))] string valueExpression = "");
}