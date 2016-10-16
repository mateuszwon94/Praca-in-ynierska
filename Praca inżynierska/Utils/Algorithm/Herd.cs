using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracaInzynierska.Beeing;
using PracaInzynierska.Events;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Map;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static PracaInzynierska.Textures.GUITextures;
using static PracaInzynierska.Textures.MapTextures;
using static PracaInzynierska.Beeing.Beeing;

namespace PracaInzynierska.Utils.Algorithm {
	public class Herd : Drawable, IEnumerable<Animal> {

		public Herd(MapField position, uint count) {
			Random rand = new Random();

			position_ = position;

			for ( uint i = 0 ; i < count ; ++i ) {
				MapField mapField;
				do {
					mapField = position_;
					int x = rand.Next(-(int)(count / 2), (int)(count / 2) + 1);
					int y = rand.Next(-(int)(count / 2), (int)(count / 2) + 1);

					try { mapField = mapField.Neighbour[x, y]; }
					catch ( NoSouchNeighbourException ) { }
				} while ( !mapField.IsAvaliable || mapField.IsUnitOn );

				herd_.Add(new Animal() {
					MoveSpeed = 1,
					Location = mapField,
					Texture = new Sprite(AnimalTexture),
				});
			}
		}
		
		public void Draw(RenderTarget target, RenderStates states) {
			foreach ( Animal animal in this ) { animal.Draw(target, states); }
		}

		public MapField position_ { get; private set; }

		private List<Animal> herd_ = new List<Animal>();

		public IEnumerator<Animal> GetEnumerator() { return herd_.GetEnumerator(); }

		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

		private Vector2f Cohesion(Animal animal) {
			Vector2f centerOfMass = herd_.Where(a => a != animal)
										 .Aggregate(new Vector2f(0, 0),
													(current, a) => current + new Vector2f(a.Location.MapPosition.X, a.Location.MapPosition.Y));

			centerOfMass /= herd_.Count - 1;

			Vector2f animalPos = new Vector2f(animal.Location.MapPosition.X, animal.Location.MapPosition.Y);

			return centerOfMass - animalPos;
		}

		private Vector2f Separation(Animal animal, float minDistance) {
			return herd_.Where(a => (a != animal) && (Distance(a, animal) < minDistance))
						.Aggregate(new Vector2f(0, 0),
								   (current, a) =>
										   current - new Vector2f(a.MapPosition.X - animal.MapPosition.X, a.MapPosition.Y - animal.MapPosition.Y));
		}

		private Vector2f Alignment(Animal animal) {
			return new Vector2f();
		}
	}
}
