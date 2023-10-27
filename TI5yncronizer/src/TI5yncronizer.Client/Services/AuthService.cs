using Grpc.Core;
using TI5yncronizer.Client.Authentication;
using TI5yncronizer.Client.ValueObject;

namespace TI5yncronizer.Client.Services;

public class AuthService(IAuthSession authSession, ILogger<AuthService> logger) : Auth.AuthBase
{
    public override Task<SignInReply> SignIn(SignInRequest request, ServerCallContext context)
    {
        logger.LogInformation("Try login with \"{User}\"", request.User);
        try
        {
            var sessionId = authSession
                .SignIn(user: request.User, password: request.Password, host: request.Host);

            if (SessionId.Empty.Equals(sessionId))
                throw new RpcException(new Status(StatusCode.Internal, "Failed to sign in"));

            logger.LogInformation("Success login with \"{User}\"", request.User);
            return Task.FromResult(new SignInReply
            {
                SessionId = sessionId.ToString(),
            });
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Exception on SignIn");
            throw new RpcException(new Status(StatusCode.Internal, "Unknown error"));
        }
    }

    public override Task<SignOutReply> SignOut(SignOutRequest request, ServerCallContext context)
    {
        logger.LogInformation("Try sign out");
        try
        {
            authSession.SignOut();
            logger.LogInformation("Success sign out");
            return Task.FromResult(new SignOutReply());
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Exception on SignOut");
            throw new RpcException(new Status(StatusCode.Internal, "Unknown error"));
        }
    }
}
