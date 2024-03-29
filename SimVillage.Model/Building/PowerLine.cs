﻿using System;
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
            FireChance = 0;
            IsOnFire = false;
            Health = 50;
            PowerConsumption = 0;
            Size = (1, 1);
            Cost = 20;
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
