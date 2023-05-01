using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    internal class Tile
    {
        private int X;
        private int Y;
        private Building Building;

        public Tile(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            Building = null!;
        }
        public int GetX()
        {
            return X; 
        }
        public int GetY()
        {
            return Y;
        }
        public void SetBulding(Building B)
        {
            Building = B;
        }
        public Building GetBuilding() { return Building; }
    }
}
