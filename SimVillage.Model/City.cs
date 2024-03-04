using SimVillage.Model.Building;
using SimVillage.Persistence;

namespace SimVillage.Model
{
    public class City
    {
        private const int mapWidth = 60;

        private const int mapHeight = 30;

        private int peopleAtStart = 30;

        private bool canDemolish = false;

        private bool gameOver;

        private string cityName = string.Empty;

        private DateTime date = new DateTime(2000, 1, 1);

        private Persistence dataAccess;

        public Finances Finances { get; private set; } = null!;

        public StoredGame[] StoredGames { get { return dataAccess.StoredGames; } }

        private List<List<Zone>> map = null!;

        public int Width() { return mapWidth;}

        public int Height() { return mapHeight;}

        public string Name { get { return cityName; } }

        public DateTime Date { get { return date; } }

        public List<List<Zone>> Map { get { return map; } }

        private List<Citizen> citizens = null!;

        private List<Industrial> availableIndustrials = null!;

        private List<Store> availableStores = null!;

        private List<Residental> availableHouses = null!;

        private List<School> availableSchools = null!;

        private List<PowerPlant> powerPlants = null!;

        public EventHandler? GameAdvanced;

        public EventHandler? GameCreated;

        public EventHandler? GameChanged;

        public EventHandler? ConflictDemolish;

        public EventHandler? StoredGamesChanged;

        public EventHandler? GameOver;

        public List<Citizen> Citizens { get { return citizens; } }

        public City(Persistence dataAccess)
        {
            this.dataAccess = dataAccess;
            UpdateStoredGames();
        }

        private async void UpdateStoredGames()
        {
            if (dataAccess.GetType() == typeof(Persistence))
                await dataAccess.UpdateStoredGames();
            OnStoredGamesChanged();
        }

        private void OnStoredGamesChanged()
        {
            StoredGamesChanged?.Invoke(this, EventArgs.Empty);
        }

        public void NewGame(string name)
        {
            cityName = name;
            date = new DateTime(2000, 1, 1);
            Finances = new Finances(5000);
            citizens = new List<Citizen>();
            availableStores = new List<Store>();
            availableIndustrials = new List<Industrial>();
            availableHouses = new List<Residental>();
            availableSchools = new List<School>();
            powerPlants = new List<PowerPlant>();
            gameOver = false;

            map = new List<List<Zone>>(mapHeight);

            for (int i = 0; i < mapHeight; i++)
            {
                map.Add(new List<Zone>(mapWidth));
                for (int j = 0; j < mapWidth; j++)
                {
                    map[i].Add(new Zone
                    {
                        X = i,
                        Y = j,
                        Occupied = false
                    });
                    if (j == 0 && i == 29)
                    {
                        BuildBuilding(new Road(i, j), true);
                    }
                }
            }
            Random random = new Random();

            for (int i = 0; i < 15; i++)
            {
                int x = random.Next(0, mapHeight);
                int y = random.Next(0, mapWidth);
                Forest forest = new Forest(x, y);
                forest.Age = 10;
                BuildBuilding(forest, true);

            }
            OnGameCreated();
        }

        public int GetBudget()
        {
            return Finances != null ? Finances.Budget : 0;
        }

        public void CanDemolish(bool boolean)
        {
            canDemolish = boolean;
        }

