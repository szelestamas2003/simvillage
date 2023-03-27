using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    abstract class Building
    {
        List<Tile> Tiles;
        (int, int) Size;
        bool Occupied;
        int Density;
        int PowerConsumption;


        public Building(List<Tile> tiles, (int, int) size, bool occupied, int density, int powerConsumption)
        {
            Tiles = tiles;
            Size = size;
            Occupied = occupied;
            Density = density;
            PowerConsumption = powerConsumption;
        }
        public Building() { }

        public List<Tile> GetTiles() { return Tiles; }
        public (int,int) GetSize() { return Size; }
        public int GetPowerConsumption() {  return PowerConsumption; }
        public int GetDensity() { return Density;}
        public bool GetOccupied() {  return Occupied;}

        
        public void SetSize(int size1,int size2) { Size = (size1, size2); }
        public void SetDensity(int density) { Density = density;}
        public void SetOccupied(bool occupied) {  Occupied = occupied;}
        public void SetPowerConsumption(int powerconsumption) { PowerConsumption = powerconsumption; }
        public void SetTiles(List<Tile> tiles) 
        { 
            Tiles = tiles;
            foreach(Tile t in Tiles)
            {
                t.SetBulding(this);
            }
        }
        public void Demolish()
        {
            Density = 0;
            Occupied = false;
            PowerConsumption = 0;
            Size = (0, 0);
            foreach(Tile tile in Tiles)
            {
                tile.SetBulding(null);
            }
            Tiles = null;
        }

        
    }
}
