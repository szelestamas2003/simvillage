using SimVillage.Model.Building;

namespace SimVillage.Model
{
    public class Zone
    {
        public int X {  get; set; }

        public int Y { get; set; }

        public bool Occupied { get; set; }

        public ZoneType ZoneType { get; private set; }

        private Building.Building building = null!;

        private List<Citizen> citizens = null!;

        private const int cost = 400;

        public Building.Building getBuilding() { return building; }

        public int getCost() { return cost; }

        public int getPeople()
        {
            return citizens.Count;
        }

        public void addCitizensHome(Citizen person)
        {
            citizens.Add(person);
        }

        public void RemoveCitizenFromWorkPlace(Citizen person)
        {
            citizens.Remove(person);
        }

        public void AddCitizensWorkPlace(Citizen person)
        {
            citizens.Add(person);
        }

        public void MoveOutFromHome(Citizen person)
        {
            citizens.Remove(person);
        }

        public Zone(ZoneType zoneType = ZoneType.General)
        {
            ZoneType = zoneType;
            citizens = new List<Citizen>();
        }

        public void BuildBuilding(Building.Building? building = null)
        {
            Occupied = true;
            if (building == null)
            {
                switch (ZoneType)
                {
                    case ZoneType.Residental:
                        this.building = new Residental(new List<Tile> { new Tile(X, Y)});
                        break;
                    case ZoneType.Industrial:
                        this.building = new Industrial(new List<Tile> { new Tile(X, Y) });
                        break;
                    case ZoneType.Store:
                        this.building = new Store(new List<Tile> { new Tile(X, Y) });
                        break;
                    default:
                        throw new ArgumentNullException();
                }
            } else
            {
                if (ZoneType == ZoneType.General)
                    this.building = building;
                else
                    throw new ArgumentOutOfRangeException();
            }
        }

        public int getHappiness()
        {
            int happiness = 0;
            foreach (Citizen i in citizens)
            {
                happiness += i.calcHappiness();
            }
            return citizens.Count == 0 ? 0 : happiness / citizens.Count;
        }

        public bool DowngradeZone()
        {
            if (ZoneType != ZoneType.General)
            {
                ZoneType = ZoneType.General;
                building = null!;
                Occupied = false;
                citizens.Clear();
                return true;
            } else if (building != null)
            {
                building = null!;
                Occupied = false;
                return true;
            }
            return false;
        }

        public bool SetZone(ZoneType zoneType)
        {
            if (ZoneType == ZoneType.General && building == null)
            {
                ZoneType = zoneType;
                if (ZoneType != ZoneType.Residental)
                    BuildBuilding();
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return ZoneType.ToString() + " Zone" + (building != null ? " with " + building.GetType().Name + " on it" : "");
        }
    }
}