        private int calcHappiness()
        {
            int total_happiness = 0;
            List<Citizen> remove = new List<Citizen>();
            foreach (Citizen c in citizens)
            {
                int taxes = 0;
                taxes += Finances.ResidentTax;
                if (c.WorkPlace?.GetType() == typeof(Industrial))
                    taxes += Finances.IndustrialTax;
                else
                    taxes += Finances.StoreTax;
                int tax_effect = 40 - taxes;
                bool availableForest = false;
                bool availableStadium = false;
                bool availablePolice = false;
                bool availableIndustrial = false;
                int Happiness = 0;

                Happiness -= c.HadToMove * 10;
                if (c.WorkPlace == null)
                    Happiness -= 20;
                else if (c.Home != null)
                {
                    int work_distance = calcDistance(c.Home, c.WorkPlace);
                    Happiness += tax_effect;
                    work_distance = 15 - work_distance;
                    Happiness += work_distance;
                }
                Happiness -= Finances.NegativeBudgetYears * 20;
                Happiness += c.Salary / 10;

                if (c.EducationLevel == EducationLevel.Basic)
                    Happiness -= 5;
                else if (c.EducationLevel == EducationLevel.Middle)
                    Happiness += 5;
                else
                    Happiness += 10;

                if (c.Home != null)
                {
                    int j = 0;
                    int homeY = c.Home.Y;
                    int homeX = c.Home.X;
                    for (int step = 1; step <= Stadium.GetRadius() && !availableStadium; step++)
                    {
                        for (int i = 0; i < step + j && !availableStadium; i++)
                        {
                            if (c.Home != null && homeX >= 0 && homeX < mapHeight && ++homeY < mapWidth && homeY >= 0 && map[homeX][homeY].Building?.GetType() == typeof(Stadium))
                            {
                                Happiness += 15;
                                availableStadium = true;
                            }
                        }

                        for (int i = 0; i < step + j && !availableStadium; i++)
                        {
                            if (c.Home != null && ++homeX < mapHeight && homeX >= 0 && homeY < mapWidth && homeY >= 0 && map[homeX][homeY].Building?.GetType() == typeof(Stadium))
                            {
                                Happiness += 15;
                                availableStadium = true;
                            }
                        }
                        j++;
                        for (int i = 0; i < step + j && !availableStadium; i++)
                        {
                            if (c.Home != null && --homeY >= 0 && homeY < mapWidth && homeX >= 0 && homeX < mapHeight && map[homeX][homeY].Building?.GetType() == typeof(Stadium))
                            {
                                Happiness += 15;
                                availableStadium = true;
                            }
                        }

                        for (int i = 0; i < step + j && !availableStadium; i++)
                        {
                            if (c.Home != null && --homeX >= 0 && homeX < mapHeight && homeY < mapWidth && homeY >= 0 && map[homeX][homeY].Building?.GetType() == typeof(Stadium))
                            {
                                Happiness += 15;
                                availableStadium = true;
                            }
                        }
                    }
                    for (int i = 0; i < Stadium.GetRadius() * 2 && !availableStadium; i++)
                    {
                        if (c.Home != null && ++homeY < mapWidth && homeY >= 0 && homeX >= 0 && homeX < mapHeight && map[homeX][homeY].Building?.GetType() == typeof(Stadium))
                        {
                            Happiness += 15;
                            availableStadium = true;
                        }
                    }
                }

                if (c.Home != null)
                {
                    int j = 0;
                    int homeY = c.Home.Y;
                    int homeX = c.Home.X;
                    for (int step = 1; step <= Industrial.GetRadius() && !availableIndustrial; step++)
                    {
                        for (int i = 0; i < step + j && !availableIndustrial; i++)
                        {
                            if (c.Home != null && ++homeY < mapWidth && homeY >= 0 && homeX >= 0 && homeX < mapHeight && map[homeX][homeY].Building?.GetType() == typeof(Industrial))
                            {
                                Happiness -= 8;
                                availableIndustrial = true;
                            }
                        }

                        for (int i = 0; i < step + j && !availableIndustrial; i++)
                        {
                            if (c.Home != null && ++homeX < mapHeight && homeX >= 0 && homeY >= 0 && homeY < mapWidth && map[homeX][homeY].Building?.GetType() == typeof(Industrial))
                            {
                                Happiness -= 8;
                                availableIndustrial = true;
                            }
                        }
                        j++;
                        for (int i = 0; i < step + j && !availableIndustrial; i++)
                        {
                            if (c.Home != null && --homeY >= 0 && homeY < mapWidth && homeX < mapHeight && homeX >= 0 && map[homeX][homeY].Building?.GetType() == typeof(Industrial))
                            {
                                Happiness -= 8;
                                availableIndustrial = true;
                            }
                        }

                        for (int i = 0; i < step + j && !availableIndustrial; i++)
                        {
                            if (c.Home != null && --homeX >= 0 && homeX < mapHeight && homeY >= 0 && homeY < mapWidth && map[homeX][homeY].Building?.GetType() == typeof(Industrial))
                            {
                                Happiness -= 8;
                                availableIndustrial = true;
                            }
                        }
                    }
                    for (int i = 0; i < Industrial.GetRadius() * 2 && !availableIndustrial; i++)
                    {
                        if (c.Home != null && ++homeY < mapWidth && homeY >= 0 && homeX < mapHeight && homeX >= 0 && map[homeX][homeY].Building?.GetType() == typeof(Industrial))
                        {
                            Happiness -= 8;
                            availableIndustrial = true;
                        }
                    }
                }

                if (c.Home != null)
                {
                    int j = 0;
                    int homeY = c.Home.Y;
                    int homeX = c.Home.X;
                    for (int step = 1; step <= PoliceDepartment.GetRadius() && !availablePolice; step++)
                    {
                        for (int i = 0; i < step + j && !availablePolice; i++)
                        {
                            if (c.Home != null && ++homeY < mapWidth && homeY >= 0 && homeX >= 0 && homeX < mapHeight && map[homeX][homeY].Building?.GetType() == typeof(PoliceDepartment))
                            {
                                Happiness += 7;
                                availablePolice = true;
                            }
                        }

                        for (int i = 0; i < step + j && !availablePolice; i++)
                        {
                            if (c.Home != null && ++homeX < mapHeight && homeX >= 0 && homeY >= 0 && homeY < mapWidth && map[homeX][homeY].Building?.GetType() == typeof(PoliceDepartment))
                            {
                                Happiness += 7;
                                availablePolice = true;
                            }
                        }
                        j++;
                        for (int i = 0; i < step + j && !availablePolice; i++)
                        {
                            if (c.Home != null && --homeY >= 0 && homeX >= 0 && homeY < mapWidth && homeX < mapHeight && map[homeX][homeY].Building?.GetType() == typeof(PoliceDepartment))
                            {
                                Happiness += 7;
                                availablePolice = true;
                            }
                        }

                        for (int i = 0; i < step + j && !availablePolice; i++)
                        {
                            if (c.Home != null && --homeX >= 0 && homeX < mapHeight && homeY < mapWidth && homeY >= 0 && map[homeX][homeY].Building?.GetType() == typeof(PoliceDepartment))
                            {
                                Happiness += 7;
                                availablePolice = true;
                            }
                        }
                    }
                    for (int i = 0; i < PoliceDepartment.GetRadius() * 2 && !availablePolice; i++)
                    {
                        if (c.Home != null && ++homeY < mapWidth && homeY >= 0 && homeX >= 0 && homeX < mapHeight && map[homeX][homeY].Building?.GetType() == typeof(PoliceDepartment))
                        {
                            Happiness += 7;
                            availablePolice = true;
                        }
                    }
                }

                if (c.Home != null)
                {
                    int j = 0;
                    int homeY = c.Home.Y;
                    int homeX = c.Home.X;
                    for (int step = 1; step <= Forest.GetRadius() && !availableForest; step++)
                    {
                        for (int i = 0; i < step + j && !availableForest; i++)
                        {
                            if (c.Home != null && ++homeY < mapWidth && homeY >= 0 && homeX >= 0 && homeX < mapHeight && map[homeX][homeY].Building?.GetType() == typeof(Forest) && !BuildingBlocksSight(c.Home.X, c.Home.Y, homeX, homeY))
                            {
                                Forest f = (Forest)map[homeX][homeY].Building;
                                Happiness += f.Age;
                                availableForest = true;
                            }
                        }

                        for (int i = 0; i < step + j && !availableForest; i++)
                        {
                            if (c.Home != null && ++homeX < mapHeight && homeX >= 0 && homeY >= 0 && homeY < mapWidth && map[homeX][homeY].Building?.GetType() == typeof(Forest) && !BuildingBlocksSight(c.Home.X, c.Home.Y, homeX, homeY))
                            {
                                Forest f = (Forest)map[homeX][homeY].Building;
                                Happiness += f.Age;
                                availableForest = true;
                            }
                        }
                        j++;
                        for (int i = 0; i < step + j && !availableForest; i++)
                        {
                            if (c.Home != null && --homeY >= 0 && homeY < mapWidth && homeX < mapHeight && homeX >= 0 && map[homeX][homeY].Building?.GetType() == typeof(Forest) && !BuildingBlocksSight(c.Home.X, c.Home.Y, homeX, homeY))
                            {
                                Forest f = (Forest)map[homeX][homeY].Building;
                                Happiness += f.Age;
                                availableForest = true;
                            }
                        }

                        for (int i = 0; i < step + j && !availableForest; i++)
                        {
                            if (c.Home != null && --homeX >= 0 && homeX < mapHeight && homeY < mapWidth && homeY >= 0 && map[homeX][homeY].Building?.GetType() == typeof(Forest) && !BuildingBlocksSight(c.Home.X, c.Home.Y, homeX, homeY))
                            {
                                Forest f = (Forest)map[homeX][homeY].Building;
                                Happiness += f.Age;
                                availableForest = true;
                            }
                        }
                    }
                    for (int i = 0; i < Forest.GetRadius() * 2 && !availableForest; i++)
                    {
                        if (c.Home != null && ++homeY < mapWidth && homeY >= 0 && homeX >= 0 && homeX < mapHeight && map[homeX][homeY].Building?.GetType() == typeof(Forest) && !BuildingBlocksSight(c.Home.X, c.Home.Y, homeX, homeY))
                        {
                            Forest f = (Forest)map[homeX][homeY].Building;
                            Happiness += f.Age;
                            availableForest = true;
                        }
                    }
                }

                if (Happiness < 0)
                    Happiness = 0;
                else if (Happiness > 100)
                    Happiness = 100;

                if (!c.Pensioner && Happiness <= 5)
                {
                    c.Home?.MoveOut();
                    if (c.Home != null && c.Home.FreeSpace())
                        availableHouses.Add(c.Home);
                    if (c.WorkPlace?.GetType() == typeof(Store))
                    {
                        Store store = (Store)c.WorkPlace;
                        store.WorkerLeft();
                        if (store.FreeSpace())
                            availableStores.Add(store);
                    }
                    else if (c.WorkPlace?.GetType() == typeof(Industrial))
                    {
                        Industrial industrial = (Industrial)c.WorkPlace;
                        industrial.WorkerLeft();
                        if (industrial.FreeSpace())
                            availableIndustrials.Add(industrial);
                    }
                    if (c.Home != null)
                        map[c.Home.X][c.Home.Y].RemoveCitizenFromZone(c);
                    if (c.WorkPlace != null)
                        map[c.WorkPlace.X][c.WorkPlace.Y].RemoveCitizenFromZone(c);
                    c.MoveOut();
                    remove.Add(c);
                }
                c.Happiness = Happiness;
                total_happiness += Happiness;
            }
            citizens.RemoveAll(c => remove.Contains(c));
            return total_happiness;
        }
        private bool BuildingBlocksSight(int startX, int startY, int endX, int endY)
        {
            int dx = Math.Abs(endX - startX);
            int dy = Math.Abs(endY - startY);
            int x = startX;
            int y = startY;
            int sx = startX < endX ? 1 : -1;
            int sy = startY < endY ? 1 : -1;
            int err = dx - dy;

            while (x != endX || y != endY)
            {
                if (map[x][y].Building != null)
                {
                    return true;
                }

                int err2 = 2 * err;
                if (err2 > -dy)
                {
                    err -= dy;
                    x += sx;
                }
                if (err2 < dx)
                {
                    err += dx;
                    y += sy;
                }
            }

            return false;
        }

