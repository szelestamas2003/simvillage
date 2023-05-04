using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class PoliceDepartment : Building
    {
        int Radius = 40;
        public PoliceDepartment(int x, int y)
        {
            PowerConsumption = 45;
            Size = (1, 1);
            cost = 600;
            X = x;
            Y = y;
        }

        public int GetRadius() { return Radius; }

        public override String ToString()
        {
            return "Power consumption: " + PowerConsumption + "\nMaintenance cost: " + cost/100 + "\nRadius: "+ Radius;
        }
    }
}
