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
        public FireDepartment(List<Tile> tile)
        {
            SetTiles(tile);
            SetPowerConsumption(45);
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
    }
}
