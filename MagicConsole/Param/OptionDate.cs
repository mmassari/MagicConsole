using System;
using System.Collections.Generic;

namespace MagicConsole
{
    public enum DateInputFormat { OnlyDate, OnlyTime, DateAndTime }
    public class OptionDate : Option
    {
        public DateTime? DefaultValue { get; private set; }
        public DateInputFormat Format { get; set; }
        public DateTime? ValidationMaxValue { get; set; }
        public DateTime? ValidationMinValue { get; set; }
        public OptionDate(string name, string description, DateTime? defaultValue, DateInputFormat format, DateTime? max, DateTime? min, params string[] aliases) :
            base(name, description, OptionType.Date, aliases)
        {
            DefaultValue = defaultValue;
            Format = format;
            ValidationMaxValue = max;
            ValidationMinValue = min;
        }
        
        public OptionDate(string name, string description, DateTime defaultValue, params string[] aliases) :
            this(name, description, null, DateInputFormat.OnlyDate, null, null, aliases) { }

        public override string DefaultInfo()
        {
            return "yyyy/mm/dd hh:mm:ss";
        }
        public override object ReadInput()
        {
            //Leggo l'input utente
            var input = Console.ReadLine().ToString().Trim();

            //Se l'utente non digita il valore torno il valore di default dell'opzione
            if (input == string.Empty)
                return DefaultValue;

            DateTime dt;
            if (DateTime.TryParse(input, out dt))
            {
                if (Format == DateInputFormat.OnlyDate && (!ValidationMinValue.HasValue || dt.Date >= ValidationMinValue.Value.Date) && (!ValidationMaxValue.HasValue || dt.Date <= ValidationMaxValue.Value.Date))
                    return dt.Date;

                else if (Format == DateInputFormat.OnlyTime && (!ValidationMinValue.HasValue || dt.TimeOfDay >= ValidationMinValue.Value.TimeOfDay) &&
                    (!ValidationMaxValue.HasValue || dt.TimeOfDay <= ValidationMaxValue.Value.TimeOfDay))
                    return dt.TimeOfDay;

                else if (Format == DateInputFormat.DateAndTime && (!ValidationMinValue.HasValue || dt >= ValidationMinValue.Value) &&
                    (!ValidationMaxValue.HasValue || dt <= ValidationMaxValue.Value))
                    return dt;
                else
                    throw new ArgumentException(String.Format("Input Validation Failed. Date input not in accepted range Min: {0}, Max: {1}",
                        ValidationMinValue.HasValue ? dt.Date.ToString() : "(No limit)", ValidationMaxValue.HasValue ? dt.Date.ToString() : "(No limit)"));
            }
            else
                throw new ArgumentException($"Input Validation Failed. The input value is not a valid date");
        }
    }
}