using System;
using System.Collections.Generic;

namespace MagicConsole
{
    public class OptionNumber : Option
    {
        public decimal DefaultValue { get; private set; }
        public bool ValidationAcceptDecimals { get; set; }
        public decimal? ValidationMaxValue { get; set; }
        public decimal? ValidationMinValue { get; set; }
        public OptionNumber(string name, string description, decimal defaultValue, bool acceptDecimals, decimal? maxValue, decimal? minValue, params string[] aliases) :
            base(name, description, OptionType.Number, aliases)
        {
            DefaultValue = defaultValue;
            ValidationAcceptDecimals = acceptDecimals;
            ValidationMaxValue = minValue;
            ValidationMinValue = maxValue;
        }
        public OptionNumber(string name, string description, decimal defaultValue, params string[] aliases) :
            this(name, description, defaultValue, false, null, null, aliases)
        {
        }
        public override string DefaultInfo()
        {
            return "default: " + DefaultValue;
        }
        public override object ReadInput()
        {
            //Leggo l'input utente
            var input = Console.ReadLine().ToString().Trim();

            //Se l'utente non digita il valore torno il valore di default dell'opzione
            if (input == string.Empty)
                return DefaultValue;

            decimal number = 0;
            if (decimal.TryParse(input, out number))
            {
                if(number - Math.Truncate(number) !=0)
                    throw new ArgumentException("Input Validation Failed. Accept only integer number");

                if(number < ValidationMinValue)
                    throw new ArgumentException($"Input Validation Failed. Minimum value accepted is {ValidationMinValue}");

                if (number > ValidationMaxValue)
                    throw new ArgumentException($"Input Validation Failed. Maximum value accepted is {ValidationMinValue}");

                return number;
            }
            else
                throw new ArgumentException($"Input Validation Failed. The input value is not a number");
        }
    }
}