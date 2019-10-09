using MagicConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MagicConsole.Test
{
    class EntryPoint
    {
        public static void Main(params string[] args)
        {
            new SyncProgram("magic.json").Start(args);
        }
    }
}
