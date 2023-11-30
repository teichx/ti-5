using System.Net.NetworkInformation;

namespace TI5yncronizer.Core.ValueObject;

public readonly struct MacAddress : IEquatable<string?>
{
    public readonly string Value;

    public MacAddress()
        => Value = NetworkInterface
        .GetAllNetworkInterfaces()
        .Where(nic => nic.OperationalStatus == OperationalStatus.Up
            && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
        .Select(nic => nic.GetPhysicalAddress().ToString())
        .FirstOrDefault() ?? string.Empty;

    MacAddress(string value)
        => Value = value;

    public override string ToString()
        => Value;
    public override int GetHashCode()
        => Value.GetHashCode();

    public bool Equals(string? other)
        => Value.Equals(other);

    public static implicit operator string(MacAddress macAddress) => macAddress.Value;
    public static implicit operator MacAddress(string macAddress) => new(macAddress);
    public static MacAddress Address = new();
}
