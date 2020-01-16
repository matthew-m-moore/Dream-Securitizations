using System;
using Dream.Common;
using Dream.IO.Database;

namespace Dream.ConsoleApp
{
    public class Program
    {
        public static string ConsoleTitle = "Renovate America: Dream.ConsoleApp";
        private static IntPtr _windowHandle;

        private const string _listCommand = "List";
        private const string _exitCommand = "Exit";     

        // Note that the default value of a boolean is false
        public static bool ExitProgram { get; set; }

        public static void Main(string[] args)
        {
            _windowHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            Console.Title = ConsoleTitle;

            DatabaseConnectionSettings.SetDatabaseContextConnectionsDictionary(DatabaseContextSettings.DatabaseContextConnectionsDictionary);

            // If no arguments are supplied to the console application, enter the main wait loop
            if (args.Length < 1)
            {
                var buildNumber = BuildIdentifier.AbbreviatedBuildId;

                Console.WriteLine("Welcome to Dream.");
                Console.WriteLine("Build#: " + buildNumber);
                Console.WriteLine("----------------------------------------------------------");

                MainWaitLoop();
            }
            // If the application is started with the script specified, go ahead an run it with no error try/catch
            else
            {
                ScriptsManager.ExecuteScript(args);
            }
        }

        private static void MainWaitLoop()
        {
            do
            {
                Console.WriteLine("Options:");
                Console.WriteLine("1. Type '" + _listCommand + "' to view all available scripts.");
                Console.WriteLine("2. Type the name of a script to view it's required arguments.");
                Console.WriteLine("3. Type the name of script and it's required arguments, then press enter to run it.");
                Console.WriteLine("4. Type '" + _exitCommand + "' to close the program.");
                Console.WriteLine("(Note: Each argument for a script should be separated by a space.)");
                Console.WriteLine("----------------------------------------------------------");

                var argumentsEntered = Console.ReadLine();
                var arguments = CommandLineArgumentsProcessor.ParseArguments(argumentsEntered);

                InterpretArguments(arguments);

                // Just add some spacing between each iteration
                Console.WriteLine();
                Console.WriteLine();
            }
            while (!ExitProgram);
        }

        private static void InterpretArguments(string[] arguments)
        {
            if (arguments == null)
            {
                return;
            }

            // One word arguments are assumed to be keywords
            if (arguments.Length == 1)
            {
                var commandEntered = arguments[0];
                switch (commandEntered)
                {
                    case _listCommand:
                        ScriptsManager.ListAllScripts();
                        return;

                    case _exitCommand:
                        ExitProgram = true;
                        return;

                    default:
                        if (ScriptsManager.GetListOfAllScriptNames().Contains(commandEntered))
                        {
                            ScriptsManager.ListScriptArguments(commandEntered, arguments);
                        }
                        else
                        {
                            Console.WriteLine("Sorry, '" + commandEntered + "' is not a valid command.");
                            Console.WriteLine("Press any key to continue.");
                            Console.ReadLine();
                        }                      
                        return;
                }
            }

            if (arguments.Length < 1)
            {
                Console.WriteLine("No script name was provided. Please try again.");
                Console.WriteLine("Press any key to continue.");
                Console.ReadLine();
                return;
            }

            ScriptsManager.TryExecuteScript(arguments);
        }
    }
}
