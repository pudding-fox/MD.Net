using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Diagnostics;

namespace MD.Net.Tests
{
    [TestFixture]
    public class AddTrackActionTests
    {
        [TestCase(@"C:\My Music\Test.wav", Compression.None)]
        public void Apply(string location, Compression compression)
        {
            var track = new Track(0, Protection.None, compression, TimeSpan.Zero, string.Empty)
            {
                Location = location
            };
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, string.Format("{0} {1} \"{2}\"", Constants.NETMDCLI_VERBOSE, Constants.NETMDCLI_SEND, track.Location)));
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, Arg<Action<string>>.Is.Anything, Arg<Action<string>>.Is.Anything)).WhenCalled(invocation =>
            {
                Utility.EmitLines(Resources.Send_SP, invocation.Arguments[1] as Action<string>);
            }).Return(0);
            var status = MockRepository.GenerateStrictMock<IStatus>();
            for (var a = 0; a <= 100; a++)
            {
                status.Expect(s => s.Update(Arg<string>.Is.Anything, Arg<int>.Is.Equal(a), Arg<int>.Is.Equal(100), Arg<StatusType>.Is.Equal(StatusType.Transfer))).Repeat.Once();
            }
            var formatManager = MockRepository.GenerateStub<IFormatManager>();
            formatManager.Expect(fm => fm.Convert(location, compression, status)).Return(location);
            var action = new AddTrackAction(formatManager, Device.None, Disc.None, Disc.None, track);
            action.Prepare(toolManager, status);
            action.Apply(toolManager, status);
            action.Commit();
            status.VerifyAllExpectations();
        }
    }
}
