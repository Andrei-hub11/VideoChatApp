using FluentEmail.Core;

using VideoChatApp.Application.Contracts.Email;
using VideoChatApp.Common.Helpers;
using VideoChatApp.Contracts.EmailModels;
using VideoChatApp.Domain.ValueObjects;

namespace VideoChatApp.Infrastructure.Email;

internal class EmailSender : IEmailSender
{
    private readonly IFluentEmail _fluentEmail;
    private readonly string _srcDirectory;

    public EmailSender(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail
            ?? throw new ArgumentNullException(nameof(fluentEmail));
        _srcDirectory = DirectoryHelper.FindDirectoryAbove(AppDomain.CurrentDomain.BaseDirectory, "src");
    }

    public async Task Send(EmailMetadata emailMetadata)
    {
        await _fluentEmail.To(emailMetadata.ToAddress)
            .Subject(emailMetadata.Subject)
            .Body(emailMetadata.Body)
            .SendAsync();
    }

    public async Task SendPasswordResetEmail(string toAddress, string resetLink, TimeSpan tokenValidity)
    {
        var model = new PasswordResetEmail(resetLink, tokenValidity);

        string templatePath = Path.Combine(_srcDirectory, "VideoChatApp.Infrastructure", "Email", "Templates", "PasswordReset.cshtml");

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template file not found at: {templatePath}");
        }

        await _fluentEmail
            .To(toAddress)
            .Subject("Password Reset")
            .UsingTemplateFromFile(templatePath, model)
            .SendAsync();
    }
}
