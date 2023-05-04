using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class Stadium : Building
    {
        public Stadium(int x, int y)
        {
            PowerConsumption = 60;
            Size = (2, 2);
            cost = 600;

        }

        public int GetRadius() { return Radius; }

        public override String ToString()
        {
            return "Power consumption: " + PowerConsumption + "\nMaintenance cost: " + cost / 100 + "\nRadius: " + Radius;
            X = x;
            Y = y;
        }
    }
}
