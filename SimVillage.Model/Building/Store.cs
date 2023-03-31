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
        }
        public int GetMaxWorkers() { return MaxWorkers; }
        public int GetWorkers() { return Workers; }

        public void SetMaxWrokers(int MaxWorkers) { this.MaxWorkers = MaxWorkers; }
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
            if (Workers == MaxWorkers)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
