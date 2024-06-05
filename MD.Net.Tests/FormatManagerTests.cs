using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Diagnostics;
using System.IO;

namespace MD.Net.Tests
{
    [TestFixture]
    public class FormatManagerTests
    {
        public static readonly object[][] ConvertSource = new object[][]
        {
            //No conversion required.
            new object[] { "Track_001", Compression.None, WavHeader.WAV_FORMAT_PCM, 2, 44100, 16, 4, 176400 },
            new object[] { "Track_002", Compression.None, WavHeader.WAV_FORMAT_PCM, 2, 44100, 16, 4, 176400 },
            new object[] { "Track_003", Compression.None, WavHeader.WAV_FORMAT_PCM, 2, 44100, 16, 4, 176400 },
            //Supported conversions.
            new object[] { "Track_001", Compression.LP2, WavHeader.WAV_FORMAT_ATRAC3, 2, 44100, 0, 384, 16537 },
            new object[] { "Track_002", Compression.LP2, WavHeader.WAV_FORMAT_ATRAC3, 2, 44100, 0, 384, 16537 },
            new object[] { "Track_003", Compression.LP2, WavHeader.WAV_FORMAT_ATRAC3, 2, 44100, 0, 384, 16537 },
            new object[] { "Track_001", Compression.LP4, WavHeader.WAV_FORMAT_ATRAC3, 2, 44100, 0, 192, 8268 },
            new object[] { "Track_002", Compression.LP4, WavHeader.WAV_FORMAT_ATRAC3, 2, 44100, 0, 192, 8268 },
            new object[] { "Track_003", Compression.LP4, WavHeader.WAV_FORMAT_ATRAC3, 2, 44100, 0, 192, 8268 },
            //No conversion required.
            new object[] { "Track_001_LP2_at3", Compression.LP2, WavHeader.WAV_FORMAT_ATRAC3, 2, 44100, 0, 384, 16537 },
            new object[] { "Track_002_LP2_at3", Compression.LP2, WavHeader.WAV_FORMAT_ATRAC3, 2, 44100, 0, 384, 16537 },
            new object[] { "Track_003_LP2_at3", Compression.LP2, WavHeader.WAV_FORMAT_ATRAC3, 2, 44100, 0, 384, 16537 },
            //No conversion required.
            new object[] { "Track_001_LP4_at3", Compression.LP4, WavHeader.WAV_FORMAT_ATRAC3, 2, 44100, 0, 192, 8268 },
            new object[] { "Track_002_LP4_at3", Compression.LP4, WavHeader.WAV_FORMAT_ATRAC3, 2, 44100, 0, 192, 8268 },
            new object[] { "Track_003_LP4_at3", Compression.LP4, WavHeader.WAV_FORMAT_ATRAC3, 2, 44100, 0, 192, 8268 },
        };

        [Explicit]
        [TestCaseSource("ConvertSource")]
        public void Convert(string name, Compression compression, int format, int channelCount, int sampleRate, int bitsPerSample, int blockAlign, int byteRate)
        {
            var status = Status.Ignore;
            var toolManager = new ToolManager();
            var formatValidator = new FormatValidator();
            var formatManager = new FormatManager(toolManager);
            var inputFileName = Path.Combine(Path.GetTempPath(), name + ".wav");
            using (var reader = Resources.ResourceManager.GetStream(name))
            {
                using (var writer = File.Create(inputFileName))
                {
                    reader.CopyTo(writer);
                }
            }
            var length = default(TimeSpan);
            formatValidator.Validate(inputFileName, out length);
            var outputFileName = formatManager.Convert(inputFileName, compression, status);
            var info = default(WavHeader.WavInfo);
            using (var reader = File.OpenRead(outputFileName))
            {
                Assert.IsTrue(WavHeader.Read(reader, out info));
                Assert.AreEqual(reader.Length - WavHeader.WAV_HEADER_OFFSET, info.FileSize);
                Assert.AreEqual(format, info.Format);
                Assert.AreEqual(channelCount, info.ChannelCount);
                Assert.AreEqual(sampleRate, info.SampleRate);
                Assert.AreEqual(bitsPerSample, info.BitsPerSample);
                Assert.AreEqual(blockAlign, info.BlockAlign);
                Assert.AreEqual(byteRate, info.ByteRate);
                Assert.AreEqual(reader.Length - reader.Position, info.DataSize);
            }
            if (File.Exists(inputFileName))
            {
                File.Delete(inputFileName);
            }
            if (File.Exists(outputFileName))
            {
                File.Delete(outputFileName);
            }
        }

        [TestCase(@"C:\My Music\Test.wav", @"C:\My Music\Test.at3", Compression.LP2, "128")]
        [TestCase(@"C:\My Music\Test.wav", @"C:\My Music\Test.at3", Compression.LP4, "64")]
        public void Convert(string inputFileName, string outputFileName, Compression compression, string bitrate)
        {
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.ATRACDENC, string.Format("{0} {1} {2} \"{3}\" {4} \"{5}\" {6} {7}", Constants.ATRACDENC_ENCODE, Constants.ATRACDENC_ATRAC3, Constants.ATRACDENC_INPUT, inputFileName, Constants.ATRACDENC_OUTPUT, outputFileName, Constants.ATRACDENC_BITRATE, bitrate)));
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, Arg<Action<string>>.Is.Anything, Arg<Action<string>>.Is.Anything)).WhenCalled(invocation =>
            {
                Utility.EmitLines(Resources.Convert_Atrac, invocation.Arguments[1] as Action<string>);
            }).Return(0);
            var formatManager = new FormatManager(toolManager);
            var status = MockRepository.GenerateStrictMock<IStatus>();
            for (var a = 0; a <= 100; a++)
            {
                status.Expect(s => s.Update(Arg<string>.Is.Anything, Arg<int>.Is.Equal(a), Arg<int>.Is.Equal(100), Arg<StatusType>.Is.Equal(StatusType.Encode))).Repeat.Once();
            }
            Assert.AreEqual(outputFileName, formatManager.Convert(inputFileName, compression, status));
            status.VerifyAllExpectations();
        }

        [Test]
        public void Error_Unsupported()
        {
            var status = Status.Ignore;
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            var formatManager = new FormatManager(toolManager);
            var inputFileName = Path.GetTempFileName();
            try
            {
                var outputFileName = formatManager.Convert(inputFileName, Compression.LP2, status);
                Assert.Fail();
            }
            catch (WaveFormatException e)
            {
                Assert.IsTrue(e.Message.Contains("unsupported", true));
            }
            finally
            {
                File.Delete(inputFileName);
            }
        }
    }
}
