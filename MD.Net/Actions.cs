using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MD.Net
{
    public class Actions : IActions
    {
        public Actions(IDevice device, IDisc currentDisc, IDisc updatedDisc, IEnumerable<IAction> actions)
        {
            this.Device = device;
            this.CurrentDisc = currentDisc;
            this.UpdatedDisc = updatedDisc;
            this.Store = actions.ToList();
        }

        public IDevice Device { get; private set; }

        public IDisc CurrentDisc { get; private set; }

        public IDisc UpdatedDisc { get; private set; }

        public IList<IAction> Store { get; private set; }

        public int Count
        {
            get
            {
                return this.Store.Count;
            }
        }

        public IEnumerator<IAction> GetEnumerator()
        {
            return this.Store.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Store.GetEnumerator();
        }

        public static IActions None
        {
            get
            {
                return new Actions(global::MD.Net.Device.None, Disc.None, Disc.None, Enumerable.Empty<IAction>());
            }
        }
    }
}
