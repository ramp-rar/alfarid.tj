using Alfarid.Control;
using Grpc.Net.Client;

namespace Alfarid.ControlPlane.Client;

public sealed class TeacherHubClientWrapper : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly TeacherHub.TeacherHubClient _client;

    public TeacherHubClientWrapper(string teacherBaseUrl)
    {
        _channel = GrpcChannel.ForAddress(teacherBaseUrl);
        _client = new TeacherHub.TeacherHubClient(_channel);
    }

    public async Task<string> RegisterAsync(string studentId, string name, string machine, string ip, CancellationToken ct)
    {
        var resp = await _client.RegisterStudentAsync(new RegisterStudentRequest
        {
            StudentId = studentId,
            DisplayName = name,
            MachineName = machine,
            Ip = ip
        }, cancellationToken: ct);

        if (!resp.Ok)
        {
            throw new InvalidOperationException("Register failed.");
        }

        return resp.SessionId;
    }

    public Task<HeartbeatResponse> HeartbeatAsync(string studentId, string sessionId, CancellationToken ct)
        => _client.HeartbeatAsync(new HeartbeatRequest
        {
            StudentId = studentId,
            SessionId = sessionId
        }, cancellationToken: ct).ResponseAsync;

    public void Dispose() => _channel.Dispose();
}
