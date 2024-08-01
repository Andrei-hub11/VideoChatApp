namespace VideoChatApp.Common.Utils.ResultError;

public interface IError
{
    string Description { get; }
    string Code { get; }
    ErrorType ErrorType { get; }
}
