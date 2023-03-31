namespace SimVillage.Model
{
    public class City
    {
        private const int mapWidth = 50;

        private const int mapHeight = 40;

        private readonly string cityName;

        private DateTime date = new DateTime(2000, 1, 1);

        private Finances Finances;

        private int yearly_income = 25;

        private Persistence dataAccess;

        private Building.Building[,] map;

        private List<Citizen> Citizens = null!;

        public EventHandler? gameAdvanced;

        public EventHandler? failedBuilding;

        public City(Persistence dataAccess, string name)
        {
            Finances = new Finances(5000000);
            this.dataAccess = dataAccess;
            cityName = name;

            map = new Building.Building[mapHeight, mapWidth];
            
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    map[i, j] = new Building.Building(i, j);
                }
            }
            map[0, 0] = new Building.Forest();
            demolishZone(0, 0);
        }

        public string getName() { return cityName; }

        public DateTime getDate() { return date; }

        public void demolishZone(int x, int y)
        {
            if (map[x, y].GetType() !=  typeof(Building.Building))
            {
                Finances.addIncome("Demolished a " + map[x, y].GetType().Name, map[x, y].getCost() / 2, date);
                map[x, y] = new Building.Building(x, y);
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

        public void newBuilding(int x, int y, Building.Building building)
        {
            if (map[x, y].GetType() == typeof(Building.Building))
            {
                map[x, y] = building;
                Finances.addExpenses("Built a " + building.GetType().Name, building.getCost(), date);
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