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

        public int CitizenCount { get; private set; }

        public int Happiness { get { return model.getHappiness(); } }

        public List<Transaction> Expenses { get { var asd = model.Finances.Expenses; asd.Reverse(); return asd; } }

        public List<Transaction> Incomes { get { var asd = model.Finances.Incomes; asd.Reverse(); return asd; } }

        public int ResidentTax { get { return model.Finances.getTax(ZoneType.Residental); } }

        public int IndustrialTax { get { return model.Finances.getTax(ZoneType.Industrial); } }

        public int StoreTax { get { return model.Finances.getTax(ZoneType.Store); } }

        public bool IsMoneyNegative { get; private set; }

        public int Money { get; private set; }

        public string BuildInfo { get; private set; }

        private Option building = null!;

        private Field field = null!;

        private bool doSave = false;

        public event EventHandler? NewGame;

        public event EventHandler? LoadGame;

        public event EventHandler? SaveGame;

        public event EventHandler? PauseGame;

        public event EventHandler? OneSpeed;

        public event EventHandler? FiveSpeed;

        public event EventHandler? TenSpeed;

        public event EventHandler? Info;

        public event EventHandler? Rename;

        public event EventHandler? ExitGame;

        public event EventHandler? ContinueGame;

        public event EventHandler? PauseMenu;

        public event EventHandler<SlotEventArgs>? LoadingSlot;
        public event EventHandler<SlotEventArgs>? SavingSlot;

        public event EventHandler<SlotEventArgs>? SlotDelete;

        public DelegateCommand RenameCommand { get; private set; }

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
        public DelegateCommand Slot1Command { get; private set; }
        public DelegateCommand Slot2Command { get; private set; }
        public DelegateCommand Slot3Command { get; private set; }
        public DelegateCommand Slot4Command { get; private set; }
        public DelegateCommand Slot5Command { get; private set; }
        public DelegateCommand Slot1DeleteCommand { get; private set; }
        public DelegateCommand Slot2DeleteCommand { get; private set; }
        public DelegateCommand Slot3DeleteCommand { get; private set; }
        public DelegateCommand Slot4DeleteCommand { get; private set; }
        public DelegateCommand Slot5DeleteCommand { get; private set; }

        public DelegateCommand SetSaveCommand { get; private set; }
        public DelegateCommand SetLoadCommand { get; private set; }

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
            InfoCommand = new DelegateCommand(param => OnInfoCommand());
            NewGameCommand = new DelegateCommand(param => OnNewGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());
            ContinueGameCommand = new DelegateCommand(param => OnContinueGame());
            PauseMenuCommand = new DelegateCommand(param => OnPauseMenu());

            Slot1Command = new DelegateCommand(param => OnSlot(1));
            Slot2Command = new DelegateCommand(param => OnSlot(2));
            Slot3Command = new DelegateCommand(param => OnSlot(3));
            Slot4Command = new DelegateCommand(param => OnSlot(4));
            Slot5Command = new DelegateCommand(param => OnSlot(5));

            Slot1DeleteCommand = new DelegateCommand(param => OnSlotDelete(1));
            Slot2DeleteCommand = new DelegateCommand(param => OnSlotDelete(2));
            Slot3DeleteCommand = new DelegateCommand(param => OnSlotDelete(3));
            Slot4DeleteCommand = new DelegateCommand(param => OnSlotDelete(4));
            Slot5DeleteCommand = new DelegateCommand(param => OnSlotDelete(5));

            SetSaveCommand = new DelegateCommand(param => OnSetSave());
            SetLoadCommand = new DelegateCommand(param => OnSetLoad());
        }

        private void OnSetSave()
        {
            doSave = true;
            return;
        }

        private void OnSetLoad()
        {
            doSave = false;
            return;
        }

        private void OnSlot(int n)
        {
            if (doSave)
            {
                SavingSlot?.Invoke(this, new SlotEventArgs { Slot = n });
            }
            else
            {
                LoadingSlot?.Invoke(this, new SlotEventArgs { Slot = n });
            }
        }

        private void OnSlotDelete(int n)
        {
            SlotDelete?.Invoke(this, new SlotEventArgs { Slot = n });
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
            SaveGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnNewGame()
        {
            NewGame?.Invoke(this, EventArgs.Empty);
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

            foreach (List<Zone> rows in model.Map)
            {
                foreach (Zone zone in rows)
                {
                    if (zone.Building != null)
                    {
                        switch (zone.Building)
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
                    Fields[zone.X * Width + zone.Y].Info = zone.Building != null ? zone.Building.ToString() : "";
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
            Date = model.Date.ToString("yyyy") + " " + model.Date.ToString("M");
            OnPropertyChanged(nameof(Date));
        }
    }
}
