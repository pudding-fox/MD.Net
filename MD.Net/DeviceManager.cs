using System.Collections.Generic;
using System.Linq;
using TinyJson;

namespace MD.Net
{
    public class DeviceManager : IDeviceManager
    {
        public DeviceManager(IToolManager toolManager)
        {
            this.ToolManager = toolManager;
        }

        public IToolManager ToolManager { get; private set; }

        public IEnumerable<IDevice> GetDevices()
        {
            var output = default(string);
            var error = default(string);
            var process = this.ToolManager.Start(Tools.NETMDCLI);
            var code = this.ToolManager.Exec(process, out output, out error);
            if (code != 0 || string.IsNullOrEmpty(output) || output.Contains(Constants.NETMDCLI_NO_DEVICE, true) || output.Contains(Constants.NETMDCLI_POLL_FAILED, true))
            {
                return Enumerable.Empty<IDevice>();
            }
            return this.GetDevices(output);
        }

        protected IEnumerable<IDevice> GetDevices(string output)
        {
            //Only a single device is supported.
            var device = output.FromJson<_Device>();
            return new[]
            {
                new Device(device.device)
            };
        }

        public IResult Erase(IDevice device)
        {
            var output = default(string);
            var error = default(string);
            var process = this.ToolManager.Start(Tools.NETMDCLI, Constants.NETMDCLI_ERASE);
            var code = this.ToolManager.Exec(process, out output, out error);
            if (code != 0)
            {
                return Result.Failure(error);
            }
            return Result.Success;
        }

#pragma warning disable 0169, 0649

        private struct _Device
        {
            public string device;
        }

#pragma warning restore 0169, 0649

    }
}
