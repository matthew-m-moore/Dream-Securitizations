using Dream.ConsoleApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dream.ConsoleApp
{
    public class ScriptsManager
    {
        internal static void TryExecuteScript(string[] args)
        {
            try
            {
                ExecuteScript(args);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Console.WriteLine("Press any key to continue.");
                Console.ReadLine();
            }
        }

        internal static void ExecuteScript(string[] args)
        {
            var scriptName = args[0];
            var scriptToExecute = GetInstanceOfScript<IScript>(scriptName);

            if (scriptToExecute != null)
            {
                var consoleTitleWhileRunning = Program.ConsoleTitle + " (Running " + scriptName + ")";

                Console.Title = consoleTitleWhileRunning;
                scriptToExecute.RunScript(args);
                Console.Title = Program.ConsoleTitle;
            }
            else
            {
                Console.WriteLine("Sorry, '" + scriptName + "' is not a valid script.");
                Console.WriteLine("Press any key to continue.");
                Console.ReadLine();
            }
        }

        internal static void ListAllScripts()
        {
            var listOfScriptNames = GetListOfAllScriptNames();
            var orderedListOfScriptNames = listOfScriptNames.OrderBy(n => n);

            var scriptNumber = 1;
            foreach (var scriptName in orderedListOfScriptNames)
            {
                Console.WriteLine(scriptNumber + " : " + scriptName);
                scriptNumber++;
            }
        }

        internal static void ListScriptArguments(string scriptName, string[] arguments)
        {
            var scriptToListArguments = GetInstanceOfScript<IScript>(scriptName);
            var argumentsList = scriptToListArguments.GetArgumentsList();

            // Some scripts have no arguments. In such a case, go ahead and try to execute.
            if (argumentsList.Any())
            {
                foreach (var argument in argumentsList)
                {
                    Console.WriteLine(argument);
                }
            }
            else
            {
                TryExecuteScript(arguments);
            }
        }

        public static IEnumerable<Type> GetEnumerableOfAllScripts()
        {
            // Use reflection to find anything that belongs to the script interface,
            // and make sure that an empty constructor exists.

            var listOfScripts = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IScript))
                         && t.GetConstructor(Type.EmptyTypes) != null);

            return listOfScripts;
        }

        public static List<string> GetListOfAllScriptNames()
        {
            return GetEnumerableOfAllScripts().Select(s => s.Name).ToList();
        }

        public static T GetInstanceOfScript<T>(string scriptName) where T : class, IScript
        {
            var listOfAllScripts = GetEnumerableOfAllScripts().ToList();
            var specificScriptToRun = listOfAllScripts.FirstOrDefault(s => s.Name == scriptName);

            if (specificScriptToRun != null)
            {
                return (T)Activator.CreateInstance(specificScriptToRun);
            }

            return null;
        }
    }
}
