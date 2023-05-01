using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model.Building
{
    public class FireDepartment : Building
    {
        bool UnitAvailable;
        int Radius = 40;
        public FireDepartment(List<Tile> tile)
        {
            SetTiles(tile);
            SetPowerConsumption(45);
            Size = (1, 1);
            cost = 500;
        }
        public bool IsAvailable() { return  UnitAvailable; }
        public void SendUnit()
        {
            UnitAvailable = false;
        }
        public void UnitArrive()
        {
            UnitAvailable = true;
        }

        public int GetRadius() { return Radius; }
        public override String ToString()
        {
            return "Power consumption: " + PowerConsumption + "\nUnit available: " + UnitAvailable + "\nMaintenance cost: "+ cost/100 + "\nRadius: "+ Radius;
        }
    }
}
