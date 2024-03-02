using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class PoliceDepartment : Building
    {
        const int Radius = 30;
        public PoliceDepartment(int x, int y)
        {
            FireChance = 5;
            IsOnFire = false;
            Health = 50;
            PowerConsumption = 45;
            Size = (1, 1);
            Cost = 600;
            X = x;
            Y = y;
        }

        public static int GetRadius() { return Radius; }

        public override String ToString()
        {
            return "Power consumption: " + PowerConsumption + "\nMaintenance cost: " + Cost/100 + "\nRadius: "+ Radius + "\nHealth: " + Health + "\n";
        }
    }
}
