﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class PowerLine : Building
    {
        public PowerLine()
        {
            SetPowerConsumption(0);
            Size = (1, 1);
            cost = 20;
        }

        public override String ToString()
        {
            return "";
        }
    }
}