        private void GiveEducation(School school)
        {
            school.GiveEducation();
        }

        public void DemolishZone(int x, int y, bool fire = false)
        {
            if (y == 0 && x == 29)
                return;

            if (gameOver)
            {
                OnGameOver();
                return;
            }
            Building.Building building = map[x][y].Building;
            Zone zone = map[x][y];
            bool conflict = false;
            if (building != null && (building.GetType() == typeof(Residental) || building.GetType() == typeof(Industrial) || building.GetType() == typeof(Store) || building.GetType() == typeof(Road)))
            {
                switch (building)
                {
                    case Road:
                        zone.DowngradeZone();
                        foreach (Citizen citizen in citizens)
                        {
                            if (citizen.WorkPlace != null && calcDistance(citizen.Home, citizen.WorkPlace) == -1)
                            {
                                conflict = true;
                                break;
                            }
                        }
                        zone.BuildBuilding(building);
                        break;
                    case Residental:
                        if (((Residental)building).Inhabitants > 0)
                            conflict = true;
                        break;
                    case Industrial:
                        if (((Industrial)building).Workers > 0)
                            conflict = true;
                        break;
                    case Store:
                        if (((Store)building).Workers > 0)
                            conflict = true;
                        break;
                    default:
                        break;
                }
            }
            if (!fire && conflict)
                OnConflictDemolish();
            if (building != null)
            {
                if (!conflict || (conflict && canDemolish))
                {
                    bool added_money = false;
                    if (building.GetType() == typeof(Road))
                    {
                        zone.DowngradeZone();
                        foreach (List<Zone> rows in map)
                        {
                            foreach (Zone zones in rows)
                            {
                                if (zones.Building != null && zones.Building.GetType() != typeof(Road) && zones.Building.GetType() != typeof(PowerLine) && zones.Building.GetType() != typeof(Residental) && zones.Building.GetType() != typeof(Industrial) && zones.Building.GetType() != typeof(Store) && zones.Building.GetType() != typeof(Forest) && calcDistance(map[29][0].Building, zones.Building) == -1)
                                {
                                    zone.BuildBuilding(building);
                                    return;
                                }
                            }
                        }
                        zone.BuildBuilding(building);
                    }
                    if (building.GetType() == typeof(Residental))
                    {
                        if (availableHouses.Contains(building))
                            availableHouses.Remove((Residental)building);
                    }
                    List<(int, int)> buildingZones = new List<(int, int)>();
                    if (building.GetSize().Item1 == 1)
                    {
                        buildingZones.Add((building.X, building.Y));
                        if (building.GetSize().Item2 == 2)
                            buildingZones.Add((building.X, building.Y + 1));
                    } else
                    {
                        buildingZones.Add((building.X, building.Y));
                        buildingZones.Add((building.X + 1, building.Y + 1));
                        buildingZones.Add((building.X + 1, building.Y));
                        buildingZones.Add((building.X, building.Y + 1));
                    }
                    foreach ((int, int) zones in buildingZones)
                    {
                        if (map[zones.Item1][zones.Item2].DowngradeZone())
                        {
                            if (map[zones.Item1][zones.Item2].ZoneType != ZoneType.General)
                                Finances.AddIncome("Demolished a " + map[x][y].ToString(), map[x][y].GetCost() / 2, date.ToString("d"));
                        }
                        if (building != null && added_money == false)
                        {
                            Finances.AddIncome("Demolished a " + map[x][y].ToString(), building.Cost / 2, date.ToString("d"));
                            added_money = true;
                        }
                    }
                }
            } else
            {
                if (map[x][y].DowngradeZone())
                {
                    Finances.AddIncome("Demolished a " + map[x][y].ToString(), map[x][y].GetCost() / 2, date.ToString("d"));
                }
            }
            if (conflict && canDemolish)
            {
                List<Citizen> CitizensLeft = new List<Citizen>();
                if (building!.GetType() == typeof(Road))
                {
                    List<Citizen> mayMoveInAgain = new List<Citizen>();
                    foreach (Citizen citizen in Citizens)
                    {
                        if (citizen.WorkPlace != null && calcDistance(citizen.Home!, citizen.WorkPlace) == -1)
                        {
                            Building.Building workPlace = citizen.WorkPlace;
                            workPlace.IsAccessible = false;
                            if (workPlace.GetType() == typeof(Store))
                            {
                                Store store = (Store)workPlace;
                                store.WorkerLeft();
                                if (!availableStores.Contains(store))
                                    availableStores.Add(store);
                            }
                            else if (workPlace.GetType() == typeof(Industrial))
                            {
                                Industrial industrial = (Industrial)workPlace;
                                industrial.WorkerLeft();
                                if (!availableIndustrials.Contains(industrial))
                                    availableIndustrials.Add(industrial);
                            }
                            map[workPlace.X][workPlace.Y].RemoveCitizenFromZone(citizen);
                            citizen.Home!.MoveOut();
                            if (!availableHouses.Contains(citizen.Home))
                                availableHouses.Add(citizen.Home);
                            map[citizen.Home.X][citizen.Home.Y].RemoveCitizenFromZone(citizen);
                            citizen.MoveOut();
                            mayMoveInAgain.Add(citizen);
                        }
                    }
                    foreach (Citizen c in mayMoveInAgain)
                    {
                        PeopleMoveIn(c);
                        if (c.Home == null)
                            CitizensLeft.Add(c);
                        else
                            c.PlusHadToMove();
                    }
                }
                else if (building!.GetType() == typeof(Residental))
                {
                    List<Citizen> mayMoveInAgain = new List<Citizen>();
                    foreach (Citizen citizen in citizens)
                    {
                        if (citizen.Home == building)
                        {
                            Building.Building? workPlace = citizen.WorkPlace;
                            if (workPlace?.GetType() == typeof(Store))
                            {
                                Store store = (Store)workPlace;
                                store.WorkerLeft();
                                if (!availableStores.Contains(store))
                                    availableStores.Add(store);
                            }
                            else if (workPlace?.GetType() == typeof(Industrial))
                            {
                                Industrial industrial = (Industrial)workPlace;
                                industrial.WorkerLeft();
                                if (!availableIndustrials.Contains(industrial))
                                    availableIndustrials.Add(industrial);
                            }
                            if (workPlace != null)
                                map[workPlace.X][workPlace.Y].RemoveCitizenFromZone(citizen);
                            citizen.MoveOut();
                            mayMoveInAgain.Add(citizen);
                        }
                    }
                    foreach (Citizen c in mayMoveInAgain)
                    {
                        PeopleMoveIn(c);
                        if (c.Home == null)
                            CitizensLeft.Add(c);
                        else
                            c.PlusHadToMove();
                    }
                }
                else if (building!.GetType() == typeof(Industrial))
                {
                    foreach (Citizen citizen in Citizens)
                    {
                        if (citizen.WorkPlace == building)
                        {
                            citizen.WorkPlace = null!;
                            citizen.Salary = 0;
                        }
                    }
                }
                else
                {
                    foreach (Citizen citizen in Citizens)
                    {
                        if (citizen.WorkPlace == building)
                        {
                            citizen.WorkPlace = null!;
                            citizen.Salary = 0;
                        }
                    }
                }
                citizens.RemoveAll(i => CitizensLeft.Contains(i));
                Finances.AddExpenses("Demolished a " + zone.ToString() + " and you had conflict with people", building!.Cost / 2, date.ToString("d"));
                canDemolish = false;
            }
            OnGameChanged();
        }

