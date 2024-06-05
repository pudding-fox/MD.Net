namespace MD.Net
{
    public abstract class ActionBase : IAction
    {
        protected ActionBase(IDevice device)
        {
            this.Device = device;
        }

        public IDevice Device { get; private set; }

        public abstract string Description { get; }

        public virtual void Prepare(IToolManager toolManager, IStatus status)
        {
            //Nothing to do.
        }

        public abstract void Apply(IToolManager toolManager, IStatus status);

        public virtual void Commit()
        {
            //Nothing to do.
        }
    }
}
