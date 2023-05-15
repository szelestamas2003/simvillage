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
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public Finances Finances { get; set; } = null!;
        public List<Citizen> Citizens { get; set; } = null!;
        public List<List<Zone>> Zones { get; set; } = null!;
    }
}
