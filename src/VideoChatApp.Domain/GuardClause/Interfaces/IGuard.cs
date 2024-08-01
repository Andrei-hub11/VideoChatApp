using static VideoChatApp.Domain.GuardClause.Guard;

namespace VideoChatApp.Domain.GuardClause;

public interface IGuard
{
    GuardResult Use(Action<IGuardInternal> action);
}
