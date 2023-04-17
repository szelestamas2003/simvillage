using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class Stadium : Building
    {
        public Stadium(List<Tile> tile)
        {
            SetTiles(tile);
            PowerConsumption = 60;
            Size = (2, 2);
            cost = 600;
        }
    }
}