        public async Task Save(int slot)
        {
            if (dataAccess == null)
            {
                throw new InvalidOperationException("No data access is provided");
            }
            GameState g = new GameState();
            g.Name = cityName;
            g.Citizens = citizens;
            g.Finances = Finances;
            g.Date = date;
            g.Zones = map;
            await dataAccess.saveGame(dataAccess.StoredGames[slot - 1], g);
            UpdateStoredGames();
        }

        public async Task Load(int slot)
        {
            NewGame("Loading");
            if (dataAccess == null)
            {
                throw new InvalidOperationException("No data access is provided");
            }
            GameState g = await dataAccess.LoadGame(dataAccess.StoredGames[slot - 1]);
            cityName = g.Name;
            citizens = g.Citizens;
            Finances = g.Finances;
            date = g.Date;
            map = g.Zones;
            foreach (List<Zone> rows in map)
            {
                foreach (Zone zone in rows)
                {
                    if (zone.Building != null && zone.Building.IsAccessible && zone.Building.IsPowered)
                    {
                        switch (zone.Building)
                        {
                            case Industrial i:
                                if (i.FreeSpace())
                                    availableIndustrials.Add(i);
                                break;
                            case Residental r:
                                if (r.FreeSpace())
                                    availableHouses.Add(r);
                                break;
                            case Store s:
                                if (s.FreeSpace())
                                    availableStores.Add(s);
                                break;
                            case School sch:
                                availableSchools.Add(sch);
                                break;
                            case PowerPlant p:
                                powerPlants.Add(p);
                                break;
                        }
                    }
                }
            }
            OnGameChanged();
        }

        public void UpgradeZone(int x, int y)
        {
            if (gameOver)
            {
                OnGameOver();
                return;
            }

            if (map[x][y].ZoneType != ZoneType.General && map[x][y].Building != null && map[x][y].Building.Density < 3)
            {
                switch (map[x][y].Building)
                {
                    case Residental r:
                        r.Density++;
                        r.MaxInhabitants = r.MaxInhabitants * 2 + 2;
                        if (!availableHouses.Contains(r))
                            availableHouses.Add(r);
                        break;
                    case Industrial i:
                        i.Density++;
                        i.MaxWorkers = i.MaxWorkers * 2 + 2;
                        if (!availableIndustrials.Contains(i))
                            availableIndustrials.Add(i);
                        break;
                    case Store s:
                        s.Density++;
                        s.MaxWorkers = s.MaxWorkers * 2 + 2;
                        if (!availableStores.Contains(s))
                            availableStores.Add(s);
                        break;
                }
                if (map[x][y].Building.Density == 2)
                    Finances.AddExpenses("Upgraded " + map[x][y].ToString() + " to level 2", map[x][y].GetCost(), date.ToString("d"));
                else
                    Finances.AddExpenses("Upgraded " + map[x][y].ToString() + " to level 3", map[x][y].GetCost(), date.ToString("d"));
                OnGameChanged();
            }
        }

