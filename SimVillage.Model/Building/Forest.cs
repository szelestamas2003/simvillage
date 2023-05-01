using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class Forest : Building
    {
        int Age;
        int Radius = 10;
        public Forest(List<Tile> tile) {
            SetTiles(tile);
            Age = 0;
            Size = (1, 1);
            SetPowerConsumption(3);
            cost = 80;

        }

        public void AgeUp()
        {
            Age += 1;
        }
        public int GetAge() { return Age; }

        public String ToString()
        {
            return "Age: " + Age + " Power consumption: " + PowerConsumption + " Radius: " + Radius;
        }
        
    }
}
