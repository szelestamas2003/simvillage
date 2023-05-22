using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class Road : Building
    {
        public Road(int x, int y)
        {
            FireChance = 0;
            IsOnFire = false;
            Health = 50;
            Size = (1, 1);
            PowerConsumption = 4;
            Cost = 40;
            IsAccessible = true;
            X = x;
            Y = y;
        }

        public override String ToString()
        {
            return "Power consumption: " + PowerConsumption + "\nMaintenance cost " + Cost / 100;
        }
    }
}