        private void CollectingTaxes()
        {
            double tax = 0;
            double pension = 0;
            foreach (List<Zone> rows in map)
            {
                foreach (Zone zone in rows)
                {
                    if (zone.ZoneType == ZoneType.Store)
                    {
                        if (zone.Building.IsPowered)
                            tax += Finances.StoreTax / 100 * zone.Citizens.Count * 50;
                        else
                            tax += Finances.StoreTax / 100 * zone.Citizens.Count * 25;
                    }
                    else if (zone.ZoneType == ZoneType.Industrial)
                    {
                        if (zone.Building.IsPowered)
                            tax += Finances.IndustrialTax / 100 * zone.Citizens.Count * 50;
                        else
                            tax += Finances.IndustrialTax / 100 * zone.Citizens.Count * 25;
                    }
                    else if (zone.ZoneType == ZoneType.Residental)
                    {
                        foreach (Citizen citizen in zone.Citizens)
                        {
                            if (!citizen.Pensioner)
                            {
                                double paid_tax = citizen.Salary * Finances.ResidentTax / 100;
                                if (citizen.Age > 45)
                                {
                                    citizen.PaidTaxes.Add(paid_tax);
                                }
                                tax += paid_tax;
                            }
                            else
                            {
                                pension += citizen.Pension;
                            }
                        }
                    }
                }
            }
            tax = Math.Round(tax);
            if (tax != 0)
                Finances.AddIncome("Tax", Convert.ToInt32(tax), date.ToString("d"));
            if(pension != 0)
            {
                Finances.AddExpenses("Pension", Convert.ToInt32(pension), date.ToString("d"));
            }
        }

        private void calcElectricity()
        {
            HashSet<Building.Building> visited = new HashSet<Building.Building>();
            foreach (List<Zone> rows in map)
            {
                foreach (Zone zone in rows)
                {
                    if (zone.Building != null)
                        zone.Building.IsPowered = false;
                }
            }

            if (powerPlants is null) return;
            foreach (PowerPlant powerPlant in powerPlants)
            {
                int powerConsumption = 0;
                spreadingElectricity(powerConsumption, visited, powerPlant, powerPlant.GetGeneratedPower());
            }

        }

