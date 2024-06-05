using System;

namespace MD.Net
{
    public interface IStatusEmitter : IDisposable
    {
        Action<string> Action { get; }
    }
}
