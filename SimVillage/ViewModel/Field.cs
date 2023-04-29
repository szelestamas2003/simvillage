namespace SimVillage.ViewModel
{
    public class Field : ViewModelBase
    {
        private string text = string.Empty;

        private string name = string.Empty;

        private string citizenCount = string.Empty;

        private string happiness = string.Empty;

        public bool IsXGtThan25 { get { return X > 25; } }

        public int X { get; set; }

        private bool isclicked = false;

        public string Happiness
        {
            get { return happiness; }
            set
            {
                if (happiness != value)
                {
                    happiness = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CitizenCount
        {
            get { return citizenCount; }
            set
            {
                if (citizenCount != value)
                {
                    citizenCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsClicked
        {
            get { return isclicked; }
            set
            {
                if (isclicked != value)
                {
                    isclicked = value;
                    OnPropertyChanged();
                }
            }
        }
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
        public DelegateCommand? UpgradeCommand { get; set; }
    }
}
