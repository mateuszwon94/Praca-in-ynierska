using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static PracaInzynierska.LoadedTextures;

namespace PracaInzynierska {
	class MapField : Drawable {
		public MapField() {
			Size = 20;

			Neighbour = new FieldNeighvours();
			Neighbour[0, 0] = this;

			FieldSeed = null;
		}
		
		/// <summary>
		/// Konstruktor tworzacy pojedyncze pole na planszy
		/// </summary>
		/// <param name="texture">Obraz z jakiego ma zostac wylosowana tekstora</param>
		public MapField(MapSeed.Value seed) : this() {
			FieldSeed = seed;
        }

		public float MoveSpeed { get; private set; }
		
		/// <summary>
		/// Funkcja rysujaca teksture
		/// </summary>
		/// <param name="target">Cel na ktorym jest rysowana</param>
		/// <param name="states">Stan</param>
		public void Draw(RenderTarget target, RenderStates states) {
			if (FieldSeed != null)
				target.Draw(Field);
		}
		
		/// <summary>
		/// Rozmiar pola
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		/// Obiekt slozacy do rysowania pola
		/// </summary>
		public Sprite Field { get; internal set; }

		/// <summary>
		/// Tekstura ktora posluzyla do zainicjalizowania tego pola
		/// </summary>
		public Image FieldImage { get; private set; }

		public MapSeed.Value? FieldSeed {
			get { return fieldSeed; }
			set {
				fieldSeed = value;
				if ( value == null ) return;
				switch ( fieldSeed ) {
					case MapSeed.Value.Sand:
						FieldImage = SandTexture;
						MoveSpeed = 0.7f;
						break;
					case MapSeed.Value.Grass:
						FieldImage = GrassTexture;
						MoveSpeed = 0.9f;
						break;
					case MapSeed.Value.Rock:
						FieldImage = RockTexture;
						MoveSpeed = 0.0f;
						break;
					default:
						fieldSeed = null;
                        throw new NoSouchSeed();
				}

				int WhereX = r.Next((int)(FieldImage.Size.X - Size));
				int WhereY = r.Next((int)(FieldImage.Size.Y - Size));
				TextureRect = new IntRect(WhereX, WhereY, Size, Size);

				Field = new Sprite(new Texture(FieldImage, TextureRect));
				Field.Texture.Smooth = true;
			}
		}

		public IntRect TextureRect { get; private set; }

		private static Random r = new Random();

		public FieldNeighvours Neighbour { get; internal set; }

		public class FieldNeighvours {
			public FieldNeighvours() {
				neighbours = new MapField[3, 3];
            }

			public MapField this[int x, int y] {
				set { neighbours[x + 1, y + 1] = value; }
				get { return neighbours[x + 1, y + 1]; }
			}

			private MapField[,] neighbours;
        }

		private MapSeed.Value? fieldSeed;
    }
}
