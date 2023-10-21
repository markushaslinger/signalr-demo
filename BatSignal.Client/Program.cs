using BatSignal.Contract;
using Microsoft.AspNetCore.SignalR.Client;
using TypedSignalR.Client;

// start at least two client instances (we are sending messages to _others_ only) to see something happen!

var connection = new HubConnectionBuilder()
                 .WithUrl($"http://localhost:5174/{ChatHubConfig.Route}")
                 .Build();
await connection.StartAsync();

Console.Write("Enter your username: ");
var user = Console.ReadLine() ?? "somebody";


var hubProxy = connection.CreateHubProxy<IChatHub>();

using var subscription = connection.Register<IChatHubClient>(new ChatHubClient());

// we can call the strongly typed method
await hubProxy.SendMessage(user, "Hi, I just joined!");

while (true)
{
    await Task.Delay(TimeSpan.FromSeconds(2));
    await hubProxy.SendMessage(user, "I'm still here!");
}


sealed class ChatHubClient : IChatHubClient
{
    public Task ReceiveMessage(string user, string message)
    {
        Console.WriteLine($"[{DateTime.Now}] {user}: {message}");
        return Task.CompletedTask;
    }
}