using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class Residental : Building
    {
        public int MaxInhabitants { get; set; }
        public int Inhabitans { get; set; }
        public Residental(int x, int y)
        {
            PowerConsumption = 30;
            Density = 1;
            Size = (1, 1);
            MaxInhabitants = 6;
            X = x;
            Y = y;
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
