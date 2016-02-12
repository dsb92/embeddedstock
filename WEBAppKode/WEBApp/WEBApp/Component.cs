using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEBApp
{
    class Component
    {
        private string _category;
        private int _componentId;
        private int _componentNumber;
        private string _datasheet;
        private string _image;
        private string _name;
        private int _ownerId;
        private string _tags;
        private LoanInformation _loanInformation;

        public Component(string category, int componentId, int componentNumber, string datasheet, string image, string name, int ownerId, string tags, LoanInformation loanInformation)
        {
            _category = category;
            _componentId = componentId;
            _componentNumber = componentNumber;
            _datasheet = datasheet;
            this._image = image;
            _name = name;
            _ownerId = ownerId;
            _tags = tags;
            _loanInformation = loanInformation;
        }
    }
}
