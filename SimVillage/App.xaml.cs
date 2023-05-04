using SimVillage.Model;
using SimVillage.View;
using SimVillage.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SimVillage
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Timer timer = null!;

        private Page gamePage = null!;

        private NavigationWindow mainWindow = null!;

        private SimVillageViewModel viewModel = null!;

        private City city = null!;

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        public void App_Startup(object? sender, StartupEventArgs e)
        {
            city = new City(new Model.Persistence());
            city.ConflictDemolish += new EventHandler(Model_ConflictDemolish);

            viewModel = new SimVillageViewModel(city);
            viewModel.PauseGame += new EventHandler(ViewModel_PauseGame);
            viewModel.OneSpeed += new EventHandler(ViewModel_OneSpeed);
            viewModel.FiveSpeed += new EventHandler(ViewModel_FiveSpeed);
            viewModel.TenSpeed += new EventHandler(ViewModel_TenSpeed);
            viewModel.Info += new EventHandler(ViewModel_Info);
            viewModel.NewGame += new EventHandler(ViewModel_NewGame);

            mainWindow = new MainWindow();
            mainWindow.DataContext = viewModel;
            mainWindow.Navigate(new Welcome());
            mainWindow.Show();

            gamePage = new GamePage();

            city.newGame("SimVillage");

            timer = new Timer();
            timer.Interval = 5000;
            timer.Elapsed += new ElapsedEventHandler(Timer_Tick);
            timer.Start();
        }

        private void ViewModel_NewGame(object? sender, EventArgs e)
        {
            
        }

        private void ViewModel_Info(object? sender, EventArgs e)
        {
            timer.Stop();
            InfoWindow info = new InfoWindow();
            info.DataContext = viewModel;
            if ((bool)info.ShowDialog()!)
            {
                city.Finances.setTax(ZoneType.Residental, (int)info.Rslider.Value);
                city.Finances.setTax(ZoneType.Industrial, (int)info.Islider.Value);
                city.Finances.setTax(ZoneType.Store, (int)info.Sslider.Value);
            }
            timer.Start();
        }

        private void Model_ConflictDemolish(object? sender, EventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Some people may have to move out. Do you still want to proceed?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);
            city.CanDemolish(dialogResult == MessageBoxResult.Yes);
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
