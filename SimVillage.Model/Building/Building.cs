using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class Building
    {
        protected (int, int) Size;
        protected bool Occupied;
        protected int Density;
        protected int PowerConsumption;
        protected int X;
        protected int Y;
        protected int Cost;


        public Building(int x, int y)
        {
            Size = (1, 1);
            Occupied = false;
            Density = 0;
            PowerConsumption = 0;
            Y = y;
            X = x;
            Cost = 0;

        }
        protected Building() { }

        public (int,int) GetSize() { return Size; }
        public int GetPowerConsumption() {  return PowerConsumption; }
        public int GetDensity() { return Density;}
        public bool GetOccupied() {  return Occupied;}
        public int getCost() { return Cost;}

        
        public void SetSize(int size1,int size2) { Size = (size1, size2); }
        public void SetDensity(int density) { Density = density;}
        public void SetOccupied(bool occupied) {  Occupied = occupied;}
        public void SetPowerConsumption(int powerconsumption) { PowerConsumption = powerconsumption; }
        public void Demolish()
        {
            Density = 0;
            Occupied = false;
            PowerConsumption = 0;
            Size = (0, 0);
        }

        
    }
}
