using System.Net;
using System.Windows;
using Alfarid.ControlPlane.Client;
using Alfarid.Shared.Network;

namespace Alfarid.Student;

public partial class MainWindow : Window
{
    private readonly CancellationTokenSource _cts = new();

    public string Status { get; private set; } = "Discovering teacher...";

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        Loaded += async (_, _) => await ConnectAsync();
    }

    private async Task ConnectAsync()
    {
        try
        {
            var endpoint = await Discovery.ListenTeacherAsync(TimeSpan.FromSeconds(10), _cts.Token);
            if (endpoint is null)
            {
                Status = "Teacher not found in LAN discovery.";
                return;
            }

            var address = $"http://{endpoint.Value.address}:{endpoint.Value.port}";
            using var client = new TeacherHubClientWrapper(address);

            var studentId = Guid.NewGuid().ToString("N");
            var sessionId = await client.RegisterAsync(studentId, Environment.UserName, Environment.MachineName, GetLocalIp(), _cts.Token);

            Status = $"Connected to {address}. Session={sessionId[..8]}...";
            await Dispatcher.InvokeAsync(() => DataContext = null);
            await Dispatcher.InvokeAsync(() => DataContext = this);

            _ = Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    await client.HeartbeatAsync(studentId, sessionId, _cts.Token);
                    await Task.Delay(3000, _cts.Token);
                }
            }, _cts.Token);
        }
        catch (Exception ex)
        {
            Status = $"Connection failed: {ex.Message}";
        }
        finally
        {
            await Dispatcher.InvokeAsync(() => DataContext = null);
            await Dispatcher.InvokeAsync(() => DataContext = this);
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        _cts.Cancel();
        _cts.Dispose();
        base.OnClosed(e);
    }

    private static string GetLocalIp()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        var ip = host.AddressList.FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        return ip?.ToString() ?? "127.0.0.1";
    }
}
