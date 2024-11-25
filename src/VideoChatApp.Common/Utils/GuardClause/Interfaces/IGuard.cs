namespace VideoChatApp.Common.Utils.GuardClause;

public interface IGuard
{
    GuardResult Use(Action<IGuardInternal> action);
}
