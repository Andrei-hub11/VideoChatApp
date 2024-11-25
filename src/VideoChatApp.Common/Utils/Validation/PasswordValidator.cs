using VideoChatApp.Common.Utils.GuardClause;
using VideoChatApp.Common.Utils.ResultError;

namespace VideoChatApp.Common.Utils.Validation;

public static class PasswordValidator
{
    public static IReadOnlyList<ValidationError> ValidatePassword(
        string password,
        string fieldName = "Password"
    )
    {
        var result = Guard
            .For()
            .Use(
                (guard) =>
                {
                    guard
                        .IsNullOrWhiteSpace(password, fieldName)
                        .MinLength(password, 8, fieldName)
                        .MatchesPattern(
                            password,
                            @"(?:.*[!@#$%^&*]){2,}",
                            "Invalid password. The password must have at least two special characters",
                            "ERR_INVALID_PASSWORD",
                            fieldName
                        )
                        .DoNotThrowOnError();
                }
            );

        return result.Errors;
    }
}
