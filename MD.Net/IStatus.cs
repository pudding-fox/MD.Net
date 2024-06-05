using System;

namespace MD.Net
{
    public interface IStatus
    {
        void Update(string message, int position, int count, StatusType type);

        event StatusEventHandler Updated;
    }

    public delegate void StatusEventHandler(object sender, StatusEventArgs e);

    public class StatusEventArgs : EventArgs
    {
        public StatusEventArgs(string message, int position, int count, StatusType type)
        {
            this.Message = message;
            this.Position = position;
            this.Count = count;
            this.Type = type;
        }

        public string Message { get; private set; }

        public int Position { get; private set; }

        public int Count { get; private set; }

        public StatusType Type { get; private set; }
    }

    public enum StatusType
    {
        None,
        Action,
        Transfer,
        Encode
    }
}
