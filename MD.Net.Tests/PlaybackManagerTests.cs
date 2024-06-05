using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Diagnostics;

namespace MD.Net.Tests
{
    [TestFixture]
    public class PlaybackManagerTests
    {
        [Test]
        public void GetStatus()
        {
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, Constants.NETMDCLI_STATUS)).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(Resources.Status_You).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var playbackManager = new PlaybackManager(toolManager);
            var name = default(string);
            var position = default(TimeSpan);
            Assert.IsTrue(playbackManager.GetStatus(Device.None, out name, out position));
            Assert.AreEqual("You", name);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 40, 638), position);
        }

        [Test]
        public void Play()
        {
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, Constants.NETMDCLI_PLAY)).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var playbackManager = new PlaybackManager(toolManager);
            playbackManager.Play(Device.None);
        }

        [Test]
        public void PlayTrack()
        {
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, string.Format("{0} {1}", Constants.NETMDCLI_PLAY, Track.None.Position))).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var playbackManager = new PlaybackManager(toolManager);
            playbackManager.Play(Device.None, Track.None);
        }

        [Test]
        public void Next()
        {
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, Constants.NETMDCLI_NEXT)).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var playbackManager = new PlaybackManager(toolManager);
            playbackManager.Next(Device.None);
        }

        [Test]
        public void Previous()
        {
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, Constants.NETMDCLI_PREVIOUS)).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var playbackManager = new PlaybackManager(toolManager);
            playbackManager.Previous(Device.None);
        }

        [Test]
        public void Pause()
        {
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, Constants.NETMDCLI_PAUSE)).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var playbackManager = new PlaybackManager(toolManager);
            playbackManager.Pause(Device.None);
        }

        [Test]
        public void Stop()
        {
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, Constants.NETMDCLI_STOP)).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var playbackManager = new PlaybackManager(toolManager);
            playbackManager.Stop(Device.None);
        }

        [TestCase("00:03:45.0", 0, 3, 45, 0)]
        public void Seek(string position, int hour, int minute, int second, int frame)
        {
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, string.Format("{0} {1} {2} {3} {4}", Constants.NETMDCLI_SETTIME, hour, minute, second, frame))).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var playbackManager = new PlaybackManager(toolManager);
            playbackManager.Seek(Device.None, TimeSpan.Parse(position));
        }
    }
}
