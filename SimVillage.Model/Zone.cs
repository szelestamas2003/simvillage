﻿namespace SimVillage.Model
{
    public class Zone
    {
        public int X {  get; set; }

        public int Y { get; set; }

        public bool Occupied { get; set; }

        public ZoneType ZoneType { get; private set; }

        private Building.Building building = null!;

        private List<Citizen> citizens = null!;

        private const int cost = 50000;

        public int getCost() { return cost; }

        public int getPeople()
        {
            return citizens.Count;
        }

        public Zone(ZoneType zoneType = ZoneType.General)
        {
            ZoneType = zoneType;
            citizens = new List<Citizen>();
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

        public override string ToString()
        {
            return ZoneType.ToString() + " Zone" + building != null ? " with " + building.GetType().Name + " on it" : "";
        }
    }
}
