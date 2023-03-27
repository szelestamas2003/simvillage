namespace SimVillage.Model
{
    public class Zone
    {
        public int X {  get; set; }

        public int Y { get; set; }

        public bool Occupied { get; set; }

        public ZoneType ZoneType { get; private set; }

        private const int cost = 50000;

        public int getCost() { return cost; }

        public Zone(ZoneType zoneType = ZoneType.General)
        {
            ZoneType = zoneType;
        }

        public bool DownGrade()
        {
            if (ZoneType != ZoneType.General)
            {
                ZoneType = ZoneType.General;
                return true;
            }
            return false;
        }

        public bool UpGrade(ZoneType zoneType)
        {
            if (zoneType == ZoneType.General)
            {
                ZoneType = zoneType;
                return true;
            }
            return false;
        }
    }
}
