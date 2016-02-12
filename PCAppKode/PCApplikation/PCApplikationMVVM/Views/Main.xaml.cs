using System;
using System.Collections;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PCApplikationMVVM;

namespace PCApplikationMVVM
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

        }

        private void ComponentDataGrid_OnSelectionChanged_AddToDeleteList(object sender, SelectionChangedEventArgs e)
        {
            var locator = FindResource("Locator") as ViewModelLocator;

            //If non unique, selected items is just the selected items
            if (!locator.MainViewModel.UniqeChecked)
            {
                var items = ComponentDataGrid.SelectedItems;
                locator.MainViewModel.AddToSelectedItemsList(items);
            }
            //Else select all with same same
            else
            {
                var items = new List<Component>();
                if (ComponentDataGrid.SelectedItem != null)
                {
                    var chosen = (Component) ComponentDataGrid.SelectedItem;
                    foreach (Component c in locator.MainViewModel.AllComponentList)
                    {
                        if (c.ComponentName == chosen.ComponentName)
                            items.Add(c);
                    }
                    locator.MainViewModel.AddToSelectedItemsList(items);
                }
            }
        }

	    private void SearchTextBox_OnKeyDown_Search(object sender, KeyEventArgs e)
	    {
		    if (Keyboard.IsKeyDown(Key.Enter))
		    {
			    var locator = FindResource("Locator") as ViewModelLocator;
			    locator.MainViewModel.SearchText = SearchTextBox.Text;	//databinding not working :/
			    locator.MainViewModel.Search();
		    }
	    }
    }
}
