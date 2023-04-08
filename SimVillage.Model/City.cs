using SimVillage.Model.Building;
using System.Net.Http.Json;

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

        private Zone[,] map;

        private List<Citizen> Citizens = null!;

        public EventHandler? gameAdvanced;

        public EventHandler? failedBuilding;

        public EventHandler? gameChanged;

        public City(Persistence dataAccess, string name)
        {
            this.dataAccess = dataAccess;
            cityName = name;
            Finances = new Finances(5000);
            Citizens = new List<Citizen>();

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
                }
            }
        }

        public string Name { get { return cityName; } }

        public DateTime Date { get { return date; } }

        public Zone[,] Map { get { return map; } }

        public void demolishZone(int x, int y)
        {
            if (map[x, y].DowngradeZone())
            {
                Finances.addIncome("Demolished a " + map[x, y].ToString(), map[x, y].getCost() / 2, date);
                OnGameChanged();
            }
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

        private int calcDistance(Building.Building from, Building.Building to)
        {
            List<int> distances = new List<int>();
            int n = 0;
            distancesFromTo(null, from, to, distances, n);
            distances.Sort();
            return distances.Count != 0 ? distances[0] : -1;
        }

        private void distancesFromTo(Building.Building from, Building.Building current, Building.Building to, List<int> distances, int n)
        {
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
                                if (map[tile.GetX() + i, tile.GetY() + j].getBuilding() != from && map[tile.GetX() + i, tile.GetY() + j].getBuilding() != null && map[tile.GetX() + i, tile.GetY() + j].getBuilding().GetType() == typeof(Building.Road))
                                {
                                    distancesFromTo(current, map[tile.GetX() + i, tile.GetY() + j].getBuilding(), to, distances, ++n);
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
            foreach (Citizen i in Citizens)
            {
                happiness += i.calcHappiness();
            }
            return happiness;
        }

        public void newZone(int x, int y, ZoneType zoneType)
        {
            if (map[x, y].SetZone(zoneType))
            {
                Finances.addExpenses("Built a " + map[x, y].ToString(), map[x, y].getCost(), date);
                OnGameChanged();
            } else
            {
                OnBuildFailed();
            }
        }

        public void AdvanceTime()
        {
            PeopleMoveIn();
            DateTime previous_date = date;
            date.AddDays(1);
            if (date.Year > previous_date.Year)
            {
                CollectingTaxes();
            }
            OnTimeAdvanced();
        }

        private void PeopleMoveIn()
        {
            if (peopleAtStart > 0)
            {
                foreach (Zone zone in map)
                {
                    if (zone.ZoneType == ZoneType.Residental && !zone.Occupied)
                    {
                        zone.Occupied = true;
                        zone.BuildBuilding();

                    }
                }
            }
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