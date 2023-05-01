using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    internal class PowerPlant : Building
    {
        int GeneratedPower = 1000;
        public PowerPlant(List<Tile> tile)
        {
            SetTiles(tile);
            SetPowerConsumption(0);
        }
        public int GetGeneratedPower() {  return GeneratedPower; }
        public void SetGeneratedPower(int value) {  GeneratedPower = value; }

    }
}
