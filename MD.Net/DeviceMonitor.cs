using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

namespace MD.Net
{
    public class DeviceMonitor : IDeviceMonitor
    {
        public static readonly Regex DeviceID = new Regex(
            @"\.DeviceID=""(.*)""",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public static readonly Regex VendorId = new Regex(
            @"(?:VID|VEN)_(\w+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public static readonly Regex ProductId = new Regex(
            @"(?:PID|PROD)_(\w+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public static readonly Regex SerialNumber = new Regex(
            @"\\([\w&]+)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        private static bool GetDeviceID(string value, out string deviceID)
        {
            var match = DeviceID.Match(value);
            if (!match.Success)
            {
                deviceID = string.Empty;
                return false;
            }
            deviceID = match.Groups[1].Value;
            return true;
        }

        private static bool GetDevice(string value, out string vendorId, out string productId, out string serialNumber)
        {
            var vendorMatch = VendorId.Match(value);
            var productMatch = ProductId.Match(value);
            var serialMatch = SerialNumber.Match(value);

            if (!vendorMatch.Success || !productMatch.Success || !serialMatch.Success)
            {
                vendorId = string.Empty;
                productId = string.Empty;
                serialNumber = string.Empty;
                return false;
            }

            vendorId = vendorMatch.Groups[1].Value;
            productId = productMatch.Groups[1].Value;
            serialNumber = serialMatch.Groups[1].Value;
            return true;
        }

        public DeviceMonitor()
        {
            this.Filters = new List<DeviceMonitorFilter>();
            this.Filters.Add(new DeviceMonitorFilter(DeviceIDs.Sony, DeviceIDs.SonyProducts.Keys.ToArray()));
            this.Filters.Add(new DeviceMonitorFilter(DeviceIDs.Sharp, DeviceIDs.SharpProducts.Keys.ToArray()));
        }

        public List<DeviceMonitorFilter> Filters { get; private set; }

        public ManagementEventWatcher InstanceCreationEvent { get; private set; }

        public ManagementEventWatcher InstanceDeletionEvent { get; private set; }

        public void AddFilter(DeviceMonitorFilter filter)
        {
            this.Filters.Add(filter);
        }

        public void ClearFilters()
        {
            this.Filters.Clear();
        }

        public void Enable()
        {
            if (this.InstanceCreationEvent == null)
            {
                const string query = "SELECT* FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBControllerDevice'";
                this.InstanceCreationEvent = new ManagementEventWatcher(query);
                this.InstanceCreationEvent.EventArrived += this.OnInstanceCreationEvent;
                this.InstanceCreationEvent.Start();
            }
            if (this.InstanceDeletionEvent == null)
            {
                const string query = "SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBControllerDevice'";
                this.InstanceCreationEvent = new ManagementEventWatcher(query);
                this.InstanceCreationEvent.EventArrived += this.OnInstanceDeletionEvent;
                this.InstanceCreationEvent.Start();
            }
        }

        public void Disable()
        {
            if (this.InstanceCreationEvent != null)
            {
                this.InstanceCreationEvent.EventArrived -= this.OnInstanceCreationEvent;
                this.InstanceCreationEvent.Stop();
                this.InstanceCreationEvent.Dispose();
            }
            if (this.InstanceDeletionEvent != null)
            {
                this.InstanceDeletionEvent.EventArrived -= this.OnInstanceDeletionEvent;
                this.InstanceDeletionEvent.Stop();
                this.InstanceDeletionEvent.Dispose();
            }
        }

        protected virtual void OnInstanceCreationEvent(object sender, EventArrivedEventArgs e)
        {
            var targetInstance = e.NewEvent["TargetInstance"] as ManagementBaseObject;
            if (targetInstance == null)
            {
                return;
            }
            var device = default(_Device);
            if (!this.GetDevice(targetInstance, out device))
            {
                return;
            }
            this.OnAdded(new DeviceMonitorEventArgs(device.Path, device.VendorId, device.ProductId, device.SerialNumber));
        }

        protected virtual void OnInstanceDeletionEvent(object sender, EventArrivedEventArgs e)
        {
            var targetInstance = e.NewEvent["TargetInstance"] as ManagementBaseObject;
            if (targetInstance == null)
            {
                return;
            }
            var device = default(_Device);
            if (!this.GetDevice(targetInstance, out device))
            {
                return;
            }
            this.OnRemoved(new DeviceMonitorEventArgs(device.Path, device.VendorId, device.ProductId, device.SerialNumber));
        }

        private bool GetDevice(ManagementBaseObject value, out _Device device)
        {
            device = default(_Device);

            var antecedent = value["Antecedent"] as string;
            var dependent = value["Dependent"] as string;

            if (string.IsNullOrEmpty(antecedent) || string.IsNullOrEmpty(dependent))
            {
                return false;
            }

            return this.GetDevice(antecedent, dependent, out device);
        }

        private bool GetDevice(string antecedent, string dependent, out _Device device)
        {
            device = default(_Device);

            var deviceId = default(string);
            if (!GetDeviceID(dependent, out deviceId))
            {
                return false;
            }

            var vendorId = default(string);
            var productId = default(string);
            var serialNumber = default(string);

            if (!GetDevice(deviceId, out vendorId, out productId, out serialNumber))
            {
                return false;
            }

            device = new _Device()
            {
                Path = deviceId,
                VendorId = vendorId,
                ProductId = productId,
                SerialNumber = serialNumber
            };
            return true;
        }

        protected virtual void OnAdded(DeviceMonitorEventArgs e)
        {
            if (this.Added == null)
            {
                return;
            }
            if (!this.MatchesFilter(e))
            {
                return;
            }
            this.Added(this, e);
        }

        public event DeviceMonitorEventHandler Added;

        protected virtual void OnRemoved(DeviceMonitorEventArgs e)
        {
            if (this.Removed == null)
            {
                return;
            }
            if (!this.MatchesFilter(e))
            {
                return;
            }
            this.Removed(this, e);
        }

        public event DeviceMonitorEventHandler Removed;

        protected virtual bool MatchesFilter(DeviceMonitorEventArgs e)
        {
            if (this.Filters.Count == 0)
            {
                return true;
            }
            foreach (var filter in this.Filters)
            {
                if (this.MatchesFilter(filter, e))
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual bool MatchesFilter(DeviceMonitorFilter filter, DeviceMonitorEventArgs e)
        {
            var vendorId = default(int);
            var productId = default(int);

            if (!DeviceIDs.TryParse(e.VendorId, out vendorId) || !DeviceIDs.TryParse(e.ProductId, out productId))
            {
                return false;
            }

            return this.MatchesFilter(filter, vendorId, productId);
        }

        protected virtual bool MatchesFilter(DeviceMonitorFilter filter, int vendorId, int productId)
        {
            return filter.VendorId == vendorId && filter.ProductIds.Contains(productId);
        }

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
            this.Disable();
        }

        ~DeviceMonitor()
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

        private struct _Device
        {
            public string Path;

            public string VendorId;

            public string ProductId;

            public string SerialNumber;
        }
    }
}
