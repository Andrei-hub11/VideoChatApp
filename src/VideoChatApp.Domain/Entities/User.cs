using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

using VideoChatApp.Application.Common.Result;
using VideoChatApp.Common.Utils.ResultError;
using VideoChatApp.Domain.GuardClause;

namespace VideoChatApp.Domain.Entities;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string ProfileImageUrl { get; set; } = string.Empty;
    public byte[] ProfileImage { get; set; } = [];
    public IReadOnlySet<string> Roles { get; set; } = new HashSet<string>();

    private User(string id, string name, string email, string password, IReadOnlySet<string> roles)
    {
        Id = id;
        UserName = name;
        Email = email;
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        Roles = roles;
    }

    public static Result<User> Create(string id, string name, string email, string password,
        HashSet<string> roles)
    {
        var errors = ValidateUser(id, name, email, password);

        if (errors.Any())
        {
            return Result.Fail(errors);
        }

        return new User(id, name, email, password, roles);
    }

    private static ReadOnlyCollection<ValidationError> ValidateUser(string id, string name, string email, string password)
    {
        var errors = new List<IError>();

        // Validar id e name
        var guardResult = Guard
            .For()
            .Use((guard) =>
            {
                guard.IsNullOrWhiteSpace(id)
                    .IsNullOrWhiteSpace(name)
                    .MaxLength(name, 120)
                    .DoNotThrowOnError();
            });

        errors.AddRange(guardResult.Errors);

        errors.AddRange(ValidateEmail(email));

        errors.AddRange(ValidatePassword(password));

        return errors.OfType<ValidationError>().ToList().AsReadOnly();
    }


    public static IReadOnlyList<ValidationError> ValidateEmail(string email)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add(Error.Validation(
                "O email é obrigatório", 
                "ERR_EMAIL_EMPTY",
                nameof(email)));
        }

        if (!Regex.IsMatch(email, @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$"))
        {
            errors.Add(Error.Validation(
                "Email inválido", 
                "ERR_EMAIL_INVALID",
                nameof(email)));
        }

        return errors.OfType<ValidationError>().ToList().AsReadOnly();
    }

    public static IReadOnlyList<ValidationError> ValidatePassword(string password)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add(Error.Validation(
                "A senha é obrigatória", 
                "ERR_PASSWORD_EMPTY",
                nameof(password)));
        }

        if (password.Length < 8)
        {
            errors.Add(Error.Validation(
                "A senha deve ter pelo menos oito caracteres", 
                "ERR_TOO_LOW",
                nameof(password)));
        }

        if (!Regex.IsMatch(password, @"(?:.*[!@#$%^&*]){2,}"))
        {
            errors.Add(Error.Validation(
                "Senha inválida. A senha deve ter pelo menos dois caracteres especiais",
                "ERR_INVALID_PASSWORD",
                nameof(password)
            ));
        }

        return errors.OfType<ValidationError>().ToList().AsReadOnly();
    }
}
