using System.Collections.ObjectModel;

namespace Alfarid.Teacher.ViewModels;

public sealed class MainViewModel
{
    public ObservableCollection<StudentCardViewModel> Students { get; } =
    [
        new() { Name = "Student-01", Machine = "PC-01", PingMs = 12, Status = "Online" },
        new() { Name = "Student-02", Machine = "PC-02", PingMs = 20, Status = "✋ Hand raised" }
    ];
}
