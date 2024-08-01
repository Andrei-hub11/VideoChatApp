using VideoChatApp.Common.Utils.ResultError;

namespace VideoChatApp.Domain.GuardClause;

public partial class Guard
{
    public class GuardResult
    {
        public IReadOnlyList<IError> Errors { get; }

        internal GuardResult(IReadOnlyList<IError> errors)
        {
            Errors = errors;
        }

        //public Dictionary<string, string[]> ToValidationDictionary()
        //{
        //    return Errors.OfType<ValidationError>()
        //                 .GroupBy(error => error.Field)
        //                 .ToDictionary(
        //                     group => group.Key,
        //                     group => group.Select(error => error.Description).ToArray());
        //}
    }
}
