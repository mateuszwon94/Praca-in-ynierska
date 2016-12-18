using System.Collections;
using System.Collections.Generic;
using PracaInzynierska.Map;
using SFML.Graphics;

namespace PracaInzynierska.Constructs
{
	public partial class Construct : Drawable {

		public class MapFieldList : IEnumerable<MapField> {
			internal MapFieldList(Construct c) { parent_ = c; }

			public int Count => list_.Count;

			public void Add(MapField field) {
				list_.Add(field);
				field.ConstructOn = parent_;
			}

			public void Remove(MapField field) {
				list_.Remove(field);
				field.ConstructOn = null;
			}

			public void Clear() {
				foreach ( MapField field in list_ ) {
					field.ConstructOn = null;
				}
				list_.Clear();
			}

			/// <summary>Returns an enumerator that iterates through a collection.</summary>
			/// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
			IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

			/// <summary>Returns an enumerator that iterates through the collection.</summary>
			/// <returns>An enumerator that can be used to iterate through the collection.</returns>
			public IEnumerator<MapField> GetEnumerator() { return list_.GetEnumerator(); }

			public MapField this[int x] {
				get { return list_[x]; }
			}

			private readonly Construct parent_;
			private readonly List<MapField> list_ = new List<MapField>();
		}
	}
}