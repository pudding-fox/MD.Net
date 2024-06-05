using System;
using System.Collections.Generic;

namespace MD.Net
{
    public interface IDeviceMonitor : IDisposable
    {
        void AddFilter(DeviceMonitorFilter filter);

        void ClearFilters();

        void Enable();

        void Disable();

        event DeviceMonitorEventHandler Added;

        event DeviceMonitorEventHandler Removed;
    }

    public class DeviceMonitorFilter
    {
        public DeviceMonitorFilter(int vendorId, IEnumerable<int> productIds)
        {
            this.VendorId = vendorId;
            this.ProductIds = productIds;
        }

        public int VendorId { get; private set; }

        public IEnumerable<int> ProductIds { get; private set; }
    }

    public delegate void DeviceMonitorEventHandler(object sender, DeviceMonitorEventArgs e);

    public class DeviceMonitorEventArgs : EventArgs
    {
        public DeviceMonitorEventArgs(string path, string vendorId, string productId, string serialNumber)
        {
            this.Path = path;
            this.VendorId = vendorId;
            this.ProductId = productId;
            this.SerialNumber = serialNumber;
        }

        public string Path { get; private set; }

        public string VendorId { get; private set; }

        public string ProductId { get; private set; }

        public string SerialNumber { get; private set; }
    }
}
