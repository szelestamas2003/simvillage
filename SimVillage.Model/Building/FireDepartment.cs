﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class FireDepartment : Building
    {
        bool UnitAvailable;
        public FireDepartment(int x, int y)
        {
            PowerConsumption = 45;
            Size = (1, 1);
            cost = 500;
            X = x;
            Y = y;
        }
        public bool IsAvailable() { return  UnitAvailable; }
        public void SendUnit()
        {
            UnitAvailable = false;
        }
        public void UnitArrive()
        {
            UnitAvailable = true;
        }
        public override String ToString()
        {
            return "Fire Department: Power consmuption: " + PowerConsumption + "Unit available: " + UnitAvailable;
        }
    }
}
