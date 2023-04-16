using SimVillage.Model;
using SimVillage.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace SimVillage
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Timer timer = null!;

        private MainWindow view = null!;

        private SimVillageViewModel viewModel = null!;

        private City city = null!;

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        public void App_Startup(object? sender, StartupEventArgs e)
        {
            city = new City(new Model.Persistence(), "SimVillage");

            viewModel = new SimVillageViewModel(city);
            viewModel.PauseGame += new EventHandler(ViewModel_PauseGame);
            viewModel.OneSpeed += new EventHandler(ViewModel_OneSpeed);
            viewModel.FiveSpeed += new EventHandler(ViewModel_FiveSpeed);
            viewModel.TenSpeed += new EventHandler(ViewModel_TenSpeed);

            view =new MainWindow();
            view.DataContext = viewModel;
            view.Show();

            timer = new Timer();
            timer.Interval = 5000;
            timer.Elapsed += new ElapsedEventHandler(Timer_Tick);
            timer.Start();
        }

        private void ViewModel_TenSpeed(object? sender, EventArgs e)
        {
            timer.Stop();
            timer.Interval = 1000;
            timer.Start();
        }

        private void ViewModel_FiveSpeed(object? sender, EventArgs e)
        {
            timer.Stop();
            timer.Interval = 5000;
            timer.Start();
        }

        private void ViewModel_OneSpeed(object? sender, EventArgs e)
        {
            timer.Stop();
            timer.Interval = 10000;
            timer.Start();
        }

        private void ViewModel_PauseGame(object? sender, EventArgs e)
        {
            timer.Stop();
        }

        private void Timer_Tick(object? sender, ElapsedEventArgs e)
        {
            city.AdvanceTime();
        }
    }
}
