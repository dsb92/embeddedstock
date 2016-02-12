using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PCApplikationMVVM
{
    public class Categories : ObservableCollection<string>
    {
        public Categories()
        {
            Add("Alle");
        }
    }
}
