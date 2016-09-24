using System;
using System.Collections;
using System.Collections.Generic;
using PracaInzynierska.Events;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Textures;
using SFML.Graphics;
using SFML.System;

namespace PracaInzynierska.Map {
	public partial class MapField : Drawable {
		#region Constructors
		public MapField(int x, int y, Map map) {
            map_ = map;
			Neighbour = new FieldNeighvours(this);
			
			MapPosition = new Vector2i(x, y);

			IsFieldSeed = false;
		}

		/// <summary>
		/// Konstruktor tworzacy pojedyncze pole na planszy
		/// </summary>
		public MapField(int x, int y, Map map, MapSeed.Value seed) : this(x, y, map) {
			FieldSeed = seed;
        }

		#endregion Constructors

		public Vector2f Center => ScreenPosition + new Vector2f(FieldTexture.Size.X/2f, FieldTexture.Size.Y/2f);

		public double MoveSpeed { get; private set; }
		
		/// <summary>
		/// Funkcja rysujaca teksture
		/// </summary>
		/// <param name="target">Cel na ktorym jest rysowana</param>
		/// <param name="states">Stan</param>
		public void Draw(RenderTarget target, RenderStates states) {
			if ( IsFieldSeed && IsInside() ) {
				target.Draw(Field, states);
			}
		}

		/// <summary>
		/// Rozmiar pola
		/// </summary>
		public static int Size { get; set; } = 25;

		/// <summary>
		/// Obiekt slozacy do rysowania pola
		/// </summary>
		public Sprite Field { get; internal set; }

		public Texture FieldTexture {
			get { return Field.Texture; }
			internal set {
				Field = new Sprite(value) {
							Position = new Vector2f(MapPosition.X * Size, MapPosition.Y * Size)
						};
			}
		}

		public bool IsFieldSeed { get; private set; }

		public MapSeed.Value FieldSeed {
			get { return fieldSeed; }
			set {
				fieldSeed = value;
				IsFieldSeed = true;

				switch ( fieldSeed ) {
					case MapSeed.Value.Sand:
						FieldTexture = (MapTextures.SandTexture);
						MoveSpeed = 0.7;
						break;
					case MapSeed.Value.Grass:
						FieldTexture = (MapTextures.GrassTexture);
						MoveSpeed = 0.9;
						break;
					case MapSeed.Value.Rock:
						FieldTexture = (MapTextures.RockTexture);
						MoveSpeed = 0.0;
						break;
					default:
						IsFieldSeed = false;
						throw new NoSouchSeed();
				}
			}
		}
		
		private static Random r = new Random();

		public FieldNeighvours Neighbour { get; }

		internal void MapResized(object sender, MapResizedEventArgs e) {
			FieldSeed = FieldSeed;
		}

		private MapSeed.Value fieldSeed;

		public Vector2i MapPosition { get; }

		public Vector2f ScreenPosition {
			get { return Field.Position; }
			set { Field.Position = value; }
		}

		public bool IsUnitOn => UnitOn != null;

		public Beeing.Beeing UnitOn {
			get { return unitOn_; }
			set {
				if ( IsUnitOn && (value != null) ) throw new FieldIsNotEmptyException();
				unitOn_ = value;
			}
		}
		
		public bool IsInside() {
			return (ScreenPosition.X >= Field.Position.X) && (ScreenPosition.X <= Field.Position.X + Field.Texture.Size.X) &&
				   (ScreenPosition.Y >= Field.Position.Y) && (ScreenPosition.Y <= Field.Position.Y + Field.Texture.Size.Y);
		}

		internal void MapMoved(object sender, MapMovedEventArgs e) {
			ScreenPosition = new Vector2f(ScreenPosition.X + (float)e.dx, ScreenPosition.Y + (float)e.dy);

			if ( IsUnitOn ) UnitOn.Position = Center;
		}

        private readonly PracaInzynierska.Map.Map map_;
		private Beeing.Beeing unitOn_;

		public static bool operator ==(MapField first, MapField other) {
			return first?.MapPosition == other?.MapPosition;
		}

		public bool IsAvaliable => MoveSpeed > 0;

		public static bool operator !=(MapField first, MapField other) { return !(first == other); }

		public override string ToString() => $"Pos [{MapPosition.X}, {MapPosition.Y}]\tVal {FieldSeed}";


		public bool Equals(MapField other) {
			return MapPosition.Equals(other.MapPosition);
		}

		public override bool Equals(object obj) {
			if ( ReferenceEquals(null, obj) ) return false;
			if ( ReferenceEquals(this, obj) ) return true;
			if ( obj.GetType() != this.GetType() ) return false;

			return Equals((MapField)obj);
		}

		public override int GetHashCode() {
			return MapPosition.GetHashCode();
		}
	}
}
