using System.Collections.Generic;

namespace MD.Net
{
    public interface IDeviceManager
    {
        IEnumerable<IDevice> GetDevices();

        IResult Erase(IDevice device);
    }
}
