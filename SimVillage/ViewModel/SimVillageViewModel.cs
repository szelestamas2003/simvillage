using SimVillage.Model;
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
                new Option { Text = "Stadium", Number = 11, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param)))},
                new Option { Text = "Demolish", Number = 12, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param)))}
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
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Fields.Add(new Field
                    {
                        X = i,
                        Y = j,
                        Text = string.Empty,
                        Number = i * Width + j,
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
                        model.BuildBuilding(new Forest(new List<Tile> { new Tile(field.X, field.Y) }));
                        break;
                    case "Road":
                        model.BuildBuilding(new Road(new List<Tile> { new Tile(field.X, field.Y) }));
                        break;
                    case "Police":
                        model.BuildBuilding(new PoliceDepartment(new List<Tile> { new Tile(field.X, field.Y) }));
                        break;
                    case "Fire Department":
                        model.BuildBuilding(new FireDepartment(new List<Tile> { new Tile(field.X, field.Y) }));
                        break;
                    case "Power Line":
                        model.BuildBuilding(new PowerLine(new List<Tile> { new Tile(field.X, field.Y) }));
                        break;
                    case "Power Plant":
                        model.BuildBuilding(new PowerPlant(new List<Tile> { new Tile(field.X, field.Y), new Tile(field.X, field.Y + 1), new Tile(field.X + 1, field.Y), new Tile(field.X + 1, field.Y + 1) }));
                        break;
                    case "School":
                        model.BuildBuilding(new School(new List<Tile> { new Tile(field.X, field.Y), new Tile(field.X, field.Y + 1) }, SchoolTypes.Elementary));
                        break;
                    case "University":
                        model.BuildBuilding(new School(new List<Tile> { new Tile(field.X, field.Y), new Tile(field.X + 1, field.Y), new Tile(field.X, field.Y + 1), new Tile(field.X + 1, field.Y + 1) }, SchoolTypes.University));
                        break;
                    case "Stadium":
                        model.BuildBuilding(new Stadium(new List<Tile> { new Tile(field.X, field.Y), new Tile(field.X, field.Y + 1), new Tile(field.X + 1, field.Y), new Tile(field.X + 1, field.Y + 1) }));
                        break;
                    case "Demolish":
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

            foreach (Zone zone in model.Map)
            {
                if (zone.getBuilding() != null)
                {
                    switch (zone.getBuilding())
                    {
                        case Road:
                            Fields[zone.X * Width + zone.Y].Text = "Road";
                            break;
                        case Forest:
                            Fields[zone.X * Width + zone.Y].Text = "Forest";
                            break;
                        case PoliceDepartment:
                            Fields[zone.X * Width + zone.Y].Text = "Police";
                            break;
                        case FireDepartment:
                            Fields[zone.X * Width + zone.Y].Text = "Fire Department";
                            break;
                        case PowerLine:
                            Fields[zone.X * Width + zone.Y].Text = "Power Line";
                            break;
                        case PowerPlant:
                            Fields[zone.X * Width + zone.Y].Text = "Power Plant";
                            break;
                        case School s:
                            if (s.GetSchoolType() == SchoolTypes.Elementary)
                                Fields[zone.X * Width + zone.Y].Text = "School";
                            else
                                Fields[zone.X * Width + zone.Y].Text = "University";
                            break;
                        case Stadium:
                            Fields[zone.X * Width + zone.Y].Text = "Stadium";
                            break;
                        case Residental:
                            Fields[zone.X * Width + zone.Y].Text = "Residental Building";
                            break;
                        case Industrial:
                            Fields[zone.X * Width + zone.Y].Text = "Industrial Building";
                            break;
                        case Store:
                            Fields[zone.X * Width + zone.Y].Text = "Store Building";
                            break;
                        default:
                            Fields[zone.X * Width + zone.Y].Text = string.Empty;
                            break;
                    }
                } else
                {
                    Fields[zone.X * Width + zone.Y].Text = string.Empty;
                }
            }
            OnPropertyChanged(nameof(CitizenCount));
            OnPropertyChanged(nameof(IsMoneyNegative));
            OnPropertyChanged(nameof(Money));
            OnPropertyChanged(nameof(Fields));
        }

        private void Model_GameAdvanced(object? sender, EventArgs e)
        {
            Date = model.Date.ToString("d");
            OnPropertyChanged(nameof(Date));
        }
    }
}
