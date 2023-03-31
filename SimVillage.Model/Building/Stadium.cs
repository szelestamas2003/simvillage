using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class Stadium : Building
    {
        public Stadium(int x, int y)
        {
            PowerConsumption = 60;
            Size = (2, 2);
            Cost = 2000;
            X = x;
            Y = y;
        }
    }
}
