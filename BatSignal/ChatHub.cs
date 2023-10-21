using BatSignal.Contract;
using Microsoft.AspNetCore.SignalR;

namespace BatSignal;

internal sealed class ChatHub : Hub<IChatHubClient>, IChatHub
{
    public async Task SendMessage(string user, string message)
    {
        // sent to everyone except sender
        await Clients.Others.ReceiveMessage(user, message);
    }
}
