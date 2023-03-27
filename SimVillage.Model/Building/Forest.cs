using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    internal class Forest : Building
    {
        int Age;
        public Forest(List<Tile> tile) {
            SetTiles(tile);
            Age = 0;
            SetPowerConsumption(3);

        }

        public void AgeUp()
        {
            Age += 1;
        }
        public int GetAge() { return Age; }

        
    }
}
