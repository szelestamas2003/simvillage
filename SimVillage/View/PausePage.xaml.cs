using SimVillage.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimVillage
{
    /// <summary>
    /// Interaction logic for PausePage.xaml
    /// </summary>
    public partial class PausePage : Page
    {
        public PausePage()
        {
            InitializeComponent();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                ((SimVillageViewModel)DataContext).NewGameCommand.Execute(CityName.Text);
            }
        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            if (newStackPanel.Visibility == Visibility.Collapsed)
                newStackPanel.Visibility = Visibility.Visible;
            else
                newStackPanel.Visibility = Visibility.Collapsed;
        }
    }
}
