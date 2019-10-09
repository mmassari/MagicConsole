using System;
using System.Collections.Generic;
using System.Text;

namespace MagicConsole
{
    public class MenuOption
    {
        public string Name { get; private set; }
        public Action Callback { get; private set; }

        public MenuOption(string name, Action callback)
        {
            Name = name;
            Callback = callback;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
