using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class Store : Building
    {
        public int MaxWorkers { get; set; }
        public int Workers { get; set; }
        public Store(int x, int y)
        {
            FireChance = 5;
            IsOnFire = false;
            Health = 50;
            PowerConsumption = 30;
            Density = 1;
            MaxWorkers = 10;
            Size = (1, 1);
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
            return "Current workers: " + Workers + "\nMaximum workers: " + MaxWorkers + "\nBuilding level " + Density + "\nPower consumption: " + PowerConsumption + "\nHealth: " + Health + "\n";

        }
    }
}
