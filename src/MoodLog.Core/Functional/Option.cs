namespace MoodLog.Core.Functional;

public abstract record Option<T>
{
    private Option() { }

    public sealed record Some(T Data) : Option<T>;
    public sealed record None : Option<T>;

    public bool IsSome => this is Some;
    public bool IsNone => this is None;

    public static Option<T> From(T? value) =>
        value is null ? new None() : new Some(value);

    public TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone) =>
        this switch
        {
            Some s => onSome(s.Data),
            None => onNone(),
            _ => throw new InvalidOperationException()
        };

    public Option<TNext> Map<TNext>(Func<T, TNext> map) =>
        this switch
        {
            Some s => Option<TNext>.From(map(s.Data)),
            None => new Option<TNext>.None(),
            _ => throw new InvalidOperationException()
        };

    public Option<TNext> Bind<TNext>(Func<T, Option<TNext>> bind) =>
        this switch
        {
            Some s => bind(s.Data),
            None => new Option<TNext>.None(),
            _ => throw new InvalidOperationException()
        };

    public T ValueOr(T fallback) =>
        this switch
        {
            Some s => s.Data,
            None => fallback,
            _ => throw new InvalidOperationException()
        };
}
