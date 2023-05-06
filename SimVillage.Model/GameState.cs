using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimVillage.Model;

namespace SimVillage.Persistence
{
    public class GameState
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public Finances Finances { get; set; }
        public List<Citizen> Citizens { get; set; }
        public List<List<Zone>> Zones { get; set; }
    }
}
