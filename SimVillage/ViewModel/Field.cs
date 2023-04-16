namespace SimVillage.ViewModel
{
    public class Field : ViewModelBase
    {
        public int X { get; set; }
        public string Text { get; set; }
        public int Y { get; set; }
        public int Number { get; set; }
        public DelegateCommand? Clicked { get; set; }
    }
}
