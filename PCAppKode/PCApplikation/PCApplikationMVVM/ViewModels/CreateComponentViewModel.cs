using System;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;

// Toolkit namespace
using PCApplikation;
using PCApplikationMVVM.Views;
using SimpleMvvmToolkit;

namespace PCApplikationMVVM.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class CreateComponentViewModel : ViewModelBase<CreateComponentViewModel>
    {
        private Component newComponent_;

        // Default ctor
        public CreateComponentViewModel(Component newComponent)
        {
            if (newComponent != null)
                NewComponent = newComponent;
            else
                throw new ArgumentException(
                    "You need to create NewComponentViewModel with a Component model from ComponentsViewModel");
        }

        public Component NewComponent
        {
            get { return newComponent_; }
            set
            {
                newComponent_ = value;
                NotifyPropertyChanged(m => m.NewComponent);
            }
        }

        public int AddThisManyComponents { get; set; }

        public void Cancel()
        {
            NewComponent = null;
        }

        public void NewCategory()
        {
            var ans = Microsoft.VisualBasic.Interaction.InputBox("Indtast navnet på den nye kategori.", "Ny kategori", "");    //referencing Microsoft.VisualBasic
            if (ans.Length != 0 && ans.ToLower() != "alle")
            {
                //Add it to the global list of categories
                var cat = (Categories) Application.Current.FindResource("Categories");
                cat.Add(ans);

                newComponent_.Category = ans; //so the new category pops up in the combobox
            }
            else
                MessageBox.Show("Kategorien skal have et navn. Ingen kategori oprettet.");
        }

        public void Clear()
        {
            NewComponent.Category = "";
            NewComponent.ComponentInfo = "";
            NewComponent.ComponentName = "";
            NewComponent.Datasheet = "";
            NewComponent.Image = "";
            NewComponent.SerieNr = "";
            NewComponent.ComponentNumber = 1;
            NewComponent.ManufacturerLink = "";
        }
    }
}