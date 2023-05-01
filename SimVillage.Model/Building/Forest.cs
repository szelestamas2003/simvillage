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
        int Radius = 3;
        public Forest(List<Tile> tile) {
            SetTiles(tile);
            Age = 0;
            Size = (1, 1);
            SetPowerConsumption(3);
            cost = 80;

        }

        public int GetRadius() { return Radius; }

        public void AgeUp()
        {
            Age += 1;
        }
        public int GetAge() { return Age; }

        public override String ToString()
        {
            return "Age: " + Age + "\nPower consumption: " + PowerConsumption + "\nRadius: " + Radius;
        }
        
    }
}
