using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class Store : Building
    {
        int MaxWorkers;
        int Workers;
        public Store()
        {
            PowerConsumption = 30;
            Density = 1;
            MaxWorkers = 10;
            Size = (1, 1);
        }
        public int GetMaxWorkers() { return MaxWorkers; }
        public int GetWorkers() { return Workers; }

        public void SetMaxWorkers(int MaxWorkers) { this.MaxWorkers = MaxWorkers; }
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
