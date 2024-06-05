using System;
using System.IO;
using System.Text;
using System.Linq;

namespace MD.Net
{
    public static class OMAHeader
    {
        public static byte[] OMA_TAG_EA3 = Encoding.ASCII.GetBytes("EA3");

        public const int OMA_HEADER_SIZE = 96;

        public const byte OMA_CODEC_ATRAC3 = 0;

        public const byte OMA_CODEC_ATRAC3PLUS = 1;

        public static int[] OMA_SAMPLERATES = { 32000, 44100, 48000, 88200, 96000 };

        public const byte OMA_MONO = 0;

        public const byte OMA_STEREO = 1;

        public const byte OMA_JOINT_STEREO = 2;

        public const byte OMA_3 = 3;

        public const byte OMA_4 = 4;

        public const byte OMA_6 = 5;

        public const byte OMA_7 = 6;

        public const byte OMA_8 = 7;

        public static int[] OMA_CHANNEL_ID = { OMA_MONO, OMA_STEREO, OMA_3, OMA_4, OMA_6, OMA_7, OMA_8 };

        public static bool Read(Stream stream, out OMAInfo info)
        {
            var buffer = new byte[OMA_HEADER_SIZE];
            if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                //No header?
                info = default(OMAInfo);
                return false;
            }
            if (!Utility.BufferEquals(buffer, OMA_TAG_EA3, 0))
            {
                //Not OMA?
                info = default(OMAInfo);
                return false;
            }
            if (buffer[6] != 0xff || buffer[7] != 0xff)
            {
                //Encrypted?
                info = default(OMAInfo);
                return false;
            }
            var parameters = buffer[33] << 16 | buffer[34] << 8 | buffer[35];
            switch (buffer[32])
            {
                case OMA_CODEC_ATRAC3:
                    return ReadAtrac3(parameters, out info);
                case OMA_CODEC_ATRAC3PLUS:
                    return ReadAtrac3Plus(parameters, out info);
                default:
                    //Unrecognized codec.
                    info = default(OMAInfo);
                    return false;
            }
        }

        private static bool ReadAtrac3(int parameters, out OMAInfo info)
        {
            var jointStereo = (parameters >> 0x11) & 0x1;
            var sampleRate = GetSampleRate((parameters >> 0xd) & 0x7);
            if (sampleRate == 0)
            {
                //Unrecognized sample rate.
                info = default(OMAInfo);
                return false;
            }
            info.Codec = OMA_CODEC_ATRAC3;
            info.Framesize = (parameters & 0x3FF) * 0x8;
            info.SampleRate = sampleRate;
            info.ChannelFormat = jointStereo == 0 ? OMA_STEREO : OMA_JOINT_STEREO;
            return true;
        }

        //This code is untested, I don't have access to any ATRAC3plus media.
        private static bool ReadAtrac3Plus(int parameters, out OMAInfo info)
        {
            var sampleRate = GetSampleRate((parameters >> 13) & 0x7);
            if (sampleRate == 0)
            {
                //Unrecognized sample rate.
                info = default(OMAInfo);
                return false;
            }
            var channelFormat = GetChannelFormat((parameters >> 10) & 7);
            if (channelFormat == 0)
            {
                //Unrecognized channel format.
                info = default(OMAInfo);
                return false;
            }
            info.Codec = OMA_CODEC_ATRAC3PLUS;
            info.Framesize = ((parameters & 0x3FF) * 8) + 8;
            info.SampleRate = sampleRate;
            info.ChannelFormat = channelFormat;
            return true;
        }

        public static bool Write(Stream stream, OMAInfo info)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(OMA_TAG_EA3);
            writer.Write((byte)0x1);
            writer.Write((byte)0x0);
            writer.Write((byte)OMA_HEADER_SIZE);
            writer.Write((byte)0xff);
            writer.Write((byte)0xff);
            while (stream.Position < 32)
            {
                writer.Write((byte)0x0);
            }
            switch (info.Codec)
            {
                case OMA_CODEC_ATRAC3:
                    WriteAtrac3(writer, info);
                    break;
                case OMA_CODEC_ATRAC3PLUS:
                    WriteAtrac3Plus(writer, info);
                    break;
            }
            while (stream.Position < OMA_HEADER_SIZE)
            {
                writer.Write((byte)0x0);
            }
            return true;
        }

        private static void WriteAtrac3(BinaryWriter writer, OMAInfo info)
        {
            var jointStereo = info.ChannelFormat == OMA_JOINT_STEREO ? 1 : 0;
            var sampleRateIndex = GetSampleRateIndex(info.SampleRate);
            var frameSize = info.Framesize / 8;
            var parameters = (OMA_CODEC_ATRAC3 << 24) | (jointStereo << 17) | (sampleRateIndex << 13) | frameSize;
            writer.Write(Utility.BEWord32(parameters));
        }

        //This code is untested, I don't have access to any ATRAC3plus media.
        private static void WriteAtrac3Plus(BinaryWriter writer, OMAInfo info)
        {
            var channelFormatIndex = GetChannelFormatIndex(info.ChannelFormat);
            var sampleRateIndex = GetSampleRateIndex(info.SampleRate);
            var frameSize = (info.Framesize - 8) / 8;
            var parameters = (OMA_CODEC_ATRAC3PLUS << 24) | ((channelFormatIndex + 1) << 10) | (sampleRateIndex << 13) | frameSize;
            writer.Write(Utility.BEWord32(parameters));
        }

        private static int GetChannelFormat(int index)
        {
            if (index < OMA_CHANNEL_ID.Length)
            {
                return OMA_CHANNEL_ID[index];
            }
            return 0;
        }
        public static int GetChannelFormatIndex(int channelFormat)
        {
            var index = Array.IndexOf(OMA_CHANNEL_ID, channelFormat);
            if (index >= 0)
            {
                return index;
            }
            return 0;
        }


        private static int GetSampleRate(int index)
        {
            if (index < OMA_SAMPLERATES.Length)
            {
                return OMA_SAMPLERATES[index];
            }
            return 0;
        }

        public static int GetSampleRateIndex(int sampleRate)
        {
            var index = Array.IndexOf(OMA_SAMPLERATES, sampleRate);
            if (index >= 0)
            {
                return index;
            }
            return 0;
        }

        public static int GetBitRate(OMAInfo info)
        {
            switch (info.Codec)
            {
                case OMA_CODEC_ATRAC3:
                    return info.SampleRate * info.Framesize * 8 / 1024;
                case OMA_CODEC_ATRAC3PLUS:
                    return info.SampleRate * info.Framesize * 8 / 2048;
            }
            return 0;
        }

        public struct OMAInfo
        {
            public int Codec;

            public int Framesize;

            public int SampleRate;

            public int ChannelFormat;
        }
    }
}
