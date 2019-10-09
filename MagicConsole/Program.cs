using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MagicConsole
{
    public enum ExecutionMode
    {
        Silent,
        Interactive,
        Automatic,
        AskForExit
    }
    public abstract class Program<TCommand, TOption>
        where TCommand : struct, Enum
        where TOption : struct, Enum
    {
        private const string DEFAULT_MENU_HEADER = "MENU:";
        private const string DEFAULT_MENU_FOOTER = "Please choose a menu item: ";
        /// <summary>
        /// Program settings
        /// </summary>
        public static class Settings
        {
            /// <summary>
            /// Clear console when program start
            /// </summary>
            public static bool ClearConsole { get; set; }
            /// <summary>
            /// The type of menu for choose the command
            /// </summary>
            public static MenuType MenuType { get; set; }
            /// <summary>
            /// The print template for menu header
            /// </summary>
            public static string MenuHeader { get; set; }
            /// <summary>
            /// The print template for menu footer
            /// </summary>
            public static string MenuFooter { get; set; }
            /// <summary>
            /// Specify if the program have a help command
            /// </summary>
            public static bool Help { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected JObject JsonSettings { get; private set; }
        public AssemblyInfo Info { get; }
        public bool HaveCommands { get { return Commands.Count(c => c.Value.Type == CommandTypes.Custom) > 0; } }
        public bool HaveOptions { get { return Options.Count > 0; } }
        public ExecutionMode Mode { get; set; }
        private Dictionary<TCommand, Command<TOption>> Commands { get; set; }
        private Dictionary<TOption, Option> Options { get; set; }
        public Command<TOption> HelpCommand { get; set; }
        public Command<TOption> ExitCommand { get; set; }
        public TCommand DefaultCommand { get; set; }
        public TCommand SelectedCommand { get; set; }
        public Dictionary<TOption, object> SelectedOptions { get; set; }

        public delegate void ExecuteDelegate(TCommand command, Dictionary<TOption, object> options);
        public ExecuteDelegate ExecuteMethod { get; set; }
        public Program()
        {
            Info = new AssemblyInfo();
            Commands = new Dictionary<TCommand, Command<TOption>>();
            Options = new Dictionary<TOption, Option>();
            SelectedCommand = default;
            SelectedOptions = new Dictionary<TOption, object>();
            Settings.ClearConsole = false;
            Settings.MenuType = MenuType.Standard;
            Settings.MenuHeader = DEFAULT_MENU_HEADER;
            Settings.MenuFooter = DEFAULT_MENU_FOOTER;
            Settings.Help = true;
            HelpCommand = Command<TOption>.DEFAULT_HELP_COMMAND;
            ExitCommand = Command<TOption>.DEFAULT_EXIT_COMMAND;

        }
        public Program(string configFile) : this()
        {
            if (!string.IsNullOrWhiteSpace(configFile) && File.Exists(configFile))
            {
                JsonSettings = JObject.Parse(File.ReadAllText(configFile));

                //Se sono state definite delle settings di base le carico
                if (JsonSettings["settings"] != null)
                {
                    var s = (JObject)JsonSettings["settings"];
                    Settings.ClearConsole = SetJsonValue(s, "clearConsole", Settings.ClearConsole);
                    Settings.MenuType = SetJsonValue(s, "menuType", Settings.MenuType);
                    Settings.MenuHeader = SetJsonValue(s, "menuHeader", Settings.MenuHeader);
                    Settings.MenuFooter = SetJsonValue(s, "menuFooter", Settings.MenuFooter);
                    Settings.Help = SetJsonValue(s, "help", Settings.Help);
                }
                if (JsonSettings["options"] != null)
                {
                    var optList = (JArray)JsonSettings["options"];
                    foreach (JObject j in optList)
                    {
                        var jType = (OptionType)Enum.Parse(typeof(OptionType), j["type"].ToString());
                        Option opt;
                        var enumValue = (TOption)Enum.Parse(typeof(TOption), j["name"].ToString());
                        switch (jType)
                        {
                            case OptionType.Flag:
                                opt = new OptionFlag(
                                    j["name"].ToString(),
                                    j["description"] != null ? j["description"].ToString() : string.Empty,
                                    j["default"].ToObject<bool>(),
                                    j["alias"].ToList().ConvertAll(c => c.ToString()).ToArray()
                                );
                                break;
                            case OptionType.Date:
                                opt = new OptionDate(
                                    j["name"].ToString(),
                                    j["description"] != null ? j["description"].ToString() : string.Empty,
                                    j["default"].ToObject<DateTime>(),
                                    j["alias"].ToList().ConvertAll(c => c.ToString()).ToArray()
                                );
                                break;
                            case OptionType.Number:
                                opt = new OptionNumber(
                                    j["name"].ToString(),
                                    j["description"] != null ? j["description"].ToString() : string.Empty,
                                    j["default"].ToObject<decimal>(),
                                    j["alias"].ToList().ConvertAll(c => c.ToString()).ToArray()
                                );
                                break;
                            case OptionType.Enum:
                                opt = new OptionEnum(
                                    j["name"].ToString(),
                                    j["description"] != null ? j["description"].ToString() : string.Empty,
                                    j["items"].ToList().ConvertAll(c => c.ToString()),
                                    j["default"].ToString(),
                                    j["alias"].ToList().ConvertAll(c => c.ToString()).ToArray()
                                );
                                break;

                            default:
                                opt = new OptionString(
                                    j["name"].ToString(),
                                    j["description"] != null ? j["description"].ToString() : string.Empty,
                                    j["default"].ToString(),
                                    j["alias"].ToList().ConvertAll(c => c.ToString()).ToArray()
                                );
                                break;
                        }

                        Options.Add(enumValue, opt);
                    }
                }

                //Se sono state definite dei commands di base le carico
                if (JsonSettings["commands"] != null)
                {
                    var cmdList = (JArray)JsonSettings["commands"];
                    foreach (JObject j in cmdList)
                    {
                        var enumValue = (TCommand)Enum.Parse(typeof(TCommand), j["name"].ToString());

                        var cmd = new Command<TOption>(
                            j["name"].ToString(),
                            j["description"] != null ? j["description"].ToString() : string.Empty,
                            j["alias"].ToList().ConvertAll(c => c.ToString()).ToArray()
                        );

                        if (j["menuId"] != null)
                        {
                            var id = int.Parse(j["menuId"].ToString());
                            if (Commands.Values.Count(c => c.MenuID == id) > 0)
                                throw new ArgumentException("MenuID of commands must be unique");
                            cmd.MenuID = id;
                        }
                        else
                            cmd.MenuID = Commands.Values.Max(c => c.MenuID) + 1;

                        cmd.ShowInMenu = j["showInMenu"] != null ? j["showInMenu"].ToObject<bool>() : true;
                        foreach (var item in j["validOptions"].ToList().ConvertAll(c => c.ToString()).ToArray())
                        {
                            if (Options.Count(c => c.Value.Name == item) > 0)
                                cmd.ValidOptions.Add(Options.FirstOrDefault(c => c.Value.Name == item).Key);
                        }

                        Commands.Add(enumValue, cmd);
                    }
                }
                if (Settings.Help)
                    HelpCommand.MenuID = Commands.Values.Max(c => c.MenuID) + 1;
                ExitCommand.MenuID = Commands.Values.Max(c => c.MenuID) + 1;

            }
        }

        private T SetJsonValue<T>(JObject s, string v, T clearConsole)
        {
            if (s.TryGetValue(v, out JToken tk))
            {
                if (typeof(T).IsEnum)
                    return (T)Enum.Parse(typeof(T), tk.Value<string>());
                else
                    return tk.Value<T>();
            }


            return clearConsole;
        }

        #region AddOption

        protected OptionFlag AddOptionFlag(TOption type, string name, string description, bool defaultValue, params string[] alias)
        {
            var opt = new OptionFlag(name, description, defaultValue, alias);
            Options.Add(type, opt);
            return opt;
        }
        protected OptionString AddOptionString(TOption type, string name, string description, string defaultValue, params string[] alias)
        {
            var opt = new OptionString(name, description, defaultValue, alias);
            Options.Add(type, opt);
            return opt;
        }
        protected OptionNumber AddOptionNumber(TOption type, string name, string description, decimal defaultValue, params string[] alias)
        {
            var opt = new OptionNumber(name, description, defaultValue, alias);
            Options.Add(type, opt);
            return opt;
        }
        protected OptionNumber AddOptionNumber(TOption type, string name, string description, decimal defaultValue, bool acceptDecimals, decimal? max, decimal? min, params string[] alias)
        {
            var opt = new OptionNumber(name, description, defaultValue, acceptDecimals, max, min, alias);
            Options.Add(type, opt);
            return opt;
        }
        protected OptionDate AddOptionDate(TOption type, string name, string description, DateTime defaultValue, params string[] alias)
        {
            var opt = new OptionDate(name, description, defaultValue, alias);
            Options.Add(type, opt);
            return opt;
        }
        protected OptionDate AddOptionDate(TOption type, string name, string description, DateTime defaultValue, DateInputFormat format, DateTime? max, DateTime? min, params string[] alias)
        {
            var opt = new OptionDate(name, description, defaultValue, format, max, min, alias);
            Options.Add(type, opt);
            return opt;
        }
        protected OptionEnum AddOptionEnum(TOption type, string name, string description, List<string> values, string defaultValue, params string[] alias)
        {
            var opt = new OptionEnum(name, description, values, defaultValue, alias);
            Options.Add(type, opt);
            return opt;
        }

        #endregion
        public virtual void Start(params string[] args)
        {
            PrintHeader();

            switch (Mode)
            {
                case ExecutionMode.Automatic:
                    if (!ReadParameters(args))
                        ShowMenu();
                    break;
                case ExecutionMode.Silent:
                    ReadParameters(args);
                    break;
                case ExecutionMode.Interactive:
                    ShowMenu();
                    break;
                default:
                    break;
            }
            ExecuteMethod(SelectedCommand, SelectedOptions);

            if (Mode == ExecutionMode.AskForExit)
            {
                Console.WriteLine("Esecuzione terminata. Premi un tasto per uscire.");
                Console.Read();
            }
            else if (Mode == ExecutionMode.Interactive)
                Start(args);
        }
        public void ShowMenu()
        {
            PrintMenu();
            SelectedCommand = GetMenuInput();
            if (HaveOptions)
            {
                foreach (var option in Options)
                {
                    option.Value.PrintItemOption();
                    AddSelectedOption(option.Key, GetOptionInput(option.Value));
                }
            }
        }

        private string GetOptionInput(Option option)
        {
            ConsoleHelper.StartGettingInput();

            while (0 == 0)
            {
                try
                {
                    var input = option.ReadInput();
                    ConsoleHelper.CleanError();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(input);
                    Console.ResetColor();
                    return input.ToString();
                }
                catch (Exception)
                {
                    ConsoleHelper.PrintInputError();
                }
            }
        }

        private TCommand GetMenuInput()
        {
            ConsoleHelper.StartGettingInput();

            while (0 == 0)
            {
                var input = ConsoleHelper.ReadInt();
                if (input.HasValue && Commands.Count(c => c.Value.MenuID == input.Value) > 0)
                {
                    ConsoleHelper.CleanError();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(input.Value);
                    Console.ResetColor();
                    return Commands.First(c => c.Value.MenuID == input.Value).Key;
                }
                else
                    ConsoleHelper.PrintInputError();
            }
        }


        private bool ReadParameters(string[] args)
        {
            var cmdFound = false;
            foreach (var cmd in Commands)
            {
                if (args.CheckExistsAny(cmd.Value.AliasList))
                {
                    SelectedCommand = cmd.Key;
                    cmdFound = true;
                    break;
                }
            }

            foreach (var opt in Options)
            {
                if (args.CheckExistsAny(opt.Value.AliasList))
                {
                    var value = string.Empty;
                    if (opt.Value.Type == OptionType.Flag)
                        value = "y";
                    else
                    {
                        var arg = args.First(c => opt.Value.AliasList.Contains(c));
                        value = arg.Split(new char[] { ':', '=' })[1].Trim();
                    }
                    AddSelectedOption(opt.Key, value);
                }
            }

            return cmdFound;
        }

        private void PrintMenu()
        {
            if (Settings.ClearConsole)
                Console.Clear();

            Console.WriteLine(Settings.MenuHeader);
            foreach (var item in Commands)
            {
                item.Value.PrintItemMenu();
            }
            if (Settings.Help)
                HelpCommand.PrintItemMenu();

            ExitCommand.PrintItemMenu();

            Console.Write(Settings.MenuFooter);
            var menuId = Input.ReadInt(Commands.Values.Min(c=>c.MenuID).Value, ExitCommand.MenuID.Value);
            if (Commands.Count(c => c.Value.MenuID == menuId) > 0)
                SelectedCommand = Commands.First(c => c.Value.MenuID == menuId).Key;
            else if (menuId == HelpCommand.MenuID)
                PrintHelp();
            else if (menuId == ExitCommand.MenuID)
                Environment.Exit(0);

            if (Commands[SelectedCommand].ValidOptions.Count>0)
            {
                Console.WriteLine("\nPlease specify options: ");
                foreach (var opt in Commands[SelectedCommand].ValidOptions)
                {
                    Options[opt].PrintItemOption();
                    GetOptionInput(opt);
                }
            }
        }

        private void GetOptionInput(TOption opt)
        {
            var errorPrinted = false;
            var cursorLeft = Console.CursorLeft;
            var cursorTop = Console.CursorTop;

            string value = null;
            while (value == null)
            {
                var input = Console.ReadLine();
                if ((Options[opt].Type == OptionType.Flag && input.In(true, true, "y", "n", "")) || Options[opt].Type == OptionType.String)
                {
                    AddSelectedOption(opt, input);
                    if (errorPrinted)
                        CleanError();
                    break;
                }
                else
                {
                    PrintError(input, cursorLeft, cursorTop);
                    errorPrinted = true;
                }
            }
        }

        private void AddSelectedOption(TOption opt, string input)
        {
            if (SelectedOptions.ContainsKey(opt))
                SelectedOptions[opt] = input;
            else
            {
                SelectedOptions.Add(opt, input);
            }
        }

        //private TC GetMenuInput()
        //{
        //    var errorPrinted = false;
        //    var cursorLeft = Console.CursorLeft;
        //    var cursorTop = Console.CursorTop;

        //    while (0 == 0)
        //    {
        //        var input = Console.ReadLine();
        //        var id = 0;
        //        if (int.TryParse(input, out id) && Commands.Values.Count(c => c.MenuID == id) > 0)
        //        {
        //            if (errorPrinted)
        //                CleanError();
        //            return Commands.First(c => c.Value.MenuID == id).Key;
        //        }
        //        else
        //        {
        //            PrintError(input, cursorLeft, cursorTop);
        //            errorPrinted = true;
        //        }
        //    }
        //}
        //private void AssignIDToCommands()
        //{
        //    var x = 0;
        //    foreach (var item in Commands)
        //    {
        //        x++;
        //        item.Value.MenuID = x;
        //    }
        //}
        private void CleanError()
        {
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
        }
        private void PrintError(string input, int cursorLeft, int cursorTop)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            // PadRight ensures that this line extends the width
            // of the console window so it erases itself each time
            Console.Write($"Error! '{input}' is not a valid response".PadRight(Console.WindowWidth));
            Console.ResetColor();

            // Set cursor position to just after the promt again, write
            // a blank line, and reset the cursor one more time
            Console.SetCursorPosition(cursorLeft, cursorTop);
            Console.Write(new string(' ', input.Length));
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }
        private void PrintHeader()
        {
            if (Settings.ClearConsole) Console.Clear();
            Console.WriteLine(Info[InfoItem.product].ToUpper());
            Console.WriteLine(Info[InfoItem.description]);
            Console.WriteLine(Info[InfoItem.copyright]);
            Console.WriteLine(new string('-', Console.BufferWidth - 1));
            Console.WriteLine();
        }

        private void PrintHelp()
        {
            PrintHeader();

            Console.WriteLine($"\n{Info[InfoItem.filename]}" +
                (HaveCommands ? " command" : "") +
                (HaveOptions ? " [options]" : ""));

            Console.WriteLine("\nDescription");
            Console.WriteLine($"\t{Info[InfoItem.description]}");
            Console.WriteLine("\nList of available commands:");
            foreach (var cmd in Commands)
                Console.WriteLine($"\t{cmd.Value.AliasList[0]}\t\t{cmd.Value.Description}");

            Console.WriteLine("\nOptions list");
            foreach (var opt in Options)
                Console.WriteLine($"\t{opt.Value.AliasList[0]}\t\t{opt.Value.Type} ({opt.Value.DefaultInfo()})\t{opt.Value.Description}");
        }
    }
}

