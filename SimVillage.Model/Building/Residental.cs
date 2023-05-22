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
        public int Inhabitants { get; set; }
        public Residental(int x, int y)
        {
            FireChance = 2;
            IsOnFire = false;
            Health = 50;
            PowerConsumption = 30;
            Density = 1;
            Size = (1, 1);
            MaxInhabitants = 6;
            X = x;
            Y = y;
        }

        public void MoveOut()
        {
            Inhabitants--;
        }

        public void MoveIn()
        {
            Inhabitants++;
        }

        public bool FreeSpace()
        {
            return Inhabitants < MaxInhabitants;
        }

        public override String ToString()
        {
            return "Current inhabitants: " + Inhabitants + "\nMaximum inhabitants: " + MaxInhabitants + "\nBuilding level " + Density + "\nPower consumption: " + PowerConsumption + "\nHealth: " + Health + "\n";

        }
    }
}
