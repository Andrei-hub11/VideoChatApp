﻿using System.Collections.ObjectModel;

using VideoChatApp.Application.Common.Result;
using VideoChatApp.Common.Utils.ResultError;
using VideoChatApp.Contracts.DapperModels;
using VideoChatApp.Domain.GuardClause;

namespace VideoChatApp.Domain.Entities;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string ProfileImagePath { get; set; } = string.Empty;
    public byte[] ProfileImage { get; set; } = [];
    public IReadOnlySet<string> Roles { get; set; } = new HashSet<string>();

    private User(string id, string name, string email, string passwordHash, byte[] profileImage, string profileImagePath, IReadOnlySet<string> roles)
    {
        Id = id;
        UserName = name;
        Email = email;
        PasswordHash = passwordHash;
        ProfileImage = profileImage;
        ProfileImagePath = profileImagePath;
        Roles = roles;
    }

    public static Result<User> Create(string id, string name, string email, string password, byte[] profileImage,
        string profileImagePath, IReadOnlySet<string> roles)
    {
        var errors = ValidateUser(id, name, email, roles, password, profileImage, profileImagePath);

        if (errors.Any())
        {
            return Result.Fail(errors);
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        return new User(id, name, email, passwordHash, profileImage, profileImagePath, roles);
    }

    public static Result<User> From(ApplicationUserMapping applicationUser)
    {
        var errors = ValidateUser(applicationUser.Id, applicationUser.UserName, applicationUser.Email,
            applicationUser.Roles);

        if (errors.Any())
        {
            return Result.Fail(errors);
        }

        return new User(applicationUser.Id, applicationUser.UserName, applicationUser.Email,
            applicationUser.PasswordHash, Array.Empty<byte>(), applicationUser.ProfileImagePath,
            applicationUser.Roles);
    }

    private static ReadOnlyCollection<ValidationError> ValidateUser(string id, string name, string email,
        IReadOnlySet<string> roles, string? password = null, byte[]? profileImage = null,
        string? profileImagePath = null)
    {
        var errors = new List<IError>();
        var isValidRole = new HashSet<string> { "Admin", "User", "Manager" };

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

        if (password is not null)
        {
            errors.AddRange(ValidatePassword(password));
        }

        errors.AddRange(ValidateRoles(roles));

        if (profileImage is not null && profileImagePath is not null && profileImage.Length > 0)
        {
            errors.AddRange(ValidateImage(profileImage, profileImagePath));
        }

        return errors.OfType<ValidationError>().ToList().AsReadOnly();
    }


    public static IReadOnlyList<ValidationError> ValidateEmail(string email)
    {
        var result = Guard.For().Use((guard) =>
        {
            guard
            .IsNullOrWhiteSpace(email)
            .MatchesPattern(
                email,
                 @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$",
                "Email inválido",
                "ERR_EMAIL_INVALID"
                )
            .DoNotThrowOnError();
        });

        return result.Errors.OfType<ValidationError>().ToList();
    }

    public static IReadOnlyList<ValidationError> ValidatePassword(string password)
    {
        var result = Guard.For().Use((guard) =>
        {
            guard
            .IsNullOrWhiteSpace(password)
            .MinLength(password, 8)
            .MatchesPattern(
                password,
                @"(?:.*[!@#$%^&*]){2,}",
                "Senha inválida. A senha deve ter pelo menos dois caracteres especiais",
                "ERR_INVALID_PASSWORD",
                nameof(password)
                )
            .DoNotThrowOnError();
        });

        return result.Errors.OfType<ValidationError>().ToList();
    }

    public static IReadOnlyList<ValidationError> ValidateImage(byte[] profileImage, string profileImagePath)
    {
        var result = Guard.For().Use((guard) =>
        {
            guard
            .IsNullOrWhiteSpace(profileImagePath)
            .MaxSize(
             profileImage,
             2 * 1024 * 1024, "A imagem não pode ter mais que dois 2 megabytes de tamanho",
             "ERR_INVALID_PROFILE-IMAGE"
             )
            .DoNotThrowOnError();
        });

        return result.Errors.OfType<ValidationError>().ToList();
    }

    public static IReadOnlyList<ValidationError> ValidateRoles(IReadOnlySet<string> roles)
    {
        var isValidRole = new HashSet<string> { "Admin", "User" };

        var result = Guard
             .For().Use(guard =>
             {
                 guard
                 .Contains(roles, role => isValidRole.Contains(role))
                 .DoNotThrowOnError();
             });

        return result.Errors.OfType<ValidationError>().ToList();
    }


    public bool VerifyPassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }

    public Result<bool> UpdateProfileImage(byte[] newProfileImage, string newProfileImagePath)
    {
        var errors = ValidateImage(newProfileImage, newProfileImagePath);

        if (errors.Any())
        {
            return Result.Fail(errors);
        }

        ProfileImage = newProfileImage;
        ProfileImagePath = newProfileImagePath;

        return true;
    }
}