using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Models
{
    //public class Component
    //{
    //    public string _category;
    //    public int _componentId;
    //    public int _componentNumber;
    //    public string _datasheet;
    //    public string _image;
    //    public string _name;
    //    public int _ownerId;
    //    public string _tags;
    //    public LoanInformation _loanInformation;
    //}
    public class Component
    {
        public virtual int ComponentID
        {
            get;
            set;
        }

        public virtual string ComponentName
        {
            get;
            set;
        }

        public virtual string Category
        {
            get;
            set;
        }

        public virtual string Datasheet
        {
            get;
            set;
        }

        public virtual int OwnerID
        {
            get;
            set;
        }

        public virtual int ComponentNumber
        {
            get;
            set;
        }

        public virtual string Image
        {
            get;
            set;
        }

        public virtual int LoanInformation
        {
            get;
            set;
        }

        public virtual string ComponentInfo
        {
            get;
            set;
        }

        public virtual string SerieNr
        {
            get;
            set;
        }

        public virtual LoanInformation Fra
        {
            get;
            set;
        }

    }

}
