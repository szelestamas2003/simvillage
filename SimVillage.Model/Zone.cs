﻿using SimVillage.Model.Building;

namespace SimVillage.Model
{
    public class Zone
    {
        public int X {  get; set; }

        public int Y { get; set; }

        public bool Occupied { get; set; }

        public ZoneType ZoneType { get; private set; }

        public Building.Building Building { get; set; } = null!;

        public List<Citizen> Citizens { get; set; }

        public string Info { get { return "Happiness: " + GetHappiness(); } }

        private const int cost = 400;
        public int GetCost() { return cost; }

        public void AddCitizenToZone(Citizen person)
        {
            Citizens.Add(person);
        }

        public void RemoveCitizenFromZone(Citizen person)
        {
            Citizens.Remove(person);
        }

        public Zone(ZoneType zoneType = ZoneType.General)
        {
            ZoneType = zoneType;
            Citizens = new List<Citizen>();
        }

        public void BuildBuilding(Building.Building? building = null)
        {
            Occupied = true;
            if (building == null)
            {
                switch (ZoneType)
                {
                    case ZoneType.Residental:
                        Building = new Residental(X, Y);
                        break;
                    case ZoneType.Industrial:
                        Building = new Industrial(X, Y);
                        break;
                    case ZoneType.Store:
                        Building = new Store(X, Y);
                        break;
                    default:
                        throw new ArgumentNullException();
                }
            } else
            {
                if (ZoneType == ZoneType.General)
                    Building = building;
                else
                    throw new ArgumentOutOfRangeException();
            }
        }

        public int GetHappiness()
        {
            int happiness = 0;
            foreach (Citizen c in Citizens)
            {
                happiness += c.Happiness;
            }
            if (Citizens.Count > 0)
            {
                return happiness / Citizens.Count;
            }
            else
            {
                return 0;
            }
        }

        public bool DowngradeZone()
        {
            if (ZoneType != ZoneType.General)
            {
                ZoneType = ZoneType.General;
                Building = null!;
                Occupied = false;
                Citizens.Clear();
                return true;
            } else if (Building != null)
            {
                Building = null!;
                Occupied = false;
                return true;
            }
            return false;
        }

        public bool SetZone(ZoneType zoneType)
        {
            if (ZoneType == ZoneType.General && Building == null)
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
            return ZoneType.ToString() + " Zone" + (Building != null ? " with " + Building.GetType().Name + " on it" : "");
        }
    }
}
