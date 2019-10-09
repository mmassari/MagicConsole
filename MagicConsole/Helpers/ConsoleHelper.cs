using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MagicConsole
{
    public static class ConsoleHelper
    {
        private static bool ErrorPrinted;
        private static Point InputCursorPos;// int CursorLeft;
        public static string LastInput;

        public static int? ReadInt()
        {
            LastInput = Console.ReadLine();
            int id;
            if (int.TryParse(LastInput, out id))
                return id;
            else
                return null;
        }

        public static void CleanError()
        {
            if (ErrorPrinted)
            {
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, InputCursorPos.Y);
            }
        }

        public static void SetCursorInput(bool clean)
        {
            if (clean)
            {
                Console.SetCursorPosition(InputCursorPos.X, InputCursorPos.Y);
                Console.Write(new string(' ', LastInput.Length));
            }
            Console.SetCursorPosition(InputCursorPos.X, InputCursorPos.Y);
        }

        public static void PrintInputError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"Error! '{LastInput}' is not a valid response".PadRight(Console.WindowWidth));
            Console.ResetColor();

            SetCursorInput(true);
        }

        internal static void StartGettingInput()
        {
            ErrorPrinted = false;
            InputCursorPos = new Point(Console.CursorLeft, Console.CursorTop);
        }
    }
}
