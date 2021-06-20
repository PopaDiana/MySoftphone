using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySoftphone.UI.Model
{
    class Caller
    {
        public Caller(string name, string phone)
        {
            this.Name = name;
            this.PhoneNumber = phone;
        }
        public string Name { get; set; }

        public string PhoneNumber { get; set; }
    }
}
