﻿namespace SimVillage.Model.Building
{
    public class FireDepartment : Building
    {
        public bool UnitAvailable { get; set; } = true;

        const int Radius = 40;

        public FireDepartment(int x, int y)
        {
            FireChance = 0;
            IsOnFire = false;
            PowerConsumption = 45;
            Size = (1, 1);
            Cost = 500;
            X = x;
            Y = y;
        }

        public static int getRadius() { return Radius; }

        public void SendUnit()
        {
            UnitAvailable = false;
        }

        public void UnitArrive()
        {
            UnitAvailable = true;
        }

        public override String ToString()
        {
            return "Power consumption: " + PowerConsumption + "\nUnit available: " + UnitAvailable + "\nMaintenance cost: "+ Cost/100 + "\nRadius: "+ Radius;
        }
    }
}
