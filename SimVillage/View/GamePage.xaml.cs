using SimVillage.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private Cursor Residental = CursorHelper.FromByteArray(Properties.Resources.residental_t);
        private Cursor Store = CursorHelper.FromByteArray(Properties.Resources.shop_t);
        private Cursor Police = CursorHelper.FromByteArray(Properties.Resources.police_t);
        private Cursor Forest = CursorHelper.FromByteArray(Properties.Resources.forest_t);
        private Cursor PowerGenerator = CursorHelper.FromByteArray(Properties.Resources.power_generator_t);
        private Cursor Road = CursorHelper.FromByteArray(Properties.Resources.road_t);
        private Cursor RoadH = CursorHelper.FromByteArray(Properties.Resources.road_t_h);
        private Cursor University = CursorHelper.FromByteArray(Properties.Resources.university_t);
        private Cursor PowerLine = CursorHelper.FromByteArray(Properties.Resources.power_line_t);
        private Cursor FireDepartment = CursorHelper.FromByteArray(Properties.Resources.fire_department_t);
        private Cursor School = CursorHelper.FromByteArray(Properties.Resources.elementary_school_t);
        private Cursor Industrial = CursorHelper.FromByteArray(Properties.Resources.factory_t);
        private Cursor Stadium = CursorHelper.FromByteArray(Properties.Resources.stadium_t);
        Canvas canvas = null!;
        Point StartPos { get; set; }
        TranslateTransform Translate { get; set; }
        public GamePage()
        {
            InitializeComponent();
            Translate = new TranslateTransform();
            KeyDown += new KeyEventHandler(OnButtonKeyDown);
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D)
            {
                if (Translate.X - 64 > -canvas.Width * 2 / 3 + 4 * 64)
                    Translate.X -= 64;
                else
                    Translate.X = -canvas.Width * 2 / 3 + 4 * 64;
            }
            else if (e.Key == Key.A)
            {
                if (Translate.X + 64 < 0)
                    Translate.X += 64;
                else
                    Translate.X = 0;
            }
            else if (e.Key == Key.W)
            {
                if (Translate.Y + 64 < 0)
                    Translate.Y += 64;
                else
                    Translate.Y = 0;
            }
            else if (e.Key == Key.S)
            {
                if (Translate.Y - 64 > -canvas.Height * 2 / 3 + 44)
                    Translate.Y -= 64;
                else
                    Translate.Y = -canvas.Height * 2 / 3 + 44;
            }
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
                if (Translate.X + diff.X < -canvas.Width * 2 / 3 + 4 * 64)
                    Translate.X = -canvas.Width * 2 / 3 + 4 * 64;
                else if (Translate.X + diff.X > 0)
                    Translate.X = 0;
                else
                    Translate.X += diff.X;
                if (Translate.Y + diff.Y < -canvas.Height * 2 / 3 + 44)
                    Translate.Y = -canvas.Height * 2 / 3 + 44;
                else if (Translate.Y + diff.Y > 0)
                    Translate.Y = 0;
                else
                    Translate.Y += diff.Y;
            }
            if (canvas.Cursor == Road || canvas.Cursor == RoadH)
            {
                Field field = null!;
                foreach (object children in canvas.Children)
                {
                    Field child = (Field)((ContentPresenter)children).Content;
                    
                    if (child.Left < e.GetPosition(canvas).X && child.Left > e.GetPosition(canvas).X - 64 && child.Top < e.GetPosition(canvas).Y && child.Top > e.GetPosition(canvas).Y - 64)
                    {
                        field = child;
                        break;
                    }
                }
                if (field != null && (((Field)((ContentPresenter)canvas.Children[field.X * (int)(canvas.Width / 64) + field.Y - 1]).Content).Text.StartsWith("Road") || ((Field)((ContentPresenter)canvas.Children[field.X * (int)(canvas.Width / 64) + field.Y + 1]).Content).Text.StartsWith("Road")))
                {
                    canvas.Cursor = RoadH;
                } else
                {
                    canvas.Cursor = Road;
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            canvas.ReleaseMouseCapture();
        }

        public static T FindChild<T>(DependencyObject parent, string childName)
            where T : DependencyObject
        {
            if (parent == null) return null!;

            T foundChild = null!;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T? childType = child as T;
                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);

                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild!;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            canvas = FindChild<Canvas>(container, "canvas");
            Translate.Y = -canvas.Height * 2 / 3 + 44;
            canvas.RenderTransform = Translate;
        }

        private void Options_Clicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Background == Brushes.White)
            {
                Image image = (Image)button.Content;
                string source = image.Source.ToString();
                string path = source.Substring(source.LastIndexOf(",") + 1);
                switch (path)
                {
                    case "/View/pixelarts/residental_t.png":
                        canvas.Cursor = Residental;
                        break;
                    case "/View/pixelarts/shop_t.png":
                        canvas.Cursor = Store;
                        break;
                    case "/View/pixelarts/fire_department_t.png":
                        canvas.Cursor = FireDepartment;
                        break;
                    case "/View/pixelarts/forest_t.png":
                        canvas.Cursor = Forest;
                        break;
                    case "/View/pixelarts/police_t.png":
                        canvas.Cursor = Police;
                        break;
                    case "/View/pixelarts/elementary_school_t.png":
                        canvas.Cursor = School;
                        break;
                    case "/View/pixelarts/factory_t.png":
                        canvas.Cursor = Industrial;
                        break;
                    case "/View/pixelarts/stadium_t.png":
                        canvas.Cursor = Stadium;
                        break;
                    case "/View/pixelarts/power_line_t.png":
                        canvas.Cursor = PowerLine;
                        break;
                    case "/View/pixelarts/power_generator_t.png":
                        canvas.Cursor = PowerGenerator;
                        break;
                    case "/View/pixelarts/university_t.png":
                        canvas.Cursor = University;
                        break;
                    case "/View/pixelarts/road_t.png":
                        canvas.Cursor = Road;
                        break;
                    case "/View/pixelarts/fire_t.png":
                        canvas.Cursor = Cursors.Hand;
                        break;
                }
            } else
                canvas.Cursor = Cursors.Hand;
        }
    }
}
