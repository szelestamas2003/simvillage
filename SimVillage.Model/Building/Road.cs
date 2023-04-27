using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class Road : Building
    {
        public Road(List<Tile> tile)
        {
            SetTiles(tile);
            Size = (1, 1);
            SetPowerConsumption(4);
            cost = 40;
            IsAccessible = true;
        }
    }
}
