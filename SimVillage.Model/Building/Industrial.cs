﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class Industrial : Building
    {
        public int MaxWorkers { get; set; }
        public int Workers { get; set; }
        public Industrial(int x, int y)
        {
            PowerConsumption = 30;
            Density = 1;
            Size = (1, 1);
            MaxWorkers = 10;
            X = x;
            Y = y;
        }

        public void NewWorker()
        {
            Workers++;
        }

        public void WorkerLeft()
        {
            Workers--;
        }

        public bool FreeSpace()
        {
            return Workers < MaxWorkers;
        }

        public override String ToString()
        {
            return "Current workers: " + Workers + "\nMaximum workers: " + MaxWorkers + "\nBuilding level " + Density + "\nPower consumption: " + PowerConsumption;
        }
    }
}
