using NUnit.Framework;
using System;
using System.IO;

namespace MD.Net.Tests
{
    [TestFixture]
    public class FormatValidatorTests
    {
        [TestCase("Track_001", "00:05:04.680")]
        [TestCase("Track_002", "00:04:20.716")]
        [TestCase("Track_003", "00:02:12.760")]
        public void Validate(string name, TimeSpan expected)
        {
            var fileName = Path.Combine(Path.GetTempPath(), name + ".wav");
            using (var reader = Resources.ResourceManager.GetStream(name))
            {
                using (var writer = File.Create(fileName))
                {
                    reader.CopyTo(writer);
                }
            }
            var formatValidator = new FormatValidator();
            var actual = default(TimeSpan);
            formatValidator.Validate(fileName, out actual);
            Assert.AreEqual(expected, actual);
        }
    }
}
