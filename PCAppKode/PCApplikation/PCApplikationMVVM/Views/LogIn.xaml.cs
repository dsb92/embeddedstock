using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.Expression.Interactivity.Core;

namespace PCApplikationMVVM.Views
{
    /// <summary>
    /// Interaction logic for LogIn.xaml
    /// </summary>
    public partial class LogIn : Window
    {
        public LogIn()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password == Properties.Settings.Default.Password && UserTextBox.Text == Properties.Settings.Default.UserName)
            {
                var locator = FindResource("Locator") as ViewModelLocator;
                locator.NewLogInViewModel.LogInButtonPressed();
            }
            else
            {
                MessageBox.Show("Forkert brugernavn eller adgangskode");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
