using Microsoft.VisualStudio.TestTools.UnitTesting;
using PracaInzynierska.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace PracaInzynierska.Map.Tests {
    [TestClass()]
    public class MapTests {
        private const int mapSize = 10;

        private readonly Map map = new Map(mapSize,
                                           new MapSeed((int)(mapSize / 5.0), (int)(mapSize / 10.0),
                                                       (int)(mapSize / 15.0)));

        [TestMethod()]
        public void GetEnumeratorTest() {
            int count = 0;
            foreach ( MapField field in map ) { ++count; }

            Assert.AreEqual(count, map.Size * map.Size);
        }
    }
}