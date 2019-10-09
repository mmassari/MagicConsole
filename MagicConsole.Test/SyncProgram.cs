using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MagicConsole.Test
{
    public enum SyncCommands { Import, Export, Transfer }
    public enum SyncOptions { Reboot, Slug, Attempts, Type }
    class SyncProgram : Program<SyncCommands, SyncOptions>
    {
        public SyncProgram(string configFile) : base(configFile) 
        {
        }
        public SyncProgram() : base()
        {

            //AddCommand(
            //    SyncCommands.Import, 
            //    "Silent Mode",
            //    "Modalità di esecuzione che non richiede input da parte dell'utente",
            //    "/silent", "/s");
            ////AddCommand(SyncCommands.Export,  )
            //AddCommand(new Command("Interactive Mode",
            //    "Modalità di esecuzione che crea un menù con le varie scelte",
            //    "/interactive", "/i"));
            //AddCommand(new Command("AskForExit Mode",
            //    "Modalità di esecuzione che non richiede input da parte dell'utente ma per essere chiuso chiede conferma",
            //    "/noexit", "/x"));
            ////tie.Commands.Add(Command.DEFAULT_HELP_COMMAND);
            ////tie.Commands.Add(Command.DEFAULT_EXIT_COMMAND);

            //Options.Add(new OptionFlag("SmartEngine attivo",
            //    "Se viene questa opzione il programma diventa intelligente", false, "-smart"));
            //Options.Add(new OptionFlag("Riavvia macchina al termine",
            //    "Se viene questa opzione il programma riavvia la macchina a fine esecuzione", false, "-reboot"));

        }

        public override void Start(params string[] args)
        {
            base.Start(args);
        }

        private static void Tie_CommandExecuted(SyncCommands command, Dictionary<SyncOptions, object> options)
        {
            switch (command)
            {
                case SyncCommands.Import:
                case SyncCommands.Export:
                case SyncCommands.Transfer:
                    Console.WriteLine($"Inizio procedura {command.ToString()}");
                    Console.WriteLine($"Opzioni selezionate {string.Join(", ", options.Keys)}");
                    for (int i = 0; i < 20; i++)
                    {
                        Console.WriteLine($"Sto lavorando... Procedura {command} - Step {i}");
                        Thread.Sleep(100);
                    }
                    Console.WriteLine($"Fine procedura {command.ToString()}");
                    break;
                default:
                    break;
            }
        }

    }
}
