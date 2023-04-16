﻿using SimVillage.Model;
using SimVillage.Model.Building;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimVillage.ViewModel
{
    public class SimVillageViewModel : ViewModelBase
    {
        private City model;

        public ObservableCollection<Field> Fields { get; private set; }

        public List<Option> Options { get; private set; }

        public int Width { get { return model.Width(); } }

        public int Height { get { return model.Height(); } }

        public string Name { get { return model.Name; } }

        public string Date { get; private set; }

        public int CitizenCount { get; private set; }

        public bool IsMoneyNegative { get; private set; }

        public int Money { get; private set; }

        private Option building = null!;

        public event EventHandler? NewGame;

        public event EventHandler? LoadGame;

        public event EventHandler? SaveGame;

        public event EventHandler? PauseGame;

        public event EventHandler? OneSpeed;

        public event EventHandler? FiveSpeed;

        public event EventHandler? TenSpeed;

        public DelegateCommand PauseGameCommand { get; private set; }

        public DelegateCommand OneSpeedCommand { get; private set; }

        public DelegateCommand FiveSpeedCommand { get; private set; }

        public DelegateCommand TenSpeedCommand { get; private set; }

        public SimVillageViewModel(City model)
        {
            this.model = model;
            model.gameAdvanced += new EventHandler(Model_GameAdvanced);
            model.gameChanged += new EventHandler(Model_GameChanged);
            Date = model.Date.ToString("d");
            Money = model.GetBudget();

            Fields = new ObservableCollection<Field>();

            GenerateMap();

            Options = new List<Option>
            {
                new Option { Text = "Residental", Number = 0, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param)))},
                new Option { Text = "Industrial", Number = 1, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param)))},
                new Option { Text = "Store" , Number = 2, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param)))},
                new Option { Text = "Road" , Number = 3, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param)))},
                new Option { Text = "Forest", Number = 4, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param)))},
                new Option { Text = "Police", Number = 5, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param)))},
                new Option { Text = "Fire Department", Number = 6, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param)))},
                new Option { Text = "Power Line", Number = 7, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param)))},
                new Option { Text = "Power Plant", Number = 8, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param)))},
                new Option { Text = "School", Number = 9, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param)))},
                new Option { Text = "University", Number = 10, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param)))},
                new Option { Text = "Stadium", Number = 11, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param)))}
            };

            PauseGameCommand = new DelegateCommand(param => OnPauseGame());
            OneSpeedCommand = new DelegateCommand(param => OnOneSpeedCommand());
            FiveSpeedCommand = new DelegateCommand(param => OnFiveSpeedCommand());
            TenSpeedCommand = new DelegateCommand(param => OnTenSpeedCommand());
        }

        private void OnOptionsClicked(int number)
        {
            if (building == null)
            {
                building = Options[number];
                Options[number].IsClicked = true;
            } else if (building == Options[number])
            {
                building = null!;
                Options[number].IsClicked = false;
            } else
            {
                building.IsClicked = false;
                building = Options[number];
                Options[number].IsClicked = true;
            }
        }

        private void GenerateMap()
        {
            Fields.Clear();
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Fields.Add(new Field
                    {
                        X = i,
                        Y = j,
                        Text = string.Empty,
                        Number = i * Height + j,
                        Clicked = new DelegateCommand(param => OnFieldClicked(Convert.ToInt32(param)))
                    });
                }
            }
            OnPropertyChanged(nameof(Fields));
        }

        private void OnFieldClicked(int number)
        {
            if (building != null)
            {
                Field field = Fields[number];

                switch (building.Text)
                {
                    case "Residental":
                        if (model.newZone(field.X, field.Y, ZoneType.Residental))
                            field.Text = "Residental";
                        break;
                    case "Industrial":
                        if (model.newZone(field.X, field.Y, ZoneType.Industrial))
                            field.Text = "Industrial";
                        break;
                    case "Store":
                        if (model.newZone(field.X, field.Y, ZoneType.Store))
                            field.Text = "Store";
                        break;
                    case "Forest":
                        if (model.BuildBuilding(new Forest(new List<Tile> { new Tile(field.X, field.Y) })))
                            field.Text = "Forest";
                        break;
                    case "Road":
                        if (model.BuildBuilding(new Road(new List<Tile> { new Tile(field.X, field.Y) })))
                            field.Text = "Road";
                        break;
                    case "Police":
                        if (model.BuildBuilding(new PoliceDepartment(new List<Tile> { new Tile(field.X, field.Y) })))
                            field.Text = "Police";
                        break;
                    case "Fire Department":
                        if (model.BuildBuilding(new FireDepartment(new List<Tile> { new Tile(field.X, field.Y) })))
                            field.Text = "Fire Department";
                        break;
                    case "Power Line":
                        if (model.BuildBuilding(new PowerLine(new List<Tile> { new Tile(field.X, field.Y) })))
                            field.Text = "Power Line";
                        break;
                    case "Power Plant":
                        if (model.BuildBuilding(new PowerPlant(new List<Tile> { new Tile(field.X, field.Y), new Tile(field.X, field.Y + 1), new Tile(field.X + 1, field.Y), new Tile(field.X + 1, field.Y + 1) })))
                            field.Text = "Power Plant";
                        break;
                    case "School":
                        if (model.BuildBuilding(new School(new List<Tile> { new Tile(field.X, field.Y), new Tile(field.X, field.Y + 1) }, SchoolTypes.Elementary)))
                            field.Text = "School";
                        break;
                    case "University":
                        if (model.BuildBuilding(new School(new List<Tile> { new Tile(field.X, field.Y), new Tile(field.X + 1, field.Y), new Tile(field.X, field.Y + 1), new Tile(field.X + 1, field.Y + 1)}, SchoolTypes.University)))
                            field.Text = "University";
                        break;
                    case "Stadium":
                        if (model.BuildBuilding(new Stadium(new List<Tile> { new Tile(field.X, field.Y), new Tile(field.X, field.Y + 1), new Tile(field.X + 1, field.Y), new Tile(field.X + 1, field.Y + 1)})))
                            field.Text = "Stadium";
                        break;
                    case "Demolish":
                        field.Text = "";
                        model.demolishZone(field.X,field.Y);
                        break;
                }
            }
        }

        private void OnTenSpeedCommand()
        {
            TenSpeed?.Invoke(this, EventArgs.Empty);
        }

        private void OnFiveSpeedCommand()
        {
            FiveSpeed?.Invoke(this, EventArgs.Empty);
        }

        private void OnOneSpeedCommand()
        {
            OneSpeed?.Invoke(this, EventArgs.Empty);
        }

        private void OnPauseGame()
        {
            PauseGame?.Invoke(this, EventArgs.Empty);
        }

        private void Model_GameChanged(object? sender, EventArgs e)
        {
            CitizenCount = model.Citizens.Count;
            Money = model.GetBudget();
            if(Money < 0)
            {
                IsMoneyNegative = true;
            }
            else
            {
                IsMoneyNegative = false;
            }
            OnPropertyChanged(nameof(CitizenCount));
            OnPropertyChanged(nameof(IsMoneyNegative));
            OnPropertyChanged(nameof(Money));
        }

        private void Model_GameAdvanced(object? sender, EventArgs e)
        {
            Date = model.Date.ToString("d");
            OnPropertyChanged(nameof(Date));
        }
    }
}
