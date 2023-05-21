namespace SimVillage.ViewModel
{
    public class Field : ViewModelBase
    {
        private string text = string.Empty;

        private string name = string.Empty;

        private string info = string.Empty;

        public bool IsXGtThan25 { get { return X > 25; } }

        public int X { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }

        private bool isclicked = false;
        private bool fire = false;

        public string Info
        {
            get { return info; }
            set
            {
                if (info != value)
                {
                    info = value;
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

        public bool Fire
        {
            get { return fire; }
            set
            {
                if(fire != value)
                {
                    fire = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Y { get; set; }
        public int Number { get; set; }
        public DelegateCommand? Clicked { get; set; }
        public DelegateCommand? UpgradeCommand { get; set; }

        public DelegateCommand? ClearFireCommand { get; set; }
    }
}
