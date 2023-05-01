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
        public String ToString()
        {
            return "Power consmuption: " + PowerConsumption + "Unit available: " + UnitAvailable + " Maintenance cost: "+ cost/100 + " Radius: "+ Radius;
        }
    }
}
