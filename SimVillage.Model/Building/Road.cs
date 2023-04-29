using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class Road : Building
    {
        public Road(int x, int y)
        {
            Size = (1, 1);
            PowerConsumption = 4;
            cost = 40;
            IsAccessible = true;
            X = x;
            Y = y;
        }
    }
}
