namespace TI5yncronizer.Core.ValueObject;

public readonly struct Pagination((int pageNumber, int limitNumber) tuple)
{
    public const int MaxLimit = 100;

    public readonly int PageNumber = Math.Max(tuple.pageNumber, 1);
    public readonly int Limit = Math.Min(Math.Max(tuple.limitNumber, 0), MaxLimit);
    public readonly int Offset { get => Limit * (PageNumber - 1); }

    public static implicit operator Pagination((int pageNumber, int limitNumber) tuple)
        => new(tuple);
}
