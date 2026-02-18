using System.Windows;
using Alfarid.Teacher.ViewModels;

namespace Alfarid.Teacher;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
