using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MD.Net.Tests
{
    public static class Utility
    {
        public static void EmitLines(string value, Action<string> handler)
        {
            using (var reader = new StringReader(value))
            {
                var line = default(string);
                while ((line = reader.ReadLine()) != null)
                {
                    handler(line);
                }
            }
        }

        public static void AssertSequenceEqual<T>(this IEnumerable<T> sequence1, IEnumerable<T> sequence2)
        {
            if (sequence1.SequenceEqual(sequence2))
            {
                return;
            }
            AssertSequenceEqual(sequence1.ToArray(), sequence2.ToArray());
        }

        private static void AssertSequenceEqual<T>(T[] sequence1, T[] sequence2)
        {
            Assert.AreEqual(sequence1.Length, sequence2.Length);
            for (var a = 0; a < sequence1.Length; a++)
            {
                Assert.AreEqual(sequence1[a], sequence2[a], "Sequence differs at position {0}: {1} != {2}", a, sequence1[a], sequence2[a]);
            }
        }

        public static byte[] Read(this UnmanagedMemoryStream stream, int count)
        {
            var buffer = new byte[count];
            Assert.IsTrue(stream.Read(buffer, 0, count) == count);
            return buffer;
        }
    }
}
