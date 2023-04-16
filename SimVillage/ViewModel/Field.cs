namespace SimVillage.ViewModel
{
    public class Field : ViewModelBase
    {
        private string text = string.Empty;

        public int X { get; set; }
        public string Text
        {
            get { return text; }
            set
            {
                if (text != value)
                {
                    text = value;
                    OnPropertyChanged();
                }  
            }
        }

        public int Y { get; set; }
        public int Number { get; set; }
        public DelegateCommand? Clicked { get; set; }
    }
}
