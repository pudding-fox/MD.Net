using MD.Net.Resources;
using System;
using System.Diagnostics;
using System.Text;

namespace MD.Net
{
    public class ToolManager : IToolManager
    {
        public Process Start(string path, string args)
        {
            var info = new ProcessStartInfo(path, args);
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            return Process.Start(info);
        }

        public int Exec(Process process, out string output, out string error)
        {
            var _output = new StringBuilder();
            var _error = new StringBuilder();
            try
            {
                return this.Exec(process, data => _output.AppendLine(data), data => _error.AppendLine(data));
            }
            finally
            {
                output = _output.ToString();
                error = _error.ToString();
            }
        }

        public int Exec(Process process, Action<string> outputHandler, Action<string> errorHandler)
        {
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    outputHandler(e.Data);
                }
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    errorHandler(e.Data);
                }
            };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            return process.ExitCode;
        }

        public void Throw(Process process, string message)
        {
            throw new ToolException(process.StartInfo.FileName, process.StartInfo.Arguments, process.ExitCode, message);
        }
    }

    public class ToolException : Exception
    {
        public ToolException(string path, string args, int code, string message) : base(GetMessage(path, args, code, message))
        {
            this.Path = path;
            this.Args = args;
            this.Code = code;
        }

        public string Path { get; private set; }

        public string Args { get; private set; }

        public int Code { get; private set; }

        private static string GetMessage(string path, string args, int code, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                message = Strings.ToolException_UnknownError;
            }
            return string.Format(Strings.ToolException_Message, path, args, code, message);
        }
    }
}
