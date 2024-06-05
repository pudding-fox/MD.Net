using MD.Net.Resources;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MD.Net
{
    public class FormatManager : IFormatManager
    {
        const string LP2 = "128";

        const int LP2_RATE = 16537;

        const string LP4 = "64";

        const int LP4_RATE = 8268;

        static readonly Regex ATRACDENC_PROGRESS = new Regex(
            @"(\d\d?)%",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public FormatManager(IToolManager toolManager)
        {
            this.ToolManager = toolManager;
        }

        public IToolManager ToolManager { get; private set; }

        public string Convert(string fileName, Compression compression, IStatus status)
        {
            switch (compression)
            {
                case Compression.None:
                    return this.ConvertWav(fileName, status);
                case Compression.LP2:
                    return this.ConvertAtrac(fileName, LP2, status);
                case Compression.LP4:
                    return this.ConvertAtrac(fileName, LP4, status);
            }
            return fileName;
        }

        protected virtual string ConvertWav(string fileName, IStatus status)
        {
            if (this.IsPCM(fileName))
            {
                //Nothing to do.
                return fileName;
            }
            throw new NotImplementedException();
        }

        protected virtual bool IsPCM(string fileName)
        {
            if (File.Exists(fileName))
            {
                var format = this.GetFormat(fileName);
                if (format == WaveFormat.PCM)
                {
                    return true;
                }
                throw new WaveFormatException(fileName);
            }
            //File could not be checked, likely a unit test.
            return false;
        }

        protected virtual string ConvertAtrac(string fileName, string bitrate, IStatus status)
        {
            if (this.IsAtrac(fileName, bitrate))
            {
                //Nothing to do.
                return fileName;
            }
            var result = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".at3");
            var process = this.ToolManager.Start(Tools.ATRACDENC, string.Format("{0} {1} {2} \"{3}\" {4} \"{5}\" {6} {7}", Constants.ATRACDENC_ENCODE, Constants.ATRACDENC_ATRAC3, Constants.ATRACDENC_INPUT, fileName, Constants.ATRACDENC_OUTPUT, result, Constants.ATRACDENC_BITRATE, bitrate));
            using (var emitter = new PercentStatusEmitter(string.Format(Strings.FormatManager_Description, Path.GetFileName(fileName)), StatusType.Encode, ATRACDENC_PROGRESS, status))
            {
                var error = new StringBuilder();
                var code = this.ToolManager.Exec(process, emitter.Action, data => error.AppendLine(data));
                if (code != 0)
                {
                    this.ToolManager.Throw(process, error.ToString());
                }
            }
            if (File.Exists(result))
            {
                result = this.ConvertOmaToWav(result);
            }
            return result;
        }

        protected virtual bool IsAtrac(string fileName, string bitrate)
        {
            if (File.Exists(fileName))
            {
                var format = this.GetFormat(fileName);
                if (format == WaveFormat.PCM)
                {
                    return false;
                }
                else if (bitrate == LP2 && format == WaveFormat.ATRAC_LP2)
                {
                    return true;
                }
                else if (bitrate == LP4 && format == WaveFormat.ATRAC_LP4)
                {
                    return true;
                }
                else
                {
                    throw new WaveFormatException(fileName);
                }
            }
            //File could not be checked, likely a unit test.
            return false;
        }

        protected virtual string ConvertOmaToWav(string fileName)
        {
            var result = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".at3.wav");
            using (var reader = File.OpenRead(fileName))
            {
                var omaInfo = default(OMAHeader.OMAInfo);
                if (OMAHeader.Read(reader, out omaInfo))
                {
                    var wavInfo = default(WavHeader.WavInfo);
                    wavInfo.FileSize = global::System.Convert.ToInt32(reader.Length - reader.Position) + (WavHeader.GetHeaderSize(wavInfo) - WavHeader.WAV_HEADER_OFFSET);
                    wavInfo.Format = WavHeader.WAV_FORMAT_ATRAC3;
                    wavInfo.ChannelCount = 2;
                    wavInfo.SampleRate = omaInfo.SampleRate;
                    wavInfo.ByteRate = OMAHeader.GetBitRate(omaInfo) / 8;
                    wavInfo.BlockAlign = omaInfo.Framesize;
                    wavInfo.BitsPerSample = 0;
                    wavInfo.DataSize = global::System.Convert.ToInt32(reader.Length - reader.Position);
                    using (var writer = File.Create(result))
                    {
                        if (!WavHeader.Write(writer, wavInfo))
                        {
                            throw new InvalidOperationException("Failed to construct WAV header.");
                        }
                        reader.CopyTo(writer);
                    }
                }
                else
                {
                    throw new InvalidOperationException("OMA header could not be read.");
                }
            }
            try
            {
                File.Delete(fileName);
            }
            catch
            {
                //Nothing can be done.
            }
            return result;
        }

        protected virtual WaveFormat GetFormat(string fileName)
        {
            using (var reader = File.OpenRead(fileName))
            {
                var info = default(WavHeader.WavInfo);
                if (WavHeader.Read(reader, out info))
                {
                    if (info.Format == WavHeader.WAV_FORMAT_PCM)
                    {
                        return WaveFormat.PCM;
                    }
                    else if (info.Format == WavHeader.WAV_FORMAT_ATRAC3)
                    {
                        if (info.ByteRate == LP2_RATE)
                        {
                            return WaveFormat.ATRAC_LP2;
                        }
                        else if (info.ByteRate == LP4_RATE)
                        {
                            return WaveFormat.ATRAC_LP4;
                        }
                    }
                }
            }
            return WaveFormat.None;
        }

        public enum WaveFormat : byte
        {
            None,
            PCM,
            ATRAC_LP2,
            ATRAC_LP4
        }
    }
}
