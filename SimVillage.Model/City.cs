namespace SimVillage.Model
{
    public class City
    {
        private const int mapWidth = 50;

        private const int mapHeight = 40;

        private readonly string cityName;

        private DateTime date = new DateTime(2000, 1, 1);

        private int Budget = 5000000;

        private Persistence dataAccess;

        private Finances Finances;

        private Zone[,] Zones;

        private List<Citizen> Citizens = null!;

        public EventHandler? gameAdvanced;

        public City(Persistence dataAccess, string name)
        {
            this.dataAccess = dataAccess;
            cityName = name;
            Finances = new Finances(5000);

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
                Finances.addIncome("Demolished a " + Zones[x, y].ToString(), Zones[x, y].getCost(), date);
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

            }
        }

        public void AdvanceTime()
        {
            date.AddDays(1);
            OnTimeAdvanced();
        }

        private void OnTimeAdvanced()
        {
            gameAdvanced?.Invoke(this, EventArgs.Empty);
        }
    }
}