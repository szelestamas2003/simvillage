using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.Model
{
    public class StoredGame
    {
        public int Slot { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Modified { get; set; }
    }
}
