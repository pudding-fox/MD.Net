using System;
using System.Diagnostics;

namespace MD.Net
{
    public interface IToolManager
    {
        Process Start(string path, string args = null);

        int Exec(Process process, out string output, out string error);

        int Exec(Process process, Action<string> outputHandler, Action<string> errorHandler);

        void Throw(Process process, string message);
    }
}
