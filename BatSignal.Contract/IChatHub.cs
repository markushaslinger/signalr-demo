namespace BatSignal.Contract;

public static class ChatHubConfig
{
    public const string Route = "hub/bat-signal";
}

public interface IChatHub
{
    // this method can be called by a client on the server
    Task SendMessage(string user, string message);
}

public interface IChatHubClient
{
    // we can strongly type the methods we are able to call on the client
    Task ReceiveMessage(string user, string message);
}
