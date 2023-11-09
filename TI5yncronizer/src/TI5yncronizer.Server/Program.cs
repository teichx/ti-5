using System.Net;
using System.Net.WebSockets;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseWebSockets();

app.Map("/", async (HttpContext context) =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return;
    }

    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
    while (true)
    {
        await webSocket.SendAsync(
            buffer: Encoding.ASCII.GetBytes($".NET Rocks -> {DateTime.Now}"),
            messageType: WebSocketMessageType.Text,
            endOfMessage: true,
            cancellationToken: CancellationToken.None
        );
        await Task.Delay(1000);
    }
});

await app.RunAsync();
