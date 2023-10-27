using SMBLibrary.Client;
using TI5yncronizer.Client.ValueObject;

namespace TI5yncronizer.Client.Authentication;

public interface IAuthSession
{
    SMB2Client? GetClient(SessionId sessionId);
    bool IsConnected { get; }
    bool IsAuthenticated { get; }
    SessionId SignIn(string user, string password, string host);
    void SignOut();
}
