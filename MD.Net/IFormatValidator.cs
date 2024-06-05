using MD.Net.Resources;
using System;

namespace MD.Net
{
    public interface IFormatValidator
    {
        void Validate(string fileName, out TimeSpan length);
    }

    public class WaveFormatException : Exception
    {
        public WaveFormatException(string fileName) : base(GetMessage(fileName))
        {
            this.FileName = fileName;
        }

        public string FileName { get; private set; }

        private static string GetMessage(string fileName)
        {
            return string.Format(Strings.WaveFormatException_Message, fileName);
        }
    }
}
