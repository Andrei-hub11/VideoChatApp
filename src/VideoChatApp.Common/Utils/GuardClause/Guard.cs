using VideoChatApp.Common.Utils.ResultError;

namespace VideoChatApp.Common.Utils.GuardClause;

public partial class Guard : IGuard, IGuardInternal, IDisposable
{
    private bool _throwCalled = false;
    private List<ValidationError> ErrorList { get; set; } = [];

    private Guard() { }

    public static IGuard For()
    {
        return new Guard();
    }

    public GuardResult Use(Action<IGuardInternal> action)
    {
        using (this)
        {
            action(this);
        }

        return new GuardResult(ErrorList);
    }

    public void ThrowIfInvalid()
    {
        _throwCalled = true;
        // ...
    }

    public void DoNotThrowOnError()
    {
        _throwCalled = true;
    }

    public void Dispose()
    {
        if (!_throwCalled)
        {
            throw new InvalidOperationException(
                "'ThrowIfInvalid' or 'DoNotThrowOnError' must be called to complete the chain."
            );
        }
    }
}
