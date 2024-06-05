using NUnit.Framework;
using System;

namespace MD.Net.Tests
{
    [TestFixture]
    public class CapacityTests
    {
        [Test]
        public void Capacity()
        {
            var disc = new Disc(string.Empty, TimeSpan.Zero, TimeSpan.FromMinutes(80), TimeSpan.FromMinutes(80), Tracks.None);
            disc.Tracks.Add(new Track(0, Protection.None, Compression.None, TimeSpan.FromMinutes(3.5), string.Empty));
            disc.Tracks.Add(new Track(1, Protection.None, Compression.LP2, TimeSpan.FromMinutes(3.5), string.Empty));
            disc.Tracks.Add(new Track(2, Protection.None, Compression.LP4, TimeSpan.FromMinutes(3.5), string.Empty));
            var capacity = new Capacity(disc);
            Assert.AreEqual(7, capacity.PercentUsed);
            Assert.AreEqual(93, capacity.PercentFree);
        }
    }
}
