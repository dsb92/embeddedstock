using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PCApplikation;
using SimpleMvvmToolkit;

namespace PCApplikationMVVM.ViewModels
{
    public class ChangeComponentViewModel : ViewModelBase<ChangeComponentViewModel>
    {
        private Component newComponent_;

        // Default ctor
        public ChangeComponentViewModel(Component newComponent)
        {
            if (newComponent != null)
                NewComponent = newComponent;
            else
                throw new ArgumentException(
                    "You need to create NewChangeComponentViewModel with a Component model from MainViewModel");

            EditComponentNumber = true;     //by default user can edit component number, but not if more than one is selected
	        RemoveReservationCheck = false;
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

        public bool EditComponentNumber { get; set; }

        public void Cancel()
        {
            NewComponent = null;
        }

		public bool RemoveReservationCheck { get; set; }

	    public void RemoveReservation()
	    {
			newComponent_.ActualLoanInformation.ReservationDate = new SqlDateTime();
		    newComponent_.ActualLoanInformation.ReservationID = "";
		    MessageBox.Show("Reservationen for denne komponent vil blive fjernet, så snart du trykker på 'Ændre komponent'");
		    RemoveReservationCheck = true;
	    }

        public void NewCategory()
        {
            var ans = Microsoft.VisualBasic.Interaction.InputBox("Indtast navnet på den nye kategori.", "Ny kategori", "");    //referencing Microsoft.VisualBasic
            if (ans.Length != 0 && ans.ToLower() != "alle")
            {
                //Add it to the global list of categories
                var cat = (Categories)Application.Current.FindResource("Categories");
                cat.Add(ans);

                newComponent_.Category = ans; //so the new category pops up in the combobox
            }
            else
                MessageBox.Show("Kategorien skal have et navn. Ingen kategori oprettet.");
        }
    }
}
