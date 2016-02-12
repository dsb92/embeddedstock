using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleMvvmToolkit;

namespace PCApplikationMVVM.ViewModels
{
    public class LoansViewModel : ViewModelBase<LoansViewModel>
    {
        public LoansViewModel(DateTime? startDate, DateTime? endDate, string studyNumber, string phonenr)
        {
            StartDate = startDate;
            EndDate = endDate;
            StudyNumber = studyNumber;
            PhoneNr = phonenr;
            
            StartDate = DateTime.Now;
            if (DateTime.Now < DateTime.Parse("15/6", CultureInfo.CreateSpecificCulture("da-DK")) && DateTime.Now > DateTime.Parse("1/2", CultureInfo.CreateSpecificCulture("da-DK")))
            {
                EndDate = DateTime.Parse("20/6", CultureInfo.CreateSpecificCulture("da-DK"));
            }
            else
            {
                EndDate = DateTime.Parse("20/1", CultureInfo.CreateSpecificCulture("da-DK"));
            }

        }

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                NotifyPropertyChanged(m => m.StartDate);
            }
        }
        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                NotifyPropertyChanged(m => m.EndDate);
            }
        }
        private string _studyNumber;
        public string StudyNumber
        {
            get { return _studyNumber; }
            set
            {
                _studyNumber = value;
                NotifyPropertyChanged(m => m.StudyNumber);
            }
        }
        private string _phoneNr;
        public string PhoneNr
        {
            get { return _phoneNr;}
            set
            {
                _phoneNr = value;
                NotifyPropertyChanged(m => m.PhoneNr);
            }
        }
      
    }
}
