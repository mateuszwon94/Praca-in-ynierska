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

		public int this[MapSeedValues e] {
			get {
				switch (e) {
					case MapSeedValues.Sand:
						return Sand;
					case MapSeedValues.Grass:
						return Grass;
					case MapSeedValues.Rock:
						return Rock;
					default:
						throw new NoSouchSeed();
				}
			}
		}

		public int Sand;
		public int Grass;
		public int Rock;
	}

	enum MapSeedValues {
		Sand, Grass, Rock
	}

	class NoSouchSeed : Exception { }
}
