using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Models
{
    public class LoanInformation
    {
        private string _adminComments;
        //Hvordan bliver LoanDate?
        private DateTime _loanDate;
        private int _loanId;
        private DateTime _submissionDate;
        //Hvordan bliver SubmissionDate?
        private string _userComments;

        public LoanInformation(string adminComments, int loanId, string userComments, DateTime loanDate, DateTime submissionDate)
        {
            _adminComments = adminComments;
            _loanId = loanId;
            _userComments = userComments;
            _loanDate = loanDate;
            _submissionDate = submissionDate;
        }
    }
}
