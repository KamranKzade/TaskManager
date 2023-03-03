using System.Windows;
using TaskManager.ViewModels;

namespace TaskManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }
}
