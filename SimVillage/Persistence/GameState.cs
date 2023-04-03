using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimVillage.Model;

namespace SimVillage.Persistence
{
    internal class GameState
    {
        public string Name;
        public DateTime Date;
        public Finances Finances;
        public List<Citizen> Citizens;
        public List<List<Zone>> Zones;
    }
}
