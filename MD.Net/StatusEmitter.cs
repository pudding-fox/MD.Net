using System;

namespace MD.Net
{
    public abstract class StatusEmitter : IStatusEmitter
    {
        public StatusEmitter(string message, StatusType type, IStatus status)
        {
            this.Message = message;
            this.Type = type;
            this.Status = status;
        }

        public string Message { get; private set; }

        public StatusType Type { get; private set; }

        public IStatus Status { get; private set; }

        public Action<string> Action
        {
            get
            {
                return this.Emit;
            }
        }

        protected abstract void Emit();

        protected abstract void Emit(string value);

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.IsDisposed || !disposing)
            {
                return;
            }
            this.OnDisposing();
            this.IsDisposed = true;
        }

        protected virtual void OnDisposing()
        {
            this.Emit();
        }

        ~StatusEmitter()
        {
            try
            {
                this.Dispose(true);
            }
            catch
            {
                //Nothing can be done, never throw on GC thread.
            }
        }
    }
}
