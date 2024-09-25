using VideoChatApp.Common.Utils.ResultError;

namespace VideoChatApp.Domain.GuardClause;

public class GuardResult
{
    public IReadOnlyList<ValidationError> Errors { get; }

    internal GuardResult(IReadOnlyList<ValidationError> errors)
    {
        Errors = errors;
    }
}
