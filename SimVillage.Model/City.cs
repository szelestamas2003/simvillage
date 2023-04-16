using SimVillage.Model.Building;

namespace SimVillage.Model
{
    public class City
    {
        private const int mapWidth = 60;

        private const int mapHeight = 30;

        private int peopleAtStart = 30;

        private readonly string cityName;

        private DateTime date = new DateTime(2000, 1, 1);

        private Persistence dataAccess;

        private Finances Finances;

        private static Zone[,] map = null!;

        public int Width() { return mapWidth;}

        public int Height() { return mapHeight;}

        public Building.Tile[,] tiles = null!;

        private List<Citizen> citizens = null!;

        private List<Industrial> avaibleIndustrials = null!;

        private List<Store> avaibleStores = null!;

        private List<Residental> avaibleHouses = null!;

        public EventHandler? gameAdvanced;

        public EventHandler? failedBuilding;

        public EventHandler? gameChanged;

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
            tiles = new Tile[mapWidth, mapHeight];
            for(int i = 0; i < mapWidth; i++)
            {
                for(int j = 0; j < mapHeight; j++)
                {
                    tiles[i, j] = new Tile(i, j);
                }
            }
            

            map = new Zone[mapWidth, mapHeight];

            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    map[i, j] = new Zone
                    {
                        X = i,
                        Y = j,
                        Occupied = false
                    };
                }
            }
        }

        public string Name { get { return cityName; } }

        public DateTime Date { get { return date; } }

        public Zone[,] Map { get { return map; } }

        public void demolishZone(int x, int y)
        {
            Building.Building building = map[x, y].getBuilding();
            if (building != null)
            {
                foreach (Tile tile in building.GetTiles())
                {
                    if (map[tile.GetX(), tile.GetY()].DowngradeZone())
                    {
                        Finances.addIncome("Demolished a " + map[x, y].ToString(), map[x, y].getCost() / 2, date);
                    }
                }
            } else
            {
                if (map[x, y].DowngradeZone())
                {
                    Finances.addIncome("Demolished a " + map[x, y].ToString(), map[x, y].getCost() / 2, date);
                }
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

        public void newZone(int x, int y, ZoneType zoneType)
        {
            if (map[x, y].SetZone(zoneType))
            {
                if (zoneType == ZoneType.Industrial)
                    avaibleIndustrials.Add((Industrial)map[x, y].getBuilding());
                else if (zoneType == ZoneType.Store)
                    avaibleStores.Add((Store)map[x, y].getBuilding());
                Finances.addExpenses("Built a " + map[x, y].ToString(), map[x, y].getCost(), date);
                OnGameChanged();
            } else
            {
                OnBuildFailed();
            }
        }

       

        public void NewBuilding(int x, int y, Building.Building building)
        {

        }

        public void BuildBuilding(Building.Building building)
        {
            if (building == null)
                return;

            bool freeZone = true;
            foreach (Tile tile in building.GetTiles())
            {
                Zone zone = map[tile.GetX(), tile.GetY()];
                freeZone = freeZone && (zone.ZoneType == ZoneType.General && zone.getBuilding() == null);
            }
            if (freeZone)
            {
                foreach (Tile tile in building.GetTiles())
                {
                    map[tile.GetX(), tile.GetY()].BuildBuilding(building);
                }
            } else
            {
                OnBuildFailed();
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

        private void PeopleMoveIn()
        {
            if (peopleAtStart > 0)
            {
                Citizen citizen = null!;
                Residental house = null!;
                if (avaibleHouses.Count == 0)
                {
                    foreach (Zone zone in map)
                    {
                        if (zone.ZoneType == ZoneType.Residental && !zone.Occupied)
                        {
                            zone.Occupied = true;
                            zone.BuildBuilding();
                            house = (Residental)zone.getBuilding();
                            avaibleHouses.Add(house);
                            break;
                        }
                    }
                } else
                {
                    foreach (Residental houses in avaibleHouses)
                    {
                        if (houses.FreeSpace())
                        {
                            citizen = Citizen.ReGen(houses);
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
                if (inIndustrial <= inStores)
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
                        citizen = Citizen.ReGen(house);
                        foreach (Tile tile in house.GetTiles())
                        {
                            map[tile.GetX(), tile.GetY()].addCitizen(citizen);
                        }
                        citizens.Add(citizen);
                        peopleAtStart--;
                        building.NewWorker();
                        citizen.SetWorkPlace(building);
                    }
                } else
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
                        } else if (!building.FreeSpace())
                            avaibleStores.Remove(building);
                    }
                    if (building != null)
                    {
                        citizen = Citizen.ReGen(house);
                        foreach (Tile tile in house.GetTiles())
                        {
                            map[tile.GetX(), tile.GetY()].addCitizen(citizen);
                        }
                        citizens.Add(citizen);
                        peopleAtStart--;
                        building.NewWorker();
                        citizen.SetWorkPlace(building);
                    }
                }
            } else
            {
                int AVGhappiness = getHappiness() / citizens.Count;
                Residental house = null!;
                if (avaibleHouses.Count == 0)
                {
                    foreach (Zone zone in map)
                    {
                        if (zone.ZoneType == ZoneType.Residental && !zone.Occupied)
                        {
                            zone.Occupied = true;
                            zone.BuildBuilding();
                            house = (Residental)zone.getBuilding();
                            avaibleHouses.Add(house);
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
                if (inIndustrial < inStores)
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
                        else if (!building.FreeSpace())
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
                        Citizen citizen = Citizen.ReGen(house);
                        building.NewWorker();
                        citizen.SetWorkPlace(building);
                        citizens.Add(citizen);
                        foreach (Tile tile in house.GetTiles())
                        {
                            map[tile.GetX(), tile.GetY()].addCitizen(citizen);
                        }
                    }
                }
                else
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
                        else if (!building.FreeSpace())
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
                        Citizen citizen = Citizen.ReGen(house);
                        building.NewWorker();
                        citizen.SetWorkPlace(building);
                        citizens.Add(citizen);
                        foreach (Tile tile in house.GetTiles())
                        {
                            map[tile.GetX(), tile.GetY()].addCitizen(citizen);
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

        private void OnGameChanged()
        {
            gameChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}