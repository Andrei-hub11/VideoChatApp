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
}