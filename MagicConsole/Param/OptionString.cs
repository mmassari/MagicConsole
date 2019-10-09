using System;
using System.Collections.Generic;

namespace MagicConsole
{
    public class OptionString : Option
    {
        public string DefaultValue { get; set; }
        public int ValidationMaxLength { get; set; }
        public OptionString(string name, string description, string defaultValue, params string[] aliases) :
            base(name, description, OptionType.String, aliases)
        {
        }

        public override string DefaultInfo()
        {
            return "default: " + DefaultValue;
        }
        public override object ReadInput()
        {
            //Leggo l'input utente
            var input = Console.ReadLine().Trim();

            //Se l'utente non digita il valore torno il valore di default dell'opzione
            if (input == string.Empty)
                return DefaultValue;

            //Se l'utente digita una stringa troppo lunga sollevo un eccezione
            if (input.Length > ValidationMaxLength)
                throw new ArgumentException("Input Validation Failed. Exceed max length");

            //Ritorno il valore inserito dall'utente
            return input;
        }

    }
}