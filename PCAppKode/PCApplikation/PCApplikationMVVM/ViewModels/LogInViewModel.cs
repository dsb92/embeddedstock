using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SimpleMvvmToolkit;


namespace PCApplikationMVVM.ViewModels
{
    public class LogInViewModel : ViewModelBase<LogInViewModel>
    {
        private string _logInDef;
        public string LogInDef
        {
            get { return _logInDef; }
            set
            {
                _logInDef = value;
                NotifyPropertyChanged(m => m.LogInDef);
            }
        }
        private string _okDef;
        public string OkDef
        {
            get { return _okDef; }
            set
            {
                _okDef = value;
                NotifyPropertyChanged(m => m.OkDef);
            }
        }
        private string _changeDef;
        public string ChangeDef
        {
            get { return _changeDef; }
            set
            {
                _changeDef = value;
                NotifyPropertyChanged(m => m.ChangeDef);
            }
        }
        private string _changePassword1;
        public string ChangePassword1
        {
            get { return _changePassword1; }
            set
            {
                _changePassword1 = value;
                NotifyPropertyChanged(m => m.ChangePassword1);
            }
        }
        private string _changePassword2;
        public string ChangePassword2
        {
            get { return _changePassword2; }
            set
            {
                _changePassword2 = value;
                NotifyPropertyChanged(m => m.ChangePassword2);
            }
        }
       private string _rowHeightLogIn;
        public string RowHeightLogIn
        {
            get { return _rowHeightLogIn; }
            set
            {
                _rowHeightLogIn = value;
                NotifyPropertyChanged(m => m.RowHeightLogIn);
            }
        }
        private string _rowHeightChoiceBoxesChoiceBoxes;
        public string RowHeightChoiceBoxes
        {
            get { return _rowHeightChoiceBoxesChoiceBoxes; }
            set
            {
                _rowHeightChoiceBoxesChoiceBoxes = value;
                NotifyPropertyChanged(m => m.RowHeightChoiceBoxes);
            }
        }
        private string _rowHeightChangePassword;
        public string RowHeightChangePassword
        {
            get { return _rowHeightChangePassword; }
            set
            {
                _rowHeightChangePassword = value;
                NotifyPropertyChanged(m => m.RowHeightChangePassword);
            }
        }
        public LogInViewModel()
        {
            RowHeightChoiceBoxes = "0";
            RowHeightChangePassword = "0";
            RowHeightLogIn = "*";
            LogInDef = "true";
            OkDef = "false";
            ChangeDef = "false";
        }
        public void LogInButtonPressed()
        {
                MessageBox.Show("Du er nu logget ind");
                RowHeightChoiceBoxes = "*";
                RowHeightLogIn = "0";
                LogInDef = "false";
                OkDef = "true";
                Properties.Settings.Default.Save();
        }

        public void OkPressed()
        {
            RowHeightChoiceBoxes = "0";
            RowHeightChangePassword = "0";
            RowHeightLogIn = "*";
            LogInDef = "true";
            OkDef = "false";
            ChangeDef = "false";
        }
        public void ChangePasswordButtonPressed()
        {
            RowHeightChangePassword = "*";
            RowHeightChoiceBoxes = "0";
            OkDef = "false";
            ChangeDef = "true";
        }

        public void ChangePasswordButtonAgainPressed()
        {
            if (ChangePassword1 == ChangePassword2)
            {
                Properties.Settings.Default.Password = ChangePassword1;
                Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("De to kodeord var ikke ens, så kodeordet blev ikke ændret");
            }
            ChangeDef = "false";
            RowHeightChoiceBoxes = "0";
            RowHeightChangePassword = "0";
            RowHeightLogIn = "*";
            LogInDef = "true";
            OkDef = "false";
        }

        public void AnnulerPressed()
        {
            ChangeDef = "false";
            RowHeightChoiceBoxes = "0";
            RowHeightChangePassword = "0";
            RowHeightLogIn = "*";
            LogInDef = "true";
            OkDef = "false";
        }
    }
}
