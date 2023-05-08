﻿using SimVillage.Model;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace SimVillage
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Timer timer = null!;

        private NavigationWindow mainWindow = null!;

        private SimVillageViewModel viewModel = null!;

        private Uri gamePageUri = null!;

        private Uri pausePageUri = null!;

        private Uri persistenceViewUri = null!;

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
            viewModel.NewGame += new EventHandler<NewGameEventArgs>(ViewModel_NewGame);
            viewModel.LoadGame += new EventHandler(ViewModel_LoadGame);
            viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);
            viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);
            viewModel.ContinueGame += new EventHandler(ViewModel_ContinueGame);
            viewModel.PauseMenu += new EventHandler(ViewModel_PauseMenu);
            viewModel.LoadingSlot += new EventHandler<SlotEventArgs>(ViewModel_Loading);
            viewModel.SavingSlot += new EventHandler<SlotEventArgs>(ViewModel_Saving);
            viewModel.SlotDelete += new EventHandler<SlotEventArgs>(ViewModel_SlotDelete);

            mainWindow = new MainWindow();
            mainWindow.KeyDown += new KeyEventHandler(OnButtonKeyDown);
            mainWindow.DataContext = viewModel;
            mainWindow.Navigate(new MainMenu());
            mainWindow.Show();

            gamePageUri = new Uri("View/GamePage.xaml", UriKind.Relative);
            pausePageUri = new Uri("View/PausePage.xaml", UriKind.Relative);
            persistenceViewUri = new Uri("View/PersistenceView.xaml", UriKind.Relative);

            timer = new Timer();
            timer.Interval = 5000;
            timer.Elapsed += new ElapsedEventHandler(Timer_Tick);
        }

        private async void ViewModel_SlotDelete(object? sender, SlotEventArgs e)
        {
            await city.DeleteSave(e.Slot);
            MessageBox.Show("Deleted your save from slot " + e.Slot, "Note", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private async void ViewModel_Loading(object? sender, SlotEventArgs e)
        {
            //city.NewGame("Loading");
            //mainWindow.GoBack();
            //mainWindow.GoBack();
            await city.Load(e.Slot);
            timer.Start();
            mainWindow.Navigate(gamePageUri);
        }

        private async void ViewModel_Saving(object? sender, SlotEventArgs e)
        {
            await city.Save(e.Slot);
            MessageBox.Show("Saved your game to slot " + e.Slot, "Note", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void ViewModel_PauseMenu(object? sender, EventArgs e)
        {
            timer.Stop();
            mainWindow.Navigate(pausePageUri);
        }

        private void ViewModel_ContinueGame(object? sender, EventArgs e)
        {
            if (mainWindow.CanGoBack)
            {
                if (mainWindow.CurrentSource == pausePageUri)
                    timer.Start();
                mainWindow.GoBack();
            }
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (mainWindow.CurrentSource == gamePageUri)
                {
                    timer.Stop();
                    mainWindow.Navigate(pausePageUri);
                }
                else if (mainWindow.CurrentSource == pausePageUri || persistenceViewUri == mainWindow.CurrentSource)
                {
                    if (mainWindow.CurrentSource == pausePageUri)
                        timer.Start();
                    mainWindow.GoBack();
                }
            }
        }

        private void ViewModel_ExitGame(object? sender, EventArgs e)
        {
            timer.Stop();
            mainWindow.Close();
        }

        private async void ViewModel_SaveGame(object? sender, EventArgs e)
        {
            mainWindow.Navigate(persistenceViewUri);
        }

        private async void ViewModel_LoadGame(object? sender, EventArgs e)
        {
            mainWindow.Navigate(persistenceViewUri);
        }

        private void ViewModel_NewGame(object? sender, NewGameEventArgs e)
        {
            mainWindow.Navigate(gamePageUri);
            city.NewGame(e.Name);
            timer.Start();
        }

        private void ViewModel_Info(object? sender, EventArgs e)
        {
            bool paused = !timer.Enabled;
            timer.Stop();
            InfoWindow info = new InfoWindow();
            info.DataContext = viewModel;
            if ((bool)info.ShowDialog()!)
            {
                viewModel.SetTax((int)info.Rslider.Value, (int)info.Islider.Value, (int)info.Sslider.Value);
            }
            if (!paused)
                timer.Start();
            info = null!;
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
