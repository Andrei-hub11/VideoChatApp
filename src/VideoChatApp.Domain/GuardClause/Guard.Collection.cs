using System.Runtime.CompilerServices;

using VideoChatApp.Common.Utils.ResultError;

namespace VideoChatApp.Domain.GuardClause;
public partial class Guard
{
    public IGuardInternal Contains<T>(IEnumerable<T> collection, Func<T, bool> isValidItem,
    [CallerArgumentExpression(nameof(isValidItem))] string itemExpression = "")
    {
        if (collection == null || !collection.Any(isValidItem))
        {
            ErrorList.Add(Error.Validation($"{itemExpression} must contain at least one valid item", "ERR_MISSING_VALID_ITEM", 
                itemExpression));
        }

        return this;
    }

    public IGuardInternal MaxSize(byte[] value, int maxSizeInBytes, string errorMessage, string errorCode,
        [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {
        if (value != null && value.Length > maxSizeInBytes)
        {
            ErrorList.Add(Error.Validation(errorMessage, errorCode, valueExpression));
        }

        return this;
    }
}
