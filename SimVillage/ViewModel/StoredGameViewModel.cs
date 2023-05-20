using System;

namespace SimVillage.ViewModel
{
    public class StoredGameViewModel : ViewModelBase
    {
        public int SlotNumber { get; set; }
        public string Slot { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Modified { get; set; } = string.Empty;
        public DelegateCommand? SlotClickedCommand { get; set; }
    }
}
