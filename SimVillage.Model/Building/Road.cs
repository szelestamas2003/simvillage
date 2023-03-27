using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    internal class Road : Building
    {
        public Road(List<Tile> tile)
        {
            SetTiles(tile);
            SetPowerConsumption(4);
        }
    }
}
