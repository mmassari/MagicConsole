using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MagicConsole
{
    public class Options : ICollection<Option>
    {
        private List<Option> _options;
        public Menu Menu { get; set; }
        public bool Help { get; set; }
        public Command HelpCommand { get; set; }
        public Command ExitCommand { get; set; }
        public Command DefaultCommand { get; set; }
        public Delegate ExecuteMethod { get; set; }
        public string OptionsHeader { get; set; }
        public Options()
        {
            _options = new List<Option>();
            Help = false;
            HelpCommand = Command.DEFAULT_HELP_COMMAND;
            ExitCommand = Command.DEFAULT_EXIT_COMMAND;
            Menu = new Menu();
            DefaultCommand = null;
            ExecuteMethod = null;
        }

        public void PrintOptions()
        {

            if (_options.Count > 0)
            {
                Console.WriteLine(OptionsHeader);
                foreach (var opt in _options)
                {
                    opt.PrintItemOption();
                    GetOptionInput(opt);
                }
            }
            Console.Write(Menu.MenuFooter);
        }
        private void GetOptionInput(Option opt)
        {
            var errorPrinted = false;
            var cursorLeft = Console.CursorLeft;
            var cursorTop = Console.CursorTop;

            string value = null;
            while (value == null)
            {
                var input = Console.ReadLine();
                if ((opt.Type == OptionType.Flag && input.In(true, true, "y", "n", "")) || opt.Type == OptionType.Value)
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
        #region ICollection Implementation

        public int Count => _options.Count;

        public bool IsReadOnly => false;

        public void Add(Option item)
        {
            if (Contains(item))
                throw new ArgumentException("An option with the same Name is already in the collection");

            if (ContainsAlias(item))
                throw new ArgumentException("An option with the same Alias is already in the collection");

            _options.Add(item);
        }

        public void Clear()
        {
            Clear();
        }

        public bool Contains(Option item)
        {
            return _options.Exists(c => c.Name == item.Name);
        }
        public bool ContainsAlias(Option item)
        {
            return this.SelectMany(c => c.Aliases).Count(c => item.Aliases.Contains(c)) > 0;
        }

        public void CopyTo(Option[] array, int arrayIndex)
        {
            _options.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Option> GetEnumerator()
        {
            return _options.GetEnumerator();
        }

        public bool Remove(Option item)
        {
            return _options.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _options.GetEnumerator();
        }

        #endregion

    }
}