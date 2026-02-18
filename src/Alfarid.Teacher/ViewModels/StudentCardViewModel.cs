namespace Alfarid.Teacher.ViewModels;

public sealed class StudentCardViewModel
{
    public required string Name { get; init; }
    public required string Machine { get; init; }
    public required int PingMs { get; init; }
    public required string Status { get; init; }
}
