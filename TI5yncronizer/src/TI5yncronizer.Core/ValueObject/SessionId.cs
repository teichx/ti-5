namespace TI5yncronizer.Core.ValueObject;

public readonly struct SessionId(Guid _guid)
{
    private readonly Guid guid = _guid;

    public override int GetHashCode()
        => guid.GetHashCode();

    public override string ToString()
        => guid.ToString();

    public bool Equals(SessionId sessionId)
        => guid == sessionId.guid;

    public static implicit operator Guid(SessionId sessionId) => sessionId.guid;
    public static implicit operator SessionId(Guid guid) => new(guid);
    public static SessionId Empty => new(Guid.Empty);
    public static SessionId NewSessionId() => Guid.NewGuid();
}
