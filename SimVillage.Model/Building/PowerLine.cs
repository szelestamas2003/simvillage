using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class PowerLine : Building
    {
        public PowerLine(int x, int y)
        {
            PowerConsumption = 0;
            Size = (1, 1);
            cost = 20;
            IsAccessible = true;
            X = x;
            Y = y;
        }

        public override String ToString()
        {
            return "";
        }
    }
}
