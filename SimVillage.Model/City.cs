namespace SimVillage.Model
{
    public class City
    {
        private const int mapWidth = 60;

        private const int mapHeight = 30;

        private readonly string cityName;

        private DateTime date = new DateTime(2000, 1, 1);

        private Persistence dataAccess;

        private Finances Finances;

        private Zone[,] Zones;

        private List<Citizen> Citizens = null!;

        public EventHandler? gameAdvanced;

        public EventHandler? failedBuilding;

        public City(Persistence dataAccess, string name)
        {
            this.dataAccess = dataAccess;
            cityName = name;
            Finances = new Finances(5000);
            Citizens = new List<Citizen>();

            Zones = new Zone[mapHeight, mapWidth];
            
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    Zones[i, j] = new Zone
                    {
                        X = i,
                        Y = j,
                        Occupied = false
                    };
                }
            }
        }

        public string getName() { return cityName; }

        public DateTime getDate() { return date; }

        public void demolishZone(int x, int y)
        {
            if (Zones[x, y].DownGrade())
            {
                Finances.addIncome("Demolished a " + Zones[x, y].ToString(), Zones[x, y].getCost() / 2, date);
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
            if (Zones[x, y].UpGrade(zoneType))
            {
                Finances.addExpenses("Built a " + Zones[x, y].ToString(), Zones[x, y].getCost(), date);
            } else
            {
                OnBuildFailed();
            }
        }

        public void AdvanceTime()
        {
            DateTime previous_date = date;
            date.AddDays(1);
            if (date.Year > previous_date.Year)
            {

            }
            OnTimeAdvanced();
        }

        private void OnTimeAdvanced()
        {
            gameAdvanced?.Invoke(this, EventArgs.Empty);
        }

        private void OnBuildFailed()
        {
            failedBuilding?.Invoke(this, EventArgs.Empty);
        }
    }
}