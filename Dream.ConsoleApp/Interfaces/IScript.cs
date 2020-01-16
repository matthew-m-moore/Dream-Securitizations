using System.Collections.Generic;

namespace Dream.ConsoleApp.Interfaces
{
    /// <summary>
    /// Members of this interface are the scripts that can run through the console application.
    /// The interface requires that a list of arguments is able to be supplied.
    /// </summary>
    public interface IScript
    {
        void RunScript(string[] args);
        List<string> GetArgumentsList();
        string GetFriendlyName();
        bool GetVisibilityStatus();
    }
}
