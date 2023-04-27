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
        public Forest(List<Tile> tile) {
            SetTiles(tile);
            Age = 0;
            Size = (1, 1);
            SetPowerConsumption(3);
            cost = 80;
            IsAccessible = true;
        }

        public void SetAge(int age)
        {
            Age = age;
        }

        public void AgeUp()
        {
            Age += 1;
        }
        public int GetAge() { return Age; }

        public String ToString()
        {
            return "Forest: Age: " + Age + " Power consumption: " + PowerConsumption;
        }
        
    }
}
