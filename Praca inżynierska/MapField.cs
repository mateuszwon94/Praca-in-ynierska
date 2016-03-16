using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static PracaInzynierska.Textures.LoadedTextures.MapTextures;
using PracaInzynierska.Beeing;
using PracaInzynierska.GUI;
using PracaInzynierska.Events;

namespace PracaInzynierska {
	class MapField : Drawable {
		public MapField(int x, int y) {
			Size = 20;

			Neighbour = new FieldNeighvours();
			Neighbour[0, 0] = this;

			Position = new Vector2i(x, y);

			IsFieldSeed = false;
		}
		
		/// <summary>
		/// Konstruktor tworzacy pojedyncze pole na planszy
		/// </summary>
		/// <param name="texture">Obraz z jakiego ma zostac wylosowana tekstora</param>
		public MapField(int x, int y, MapSeed.Value seed) : this(x, y) {
			FieldSeed = seed;
        }

		public float MoveSpeed { get; private set; }
		
		/// <summary>
		/// Funkcja rysujaca teksture
		/// </summary>
		/// <param name="target">Cel na ktorym jest rysowana</param>
		/// <param name="states">Stan</param>
		public void Draw(RenderTarget target, RenderStates states) {
			if ( IsFieldSeed ) {
				target.Draw(Field);
			}
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

		public bool IsFieldSeed { get; private set; }

		public MapSeed.Value FieldSeed {
			get { return fieldSeed; }
			set {
				fieldSeed = value;
				IsFieldSeed = true;

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
						IsFieldSeed = false;
						throw new NoSouchSeed();

				}

				int WhereX = r.Next((int)(FieldImage.Size.X - Size));
				int WhereY = r.Next((int)(FieldImage.Size.Y - Size));
				TextureRect = new IntRect(WhereX, WhereY, Size, Size);

				Field = new Sprite(new Texture(FieldImage, TextureRect));
				Field.Texture.Smooth = true;

				Field.Position = new Vector2f(Position.X * Size, Position.Y * Size);
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
				get { return neighbours[x + 1, y + 1]; }
				internal set { neighbours[x + 1, y + 1] = value; }
			}

			private MapField[,] neighbours;
        }

		private MapSeed.Value fieldSeed;

		public Vector2i Position { get; private set; }

		public Vector2f ScreenPosition {
			get {
				return Field.Position;
			}
			set {
				Field.Position = value;
			}
		}

		public Men UnitOn { get; set; }

		public bool IsInside(Vector2f position) {
			if ( position.X >= Field.Position.X && position.X <= Field.Position.X + Field.Texture.Size.X &&
					position.Y >= Field.Position.Y && position.Y <= Field.Position.Y + Field.Texture.Size.Y )
				return true;
			return false;
		}

		internal void MapMoved(object sender, MapMovedEventArgs e) {
			ScreenPosition = new Vector2f(ScreenPosition.X + e.dx, ScreenPosition.Y + e.dy);
		}
	}
}
