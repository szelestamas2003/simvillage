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
            PowerConsumption = 4;
            Size = (1, 1);
            X = x;
            Y = y;
        }
    }
}
