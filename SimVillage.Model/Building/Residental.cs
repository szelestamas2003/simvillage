﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class Residental : Building
    {
        int MaxInhabitants;
        int Inhabitans;
        public Residental()
        {
            SetPowerConsumption(30);
            SetDensity(1);
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
            if (Inhabitans == MaxInhabitants)
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
