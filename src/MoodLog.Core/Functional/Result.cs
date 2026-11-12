namespace MoodLog.Core.Functional;

public abstract record Result<T, TError>
{
    private Result() { }

    public sealed record Success(T Data) : Result<T, TError>;
    public sealed record Failure(TError ErrorDetail) : Result<T, TError>;

    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;

    public T Value => this switch
    {
        Success s => s.Data,
        _ => throw new InvalidOperationException("Cannot access value of a failure result.")
    };

    public TError Error => this switch
    {
        Failure f => f.ErrorDetail,
        _ => throw new InvalidOperationException("Cannot access error of a success result.")
    };

    public static Result<T, TError> Ok(T value) => new Success(value);
    public static Result<T, TError> Fail(TError error) => new Failure(error);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<TError, TResult> onFailure) =>
        this switch
        {
            Success s => onSuccess(s.Data),
            Failure f => onFailure(f.ErrorDetail),
            _ => throw new InvalidOperationException()
        };

    public Result<TNext, TError> Map<TNext>(Func<T, TNext> map) =>
        this switch
        {
            Success s => Result<TNext, TError>.Ok(map(s.Data)),
            Failure f => Result<TNext, TError>.Fail(f.ErrorDetail),
            _ => throw new InvalidOperationException()
        };

    public Result<TNext, TError> Bind<TNext>(Func<T, Result<TNext, TError>> bind) =>
        this switch
        {
            Success s => bind(s.Data),
            Failure f => Result<TNext, TError>.Fail(f.ErrorDetail),
            _ => throw new InvalidOperationException()
        };

    public Result<T, TError> Tap(Action<T> action)
    {
        if (this is Success s)
        {
            action(s.Data);
        }
        return this;
    }

    public Result<T, TError> Ensure(Func<T, bool> predicate, TError error) =>
        this switch
        {
            Success s => predicate(s.Data) ? this : Result<T, TError>.Fail(error),
            Failure => this,
            _ => throw new InvalidOperationException()
        };

    public static implicit operator Result<T, TError>(T value) => Ok(value);
}

public static class ResultExtensions
{
    public static Result<TNext, TError> Select<T, TError, TNext>(
        this Result<T, TError> source,
        Func<T, TNext> selector) =>
        source.Map(selector);

    public static Result<TResult, TError> SelectMany<T, TError, TSub, TResult>(
        this Result<T, TError> source,
        Func<T, Result<TSub, TError>> subSelector,
        Func<T, TSub, TResult> resultSelector) =>
        source.Bind(v => subSelector(v).Map(s => resultSelector(v, s)));
}
