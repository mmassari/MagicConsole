using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MagicConsole
{
    public class Commands : ICollection<Command>
    {
        public Menu Menu { get; set; }

        public Command ExitCommand { get; set; }
        public Command DefaultCommand { get; set; }
        public Delegate ExecuteMethod { get; set; }
        public delegate void CommandSelectedDelegate(object sender, Command command);
        public event CommandSelectedDelegate CommandSelected;
        public Commands()
        {
            _commands = new List<Command>();
            Help = false;
            HelpCommand = Command.DEFAULT_HELP_COMMAND;
            ExitCommand = Command.DEFAULT_EXIT_COMMAND;
            Menu = new Menu();
            DefaultCommand = null;
            ExecuteMethod = null;
        }

        internal void PrintMenu()
        {
            Console.WriteLine(Menu.MenuHeader);

            foreach (var item in _commands)
                item.PrintItemMenu();

            Console.Write(Menu.MenuFooter);
        }

        #region ICollection Implementation

        public int Count => _commands.Count;

        public bool IsReadOnly => false;

        public void Add(Command item)
        {
            if (Contains(item))
                throw new ArgumentException("A command with the same Name is already in the collection");

            if (ContainsAlias(item))
                throw new ArgumentException("A command with the same Alias is already in the collection");

            if (item.MenuID.HasValue && _commands.Exists(c => c.MenuID == item.MenuID.Value))
                throw new ArgumentException("A command with the same MenuID is already in the collection");
            else if (!item.MenuID.HasValue)
                item.MenuID = _commands.Max(c => c.MenuID) + 1;

            _commands.Add(item);
        }

        public void Clear()
        {
            Clear();
        }

        public bool Contains(Command item)
        {
            return _commands.Exists(c => c.Name == item.Name);
        }
        public bool ContainsAlias(Command item)
        {
            return this.SelectMany(c => c.Aliases).Count(c => item.Aliases.Contains(c)) > 0;
        }

        public void CopyTo(Command[] array, int arrayIndex)
        {
            _commands.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Command> GetEnumerator()
        {
            return _commands.GetEnumerator();
        }

        public bool Remove(Command item)
        {
            return _commands.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _commands.GetEnumerator();
        }


        #endregion

    }
}