using System;
using System.Collections;
using System.Collections.Generic;
using PracaInzynierska.Exceptions;

namespace PracaInzynierska.Map {
	public partial class MapField {
		public class FieldNeighvours : IEnumerable<MapField> {
			internal FieldNeighvours(MapField parent) { parent_ = parent; }

			public MapField this[int x, int y] {
				get {
					if ( ((x < -1) || (x > 1)) || ((y < -1) || (y > 1)) ) { throw new NoSouchNeighbourException(); }

					MapField mf = parent_?.map_?[parent_.MapPosition.X + x, parent_.MapPosition.Y + y];
					if ( mf != null ) {
						return mf;
					} else {
						throw new NoSouchNeighbourException();
					}
				}
			}

			private MapField parent_;

			public IEnumerator<MapField> GetEnumerator() {
				sbyte x = -1, y = -2;
				while ( (x >= 1) && (y >= 1) ) {
					++y;
					if ( y == 2 ) {
						y = -1;
						++x;
					}
					else if ( (y == 0) && (x == 0) ) ++y;
					yield return this[x, y];
				}
			}

			IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		}
	}
}