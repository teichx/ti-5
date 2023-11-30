namespace TI5yncronizer.Core.ValueObject;

public readonly struct Device
{
    readonly MacAddress macAddress = new();
    public readonly string Value => $"{macAddress}/{Environment.UserName}";

    public Device() { }

    public override string ToString()
        => Value;

    public static Device DefaultDevice => new();
}
