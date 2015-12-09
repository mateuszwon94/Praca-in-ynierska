using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracaInzynierska {
	struct MapSeed {
		public MapSeed(int sand, int grass, int rock) {
			Sand = sand;
			Grass = grass;
			Rock = rock;
        }

		public int this[Value e] {
			get {
				switch (e) {
					case Value.Sand:
						return Sand;
					case Value.Grass:
						return Grass;
					case Value.Rock:
						return Rock;
					default:
						throw new NoSouchSeed();
				}
			}
		}

		public int Sand;
		public int Grass;
		public int Rock;

		public enum Value {
			Sand, Grass, Rock
		}

		public static int MaxValue() { return 3; }
	}

	

	class NoSouchSeed : Exception { }
}
