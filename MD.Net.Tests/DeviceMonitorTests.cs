using NUnit.Framework;
using System;
using System.Threading;

namespace MD.Net.Tests
{
    [TestFixture]
    public class DeviceMonitorTests
    {
        [Test]
        [Explicit]
        public void Enable()
        {
            using (var deviceMonitor = new DeviceMonitor())
            {
                deviceMonitor.Added += this.OnAdded;
                deviceMonitor.Removed += this.OnRemoved;
                deviceMonitor.Enable();
                Thread.Sleep(TimeSpan.FromMinutes(1));
                deviceMonitor.Disable();
            }
        }

        protected void OnAdded(object sender, DeviceMonitorEventArgs e)
        {
            TestContext.WriteLine("Added: {0} (Vendor={1},Product={2},Serial={3})", e.Path, e.VendorId, e.ProductId, e.SerialNumber);
        }

        protected void OnRemoved(object sender, DeviceMonitorEventArgs e)
        {
            TestContext.WriteLine("Removed: {0} (Vendor={1},Product={2},Serial={3})", e.Path, e.VendorId, e.ProductId, e.SerialNumber);
        }
    }
}
