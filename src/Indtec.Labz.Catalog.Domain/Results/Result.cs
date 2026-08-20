namespace Indtec.Labz.Catalog.Domain.Results;

public class Result
{
    protected Result(IReadOnlyCollection<Error> errors) => Errors = errors;

    public bool IsSuccess => Errors.Count == 0;
    public bool IsFailure => !IsSuccess;
    public IReadOnlyCollection<Error> Errors { get; }

    public static Result Success() => new(Array.Empty<Error>());
    public static Result Failure(params Error[] errors) => Failure((IEnumerable<Error>)errors);
    public static Result Failure(IEnumerable<Error> errors) => new(errors.ToArray());
}

public sealed class Result<T> : Result
{
    private Result(T? value, IReadOnlyCollection<Error> errors) : base(errors) => Value = value;

    public T? Value { get; }

    public static Result<T> Success(T value) => new(value, Array.Empty<Error>());
    public new static Result<T> Failure(params Error[] errors) => Failure((IEnumerable<Error>)errors);
    public static Result<T> Failure(IEnumerable<Error> errors) => new(default, errors.ToArray());
}