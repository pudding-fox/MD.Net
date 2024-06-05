using System;
using System.IO;

namespace MD.Net
{
    public class FormatValidator : IFormatValidator
    {
        public const int SAMPLE_RATE = 44100;

        public const int DEPTH = 16;

        public const int CHANNELS = 2;

        public void Validate(string fileName, out TimeSpan length)
        {
            if (!this.IsValid(fileName, out length))
            {
                throw new WaveFormatException(fileName);
            }
        }

        protected virtual bool IsValid(string fileName, out TimeSpan length)
        {
            length = default(TimeSpan);
            using (var reader = File.OpenRead(fileName))
            {
                var info = default(WavHeader.WavInfo);
                if (!WavHeader.Read(reader, out info))
                {
                    return false;
                }
                if (info.Format == WavHeader.WAV_FORMAT_PCM)
                {
                    return this.IsValidPCM(info, out length);
                }
                else if(info.Format == WavHeader.WAV_FORMAT_ATRAC3)
                {
                    return this.IsValidATRAC(info, out length);
                }
                else
                {
                    return false;
                }
            }
        }

        protected virtual bool IsValidPCM(WavHeader.WavInfo info, out TimeSpan length)
        {
            length = default(TimeSpan);
            if (info.SampleRate != SAMPLE_RATE)
            {
                return false;
            }
            if (info.BitsPerSample != DEPTH)
            {
                return false;
            }
            if (info.ChannelCount != CHANNELS)
            {
                return false;
            }
            if (info.DataSize > 0)
            {
                if (info.ByteRate > 0)
                {
                    length = TimeSpan.FromSeconds((double)info.DataSize / info.ByteRate);
                }
                else if (info.SampleRate > 0 && info.BitsPerSample > 0 && info.ChannelCount > 0)
                {
                    length = TimeSpan.FromSeconds((double)info.DataSize / (WavHeader.GetBitRate(info) / 8));
                }
            }
            return true;
        }

        protected virtual bool IsValidATRAC(WavHeader.WavInfo info, out TimeSpan length)
        {
            length = default(TimeSpan);
            if (info.SampleRate != SAMPLE_RATE)
            {
                return false;
            }
            if (info.ChannelCount != CHANNELS)
            {
                return false;
            }
            if (info.DataSize > 0)
            {
                if (info.ByteRate > 0)
                {
                    length = TimeSpan.FromSeconds((double)info.DataSize / info.ByteRate);
                }
                else if (info.SampleRate > 0 && info.BitsPerSample > 0 && info.ChannelCount > 0)
                {
                    length = TimeSpan.FromSeconds((double)info.DataSize / (WavHeader.GetBitRate(info) / 8));
                }
            }
            return true;
        }
    }
}
