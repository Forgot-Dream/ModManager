using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModManager.Common.Structs
{
    public class MenuBar : BindableBase
    {
        private string icon;

        public string Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string name_space;

        public string Name_Space
        {
            get { return name_space; }
            set { name_space = value; }
        }

    }
}
