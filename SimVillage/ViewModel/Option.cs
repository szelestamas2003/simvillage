namespace SimVillage.ViewModel
{
    public class Option : ViewModelBase
    {
        private string text = string.Empty;

        private bool isClicked = false;

        public int Number { get; set; }

        public bool IsClicked
        {
            get
            {
                return isClicked;
            }
            set
            {
                if (value != isClicked)
                {
                    isClicked = value;
                    OnPropertyChanged(nameof(IsClicked));
                }
            }
        }

        public string Text
        {
            get { return text; }
            set
            {
                if (value != text)
                {
                    text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        public DelegateCommand? Clicked { get; set; }
    }
}
