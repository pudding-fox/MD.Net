using NUnit.Framework;
using Rhino.Mocks;
using System.Diagnostics;
using System.Linq;

namespace MD.Net.Tests
{
    [TestFixture]
    public class DeviceManagerTests
    {
        [TestCase("no netmd device")]
        [TestCase("netmd_poll failed")]
        [TestCase("")]
        public void NoDevices(string output)
        {
            var toolManager = MockRepository.GenerateStub<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI)).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(output).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var deviceManager = new DeviceManager(toolManager);
            var devices = deviceManager.GetDevices();
            Assert.IsFalse(devices.Any());
        }

        [Test]
        public void NoDisk()
        {
            var toolManager = MockRepository.GenerateStub<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI)).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(Resources.Sony_MDS_JE780___No_Disk).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var deviceManager = new DeviceManager(toolManager);
            var devices = deviceManager.GetDevices();
            var device = devices.SingleOrDefault();
            Assert.IsNotNull(device);
            Assert.AreEqual("Sony MDS-JE780/JB980", device.Name);
        }

        [Test]
        public void Erase()
        {
            var toolManager = MockRepository.GenerateStub<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, Constants.NETMDCLI_ERASE)).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var deviceManager = new DeviceManager(toolManager);
            deviceManager.Erase(Device.None);
        }
    }
}
