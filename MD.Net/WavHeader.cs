using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MD.Net
{
    public static class WavHeader
    {
        public static readonly byte[] WAV_TAG_RIFF = Encoding.ASCII.GetBytes("RIFF");

        public static readonly byte[] WAV_TAG_WAVE = Encoding.ASCII.GetBytes("WAVE");

        public static readonly string WAV_CHUNK_FMT = "fmt ";

        public static readonly string WAV_CHUNK_DATA = "data";

        public const int WAV_HEADER_OFFSET = 8;

        public const int WAV_FORMAT_PCM = 0x1;

        public const int WAV_FORMAT_ATRAC3 = 0x270;

        public static bool Read(Stream stream, out WavInfo info)
        {
            var buffer = new byte[12];
            if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                //No header?
                info = default(WavInfo);
                return false;
            }
            if (!Utility.BufferEquals(buffer, WAV_TAG_RIFF, 0))
            {
                //Not RIFF?
                info = default(WavInfo);
                return false;
            }
            if (!Utility.BufferEquals(buffer, WAV_TAG_WAVE, 8))
            {
                //Not WAVE?
                info = default(WavInfo);
                return false;
            }
            info = new WavInfo()
            {
                FileSize = Utility.LEWord32(buffer, 4)
            };
            var chunks = new List<WaveChunk>();
            do
            {
                buffer = new byte[8];
                if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
                {
                    //No header?
                    info = default(WavInfo);
                    return false;
                }
                var name = Encoding.Default.GetString(buffer, 0, 4);
                var chunkSize = Utility.LEWord32(buffer, 4);
                if (string.Equals(name, WAV_CHUNK_FMT, StringComparison.OrdinalIgnoreCase))
                {
                    if (!ReadChunk(stream, chunkSize, out buffer))
                    {
                        //EOF?
                        info = default(WavInfo);
                        return false;
                    }
                    info.Format = Utility.LEWord16(buffer, 0);
                    info.ChannelCount = Utility.LEWord16(buffer, 2);
                    info.SampleRate = Utility.LEWord32(buffer, 4);
                    info.ByteRate = Utility.LEWord32(buffer, 8);
                    info.BlockAlign = Utility.LEWord16(buffer, 12);
                    info.BitsPerSample = Utility.LEWord16(buffer, 14);
                    //If we have additional data, store it.
                    if (chunkSize > 16)
                    {
                        info.Data = buffer.Skip(16).Take(chunkSize - 16).ToArray();
                    }
                    else
                    {
                        info.Data = null;
                    }
                }
                else if (string.Equals(name, WAV_CHUNK_DATA, StringComparison.OrdinalIgnoreCase))
                {
                    info.DataSize = chunkSize;
                    break;
                }
                else
                {
                    if (!ReadChunk(stream, chunkSize, out buffer))
                    {
                        //EOF?
                        info = default(WavInfo);
                        return false;
                    }
                    chunks.Add(new WaveChunk()
                    {
                        Name = name,
                        Data = buffer
                    });
                }
            } while (true);
            info.Chunks = chunks.ToArray();
            return true;
        }

        private static bool ReadChunk(Stream stream, int size, out byte[] buffer)
        {
            if (size <= 0 || size > 1024)
            {
                //Invalid chunk size.
                buffer = default(byte[]);
                return false;
            }
            buffer = new byte[size];
            return stream.Read(buffer, 0, buffer.Length) == buffer.Length;
        }

        public static bool Write(Stream stream, WavInfo info)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(WAV_TAG_RIFF);
            writer.Write(Utility.LEWord32(info.FileSize));
            writer.Write(WAV_TAG_WAVE);
            writer.Write(Encoding.ASCII.GetBytes(WAV_CHUNK_FMT));
            if (info.Data != null)
            {
                writer.Write(Utility.LEWord32(16 + info.Data.Length));
            }
            else
            {
                writer.Write(Utility.LEWord32(16));
            }
            writer.Write(Utility.LEWord16(info.Format));
            writer.Write(Utility.LEWord16(info.ChannelCount));
            writer.Write(Utility.LEWord32(info.SampleRate));
            writer.Write(Utility.LEWord32(info.ByteRate));
            writer.Write(Utility.LEWord16(info.BlockAlign));
            writer.Write(Utility.LEWord16(info.BitsPerSample));
            if (info.Data != null)
            {
                writer.Write(info.Data);
            }
            if (info.Chunks != null)
            {
                foreach (var chunk in info.Chunks)
                {
                    writer.Write(Encoding.ASCII.GetBytes(chunk.Name.ToCharArray(), 0, 4));
                    if (chunk.Data != null)
                    {
                        writer.Write(Utility.LEWord32(chunk.Data.Length));
                        writer.Write(chunk.Data);
                    }
                    else
                    {
                        writer.Write(Utility.LEWord32(0));
                    }
                }
            }
            writer.Write(Encoding.ASCII.GetBytes(WAV_CHUNK_DATA));
            writer.Write(Utility.LEWord32(info.DataSize));
            return true;
        }

        public static int GetHeaderSize(WavInfo info)
        {
            var headerSize = 44;
            if (info.Data != null)
            {
                headerSize += info.Data.Length;
            }
            if (info.Chunks != null)
            {
                foreach (var chunk in info.Chunks)
                {
                    headerSize += 4;
                    if (chunk.Data != null)
                    {
                        headerSize += chunk.Data.Length;
                    }
                }
            }
            return headerSize;
        }

        public static int GetBitRate(WavInfo info)
        {
            return info.SampleRate * info.ChannelCount * info.BitsPerSample;
        }

        public struct WavInfo
        {
            public int FileSize;

            public int Format;

            public int ChannelCount;

            public int SampleRate;

            public int ByteRate;

            public int BlockAlign;

            public int BitsPerSample;

            public int DataSize;

            public byte[] Data;

            public WaveChunk[] Chunks;
        }

        public struct WaveChunk
        {
            public string Name;

            public byte[] Data;
        }
    }
}
