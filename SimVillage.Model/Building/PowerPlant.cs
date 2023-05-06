using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class PowerPlant : Building
    {
        int GeneratedPower = 1000;
        public PowerPlant(int x, int y)
        {
            PowerConsumption = 0;
            Size = (2, 2);
            Cost = 1000;
            X = x;
            Y = y;
        }
        public int GetGeneratedPower() {  return GeneratedPower; }
        public void SetGeneratedPower(int value) {  GeneratedPower = value; }

        public override String ToString()
        {
            return "Generated Power: " + GeneratedPower + "\nMaintenance cost: " + Cost / 100;
        }

    }
}
