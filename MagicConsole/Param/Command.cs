using System;
using System.Collections.Generic;

namespace MagicConsole
{
    public class Command<TOption> : ParamBase 
        where TOption: struct, Enum 
    {
        #region Defaults

        public const string MENU_TEMPLATE_DEFAULT = "{MenuID}. {Name}";
        public static Command<TOption> DEFAULT_HELP_COMMAND
        {
            get
            {
                var c = new Command<TOption>("Help", "Program help", "/help");
                c.Type = CommandTypes.Help;
                c.ShowInMenu = false;
                return c;
            }
        }
        public static Command<TOption> DEFAULT_MAIN_COMMAND
        {
            get
            {
                var c = new Command<TOption>("Main", "Main default command");
                c.Type = CommandTypes.Custom;
                return c;
            }
        }
        public static Command<TOption> DEFAULT_EXIT_COMMAND
        {
            get
            {
                var c = new Command<TOption>("Exit", "Exit the program");
                c.Type = CommandTypes.Exit;
                return c;
            }
        }
        #endregion
        public CommandTypes Type { get; protected set; }
        public int? MenuID { get; internal set; }
        public bool ShowInMenu { get; set; }
        public List<TOption> ValidOptions { get; set; }
        public Command(string name, string description, params string[] aliases) : base(name, description, aliases)
        {
            ShowInMenu = true;
            Type = CommandTypes.Custom;
            ValidOptions = new List<TOption>();
        }
        public void PrintItemMenu()
        {
            if (ShowInMenu)
                Console.WriteLine($" {MenuID}. {Name}");
        }
    }
}