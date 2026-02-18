using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Alfarid.Shared.Network;

public static class Discovery
{
    public const int Port = 49555;
    private const string Magic = "ALFARID_TEACHER_DISCOVERY_V1";

    public static async Task BroadcastTeacherAsync(int teacherGrpcPort, CancellationToken ct)
    {
        using var udp = new UdpClient();
        udp.EnableBroadcast = true;

        var payload = Encoding.UTF8.GetBytes($"{Magic}|{teacherGrpcPort}");
        var ep = new IPEndPoint(IPAddress.Broadcast, Port);

        while (!ct.IsCancellationRequested)
        {
            await udp.SendAsync(payload, payload.Length, ep);
            await Task.Delay(1000, ct);
        }
    }

    public static async Task<(IPAddress address, int port)?> ListenTeacherAsync(TimeSpan timeout, CancellationToken ct)
    {
        using var udp = new UdpClient(Port);
        var stopAt = DateTimeOffset.UtcNow + timeout;

        while (DateTimeOffset.UtcNow < stopAt && !ct.IsCancellationRequested)
        {
            if (udp.Available == 0)
            {
                await Task.Delay(50, ct);
                continue;
            }

            var result = await udp.ReceiveAsync(ct);
            var text = Encoding.UTF8.GetString(result.Buffer);
            var parts = text.Split('|');

            if (parts.Length == 2 && parts[0] == Magic && int.TryParse(parts[1], out var port))
            {
                return (result.RemoteEndPoint.Address, port);
            }
        }

        return null;
    }
}
