using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TaskManager.Commands;

namespace TaskManager.ViewModels;

public class MainViewModel : BaseViewModel
{
    public ObservableCollection<string> BlackList { get; set; }
    public ObservableCollection<Process> Processes { get; set; }

    public RelayCommand EndTaskCommand { get; set; }
    public RelayCommand AddBlackListCommand { get; set; }
    public RelayCommand CreateProcessCommand { get; set; }



    public MainViewModel()
    {
        Processes = new(Process.GetProcesses());
        BlackList = new ObservableCollection<string>();

        DispatcherTimer timer = new DispatcherTimer();

        timer.Interval = TimeSpan.FromMilliseconds(3000);

        timer.Tick += Update_Processes;


        timer.Tick += DeleteProcessesInBlackList;

        timer.Start();


        AddBlackListCommand = new RelayCommand((o) =>
        {
            var textbox = ((o as Grid)!.Children[4] as Grid)!.Children[0] as TextBox;

            if (!string.IsNullOrWhiteSpace(textbox?.Text))
            {
                if (!BlackList.Contains(textbox.Text))
                {
                    BlackList.Add(textbox.Text);
                    MessageBox.Show("Successfully added", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                    MessageBox.Show("An element with this name already exists", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
                MessageBox.Show("Note the name of the item", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
        });

        CreateProcessCommand = new RelayCommand((o) =>
        {
            var textbox = ((o as Grid)!.Children[4] as Grid)!.Children[0] as TextBox;
            if (!string.IsNullOrWhiteSpace(textbox?.Text))
            {
                try
                {
                    Task.Delay(2000);
                    Process.Start(textbox.Text);
                    MessageBox.Show("Successfully Create Program", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show($"The program with {textbox.Text} is not available in the operating system", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
                MessageBox.Show("Enter the item", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);


        });


        EndTaskCommand = new RelayCommand((o) =>
        {
            var process = (o as ListView)!.SelectedValue as Process;


            if (process is null)
                MessageBox.Show("Select one of the items", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                Thread.Sleep(2000);
                process.Kill();
                MessageBox.Show("Successfully End Task", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        });
    }

    private void DeleteProcessesInBlackList(object? sender, EventArgs e)
    {
        if (BlackList.Count > 0)
        {
            foreach (var black in BlackList)
            {
                var processes = Processes.Where(x => x.ProcessName.ToLower() == black.ToLower());

                if (processes is null)
                    return;
                foreach (var process in processes)
                    process.Kill();

                MessageBox.Show("Items in blackList were successfully deleted", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                BlackList.Remove(black);
                return;
            }
        }

    }


    private void Update_Processes(object? sender, EventArgs e)
    {
        Processes.Clear();

        foreach (var process in Process.GetProcesses())
        {
            Processes.Add(process);
        }
    }
}
