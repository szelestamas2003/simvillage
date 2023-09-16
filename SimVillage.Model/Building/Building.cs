using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public abstract class Building
    {
        protected (int, int) Size;
        public bool Occupied { get; set; }
        public int Density { get; set; }
        public int Cost { get; protected set; }
        public int PowerConsumption { get; protected set; }
        public bool IsPowered { get; set; }
        public bool IsAccessible { get; set; }
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public int FireChance { get; protected set; }
        public bool IsOnFire { get; set; }
        public int Health { get; set; }

        public Building((int, int) size, bool occupied, int density, int powerConsumption, int x, int y)
        {
            Size = size;
            Occupied = occupied;
            Density = density;
            PowerConsumption = powerConsumption;
            IsAccessible = false;
            IsPowered = false;
            X = x;
            Y = y;
        }
        public Building() { }

        public (int,int) GetSize() { return Size; }
    }
}
