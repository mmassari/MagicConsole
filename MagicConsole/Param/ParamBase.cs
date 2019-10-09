using System;
using System.Collections.Generic;

namespace MagicConsole
{
    public abstract class ParamBase
    {
        public string Name { get; set; }
        public List<string> AliasList { get; set; }
        public string Description { get; set; }
        public ParamBase()
        {
            Name = string.Empty;
            Description = string.Empty;
            AliasList = new List<string>();
        }
        public ParamBase(string name, string description, params string[] alias) : this()
        {
            Name = name;
            Description = description;
            AliasList = new List<string>(alias);
        }

    }
}