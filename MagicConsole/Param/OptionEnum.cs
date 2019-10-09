using System;
using System.Collections.Generic;

namespace MagicConsole
{
    public class OptionEnum : Option
    {
        public string DefaultValue { get; private set; }
        public List<string> Values { get; private set; }
        public OptionEnum(string name, string description, List<string> values, string defaultValue, params string[] aliases) :
            base(name, description, OptionType.Enum, aliases)
        {
            Values = values;
            DefaultValue = defaultValue;
        }
        public override string DefaultInfo()
        {
            return "default: " + DefaultValue;
        }
        public override object ReadInput()
        {
            var x = 0;
            foreach (var item in Values)
            {
                x++;
                Console.WriteLine($"{x}. {item}");
            }
            Console.WriteLine("Select a value");

            ////Leggo l'input utente
            var input = Input.ReadInt(1,x);
            return Values[input - 1];

            ////Se l'utente non digita il valore torno il valore di default dell'opzione
            //if (input == string.Empty)
            //    return DefaultValue;

            //DateTime dt;
            //if (DateTime.TryParse(input, out dt))
            //{
            //    switch (Format)
            //    {
            //        case DateInputFormat.OnlyDate:
            //            if ((!ValidationMinValue.HasValue || dt.Date >= ValidationMinValue) && (!ValidationMaxValue.HasValue || dt.Date <= ValidationMaxValue))
            //                return dt.Date;
            //            else
            //                throw new ArgumentException(String.Format("Input Validation Failed. Date input not in accepted range Min: {0}, Max: {1}", 
            //                    ValidationMinValue.HasValue ? dt.Date.ToString() : "(No limit)", ValidationMaxValue.HasValue ? dt.Date.ToString() : "(No limit)");
            //            break;
            //        case DateInputFormat.OnlyTime:
            //            if ((!ValidationMinValue.HasValue || dt.TimeOfDay >= ValidationMinValue.Value.TimeOfDay) &&
            //                (!ValidationMaxValue.HasValue || dt.TimeOfDay <= ValidationMaxValue.Value.TimeOfDay))
            //                return dt.TimeOfDay;
            //            break;
            //        case DateInputFormat.DateAndTime:

            //            break;
            //    }
            //    if(number - Math.Truncate(number) !=0)
            //        throw new ArgumentException("Input Validation Failed. Accept only integer number");

            //    if(number < ValidationMinValue)
            //        throw new ArgumentException($"Input Validation Failed. Minimum value accepted is {ValidationMinValue}");

            //    if (number > ValidationMaxValue)
            //        throw new ArgumentException($"Input Validation Failed. Maximum value accepted is {ValidationMinValue}");

            //    return number;
            //}
            //else
            //    throw new ArgumentException($"Input Validation Failed. The input value is not a number");
        }
    }
}