using MD.Net.Resources;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Diagnostics;

namespace MD.Net.Tests
{
    [TestFixture]
    public class DiscManagerTests
    {
        [Test]
        public void NoDisk()
        {
            var formatValidator = MockRepository.GenerateStub<IFormatValidator>();
            var toolManager = MockRepository.GenerateStub<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI)).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(Resources.Sony_MDS_JE780___No_Disk).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var discManager = new DiscManager(toolManager, formatValidator);
            var device = new Device("Sony MDS-JE780/JB980");
            var disc = discManager.GetDisc(device);
            Assert.IsNotNull(disc);
            Assert.AreEqual("<Untitled>", disc.Title);
            Assert.AreEqual(new TimeSpan(1143, 67, 0), disc.RecordedTime);
            Assert.AreEqual(new TimeSpan(80, 0, 0), disc.TotalTime);
            Assert.AreEqual(new TimeSpan(0, 11, 0), disc.AvailableTime);
        }

        [Test]
        public void Evanescence()
        {
            var formatValidator = MockRepository.GenerateStub<IFormatValidator>();
            var toolManager = MockRepository.GenerateStub<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI)).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(Resources.Sony_MDS_JE780___Evanescence).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var discManager = new DiscManager(toolManager, formatValidator);
            var device = new Device("Sony MDS-JE780/JB980");
            var disc = discManager.GetDisc(device);
            Assert.IsNotNull(disc);
            Assert.AreEqual("Evanescence", disc.Title);
            Assert.AreEqual(new TimeSpan(0, 0, 47, 20, 104), disc.RecordedTime);
            Assert.AreEqual(new TimeSpan(0, 1, 14, 59, 487), disc.TotalTime);
            Assert.AreEqual(new TimeSpan(0, 0, 27, 26, 951), disc.AvailableTime);
            Assert.AreEqual(12, disc.Tracks.Count);
            var capacity = disc.GetCapacity();
            Assert.AreEqual(63, capacity.PercentUsed);
            Assert.AreEqual(37, capacity.PercentFree);
        }

        [TestCase("Original Title", "New Title")]
        public void UpdateDiscTitle(string currentTitle, string updatedTitle)
        {
            var status = Status.Ignore;
            var formatValidator = MockRepository.GenerateStub<IFormatValidator>();
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, string.Format("{0} \"{1}\"", Constants.NETMDCLI_SETTITLE, updatedTitle))).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var device = new Device("Sony MDS-JE780/JB980");
            var currentDisc = new Disc(currentTitle, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, Tracks.None);
            var updatedDisc = new Disc(currentDisc)
            {
                Title = updatedTitle
            };
            var actions = new Actions(
                device,
                currentDisc,
                updatedDisc,
                new[]
                {
                    new UpdateDiscTitleAction(device,currentDisc,updatedDisc)
                }
            );
            var discManager = new DiscManager(toolManager, formatValidator);
            var result = discManager.ApplyActions(device, actions, status, false);
            Assert.AreEqual(ResultStatus.Success, result.Status);
        }

        [TestCase("Original Name", "New Name")]
        public void UpdateTrackName(string currentName, string updatedName)
        {
            var status = Status.Ignore;
            var formatValidator = MockRepository.GenerateStub<IFormatValidator>();
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, string.Format("{0} {1} {2}", Constants.NETMDCLI_RENAME, 1, updatedName))).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var device = new Device("Sony MDS-JE780/JB980");
            var track1 = new Track(0, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var track2 = new Track(1, Protection.None, Compression.None, TimeSpan.Zero, currentName);
            var track3 = new Track(2, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var currentDisc = new Disc(string.Empty, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, new Tracks(formatValidator, new[] { track1, track2, track3 }));
            var updatedDisc = new Disc(currentDisc);
            updatedDisc.Tracks[1].Name = updatedName;
            var actions = new Actions(
                device,
                currentDisc,
                updatedDisc,
                new[]
                {
                    new UpdateTrackNameAction(device, currentDisc, updatedDisc, currentDisc.Tracks[1], updatedDisc.Tracks[1])
                }
            );
            var discManager = new DiscManager(toolManager, formatValidator);
            var result = discManager.ApplyActions(device, actions, status, false);
            Assert.AreEqual(ResultStatus.Success, result.Status);
        }

        [TestCase(@"C:\My Music\Test.wav", Compression.None, "00:03:30", "This is a test.")]
        public void AddTrack(string location, Compression compression, TimeSpan time, string name)
        {
            var status = Status.Ignore;
            var formatValidator = MockRepository.GenerateStub<IFormatValidator>();
            formatValidator.Expect(fm => fm.Validate(Arg<string>.Is.Equal(location), out Arg<TimeSpan>.Out(time).Dummy));
            var formatManager = MockRepository.GenerateStub<IFormatManager>();
            formatManager.Expect(fm => fm.Convert(location, compression, status)).Return(location);
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, string.Format("{0} {1} \"{2}\" \"{3}\"", Constants.NETMDCLI_VERBOSE, Constants.NETMDCLI_SEND, location, name))).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, Arg<Action<string>>.Is.Anything, Arg<Action<string>>.Is.Anything)).Return(0);
            var device = new Device("Sony MDS-JE780/JB980");
            var track1 = new Track(0, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var track2 = new Track(1, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var track3 = new Track(2, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var currentDisc = new Disc(string.Empty, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, new Tracks(formatValidator, new[] { track1, track2, track3 }));
            var updatedDisc = new Disc(currentDisc);
            var track = updatedDisc.Tracks.Add(location, compression);
            Assert.AreEqual(time, track.Time);
            track.Name = name;
            var actions = new Actions(
                device,
                currentDisc,
                updatedDisc,
                new[]
                {
                    new AddTrackAction(formatManager, device, currentDisc, updatedDisc, track)
                }
            );
            var discManager = new DiscManager(toolManager, formatValidator);
            var result = discManager.ApplyActions(device, actions, status, false);
            Assert.AreEqual(ResultStatus.Success, result.Status);
        }

        [Test]
        public void RemoveTrack()
        {
            var status = Status.Ignore;
            var formatValidator = MockRepository.GenerateStub<IFormatValidator>();
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, string.Format("{0} {1}", Constants.NETMDCLI_DELETE, 1))).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var device = new Device("Sony MDS-JE780/JB980");
            var track1 = new Track(0, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var track2 = new Track(1, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var track3 = new Track(2, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var currentDisc = new Disc(string.Empty, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, new Tracks(formatValidator, new[] { track1, track2, track3 }));
            var updatedDisc = new Disc(currentDisc);
            var track = updatedDisc.Tracks[1];
            updatedDisc.Tracks.Remove(track);
            var actions = new Actions(
                device,
                currentDisc,
                updatedDisc,
                new[]
                {
                    new RemoveTrackAction(device, currentDisc, updatedDisc, track)
                }
            );
            var discManager = new DiscManager(toolManager, formatValidator);
            var result = discManager.ApplyActions(device, actions, status, false);
            Assert.AreEqual(ResultStatus.Success, result.Status);
        }

        [TestCase("\"title\":\"Evanescence\"", "\"title\":\"Other\"")]
        [TestCase("\"name\":\"What You Want - Evanescence\"", "\"name\":\"Other\"")]
        public void Error_DiscWasModified(string oldValue, string newValue)
        {
            var status = Status.Ignore;
            var formatValidator = MockRepository.GenerateStub<IFormatValidator>();
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI)).Return(null).Repeat.Once();
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(Resources.Sony_MDS_JE780___Evanescence).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0).Repeat.Once();
            var discManager = new DiscManager(toolManager, formatValidator);
            var device = new Device("Sony MDS-JE780/JB980");
            var disc = discManager.GetDisc(device);
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI)).Return(null).Repeat.Once();
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(Resources.Sony_MDS_JE780___Evanescence.Replace(oldValue, newValue)).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0).Repeat.Once();
            var actions = new Actions(
                device,
                disc,
                Disc.None,
                Actions.None
            );
            var result = discManager.ApplyActions(device, actions, status, true);
            Assert.AreEqual(ResultStatus.Failure, result.Status);
            Assert.AreEqual(Strings.Error_UnexpectedDisc, result.Message);
        }
    }
}
