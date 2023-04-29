using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class PoliceDepartment : Building
    {
        public PoliceDepartment(int x, int y)
        {
            PowerConsumption = 45;
            Size = (1, 1);
            cost = 60;
            X = x;
            Y = y;
        }
    }
}
