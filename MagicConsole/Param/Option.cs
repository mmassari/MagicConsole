using System;
using System.Collections.Generic;

namespace MagicConsole
{
    public abstract class Option : ParamBase, ICloneable
    {
        public OptionType Type { get; set; }
        public Option(string name, string description, OptionType type, params string[] alias) :
            base(name, description, alias)
        {
            Type = type;
        }
        public abstract string DefaultInfo();

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        internal void PrintItemOption()
        {
            Console.Write($" - {Description} ({DefaultInfo()}): ");
        }

        public abstract object ReadInput();            
    }
}