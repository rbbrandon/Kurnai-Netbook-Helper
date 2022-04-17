using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurnaiNetbookHelper
{
    public class ComboBoxItem
    {
        private string _Name;
        private string _ID;

        public ComboBoxItem(string name, string id)
        {
            _Name = name;
            _ID = id;
        }

        public string Name
        {
            get { return _Name; }
        }

        public string ID
        {
            get { return _ID; }
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", _Name, _ID);
        }
    }
}
