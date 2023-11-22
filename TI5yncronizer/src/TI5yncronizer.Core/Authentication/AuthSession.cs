using System.Net;
using System.Reflection;
using SMBLibrary;
using SMBLibrary.Client;
using TI5yncronizer.Core.ValueObject;

namespace TI5yncronizer.Core.Authentication;

public class AuthSession : IAuthSession, IDisposable
{
    private SMB2Client smb2Client => new();
    public bool IsConnected => smb2Client.IsConnected;
    public bool IsAuthenticated { get; private set; }
    private SessionId sessionId = SessionId.Empty;

    public SMB2Client? GetClient(SessionId _sessionId)
        => sessionId.Equals(sessionId)
            ? smb2Client
            : null;

    public SessionId SignIn(string user, string password, string host)
    {
        var connectMethod = smb2Client.GetType()
            .GetMethod("Connect", BindingFlags.NonPublic | BindingFlags.Instance);

        var data = host.Split(':');
        var hostname = data[0];
        var port = int.Parse(data.ElementAtOrDefault(1) ?? SMB2Client.NetBiosOverTCPPort.ToString());

        connectMethod!.Invoke(smb2Client, [
            IPAddress.Parse(hostname),
            SMBTransportType.NetBiosOverTCP,
            port,
            SMB2Client.DefaultResponseTimeoutInMilliseconds
        ]);

        // smb2Client.Connect(host, SMBTransportType.DirectTCPTransport);
        if (!IsConnected) return SessionId.Empty;

        var status = smb2Client.Login(string.Empty, user, password);
        IsAuthenticated = status == NTStatus.STATUS_SUCCESS;
        if (!IsAuthenticated) return SessionId.Empty;

        sessionId = SessionId.NewSessionId();
        return sessionId;
    }

    public void SignOut()
    {
        sessionId = Guid.Empty;

        if (!IsAuthenticated) return;
        smb2Client.Logoff();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        smb2Client.Logoff();
        smb2Client.Disconnect();
    }
}
