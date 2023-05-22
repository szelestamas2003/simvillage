using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class Stadium : Building
    {
        int Radius = 30;
        public Stadium(int x, int y)
        {
            FireChance = 5;
            IsOnFire = false;
            Health = 250;
            PowerConsumption = 60;
            Size = (2, 2);
            Cost = 600;
            X = x;
            Y = y;
        }

        public int GetRadius() { return Radius; }

        public override String ToString()
        {
            return "Power consumption: " + PowerConsumption + "\nMaintenance cost: " + Cost / 100 + "\nRadius: " + Radius + "\nHealth: " + Health + "\n";
            
        }
    }
}
