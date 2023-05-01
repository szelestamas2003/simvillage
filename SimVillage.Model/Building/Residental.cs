using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    internal class Residental : Building
    {
        int MaxInhabitants;
        int Inhabitans;
        public Residental(List<Tile> tile)
        {
            SetTiles(tile);
            SetPowerConsumption(30);
            SetDensity(1);
            Size = (1, 1);
            MaxInhabitants = 6;
        }
        public int GetMaxInhabitants() { return MaxInhabitants; }
        public int GetInhabitans() { return Inhabitans; }

        public void SetMaxInhabitants(int MaxInhabitants) { this.MaxInhabitants = MaxInhabitants; }
        public void MoveOut()
        {
            Inhabitans--;
        }
        public void MoveIn()
        {
            Inhabitans++;
        }

        public bool FreeSpace()
        {
            return Inhabitans < MaxInhabitants;
        }

        public override String ToString()
        {
            return "Current inhabitants: " + Inhabitans + "\nMaximum workers: " + MaxInhabitants + "\nBuilding level " + Density + "\nPower consumption: " + PowerConsumption;

        }
    }
}
