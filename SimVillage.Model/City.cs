using SimVillage.Model.Building;

namespace SimVillage.Model
{
    public class City
    {
        private const int mapWidth = 60;

        private const int mapHeight = 30;

        private int peopleAtStart = 30;

        private bool canDemolish = false;

        private readonly string cityName;

        private DateTime date = new DateTime(2000, 1, 1);

        private Persistence dataAccess;

        private Finances Finances;

        private static Zone[,] map = null!;

        public int Width() { return mapWidth;}

        public int Height() { return mapHeight;}

        private List<Citizen> citizens = null!;

        private List<Industrial> avaibleIndustrials = null!;

        private List<Store> avaibleStores = null!;

        private List<Residental> avaibleHouses = null!;

        public EventHandler? gameAdvanced;

        public EventHandler? failedBuilding;

        public EventHandler? gameChanged;

        public EventHandler? ConflictDemolish;

        public List<Citizen> Citizens { get { return citizens; } }

        public City(Persistence dataAccess, string name)
        {
            this.dataAccess = dataAccess;
            cityName = name;
            Finances = new Finances(5000);
            citizens = new List<Citizen>();
            avaibleStores = new List<Store>();
            avaibleIndustrials = new List<Industrial>();
            avaibleHouses = new List<Residental>();
            

            map = new Zone[mapHeight, mapWidth];

            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    map[i, j] = new Zone
                    {
                        X = i,
                        Y = j,
                        Occupied = false
                    };
                    if (j == 0)
                    {
                        BuildBuilding(new Road(new List<Tile> { new Tile(i, 0) }));
                    }
                }
            }
            Random random = new Random();

