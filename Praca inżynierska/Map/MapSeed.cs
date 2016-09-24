using PracaInzynierska.Exceptions;

namespace PracaInzynierska.Map {
	public struct MapSeed {
		public MapSeed(int sand, int grass, int rock) {
			Sand = sand;
			Grass = grass;
			Rock = rock;
        }

        public Value SeedFromIdx(int idx) {
            if (idx > Sand + Grass) {
				return Value.Rock;
			} else if (idx > Sand) {
				return Value.Grass;
			} else {
				return Value.Sand;
			}
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

		public int Sand { get; set; }
		public int Grass { get; set; }
		public int Rock { get; set; }

		public int Count => Sand + Grass + Rock;

		public Value Next() {
			if (Sand != 0) {
				--Sand;
				return Value.Sand;
			} else if ( Grass != 0 ) {
				--Grass;
				return Value.Grass;
			} else if ( Rock != 0 ) {
				--Rock;
				return Value.Rock;
			}
			return Value.None;
		}
        
		public enum Value {
			None = 0, Sand, Grass, Rock
		}

		public static int MaxValue() { return 3; }
		
		public static implicit operator bool(MapSeed val) {
			return val.Count != 0;
		}
	}
}
