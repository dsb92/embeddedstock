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
using System.Windows.Shapes;

namespace PCApplikationMVVM.Views
{
    /// <summary>
    /// Interaction logic for ChangeComponent.xaml
    /// </summary>
    public partial class ChangeComponent : Window
    {
        public ChangeComponent()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick_Save(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
