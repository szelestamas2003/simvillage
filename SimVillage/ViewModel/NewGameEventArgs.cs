using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimVillage.ViewModel
{
    public class NewGameEventArgs : EventArgs
    {
        public string Name { get; set; } = string.Empty;
    }
}
