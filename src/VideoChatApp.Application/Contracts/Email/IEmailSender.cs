using VideoChatApp.Domain.ValueObjects;

namespace VideoChatApp.Application.Contracts.Email;

public interface IEmailSender
{
    Task Send(EmailMetadata emailMetadata);
    Task SendPasswordResetEmail(string toAddress, string resetLink, TimeSpan tokenValidity);
}
