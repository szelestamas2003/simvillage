using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    internal class PowerLine : Building
    {
        public PowerLine(List<Tile> tile)
        {
            SetTiles(tile);
            SetPowerConsumption(0);
        }
    }
}
