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

        public string ZoneName { get; private set; }

        public string ZoneCitizenCount { get; private set; }

        public string ZoneCitizenHappiness { get; private set; }

        public int CitizenCount { get; private set; }

        public bool IsMoneyNegative { get; private set; }

        public int Money { get; private set; }

        public string BuildInfo { get; private set; }

        private Option building = null!;

        public event EventHandler? NewGame;

        public event EventHandler? LoadGame;

        public event EventHandler? SaveGame;

        public event EventHandler? PauseGame;

        public event EventHandler? OneSpeed;

        public event EventHandler? FiveSpeed;

        public event EventHandler? TenSpeed;

        public event EventHandler? Rename;

        public DelegateCommand RenameCommand { get; private set; }

        public DelegateCommand PauseGameCommand { get; private set; }

        public DelegateCommand OneSpeedCommand { get; private set; }

        public DelegateCommand FiveSpeedCommand { get; private set; }

        public DelegateCommand TenSpeedCommand { get; private set; }

        public SimVillageViewModel(City model)
        {
            this.model = model;
            model.gameAdvanced += new EventHandler(Model_GameAdvanced);
            model.gameChanged += new EventHandler(Model_GameChanged);
            model.gameCreated += new EventHandler(Model_GameCreated);
            Date = model.Date.ToString("yyyy") + " " + model.Date.ToString("M");
            Money = model.GetBudget();

            Fields = new ObservableCollection<Field>();

            GenerateMap();
            BuildInfo = "Select an option!";
            Options = new List<Option>
            {
                new Option { Text = "Residental", Number = 0, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param))), Info = "1x1 - This is where people live."},
                new Option { Text = "Industrial", Number = 1, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param))), Info = "1x1 - A place where people can work."},
                new Option { Text = "Store" , Number = 2, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param))), Info = "1x1 - A place where people can work."},
                new Option { Text = "Road" , Number = 3, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param))), Info = "1x1 - Connect buildings to let people move inbetween them."},
                new Option { Text = "Forest", Number = 4, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param))), Info = "1x1 - Provides happiness to people nearby."},
                new Option { Text = "Police", Number = 5, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param))), Info = "1x1 - Protect the people to make them feel safe and happy."},
                new Option { Text = "Fire Department", Number = 6, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param))), Info = "1x1 - Protection against fires."},
                new Option { Text = "Power Line", Number = 7, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param))), Info = "1x1 - Lets you take electricity to previously unreachable zones."},
                new Option { Text = "Power Plant", Number = 8, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param))), Info = "2x2 - Supplies neighbouring zones with electricity."},
                new Option { Text = "School", Number = 9, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param))), Info = "1x2 - Provides elementary education."},
                new Option { Text = "University", Number = 10, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param))), Info = "2x2 - Provides higher education."},
                new Option { Text = "Stadium", Number = 11, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param))), Info = "2x2 - Makes people happy."},
                new Option { Text = "Demolish", Number = 12, Clicked = new DelegateCommand(param => OnOptionsClicked(Convert.ToInt32(param))), Info = "Demolish what you don't need."}
            };
            RenameCommand = new DelegateCommand(param => OnRename());
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
                BuildInfo = building.Info;
            } else if (building == Options[number])
            {
                building = null!;
                Options[number].IsClicked = false;
                BuildInfo = "Select an option!";
            } else
            {
                building.IsClicked = false;
                building = Options[number];
                Options[number].IsClicked = true;
                BuildInfo = building.Info;
            }
            OnPropertyChanged(nameof(BuildInfo));
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
            Field field = Fields[number];
            if (building != null)
            {
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
                        model.BuildBuilding(new Forest(field.X, field.Y));
                        break;
                    case "Road":
                        model.BuildBuilding(new Road(field.X, field.Y));
                        break;
                    case "Police":
                        model.BuildBuilding(new PoliceDepartment(field.X, field.Y));
                        break;
                    case "Fire Department":
                        model.BuildBuilding(new FireDepartment(field.X, field.Y));
                        break;
                    case "Power Line":
                        model.BuildBuilding(new PowerLine(field.X, field.Y));
                        break;
                    case "Power Plant":
                        model.BuildBuilding(new PowerPlant(field.X, field.Y));
                        break;
                    case "School":
                        model.BuildBuilding(new School(field.X, field.Y, SchoolTypes.Elementary));
                        break;
                    case "University":
                        model.BuildBuilding(new School(field.X, field.Y, SchoolTypes.University));
                        break;
                    case "Stadium":
                        model.BuildBuilding(new Stadium(field.X, field.Y));
                        break;
                    case "Demolish":
                        model.demolishZone(field.X,field.Y);
                        break;
                }
            } else
            {
                ZoneName = model.Map[field.X, field.Y].ToString();
                ZoneCitizenCount = "Citizens: " + model.Map[field.X, field.Y].getPeople();
                ZoneCitizenHappiness = "Happiness: " + model.Map[field.X, field.Y].getHappiness();
                OnPropertyChanged(nameof(ZoneName));
                OnPropertyChanged(nameof(ZoneCitizenCount));
                OnPropertyChanged(nameof(ZoneCitizenHappiness));
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

        private void OnRename()
        {
            Rename?.Invoke(this, EventArgs.Empty);
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
                    Fields[zone.X * Width + zone.Y].Text = zone.ZoneType switch
                    {
                        ZoneType.Residental => "Residental",
                        ZoneType.Industrial => "Industrial",
                        ZoneType.Store => "Store",
                        _ => string.Empty
                    };
                }
            }
            OnPropertyChanged(nameof(CitizenCount));
            OnPropertyChanged(nameof(IsMoneyNegative));
            OnPropertyChanged(nameof(Money));
            OnPropertyChanged(nameof(Fields));
        }

        private void Model_GameCreated(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Name));
            Model_GameChanged(sender, e);
        }

        private void Model_GameAdvanced(object? sender, EventArgs e)
        {
            Date = model.Date.ToString("yyyy") + " " + model.Date.ToString("M");
            OnPropertyChanged(nameof(Date));
        }
    }
}
