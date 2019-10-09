using System;
using System.Linq;

namespace MagicConsole
{
    public static class StringExtensions
    {
        public enum CompareType { Equals, Contains, StartsWith, EndsWith }

        public static int WordCount(this string str)
        {
            return str.Split(new char[] { ' ', '.', '?' },
                                  StringSplitOptions.RemoveEmptyEntries).Length;
        }



        public static bool In(this string str, CompareType type, bool trim, bool ignoreCase, params string[] strings)
        {
            if (trim) strings = strings.Select(x => x.Trim()).ToArray();
            if (type != CompareType.Equals && ignoreCase)
            {
                strings = strings.Select(x => x.ToLower()).ToArray();
                str = str.ToLower();
            }

            switch (type)
            {
                case CompareType.Contains:
                    return strings.Any(s => s.Contains(str));
                case CompareType.StartsWith:
                    return strings.Any(s => s.StartsWith(str));
                case CompareType.EndsWith:
                    return strings.Any(s => s.EndsWith(str));
                default:
                    return strings.Any(s => s.Equals(str, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
            }

        }
        public static bool In(this string str, bool trim, bool ignoreCase, params string[] strings)
        {
            return In(str, CompareType.Equals, trim, ignoreCase, strings);
        }
        public static bool In(this string str, bool ignoreCase, params string[] strings)
        {
            return In(str, CompareType.Equals, false, ignoreCase, strings);
        }

        public static bool In(this string str, params string[] strings)
        {
            return In(str, CompareType.Equals, false, false, strings);
        }

        public static bool NotIn(this string str, CompareType type, bool trim, bool ignoreCase, params string[] strings)
        {
            return !In(str, type, trim, ignoreCase, strings);
        }
        public static bool NotIn(this string str, bool trim, bool ignoreCase, params string[] strings)
        {
            return !In(str, CompareType.Equals, trim, ignoreCase, strings);
        }

        public static bool NotIn(this string str, bool ignoreCase, params string[] strings)
        {
            return !In(str, CompareType.Equals, false, ignoreCase, strings);
        }

        public static bool NotIn(this string str, params string[] strings)
        {
            return !In(str, CompareType.Equals, false, false, strings);
        }

    }
}
