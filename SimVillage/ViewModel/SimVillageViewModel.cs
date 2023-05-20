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

        public int CanvasW { get; private set; }

        public int CanvasH { get; private set; }

        public string Name { get { return model.Name; } }

        public string Date { get; private set; }

        public string CitizenCount { get; private set; }

        public int Speed { get; set; }

        public string Happiness { get; private set; }

        public List<Transaction> Expenses { get { var asd = model.Finances.Expenses; asd.Reverse(); return asd; } }

        public List<Transaction> Incomes { get { var asd = model.Finances.Incomes; asd.Reverse(); return asd; } }

        public int ResidentTax { get { return model.Finances.ResidentTax; } }

        public int IndustrialTax { get { return model.Finances.IndustrialTax; } }

        public int StoreTax { get { return model.Finances.StoreTax; } }

        public bool IsMoneyNegative { get; private set; }

        public ObservableCollection<StoredGameViewModel> StoredGames { get; private set; }

        public string Money { get; private set; }

        public string BuildInfo { get; private set; }

        private Option building = null!;

        private Field field = null!;

        private bool doSave = false;

        public event EventHandler<NewGameEventArgs>? NewGame;

        public event EventHandler? LoadGame;

        public event EventHandler? SaveGame;

        public event EventHandler? PauseGame;

        public event EventHandler? OneSpeed;

        public event EventHandler? FiveSpeed;

        public event EventHandler? TenSpeed;

        public event EventHandler? Info;

        public event EventHandler? ExitGame;

        public event EventHandler? ContinueGame;

        public event EventHandler? PauseMenu;

        public event EventHandler<SlotEventArgs>? LoadingSlot;

        public event EventHandler<SlotEventArgs>? SavingSlot;

        public DelegateCommand PauseGameCommand { get; private set; }

        public DelegateCommand OneSpeedCommand { get; private set; }

        public DelegateCommand FiveSpeedCommand { get; private set; }

        public DelegateCommand TenSpeedCommand { get; private set; }

        public DelegateCommand InfoCommand { get; private set; }

        public DelegateCommand NewGameCommand { get; private set; }

        public DelegateCommand LoadGameCommand { get; private set; }

        public DelegateCommand SaveGameCommand { get; private set; }

        public DelegateCommand ExitCommand { get; private set; }

        public DelegateCommand ContinueGameCommand { get; private set; }

        public DelegateCommand PauseMenuCommand { get; private set; }

        public SimVillageViewModel(City model)
        {
            this.model = model;
            model.GameAdvanced += new EventHandler(Model_GameAdvanced);
            model.GameChanged += new EventHandler(Model_GameChanged);
            model.GameCreated += new EventHandler(Model_GameCreated);
            model.StoredGamesChanged += new EventHandler(Model_StoredGamesChanged);
            Date = "📅 " + model.Date.ToString("yyyy") + " " + model.Date.ToString("M");
            Money = "💲 " + model.GetBudget();
            CitizenCount = model.Citizens != null ? "👤 " + model.Citizens.Count : "👤 0";
            Happiness = model.Citizens != null ? "🙂 " + model.GetHappiness() : "🙂 0";
            Speed = 5;

            Fields = new ObservableCollection<Field>();

            GenerateMap();
            StoredGames = new ObservableCollection<StoredGameViewModel>();
             
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
            PauseGameCommand = new DelegateCommand(param => OnPauseGame());
            OneSpeedCommand = new DelegateCommand(param => OnOneSpeedCommand());
            FiveSpeedCommand = new DelegateCommand(param => OnFiveSpeedCommand());
            TenSpeedCommand = new DelegateCommand(param => OnTenSpeedCommand());
            InfoCommand = new DelegateCommand(param => OnInfoCommand());
            NewGameCommand = new DelegateCommand(param => OnNewGame(Convert.ToString(param)));
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());
            ContinueGameCommand = new DelegateCommand(param => OnContinueGame());
            PauseMenuCommand = new DelegateCommand(param => OnPauseMenu());
        }

        private void Model_StoredGamesChanged(object? sender, EventArgs e)
        {
            StoredGames.Clear();
            foreach (StoredGame game in model.StoredGames)
            {
                StoredGames.Add(new StoredGameViewModel
                {
                    SlotNumber = game.Slot,
                    Slot = "Slot " + game.Slot,
                    Name = game.Name,
                    SlotClickedCommand = new DelegateCommand(param => OnSlotClicked(Convert.ToInt32(param))),
                    Modified = game.Name != string.Empty ? game.Modified.ToString("G") : string.Empty
                });
            }
        }

        private void OnSlotClicked(int number)
        {
            if (doSave)
            {
                SavingSlot?.Invoke(this, new SlotEventArgs { Slot = number });
            }
            else
            {
                LoadingSlot?.Invoke(this, new SlotEventArgs { Slot = number });
            }
        }

        private void OnSetSave()
        {
            doSave = true;
        }

        private void OnSetLoad()
        {
            doSave = false;
        }

        public void SetTax(int residentalTax, int industrialTax, int storeTax)
        {
            model.Finances.ResidentTax = residentalTax;
            model.Finances.IndustrialTax = industrialTax;
            model.Finances.StoreTax = storeTax;
        }

        private void OnPauseMenu()
        {
            PauseMenu?.Invoke(this, EventArgs.Empty);
        }

        private void OnContinueGame()
        {
            ContinueGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveGame()
        {
            OnSetSave();
            SaveGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            OnSetLoad();
            LoadGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnNewGame(string? param)
        {
            if (!string.IsNullOrEmpty(param))
                NewGame?.Invoke(this, new NewGameEventArgs { Name = param });
        }

        private void OnInfoCommand()
        {
            Info?.Invoke(this, EventArgs.Empty);
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
                        Top = i * 64,
                        Left = j * 64,
                        Number = i * Width + j,
                        Name = string.Empty,
                        Info = string.Empty,
                        Clicked = new DelegateCommand(param => OnFieldClicked(Convert.ToInt32(param))),
                        UpgradeCommand = new DelegateCommand(param => UpgradeZone(Convert.ToInt32(param)))
                    });
                }
            }
            CanvasW = Width * 64;
            CanvasH = Height * 64;
            OnPropertyChanged(nameof(CanvasW));
            OnPropertyChanged(nameof(CanvasH));
            OnPropertyChanged(nameof(Fields));
        }

        private void UpgradeZone(int v)
        {
            Field field = Fields[v];
            model.UpgradeZone(field.X, field.Y);
        }

        private void OnFieldClicked(int number)
        {
            Field field = Fields[number];
            if (building != null)
            {
                switch (building.Text)
                {
                    case "Residental":
                        if (model.NewZone(field.X, field.Y, ZoneType.Residental))
                            field.Text = "Residental";
                        break;
                    case "Industrial":
                        if (model.NewZone(field.X, field.Y, ZoneType.Industrial))
                            field.Text = "Industrial";
                        break;
                    case "Store":
                        if (model.NewZone(field.X, field.Y, ZoneType.Store))
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
                        model.DemolishZone(field.X,field.Y);
                        break;
                }
            } else
            {
                if (this.field == null)
                {
                    this.field = field;
                    field.IsClicked = true;
                }
                else if (this.field == field)
                {
                    this.field = null!;
                    field.IsClicked = false;
                }
                else
                {
                    this.field.IsClicked = false;
                    this.field = field;
                    field.IsClicked = true;
                }
            }
        }

        private void OnTenSpeedCommand()
        {
            Speed = 10;
            OnPropertyChanged(nameof(Speed));
            TenSpeed?.Invoke(this, EventArgs.Empty);
        }

        private void OnFiveSpeedCommand()
        {
            Speed = 5;
            OnPropertyChanged(nameof(Speed));
            FiveSpeed?.Invoke(this, EventArgs.Empty);
        }

        private void OnOneSpeedCommand()
        {
            Speed = 1;
            OnPropertyChanged(nameof(Speed));
            OneSpeed?.Invoke(this, EventArgs.Empty);
        }

        private void OnPauseGame()
        {
            Speed = 0;
            OnPropertyChanged(nameof(Speed));
            PauseGame?.Invoke(this, EventArgs.Empty);
        }

        private void Model_GameChanged(object? sender, EventArgs e)
        {
            CitizenCount = "👤 " +  model.Citizens.Count;
            Money = "💲 " +  model.GetBudget();
            Happiness = "🙂 " + model.GetHappiness();

            if(model.GetBudget() < 0)
            {
                IsMoneyNegative = true;
            }
            else
            {
                IsMoneyNegative = false;
            }

            foreach (List<Zone> rows in model.Map)
            {
                foreach (Zone zone in rows)
                {
                    if (zone.Building != null)
                    {
                        switch (zone.Building)
                        {
                            case Road:
                                List<Road> roads = new List<Road>();
                                for (int i = -1; i < 2; i++)
                                {
                                    for (int j = -1; j < 2; j++)
                                    {
                                        if (Math.Abs(i) != Math.Abs(j) && zone.X + i >= 0 && zone.X + i < Height && zone.Y + j >= 0 && zone.Y + j < Width && model.Map[zone.X + i][zone.Y + j].Building != null)
                                        {
                                            if (model.Map[zone.X + i][zone.Y + j].Building.GetType() == typeof(Road))
                                                roads.Add((Road)model.Map[zone.X + i][zone.Y + j].Building);
                                        }
                                    }
                                }
                                if (roads.Count == 0)
                                    Fields[zone.X * Width + zone.Y].Text = "Road";
                                else if (roads.Count == 1)
                                {
                                    if (roads[0].Y > zone.Y || roads[0].Y < zone.Y)
                                        Fields[zone.X * Width + zone.Y].Text = "Road H";
                                    else
                                        Fields[zone.X * Width + zone.Y].Text = "Road";
                                } else if (roads.Count == 2)
                                {
                                    bool up = false;
                                    bool right = false;
                                    int hor = 0;
                                    int ver = 0;
                                    foreach (Road road in roads)
                                    {
                                        if (road.Y == zone.Y)
                                        {
                                            ver++;
                                            if (road.X < zone.X)
                                                up = true;
                                        }
                                        else if (road.X == zone.X)
                                        {
                                            hor++;
                                            if (road.Y > zone.Y)
                                                right = true;
                                        }
                                    }
                                    if (hor == 2)
                                        Fields[zone.X * Width + zone.Y].Text = "Road H";
                                    else if (ver == 2)
                                        Fields[zone.X * Width + zone.Y].Text = "Road";
                                     else if (up)
                                    {
                                        if (right)
                                            Fields[zone.X * Width + zone.Y].Text = "Road UR";
                                        else
                                            Fields[zone.X * Width + zone.Y].Text = "Road UL";
                                    } else
                                    {
                                        if (right)
                                            Fields[zone.X * Width + zone.Y].Text = "Road BR";
                                        else
                                            Fields[zone.X * Width + zone.Y].Text = "Road BL";
                                    }
                                } else if (roads.Count == 3)
                                {
                                    bool vertical = false;
                                    List<Road> toRemoveH = new List<Road>();
                                    List<Road> toRemoveV = new List<Road>();
                                    bool horizontal = false;
                                    foreach (Road road in roads)
                                    {
                                        if (road.Y < zone.Y || zone.Y < road.Y)
                                        {
                                            if (horizontal)
                                            {
                                                toRemoveH.Add(road);
                                                horizontal = false;
                                            } else
                                            {
                                                horizontal = true;
                                                toRemoveH.Add(road);
                                            }
                                        } else if (road.X > zone.X || road.X < zone.X)
                                        {
                                            if (vertical)
                                            {
                                                toRemoveV.Add(road);
                                                vertical = false;
                                            } else
                                            {
                                                toRemoveV.Add(road);
                                                vertical = true;
                                            }
                                        }
                                    }
                                    if (!horizontal)
                                    {
                                        roads.RemoveAll(i => toRemoveH.Contains(i));
                                        if (roads[0].X < zone.X)
                                            Fields[zone.X * Width + zone.Y].Text = "Road HU";
                                        else
                                            Fields[zone.X * Width + zone.Y].Text = "Road HB";
                                    } else if (!vertical)
                                    {
                                        roads.RemoveAll(i => toRemoveV.Contains(i));
                                        if (roads[0].Y < zone.Y)
                                            Fields[zone.X * Width + zone.Y].Text = "Road VL";
                                        else
                                            Fields[zone.X * Width + zone.Y].Text = "Road VR";
                                    }
                                } else if (roads.Count == 4)
                                    Fields[zone.X * Width + zone.Y].Text = "Road 4";
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
                                if (zone.X == zone.Building.X && zone.Y == zone.Building.Y)
                                    Fields[zone.X * Width + zone.Y].Text = "Power Plant UL";
                                else if (zone.X == zone.Building.X && zone.Building.Y + 1 == zone.Y)
                                    Fields[zone.X * Width + zone.Y].Text = "Power Plant UR";
                                else if (zone.X == zone.Building.X + 1 && zone.Y == zone.Building.Y)
                                    Fields[zone.X * Width + zone.Y].Text = "Power Plant BL";
                                else
                                    Fields[zone.X * Width + zone.Y].Text = "Power Plant BR";
                                break;
                            case School s:
                                if (s.Type == SchoolTypes.Elementary)
                                {
                                    if (zone.X == zone.Building.X && zone.Y == zone.Building.Y)
                                        Fields[zone.X * Width + zone.Y].Text = "School L";
                                    else
                                        Fields[zone.X * Width + zone.Y].Text = "School R";
                                }
                                else
                                {
                                    if (zone.X == zone.Building.X && zone.Y == zone.Building.Y)
                                        Fields[zone.X * Width + zone.Y].Text = "University UL";
                                    else if (zone.X == zone.Building.X && zone.Building.Y + 1 == zone.Y)
                                        Fields[zone.X * Width + zone.Y].Text = "University UR";
                                    else if (zone.X == zone.Building.X + 1 && zone.Y == zone.Building.Y)
                                        Fields[zone.X * Width + zone.Y].Text = "University BL";
                                    else
                                        Fields[zone.X * Width + zone.Y].Text = "University BR";
                                }
                                break;
                            case Stadium:
                                if (zone.X == zone.Building.X && zone.Y == zone.Building.Y)
                                    Fields[zone.X * Width + zone.Y].Text = "Stadium UL";
                                else if (zone.X == zone.Building.X && zone.Building.Y + 1 == zone.Y)
                                    Fields[zone.X * Width + zone.Y].Text = "Stadium UR";
                                else if (zone.X == zone.Building.X + 1 && zone.Y == zone.Building.Y)
                                    Fields[zone.X * Width + zone.Y].Text = "Stadium BL";
                                else
                                    Fields[zone.X * Width + zone.Y].Text = "Stadium BR";
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
                    }
                    else
                    {
                        Fields[zone.X * Width + zone.Y].Text = zone.ZoneType switch
                        {
                            ZoneType.Residental => "Residental",
                            ZoneType.Industrial => "Industrial",
                            ZoneType.Store => "Store",
                            _ => string.Empty
                        };
                    }
                    Fields[zone.X * Width + zone.Y].Name = zone.ToString();
                    Fields[zone.X * Width + zone.Y].Info = null != zone.Building ? zone.Building.ToString() : "";
                    if (zone.ZoneType != ZoneType.General)
                        Fields[zone.X * Width + zone.Y].Info += zone.Info;
                }
            }
            OnPropertyChanged(nameof(CitizenCount));
            OnPropertyChanged(nameof(IsMoneyNegative));
            OnPropertyChanged(nameof(Money));
            OnPropertyChanged(nameof(Fields));
            OnPropertyChanged(nameof(Happiness));
        }

        private void Model_GameCreated(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Name));
            Model_GameChanged(sender, e);
        }

        private void Model_GameAdvanced(object? sender, EventArgs e)
        {
            Date = "📅 " + model.Date.ToString("yyyy") + " " + model.Date.ToString("M");
            OnPropertyChanged(nameof(Date));
        }
    }
}
