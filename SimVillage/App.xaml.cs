using SimVillage.Model;
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

        private City city = null!;

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        public void App_Startup(object? sender, StartupEventArgs e)
        {
            city = new City(new Model.Persistence(), "SimVillage");

            view =new MainWindow();
            view.Show();

            timer = new Timer();
            timer.Interval = 5000;
            timer.Elapsed += new ElapsedEventHandler(Timer_Tick);
            timer.Start();
        }

        private void Timer_Tick(object? sender, ElapsedEventArgs e)
        {
            city.AdvanceTime();
        }
    }
}
