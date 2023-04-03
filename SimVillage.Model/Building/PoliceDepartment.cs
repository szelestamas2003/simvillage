using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class PoliceDepartment : Building
    {
        public PoliceDepartment (List<Tile> tile)
        {
            SetTiles(tile);
            SetPowerConsumption(45);
        }
    }
}
