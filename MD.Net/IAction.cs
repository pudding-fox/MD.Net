namespace MD.Net
{
    public interface IAction
    {
        IDevice Device { get; }

        string Description { get; }

        void Prepare(IToolManager toolManager, IStatus status);

        void Apply(IToolManager toolManager, IStatus status);

        void Commit();
    }
}