        private void spreadingElectricity(int powerConsumption, HashSet<Building.Building> visited, Building.Building current, int capacity)
        {
            List<(int, int)> buildingZones = new List<(int, int)>();
            if (current.GetSize().Item1 == 1)
            {
                buildingZones.Add((current.X, current.Y));
                if (current.GetSize().Item2 == 2)
                    buildingZones.Add((current.X, current.Y + 1));
            }
            else
            {
                buildingZones.Add((current.X, current.Y));
                buildingZones.Add((current.X + 1, current.Y + 1));
                buildingZones.Add((current.X + 1, current.Y));
                buildingZones.Add((current.X, current.Y + 1));
            }

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    foreach ((int, int) zones in buildingZones)
                    {
                        if (zones.Item1 + i >= 0 && zones.Item1 + i < mapHeight && zones.Item2 + j >= 0 && zones.Item2 + j < mapWidth)
                        {
                            Building.Building building = map[zones.Item1 + i][zones.Item2 + j].Building;
                            if (building != null && !visited.Contains(building) && !building.IsPowered && powerConsumption + building.PowerConsumption <= capacity)
                            {
                                building.IsPowered = true;
                                powerConsumption += building.PowerConsumption;
                                visited.Add(building);
                                switch (building)
                                {
                                    case School s:
                                        if (calcDistance(map[29][0].Building, s) != -1)
                                            availableSchools.Add(s);
                                        break;
                                }
                                spreadingElectricity(powerConsumption, visited, building, capacity);
                            }
                            else if (building != null && powerConsumption + building.PowerConsumption > capacity)
                                return;
                        }
                    }
                }
            }
        }

        private int calcDistance(Building.Building from, Building.Building to)
        {
            List<int> distances = new List<int>();
            HashSet<Road> visited = new HashSet<Road>();
            int n = 0;
            distancesFromTo(null!, from, to, distances, visited, n);
            distances.Sort();
            return distances.Count != 0 ? distances[0] : -1;
        }

        private void distancesFromTo(Building.Building from, Building.Building current, Building.Building to, List<int> distances, HashSet<Road> visited, int n)
        {
            if (current == null) return;
            bool found = false;
            List<(int, int)> buildingZones = new List<(int, int)>();
            if (current.GetSize().Item1 == 1)
            {
                buildingZones.Add((current.X, current.Y));
                if (current.GetSize().Item2 == 2)
                    buildingZones.Add((current.X, current.Y + 1));
            }
            else
            {
                buildingZones.Add((current.X, current.Y));
                buildingZones.Add((current.X + 1, current.Y + 1));
                buildingZones.Add((current.X + 1, current.Y));
                buildingZones.Add((current.X, current.Y + 1));
            }
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    foreach ((int, int) zones in buildingZones)
                    {
                        if (zones.Item1 + i >= 0 && zones.Item1 + i < mapHeight && zones.Item2 + j >= 0 && zones.Item2 + j < mapWidth)
                        {
                            if (map[zones.Item1 + i][zones.Item2 + j].Building == to)
                            {
                                distances.Add(n);
                                found = true;
                            }
                        }
                    }
                }
            }

            if (!found)
            {
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        foreach ((int, int) zones in buildingZones)
                        {
                            if (!(Math.Abs(i) == Math.Abs(j)) && zones.Item1 + i >= 0 && zones.Item1 + i < mapHeight && zones.Item2 + j >= 0 && zones.Item2 + j < mapWidth)
                            {
                                Building.Building building = map[zones.Item1 + i][zones.Item2 + j].Building;
                                if (building != from && building != null && building.GetType() == typeof(Road) && !visited.Contains(building))
                                {
                                    visited.Add((Road)building);
                                    distancesFromTo(current, building, to, distances, visited, ++n);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CheckGameOver()
        {
            if (peopleAtStart == 0 && (citizens.Count == 0 || (citizens.Count != 0 && GetHappiness() < 10)))
            {
                gameOver = true;
                OnGameOver();
            }
        }

        private void OnGameOver()
        {
            GameOver?.Invoke(this, EventArgs.Empty);
        }

        public int GetHappiness()
        {
            calcHappiness();
            int happiness = 0;
            foreach(Citizen c in citizens)
            {
                happiness += c.Happiness;
            }
            if(citizens.Count > 0)
            {
                return happiness / citizens.Count;
            }
            else
            {
                return 0;
            }
        }

        public bool NewZone(int x, int y, ZoneType zoneType)
        {
            if (gameOver)
            {
                OnGameOver();
                return false;
            }

            if (map[x][y].SetZone(zoneType))
            {
                if (zoneType == ZoneType.Industrial && calcDistance(map[x][y].Building, map[29][0].Building) != -1)
                {
                    availableIndustrials.Add((Industrial)map[x][y].Building);
                    map[x][y].Building.IsAccessible = true;
                }
                else if (zoneType == ZoneType.Store && calcDistance(map[x][y].Building, map[29][0].Building) != -1)
                {
                    availableStores.Add((Store)map[x][y].Building);
                    map[x][y].Building.IsAccessible = true;
                }
                Finances.AddExpenses("Built a " + map[x][y].ToString(), map[x][y].GetCost(), date.ToString("d"));
                OnGameChanged();
                return true;
            } else
            {
                return false;
            }
        }

        public void BuildBuilding(Building.Building building, bool inConstructor = false)
        {
            if (gameOver)
            {
                OnGameOver();
                return;
            }

            bool freeZone = true;
            List<(int, int)> buildingZones = new List<(int, int)>();
            if (building.GetSize().Item1 == 1)
            {
                buildingZones.Add((building.X, building.Y));
                if (building.GetSize().Item2 == 2)
                    buildingZones.Add((building.X, building.Y + 1));
            }
            else
            {
                buildingZones.Add((building.X, building.Y));
                buildingZones.Add((building.X + 1, building.Y + 1));
                buildingZones.Add((building.X + 1, building.Y));
                buildingZones.Add((building.X, building.Y + 1));
            }
            foreach ((int, int) zones in buildingZones)
            {
                if (zones.Item1 >= 0 && zones.Item1 < mapHeight && zones.Item2 >= 0 && zones.Item2 < mapWidth)
                {
                    Zone zone = map[zones.Item1][zones.Item2];
                    freeZone = freeZone && (zone.ZoneType == ZoneType.General && zone.Building == null);
                } else
                {
                    freeZone = false;
                    break;
                }
            }
            if (freeZone)
            {
                foreach ((int, int) zones in buildingZones)
                {
                    map[zones.Item1][zones.Item2].BuildBuilding(building);
                }

                if (!inConstructor)
                {
                    Finances.AddExpenses("Built a " + building.GetType().Name, building.Cost, date.ToString("d"));
                    if (calcDistance(map[29][0].Building, building) != -1)
                    {
                        if (building.GetType() == typeof(PowerPlant))
                            powerPlants.Add((PowerPlant)building);
                        building.IsAccessible = true;
                    }
                    if (building.GetType() == typeof(Road))
                    {
                        foreach (List<Zone> rows in map)
                        {
                            foreach (Zone zone in rows)
                            {
                                if (zone.Building != null && !zone.Building.IsAccessible && calcDistance(map[29][0].Building, zone.Building) != -1)
                                {
                                    zone.Building.IsAccessible = true;
                                    switch (zone.Building)
                                    {
                                        case Industrial:
                                            availableIndustrials.Add((Industrial)zone.Building);
                                            break;
                                        case Store:
                                            availableStores.Add((Store)zone.Building);
                                            break;
                                        case PowerPlant:
                                            powerPlants.Add((PowerPlant)zone.Building);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
                 
                foreach ((int, int) zones in buildingZones)
                {
                    map[zones.Item1][zones.Item2].BuildBuilding(building);
                }
                if (!inConstructor)
                    OnGameChanged();
                return;
            } else
            {
                if (!inConstructor)
                    OnGameChanged();
                return;
            }
        }

        private void GetJobForUnemployed()
        {
            foreach(Citizen citizen in citizens)
            {
                bool found = false;
                if (citizen.WorkPlace == null)
                {
                    int inStores = 0;
                    int inIndustrial = 0;
                    foreach (Store store in availableStores)
                    {
                        inStores += store.Workers;
                    }
                    foreach (Industrial industrial in availableIndustrials)
                    {
                        inIndustrial += industrial.Workers;
                    }
                    if (inIndustrial <= inStores)
                    {
                        Industrial building = null!;
                        int minDist = int.MaxValue;
                        List<Industrial> delete = new List<Industrial>();
                        foreach (Industrial industrial in availableIndustrials)
                        {
                            int dist = calcDistance(citizen.Home, industrial);
                            if (dist != -1 && dist < minDist && industrial.FreeSpace())
                            {
                                minDist = dist;
                                building = industrial;
                            }
                            else if (building != null && !building.FreeSpace())
                                delete.Add(building);
                        }
                        availableIndustrials.RemoveAll(e => delete.Contains(e));
                        if (building != null)
                        {
                            building.NewWorker();
                            citizen.WorkPlace = building;
                            found = true;
                        }
                    }
                    else
                    {
                        Store building = null!;
                        int minDist = int.MaxValue;
                        List<Store> delete = new List<Store>();
                        foreach (Store store in availableStores)
                        {
                            int dist = calcDistance(citizen.Home, store);
                            if (dist != -1 && dist < minDist && store.FreeSpace())
                            {
                                minDist = dist;
                                building = store;
                            }
                            else if (building != null && !building.FreeSpace())
                                delete.Add(building);
                        }
                        availableStores.RemoveAll(i => delete.Contains(i));
                        if (building != null)
                        {
                            building.NewWorker();
                            citizen.WorkPlace = building;
                            found = true;
                        }
                    }
                    if (found)
                    {
                        switch (citizen.EducationLevel)
                        {
                            case EducationLevel.Basic:
                                citizen.Salary = 500;
                                break;
                            case EducationLevel.Middle:
                                citizen.Salary = 1000;
                                break;
                            case EducationLevel.Higher:
                                citizen.Salary = 1500;
                                break;
                        }
                    }
                }
            }
        }

        public void AdvanceTime()
        {
            if (gameOver)
            {
                OnGameOver();
                return;
            }
            CheckGameOver();

            DateTime previous_date = date;
            date = date.AddDays(1);
            if (date.Month > previous_date.Month)
            {
                CollectingTaxes();
                double upkeep = 0;
                foreach (List<Zone> rows in map)
                {
                    foreach (Zone zone in rows)
                    {
                        if (zone.ZoneType == ZoneType.General && zone.Building != null)
                        {
                            upkeep += zone.Building.Cost * 0.01;
                        }
                    }
                }
                Finances.AddExpenses("Monthly running expenses", Convert.ToInt32(upkeep), date.ToString("d"));
            } else if (date.Year > previous_date.Year)
            {
                if (Finances.Budget < 0)
                    Finances.NegativeBudgetYears++;
                foreach (School school in availableSchools)
                    GiveEducation(school);

                foreach (List<Zone> rows in map)
                {
                    foreach (Zone zone in rows)
                    {
                        if (zone.Building != null && zone.Building.GetType() == typeof(Forest))
                            ((Forest)zone.Building).AgeUp();
                    }
                }

                foreach (Citizen citizen in citizens)
                {
                    Residental home = citizen.Home;
                    Building.Building? workPlace = citizen.WorkPlace;
                    if (!citizen.AgeUp() && workPlace != null)
                    {
                        map[workPlace.X][workPlace.Y].RemoveCitizenFromZone(citizen);
                        map[home.X][home.Y].RemoveCitizenFromZone(citizen);
                        citizens.Remove(citizen);
                        PeopleMoveIn(Citizen.ReGen18());
                    }
                }
            }
            GoToSchool();
            GetJobForUnemployed();
            PeopleMoveIn();
            CheckFire();
            OnTimeAdvanced();
        }

        private void CheckFire()
        {
            foreach (List<Zone> rows in map)
            {
                foreach (Zone zone in rows)
                {
                    if (zone.Building != null)
                    {
                        if(zone.Building.IsOnFire == false)
                        {
                            Random random = new Random();
                            bool chance = zone.Building.FireChance / 100.0 * 0.2 > random.NextDouble();
                            if (chance)
                            {
                                zone.Building.IsOnFire = true;
                            }
                        }
                        else
                        {
                            zone.Building.Health -= 1;
                            if(zone.Building.Health <= 1)
                            {
                                zone.Building.IsOnFire = false;
                                canDemolish = true;
                                DemolishZone(zone.Building.X, zone.Building.Y, true);
                            }
                        }
                    }
                        
                }
            }
        }

        public void PutOutFire(int x, int y)
        {
            if (map[x][y].Building is null) return;
            map[x][y].Building.IsOnFire = false;
            Finances.AddExpenses("Cleared a fire in a " + map[x][y].ToString(), map[x][y].GetCost(), date.ToString("d"));
            OnGameChanged();
        }

        private void GoToSchool()
        {
            if (citizens.Count == 0 || availableSchools.Count == 0)
                return;

            Random random = new Random();
            Citizen citizen;
            
            citizen = citizens[random.Next(citizens.Count)];

            foreach (School school in availableSchools)
            {
                if (citizen.EducationLevel == EducationLevel.Basic && school.Type == SchoolTypes.Elementary && school.GetMaxStudent() > school.GetStudents())
                {
                    if (random.NextDouble() <= 40)
                        school.SetStudents(citizen);
                    break;
                } else if (citizen.EducationLevel == EducationLevel.Middle && school.Type == SchoolTypes.University && school.GetMaxStudent() > school.GetStudents())
                {
                    if (random.NextDouble() <= 20)
                        school.SetStudents(citizen);
                    break;
                }
            }
        }

        private void PeopleMoveIn(Citizen citizen = null!)
        {
            if (peopleAtStart > 0 || citizen != null)
            {
                Zone houseZone = null!;
                Residental house = null!;
                if (availableHouses is null) return;
                if (availableHouses.Count == 0)
                {
                    foreach (List<Zone> rows in map)
                    {
                        foreach (Zone zone in rows)
                        {
                            if (zone.ZoneType == ZoneType.Residental && !zone.Occupied)
                            {
                                zone.BuildBuilding();
                                calcElectricity();
                                if (calcDistance(zone.Building, map[29][0].Building) == -1 || !zone.Building.IsPowered)
                                {
                                    zone.DowngradeZone();
                                    zone.SetZone(ZoneType.Residental);
                                    continue;
                                }
                                else if (zone.Building.IsPowered)
                                {
                                    zone.DowngradeZone();
                                    zone.SetZone(ZoneType.Residental);
                                    houseZone = zone;
                                    break;
                                }
                            }
                        }
                    }
                } else
                {
                    List<Residental> delete = new List<Residental>();
                    foreach (Residental houses in availableHouses)
                    {
                        if (houses.FreeSpace())
                        {
                            house = houses;
                            break;
                        } else
                        {
                            delete.Add(houses);
                        }
                    }
                    availableHouses.RemoveAll(h => delete.Contains(h));
                }
                int inStores = 0;
                int inIndustrial = 0;
                foreach (Store store in availableStores)
                {
                    inStores += store.Workers;
                }
                foreach (Industrial industrial in availableIndustrials)
                {
                    inIndustrial += industrial.Workers;
                }
                if (houseZone != null)
                {
                    house = new Residental(houseZone.X, houseZone.Y);
                }
                if (house != null && inIndustrial <= inStores)
                {
                    Industrial building = null!;
                    int minDist = int.MaxValue;
                    List<Industrial> delete = new List<Industrial>();
                    foreach (Industrial industrial in availableIndustrials)
                    {
                        int dist = calcDistance(house, industrial);
                        if (dist != -1 && dist < minDist && industrial.FreeSpace())
                        {
                            minDist = dist;
                            building = industrial;
                        }
                        else if (building != null && !building.FreeSpace())
                            delete.Add(building);
                    }
                    availableIndustrials.RemoveAll(e => delete.Contains(e));
                    if (building != null)
                    {
                        if (houseZone != null)
                        {
                            houseZone.Occupied = true;
                            houseZone.BuildBuilding();
                            house = (Residental)houseZone.Building;
                            availableHouses.Add(house);
                        }
                        if (citizen == null)
                        {
                            citizen = Citizen.ReGen(house);
                            citizens.Add(citizen);
                            peopleAtStart--;
                        } else
                            citizen.Home = house;
                        house.MoveIn();
                        map[house.X][house.Y].AddCitizenToZone(citizen);
                        map[building.X][building.Y].AddCitizenToZone(citizen);
                        building.NewWorker();
                        citizen.WorkPlace = building;
                        citizen.Salary = 500;
                    }
                } else if (house != null)
                {
                    Store building = null!;
                    int minDist = int.MaxValue;
                    List<Store> delete = new List<Store>();
                    foreach (Store store in availableStores)
                    {
                        int dist = calcDistance(house, store);
                        if (dist != -1 && dist < minDist && store.FreeSpace())
                        {
                            minDist = dist;
                            building = store;
                        } else if (building != null &&!building.FreeSpace())
                            delete.Add(building);
                    }
                    availableStores.RemoveAll(i => delete.Contains(i));
                    if (building != null)
                    {
                        if (houseZone != null)
                        {
                            houseZone.Occupied = true;
                            houseZone.BuildBuilding();
                            house = (Residental)houseZone.Building;
                            availableHouses.Add(house);
                        }
                        if (citizen == null)
                        {
                            citizen = Citizen.ReGen(house);
                            citizens.Add(citizen);
                            peopleAtStart--;
                        } else
                            citizen.Home = house;
                        house.MoveIn();
                        map[house.X][house.Y].AddCitizenToZone(citizen);
                        map[building.X][building.Y].AddCitizenToZone(citizen);
                        building.NewWorker();
                        citizen.WorkPlace = building;
                        citizen.Salary = 500;
                    }
                }
            } else
            {
                int AVGhappiness = GetHappiness() / citizens.Count;
                Residental house = null!;
                Zone houseZone = null!;
                if (availableHouses.Count == 0)
                {
                    foreach (List<Zone> rows in map)
                    {
                        foreach (Zone zone in rows)
                        {
                            if (zone.ZoneType == ZoneType.Residental)
                            {
                                zone.BuildBuilding();
                                calcElectricity();
                                if (calcDistance(zone.Building, map[29][0].Building) == -1)
                                {
                                    zone.DowngradeZone();
                                    zone.SetZone(ZoneType.Residental);
                                    continue;
                                }
                                else if (zone.Building.IsPowered)
                                {
                                    zone.DowngradeZone();
                                    zone.SetZone(ZoneType.Residental);
                                }
                            }
                            if (zone.ZoneType == ZoneType.Residental && !zone.Occupied)
                            {
                                houseZone = zone;
                                break;
                            }
                        }
                    }
                } else
                {
                    List<Residental> delete = new List<Residental>();
                    foreach (Residental houses in availableHouses)
                    {
                        if (houses.FreeSpace())
                        {
                            house = houses;
                            break;
                        }
                        else
                        {
                            delete.Add(houses);
                        }
                    }
                    availableHouses.RemoveAll(h => delete.Contains(h));
                }
                int inStores = 0;
                int inIndustrial = 0;
                foreach (Store store in availableStores)
                {
                    inStores += store.Workers;
                }
                foreach (Industrial industrial in availableIndustrials)
                {
                    inIndustrial += industrial.Workers;
                }
                if (houseZone != null)
                {
                    house = new Residental(houseZone.X, houseZone.Y);
                }
                if (house != null && inIndustrial < inStores)
                {
                    Industrial building = null!;
                    int minDist = int.MaxValue;
                    List<Industrial> delete = new List<Industrial>();
                    foreach (Industrial industrial in availableIndustrials)
                    {
                        int dist = calcDistance(house, industrial);
                        if (dist != -1 && dist < minDist && industrial.FreeSpace())
                        {
                            minDist = dist;
                            building = industrial;
                        }
                        else if (building != null && !building.FreeSpace())
                            delete.Add(building);
                    }
                    availableIndustrials.RemoveAll(i => delete.Contains(i));
                    bool found = false;
                    for (int i = 0; i < 4 && !found; i++)
                    {
                        for (int j = 0; j < 4 && !found; j++)
                        {
                            if (map[house.X + i][house.Y + j].ZoneType == ZoneType.Industrial)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                    if (building != null && minDist < 30 && !found && AVGhappiness > 5)
                    {
                        if (houseZone != null)
                        {
                            houseZone.Occupied = true;
                            houseZone.BuildBuilding();
                            house = (Residental)houseZone.Building;
                            availableHouses.Add(house);
                        }
                        citizen = Citizen.ReGen(house);
                        house.MoveIn();
                        building.NewWorker();
                        citizen.WorkPlace = building;
                        citizen.Salary = 500;
                        citizens.Add(citizen);
                        map[house.X][house.Y].AddCitizenToZone(citizen);
                        map[building.X][building.Y].AddCitizenToZone(citizen);
                    }
                }
                else if (house != null)
                {
                    Store building = null!;
                    int minDist = int.MaxValue;
                    List<Store> delete = new List<Store>();
                    foreach (Store store in availableStores)
                    {
                        int dist = calcDistance(house, store);
                        if (dist != -1 && dist < minDist && store.FreeSpace())
                        {
                            minDist = dist;
                            building = store;
                        }
                        else if (building != null && !building.FreeSpace())
                            delete.Add(building);
                    }
                    availableStores.RemoveAll(s => delete.Contains(s));
                    bool found = false;
                    for (int i = 0; i < 4 && !found; i++)
                    {
                        for (int j = 0; j < 4 && !found; j++)
                        {
                            if (map[house.X + i][house.Y + j].ZoneType == ZoneType.Industrial)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                    if (building != null && minDist < 30 && !found && AVGhappiness > 5)
                    {
                        if (houseZone != null)
                        {
                            houseZone.Occupied = true;
                            houseZone.BuildBuilding();
                            house = (Residental)houseZone.Building;
                            availableHouses.Add(house);
                        }
                        citizen = Citizen.ReGen(house);
                        house.MoveIn();
                        building.NewWorker();
                        citizen.WorkPlace = building;
                        citizen.Salary = 500;
                        citizens.Add(citizen);
                        map[house.X][house.Y].AddCitizenToZone(citizen);
                        map[building.X][building.Y].AddCitizenToZone(citizen);
                    }
                }
            }
            OnGameChanged();
        }

        private void OnTimeAdvanced()
        {
            GameAdvanced?.Invoke(this, EventArgs.Empty);
        }

        private void OnGameCreated()
        {
            GameCreated?.Invoke(this, EventArgs.Empty);
        }

        private void OnConflictDemolish()
        {
            ConflictDemolish?.Invoke(this, EventArgs.Empty);
        }

        private void OnGameChanged()
        {
            calcElectricity();
            GameChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}