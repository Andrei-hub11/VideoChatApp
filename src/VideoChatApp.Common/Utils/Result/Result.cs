using System.Diagnostics.CodeAnalysis;

using VideoChatApp.Common.Utils.Errors;
using VideoChatApp.Common.Utils.ResultError;

namespace VideoChatApp.Application.Common.Result;

public class Result
{
    protected Result(IReadOnlyList<IError> error)
    {
        Errors = error ?? Array.Empty<IError>();
    }

    public IReadOnlyList<IError> Errors;

    public static Result<List<IError>> Fail(string errorMessage)
    {
        return new Result<List<IError>>(new List<IError>(), true, new List<IError>
        { ErrorFactory.Failure(errorMessage) });
    }

    public static Result<List<IError>> Fail(IError error)
    {
        return new Result<List<IError>>(new List<IError>(), true, new List<IError>
        { error });
    }

    public static Result<List<IError>> Fail(List<IError> errors)
    {
        return new Result<List<IError>>(new List<IError>(), true, errors);
    }

    public static Result<List<IError>> Fail(IReadOnlyList<IError> errors)
    {
        return new Result<List<IError>>(new List<IError>(), true, errors);
    }

    public static Result<List<IError>> Fail(IReadOnlyList<ValidationError> errors)
    {
        return new Result<List<IError>>(new List<IError>(), true, errors);
    }

    public static Result<T> Ok<T>(T value) => new Result<T>(value, false, Array.Empty<IError>());
}

public partial class Result<T> : Result
{
    public T? Value { get; }
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure { get; }
    public IError? Error
    {
        get
        {
            if (!IsFailure)
            {
                return ErrorFactory.Failure("Não há nenhum Error.");
            }

            return Errors[0];
        }
    }

    protected internal Result(T? value, bool isFail, IReadOnlyList<IError> errors)
        : base(errors)
    {
        Value = value;
        IsFailure = isFail;
    }

    public static implicit operator Result<T>(T value) => new Result<T>(value, false, Array.Empty<IError>());

    public static implicit operator Result<T>(Error error) =>
     new Result<T>(default, true, new List<IError> { error });


    /// <summary>
    /// Converte implicitamente um resultado contendo uma lista de erros em um resultado de um tipo especificado.
    /// </summary>
    /// <typeparam name="T">O tipo de valor retornado no resultado.</typeparam>
    /// <param name="errorResult">O resultado contendo uma lista de erros.</param>

    public static implicit operator Result<T>(Result<List<IError>> errorResult)
    {
        return new Result<T>(default, true, errorResult.Errors.ToList());
    }

}