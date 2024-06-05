using System.Collections.Generic;

namespace MD.Net
{
    public interface IActions : IEnumerable<IAction>
    {
        IDevice Device { get; }

        IDisc CurrentDisc { get; }

        IDisc UpdatedDisc { get; }

        int Count { get; }
    }
}