            for (int i = 0; i < 15; i++)
            {
                int x = random.Next(0, mapHeight);
                int y = random.Next(0, mapWidth);
                BuildBuilding(new Forest(new List<Tile> { new Tile(x, y) }));
            }
        }

        public int GetBudget()
        {
            return Finances.getBudget();
        }

        public string Name { get { return cityName; } }

        public DateTime Date { get { return date; } }

        public Zone[,] Map { get { return map; } }

        public void CanDemolish(bool boolean)
        {
            canDemolish = boolean;
        }

        public void demolishZone(int x, int y)
        {
            Building.Building building = map[x, y].getBuilding();
            Zone zone = map[x, y];
            bool conflict = false;
            if (building != null && (building.GetType() == typeof(Residental) || building.GetType() == typeof(Industrial) || building.GetType() == typeof(Store) || building.GetType() == typeof(Road)))
            {
                switch (building)
                {
                    case Road:
                        zone.DowngradeZone();
                        foreach (Citizen citizen in citizens)
                        {
                            if (calcDistance(citizen.GetHome(), citizen.GetWorkPlace()) == -1)
                            {
                                conflict = true;
                                break;
                            }
                        }
                        zone.BuildBuilding(building);
                        break;
                    case Residental:
                        if (((Residental)building).GetInhabitans() > 0)
                            conflict = true;
                        break;
                    case Industrial:
                        if (((Industrial)building).GetWorkers() > 0)
                            conflict = true;
                        break;
                    case Store:
                        if (((Store)building).GetWorkers() > 0)
                            conflict = true;
                        break;
                    default:
                        break;
                }
            }
            if (conflict)
                OnConflictDemolish();
            if (building != null)
            {
                if (!conflict || (conflict && canDemolish))
                {
                    bool added_money = false;
                    foreach (Tile tile in building.GetTiles())
                    {
                        if (map[tile.GetX(), tile.GetY()].DowngradeZone())
                        {
                            if (map[tile.GetX(), tile.GetY()].ZoneType != ZoneType.General)
                                Finances.addIncome("Demolished a " + map[x, y].ToString(), map[x, y].getCost() / 2, date);
                        }
                        if (building != null && added_money == false)
                        {
                            Finances.addIncome("Demolished a " + map[x, y].ToString(), building.GetCost() / 2, date);
                            added_money = true;
                        }
                    }
                }
            } else
            {
                if (map[x, y].DowngradeZone())
                {
                    Finances.addIncome("Demolished a " + map[x, y].ToString(), map[x, y].getCost() / 2, date);
                }
            }
            if (conflict && canDemolish)
            {
                List<Citizen> CitizensLeft = new List<Citizen>();
                if (building.GetType() == typeof(Road))
                {
                    foreach (Citizen citizen in Citizens)
                    {
                        if (calcDistance(citizen.GetHome(), citizen.GetWorkPlace()) == -1)
                        {
                            Building.Building workPlace = citizen.GetWorkPlace();
                            if (workPlace.GetType() == typeof(Store))
                            {
                                Store store = (Store)workPlace;
                                store.WorkerLeft();
                                if (!avaibleStores.Contains(store))
                                    avaibleStores.Add(store);
                            }
                            else if (workPlace.GetType() == typeof(Industrial))
                            {
                                Industrial industrial = (Industrial)workPlace;
                                industrial.WorkerLeft();
                                if (!avaibleIndustrials.Contains(industrial))
                                    avaibleIndustrials.Add(industrial);
                            }
                            foreach (Tile tile in citizen.GetWorkPlace().GetTiles())
                            {
                                map[tile.GetX(), tile.GetY()].RemoveCitizenFromWorkPlace(citizen);
                            }
                            citizen.GetHome().MoveOut();
                            if (!avaibleHouses.Contains(citizen.GetHome()))
                                avaibleHouses.Add(citizen.GetHome());
                            foreach (Tile tile in citizen.GetHome().GetTiles())
                            {
                                map[tile.GetX(), tile.GetY()].MoveOutFromHome(citizen);
                            }
                            citizen.MoveOut();
                            PeopleMoveIn(citizen);
                            if (citizen.GetHome() == null)
                                CitizensLeft.Add(citizen);
                            else
                                citizen.PlusHadToMove();
                        }
                    }
                } else if (building.GetType() == typeof(Residental))
                {
                    foreach (Citizen citizen in Citizens)
                    {
                        if (citizen.GetHome() == building)
                        {
                            Building.Building workPlace = citizen.GetWorkPlace();
                            if (workPlace.GetType() == typeof(Store))
                            {
                                Store store = (Store)workPlace;
                                store.WorkerLeft();
                                if (!avaibleStores.Contains(store))
                                    avaibleStores.Add(store);
                            }
                            else if (workPlace.GetType() == typeof(Industrial))
                            {
                                Industrial industrial = (Industrial)workPlace;
                                industrial.WorkerLeft();
                                if (!avaibleIndustrials.Contains(industrial))
                                    avaibleIndustrials.Add(industrial);
                            }
                            foreach (Tile tile in citizen.GetWorkPlace().GetTiles())
                            {
                                map[tile.GetX(), tile.GetY()].RemoveCitizenFromWorkPlace(citizen);
                            }
                            citizen.MoveOut();
                            PeopleMoveIn(citizen);
                            if (citizen.GetHome() == null)
                                CitizensLeft.Add(citizen);
                            else
                                citizen.PlusHadToMove();
                        }
                    }
                } else if (building.GetType() == typeof(Industrial))
                {
                    foreach (Citizen citizen in Citizens)
                    {
                        citizen.SetWorkPlace(null!);
                        //PeopleMoveIn(citizen);
                    }
                } else
                {
                    foreach (Citizen citizen in Citizens)
                    {
                        citizen.SetWorkPlace(null!);
                    }
                }
                Finances.addExpenses("Demolished a " + zone.ToString() + " and you had conflict with people", building.GetCost() / 2, date);
                citizens.RemoveAll(i => CitizensLeft.Contains(i));
                canDemolish = false;
            }
            OnGameChanged();
        }

        public async Task Save()
        {
            if (dataAccess == null)
            {
                throw new InvalidOperationException("No data access is provided");
            }
            await dataAccess.saveGame();
        }

        public async Task Load()
        {
            if (dataAccess == null)
            {
                throw new InvalidOperationException("No data access is provided");
            }
            await dataAccess.loadGame();
        }

        private void CollectingTaxes()
        {
            foreach (Zone zone in map)
            {

            }
        }

        static public int calcDistance(Building.Building from, Building.Building to)
        {
            List<int> distances = new List<int>();
            HashSet<Road> visited = new HashSet<Road>();
            int n = 0;
            distancesFromTo(null, from, to, distances, visited, n);
            distances.Sort();
            return distances.Count != 0 ? distances[0] : -1;
        }

        static private void distancesFromTo(Building.Building from, Building.Building current, Building.Building to, List<int> distances, HashSet<Road> visited, int n)
        {
            if (map == null)
            {
                return;
            }

            bool found = false;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    foreach (Tile tile in current.GetTiles())
                    {
                        if (tile.GetX() + i >= 0 && tile.GetX() + i < mapHeight && tile.GetY() + j >= 0 && tile.GetY() + j < mapWidth)
                        {
                            if (map[tile.GetX() + i, tile.GetY() + j].getBuilding() == to)
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
                        foreach (Tile tile in current.GetTiles())
                        {
                            if (!(Math.Abs(i) == Math.Abs(j)) && tile.GetX() + i >= 0 && tile.GetX() + i < mapHeight && tile.GetY() + j >= 0 && tile.GetY() + j < mapWidth)
                            {
                                Building.Building building = map[tile.GetX() + i, tile.GetY() + j].getBuilding();
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

        public int getHappiness()
        {
            int happiness = 0;
            foreach (Citizen i in citizens)
            {
                happiness += i.calcHappiness();
            }
            return happiness;
        }

        public bool newZone(int x, int y, ZoneType zoneType)
        {
            if (map[x, y].SetZone(zoneType))
            {
                if (zoneType == ZoneType.Industrial)
                    avaibleIndustrials.Add((Industrial)map[x, y].getBuilding());
                else if (zoneType == ZoneType.Store)
                    avaibleStores.Add((Store)map[x, y].getBuilding());
                Finances.addExpenses("Built a " + map[x, y].ToString(), map[x, y].getCost(), date);
                OnGameChanged();
                return true;
            } else
            {
                return false;
            }
        }

        public bool BuildBuilding(Building.Building building)
        {
            if (building == null)
                return false;
            
            bool freeZone = true;
            foreach (Tile tile in building.GetTiles())
            {
                if (tile.GetX() >= 0 && tile.GetX() < mapHeight && tile.GetY() >= 0 && tile.GetY() < mapWidth)
                {
                    Zone zone = map[tile.GetX(), tile.GetY()];
                    freeZone = freeZone && (zone.ZoneType == ZoneType.General && zone.getBuilding() == null);
                } else
                {
                    freeZone = false;
                    break;
                }
            }
            if (freeZone)
            {
                Finances.addExpenses("Built a ", building.GetCost(), date);
                foreach (Tile tile in building.GetTiles())
                {
                    map[tile.GetX(), tile.GetY()].BuildBuilding(building);
                }
                OnGameChanged();
                return true;
            } else
            {
                OnGameChanged();
                return false;
            }
        }

        public void AdvanceTime()
        {
            DateTime previous_date = date;
            date = date.AddDays(1);
            if (date.Year > previous_date.Year)
            {
                CollectingTaxes();
            }
            PeopleMoveIn();
            OnTimeAdvanced();
        }

        private void PeopleMoveIn(Citizen citizen = null!)
        {
            if (peopleAtStart > 0 || citizen != null)
            {
                Zone houseZone = null!;
                Residental house = null!;
                if (avaibleHouses.Count == 0)
                {
                    foreach (Zone zone in map)
                    {
                        if (zone.ZoneType == ZoneType.Residental && !zone.Occupied)
                        {
                            houseZone = zone;
                            break;
                        }
                    }
                } else
                {
                    foreach (Residental houses in avaibleHouses)
                    {
                        if (houses.FreeSpace())
                        {
                            house = houses;
                            break;
                        } else
                        {
                            avaibleHouses.Remove(houses);
                        }
                    }
                }
                int inStores = 0;
                int inIndustrial = 0;
                foreach (Store store in avaibleStores)
                {
                    inStores += store.GetWorkers();
                }
                foreach (Industrial industrial in avaibleIndustrials)
                {
                    inIndustrial += industrial.GetWorkers();
                }
                if (houseZone != null)
                {
                    house = new Residental(new List<Tile> { new Tile(houseZone.X, houseZone.Y) });
                }
                if (house != null && inIndustrial <= inStores)
                {
                    Industrial building = null!;
                    int minDist = int.MaxValue;
                    foreach (Industrial industrial in avaibleIndustrials)
                    {
                        int dist = calcDistance(house, industrial);
                        if (dist != -1 && dist < minDist && industrial.FreeSpace())
                        {
                            minDist = dist;
                            building = industrial;
                        }
                        else if (building != null && !building.FreeSpace())
                            avaibleIndustrials.Remove(building);
                    }
                    if (building != null)
                    {
                        if (houseZone != null)
                        {
                            houseZone.Occupied = true;
                            houseZone.BuildBuilding();
                            house = (Residental)houseZone.getBuilding();
                            avaibleHouses.Add(house);
                        }
                        if (citizen == null)
                        {
                            citizen = Citizen.ReGen(house);
                            citizens.Add(citizen);
                            peopleAtStart--;
                        } else
                            citizen.SetHome(house);
                        house.MoveIn();
                        foreach (Tile tile in house.GetTiles())
                        {
                            map[tile.GetX(), tile.GetY()].addCitizensHome(citizen);
                        }
                        building.NewWorker();
                        citizen.SetWorkPlace(building);
                    }
                } else if (house != null)
                {
                    Store building = null!;
                    int minDist = int.MaxValue;
                    foreach (Store store in avaibleStores)
                    {
                        int dist = calcDistance(house, store);
                        if (dist != -1 && dist < minDist && store.FreeSpace())
                        {
                            minDist = dist;
                            building = store;
                        } else if (building != null &&!building.FreeSpace())
                            avaibleStores.Remove(building);
                    }
                    if (building != null)
                    {
                        if (houseZone != null)
                        {
                            houseZone.Occupied = true;
                            houseZone.BuildBuilding();
                            house = (Residental)houseZone.getBuilding();
                            avaibleHouses.Add(house);
                        }
                        if (citizen == null)
                        {
                            citizen = Citizen.ReGen(house);
                            citizens.Add(citizen);
                            peopleAtStart--;
                        } else
                            citizen.SetHome(house);
                        house.MoveIn();
                        foreach (Tile tile in house.GetTiles())
                        {
                            map[tile.GetX(), tile.GetY()].addCitizensHome(citizen);
                        }
                        building.NewWorker();
                        citizen.SetWorkPlace(building);
                    }
                }
            } else
            {
                int AVGhappiness = getHappiness() / citizens.Count;
                Residental house = null!;
                Zone houseZone = null!;
                if (avaibleHouses.Count == 0)
                {
                    foreach (Zone zone in map)
                    {
                        if (zone.ZoneType == ZoneType.Residental && !zone.Occupied)
                        {
                            houseZone = zone;
                            break;
                        }
                    }
                } else
                {
                    foreach (Residental houses in avaibleHouses)
                    {
                        if (houses.FreeSpace())
                        {
                            house = houses;
                            break;
                        }
                        else
                        {
                            avaibleHouses.Remove(houses);
                        }
                    }
                }
                int inStores = 0;
                int inIndustrial = 0;
                foreach (Store store in avaibleStores)
                {
                    inStores += store.GetWorkers();
                }
                foreach (Industrial industrial in avaibleIndustrials)
                {
                    inIndustrial += industrial.GetWorkers();
                }
                if (houseZone != null)
                {
                    house = new Residental(new List<Tile> { new Tile(houseZone.X, houseZone.Y) });
                }
                if (house != null && inIndustrial < inStores)
                {
                    Industrial building = null!;
                    int minDist = int.MaxValue;
                    foreach (Industrial industrial in avaibleIndustrials)
                    {
                        int dist = calcDistance(house, industrial);
                        if (dist != -1 && dist < minDist && industrial.FreeSpace())
                        {
                            minDist = dist;
                            building = industrial;
                        }
                        else if (building != null && !building.FreeSpace())
                            avaibleIndustrials.Remove(building);
                    }
                    bool found = false;
                    for (int i = 0; i < 4 && !found; i++)
                    {
                        for (int j = 0; j < 4 && !found; j++)
                        {
                            foreach (Tile tile in house.GetTiles())
                            {
                                if (map[tile.GetX() + i, tile.GetY() + j].ZoneType == ZoneType.Industrial)
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (building != null && minDist < 30 && !found && AVGhappiness > 5)
                    {
                        if (houseZone != null)
                        {
                            houseZone.Occupied = true;
                            houseZone.BuildBuilding();
                            house = (Residental)houseZone.getBuilding();
                            avaibleHouses.Add(house);
                        }
                        citizen = Citizen.ReGen(house);
                        house.MoveIn();
                        building.NewWorker();
                        citizen.SetWorkPlace(building);
                        citizens.Add(citizen);
                        foreach (Tile tile in house.GetTiles())
                        {
                            map[tile.GetX(), tile.GetY()].addCitizensHome(citizen);
                        }
                    }
                }
                else if (house != null)
                {
                    Store building = null!;
                    int minDist = int.MaxValue;
                    foreach (Store store in avaibleStores)
                    {
                        int dist = calcDistance(house, store);
                        if (dist != -1 && dist < minDist && store.FreeSpace())
                        {
                            minDist = dist;
                            building = store;
                        }
                        else if (building != null && !building.FreeSpace())
                            avaibleStores.Remove(building);
                    }
                    bool found = false;
                    for (int i = 0; i < 4 && !found; i++)
                    {
                        for (int j = 0; j < 4 && !found; j++)
                        {
                            foreach (Tile tile in house.GetTiles())
                            {
                                if (map[tile.GetX() + i, tile.GetY() + j].ZoneType == ZoneType.Industrial)
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (building != null && minDist < 30 && !found && AVGhappiness > 5)
                    {
                        if (houseZone != null)
                        {
                            houseZone.Occupied = true;
                            houseZone.BuildBuilding();
                            house = (Residental)houseZone.getBuilding();
                            avaibleHouses.Add(house);
                        }
                        citizen = Citizen.ReGen(house);
                        house.MoveIn();
                        building.NewWorker();
                        citizen.SetWorkPlace(building);
                        citizens.Add(citizen);
                        foreach (Tile tile in house.GetTiles())
                        {
                            map[tile.GetX(), tile.GetY()].addCitizensHome(citizen);
                        }
                    }
                }
            }
            OnGameChanged();
        }

        private void OnTimeAdvanced()
        {
            gameAdvanced?.Invoke(this, EventArgs.Empty);
        }

        private void OnBuildFailed()
        {
            failedBuilding?.Invoke(this, EventArgs.Empty);
        }

        private void OnConflictDemolish()
        {
            ConflictDemolish?.Invoke(this, EventArgs.Empty);
        }

        private void OnGameChanged()
        {
            gameChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}