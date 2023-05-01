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
        public PoliceDepartment (List<Tile> tile)
        {
            SetTiles(tile);
            SetPowerConsumption(45);
            Size = (1, 1);
            cost = 600;
        }

        public String ToString()
        {
            return "Power consmuption: " + PowerConsumption + " Maintenance cost: " + cost/100 + " Radius: "+ Radius;

        }
    }
}
