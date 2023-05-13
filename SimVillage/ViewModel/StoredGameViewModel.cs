using System;

namespace SimVillage.ViewModel
{
    public class StoredGameViewModel : ViewModelBase
    {
        public int SlotNumber { get; set; }
        public string Slot { get; set; }
        public string Name { get; set; }
        public string Modified { get; set; }
        public DelegateCommand? SlotClickedCommand { get; set; }
    }
}
