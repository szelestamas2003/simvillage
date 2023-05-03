using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SimVillage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Canvas canvas = null!;
        Point StartPos { get; set; }
        TranslateTransform Translate { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Translate = new TranslateTransform();
            KeyDown += new KeyEventHandler(OnButtonKeyDown);
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D && Translate.X > -canvas.Width / 2)
                Translate.X -= 64;
            else if (e.Key == Key.A && Translate.X < 0)
                Translate.X += 64;
            else if (e.Key == Key.W && Translate.Y < 0)
                Translate.Y += 64;
            else if (e.Key == Key.S && Translate.Y > -canvas.Height / 2)
                Translate.Y -= 64;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StartPos = e.GetPosition(container);
            canvas.CaptureMouse();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (canvas.IsMouseCaptured)
            {
                Point p = e.GetPosition(container);
                Vector diff = p - StartPos;
                StartPos = p;
                if (Translate.X + diff.X < -canvas.Width / 2)
                    Translate.X = -canvas.Width / 2;
                else if (Translate.X + diff.X > 0)
                    Translate.X = 0;
                else
                    Translate.X += diff.X;
                if (Translate.Y + diff.Y < -canvas.Height / 2)
                    Translate.Y = -canvas.Height / 2;
                else if (Translate.Y + diff.Y > 0)
                    Translate.Y = 0;
                else
                    Translate.Y += diff.Y;
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            canvas.ReleaseMouseCapture();
        }

        public static T FindChild<T>(DependencyObject parent, string childName)
            where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null!;

            T foundChild = null!;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T? childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild!;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            canvas = FindChild<Canvas>(container, "canvas");
            Translate.Y = -canvas.Height / 2;
            canvas.RenderTransform = Translate;
        }
    }
}
