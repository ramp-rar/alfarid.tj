using Alfarid.ControlPlane.Server;
using Alfarid.Shared.Network;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

var app = builder.Build();
app.MapGrpcService<TeacherHubService>();
app.MapGet("/", () => "Alfarid ControlPlane is running.");

using var discoveryCts = new CancellationTokenSource();
_ = Discovery.BroadcastTeacherAsync(5055, discoveryCts.Token);

await app.RunAsync("http://0.0.0.0:5055");
