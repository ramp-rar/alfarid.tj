using System.Collections.Concurrent;
using Alfarid.Control;
using Grpc.Core;

namespace Alfarid.ControlPlane.Server;

public sealed class TeacherHubService : TeacherHub.TeacherHubBase
{
    private readonly ConcurrentDictionary<string, StudentState> _students = new();

    public override Task<RegisterStudentResponse> RegisterStudent(RegisterStudentRequest request, ServerCallContext context)
    {
        var sessionId = Guid.NewGuid().ToString("N");
        _students[request.StudentId] = new StudentState(
            request.StudentId,
            request.DisplayName,
            request.MachineName,
            request.Ip,
            sessionId,
            DateTimeOffset.UtcNow,
            false);

        return Task.FromResult(new RegisterStudentResponse { Ok = true, SessionId = sessionId });
    }

    public override Task<HeartbeatResponse> Heartbeat(HeartbeatRequest request, ServerCallContext context)
    {
        if (_students.TryGetValue(request.StudentId, out var st) && st.SessionId == request.SessionId)
        {
            _students[request.StudentId] = st with { LastSeenUtc = DateTimeOffset.UtcNow };
            return Task.FromResult(new HeartbeatResponse
            {
                Ok = true,
                ServerTimeUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            });
        }

        return Task.FromResult(new HeartbeatResponse
        {
            Ok = false,
            ServerTimeUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
    }

    public override Task<RaiseHandResponse> RaiseHand(RaiseHandRequest request, ServerCallContext context)
    {
        if (_students.TryGetValue(request.StudentId, out var st) && st.SessionId == request.SessionId)
        {
            _students[request.StudentId] = st with { HandRaised = true, LastSeenUtc = DateTimeOffset.UtcNow };
            return Task.FromResult(new RaiseHandResponse { Ok = true });
        }

        return Task.FromResult(new RaiseHandResponse { Ok = false });
    }

    public override Task<CommandResponse> SendCommand(CommandRequest request, ServerCallContext context)
        => Task.FromResult(new CommandResponse { Ok = true });

    public override async Task SubscribeEvents(SubscribeRequest request, IServerStreamWriter<EventMessage> responseStream, ServerCallContext context)
    {
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1000, context.CancellationToken);
        }
    }

    public IReadOnlyCollection<StudentState> GetStudents() => _students.Values;
}

public sealed record StudentState(
    string StudentId,
    string DisplayName,
    string MachineName,
    string Ip,
    string SessionId,
    DateTimeOffset LastSeenUtc,
    bool HandRaised);
