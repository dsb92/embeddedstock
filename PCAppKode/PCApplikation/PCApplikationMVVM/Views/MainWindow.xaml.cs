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

namespace PCApplikation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Component> list_ = new List<Component>();

        public MainWindow()
        {
            InitializeComponent();

            var path = System.Environment.CurrentDirectory;
            path = System.IO.Directory.GetParent((System.IO.Directory.GetParent(path).ToString())).ToString();
            MessageBox.Show(path);

            //Create Image
            var img = new Image();

            //Create image source
            var imgSource = new BitmapImage();
            imgSource.BeginInit();
            imgSource.UriSource = new Uri(path + @"\ihalogo.jpg");
            imgSource.EndInit();

            img.Source = imgSource;

            for (int i = 0; i < 10; i++)
            {
                list_.Add(new Component());
            }

            ComponentDataGrid.ItemsSource = list_;
        }


        #region Events
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Application.Current.MainWindow.ActualHeight > 150)
            {
                ComponentDataGrid.MaxHeight = Application.Current.MainWindow.ActualHeight - 150;
            }
        }
  
        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
        }

        private void ComponentDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var component = (ComponentDataGrid.SelectedItem as Component);
            if (component != null)
                ComponentImage.Source = component.Picture.Source;
        }

        #endregion




    }
}
