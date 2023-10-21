using System.Timers;
using BatSignal.Contract;
using Microsoft.AspNetCore.SignalR.Client;
using TypedSignalR.Client;
using Timer = System.Timers.Timer;

namespace BatSignal.WebClient.WebAssembly.Pages;

public partial class Index : IChatHubClient, IDisposable
{
    private IChatHub? _hubProxy;
    private IDisposable? _subscription;
    private string _user = string.Empty;
    private readonly FixedSizeQueue<string> _messages = new(50);
    private bool _connected;
    private Timer? _refreshTimer;
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var connection = new HubConnectionBuilder()
                         .WithUrl($"http://localhost:5174/{ChatHubConfig.Route}")
                         .Build();
        
        if (_refreshTimer is null)
        {
            _refreshTimer = new Timer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(2).TotalMilliseconds;
            _refreshTimer.Enabled = true;
            _refreshTimer.Elapsed += Tick;
            _refreshTimer.Start();
        }

        try
        {
            await connection.StartAsync();
            _connected = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect: {ex.Message}");
        }

        _hubProxy = connection.CreateHubProxy<IChatHub>();
        _subscription = connection.Register<IChatHubClient>(this);
    }

    private async Task HandleConnectBtnClick()
    {
        if (string.IsNullOrWhiteSpace(_user) || _hubProxy is null || !_connected)
        {
            return;
        }
        
        await _hubProxy.SendMessage(_user, "Hi, I just joined!");
        _userNameProvided = true;
    }

    public async Task ReceiveMessage(string user, string message)
    {
        var msg = $"[{DateTime.Now}] {user}: {message}";
        _messages.Enqueue(msg);
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        _subscription?.Dispose();
        
        if (_refreshTimer is null)
        {
            return;
        }

        _refreshTimer.Stop();
        _refreshTimer.Elapsed -= Tick;
        _refreshTimer.Dispose();

        _refreshTimer = null;
    }
    
    private async void Tick(object? _, ElapsedEventArgs? __)
    {
        if (!_connected || _hubProxy is null || !_userNameProvided)
        {
            return;
        }
        
        await _hubProxy.SendMessage(_user, "I'm still here!");
    }
    
    private sealed class FixedSizeQueue<T> : Queue<T>
    {
        public int Limit { get; set; }

        public FixedSizeQueue(int limit)
            : base(limit)
        {
            this.Limit = limit;
        }

        public new void Enqueue(T item)
        {
            while (this.Count >= this.Limit)
            {
                this.Dequeue();
            }
            base.Enqueue(item);
        }
    }
}

