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
        private Timer timer;

        private City city;

        private SimVillage.Model.Persistence persistence;

        public App()
        {
            InitializeComponent();
            persistence = new SimVillage.Model.Persistence();
            city = new City(persistence, "asd");
        }
    }
}
