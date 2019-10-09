using System;
using System.Collections.Generic;

namespace MagicConsole
{
    public class OptionFlag : Option
    {
        public bool DefaultValue { get; private set; }
        public string TrueString { get; set; }
        public string FalseString { get; set; }
        public OptionFlag(string name, string description, bool defaultValue, params string[] aliases) :
            base(name, description, OptionType.Flag, aliases)
        {
            DefaultValue = defaultValue;
            TrueString = "y";
            FalseString = "n";
        }
        public override string DefaultInfo()
        {
            if (DefaultValue)
                return $"{TrueString.ToUpper()}/{FalseString.ToLower()}";
            else
                return $"{TrueString.ToLower()}/{FalseString.ToUpper()}";
        }
        public override object ReadInput()
        {
            //Leggo l'input utente
            var input = Console.ReadLine().Trim();

            //Se l'utente non digita il valore torno il valore di default dell'opzione
            if (input == string.Empty)
                return DefaultValue;

            //Se l'utente digita una stringa troppo lunga sollevo un eccezione
            if (input.ToLower() == TrueString.Trim().ToLower())
                return true;
            else if (input.ToLower() == FalseString.Trim().ToLower())
                return false;
            else
                throw new ArgumentException("Input Validation Failed. The value is not correct");
        }
    }
}