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
        public Store(List<Tile> tile)
        {
            SetTiles(tile);
            SetPowerConsumption(30);
            SetDensity(1);
            MaxWorkers = 10;
            Size = (1, 1);
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
            return Workers < MaxWorkers;
        }

    }
}
