using System.Collections.Generic;
using PracaInzynierska.Events;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Textures;
using SFML.Graphics;
using SFML.System;
using PracaInzynierska.Beeings;
using PracaInzynierska.Constructs;
using static System.Math;

namespace PracaInzynierska.Map {

	/// <summary>
    /// Klasa reprezentujaca pole mapy
    /// </summary>
	public partial class MapField : Drawable {
        #region Constructors

        /// <summary>
        /// Konstruktor tworzacy pojedyncze pole na planszy
        /// </summary>
        /// <param name="x">Polozenie x na mapie</param>
        /// <param name="y">Polozenie y na mapie</param>
        /// <param name="map">Mapa na ktorej znajduje sie pole</param>
        public MapField(int x, int y, Map map) {
            map_ = map;
			Neighbour = new FieldNeighvours(this);

			MapPosition = new Vector2i(x, y);
		}

        /// <summary>
        /// Konstruktor tworzacy pojedyncze pole na planszy
        /// </summary>
        /// <param name="x">Polozenie x na mapie</param>
        /// <param name="y">Polozenie y na mapie</param>
        /// <param name="map">Mapa na ktorej znajduje sie pole</param>
        /// <param name="seed">Ziarno danego pola</param>
        public MapField(int x, int y, Map map, MapSeed.Value seed) : this(x, y, map) {
			FieldSeed = seed;
        }

        #endregion Constructors

        #region Drawable

        /// <summary>
        /// Funkcja rysujaca teksture
        /// </summary>
        /// <param name="target">Cel na ktorym jest rysowana</param>
        /// <param name="states">Stan</param>
        public void Draw(RenderTarget target, RenderStates states) {
			target.Draw(Texture, states);
        }

        #endregion Drawable

        #region Properities

        /// <summary>
        /// Wlasciwosc zwracajaca pozycje srodka pola na ekranie
        /// </summary>
        public Vector2f Center => ScreenPosition + new Vector2f(FieldTexture.Size.X / 2f, FieldTexture.Size.Y / 2f);

		/// <summary>
		/// Wlasciwosc okreslajaca szybkosc poruszania sie po danym polu
		/// </summary>
		public double MoveSpeed {
			get { return moveSpeed_; }
			private set {
				moveSpeed_ = value;
				//Cost = (float)(1d + (1d - value));
				Cost = (float)(1d / value);
			}
		}

		public float Cost { get; private set; }

        /// <summary>
        /// Rozmiar pola
        /// </summary>
        public static int ScreenSize { get; set; } = 25;

        /// <summary>
        /// Obiekt slozacy do rysowania pola
        /// </summary>
        public Sprite Texture { get; internal set; }

        /// <summary>
        /// Zwraca 'true' jesli polu zostalo przypisane juz ziarno
        /// </summary>
        public bool IsFieldSeed { get; private set; } = false;

        /// <summary>
        /// Tekstura sluzaca do rysowania pola
        /// </summary>
        public Texture FieldTexture {
            get { return Texture.Texture; }
            internal set {
                Texture = new Sprite(value) {
                    Position = new Vector2f(MapPosition.X * ScreenSize, MapPosition.Y * ScreenSize)
                };
            }
        }

        /// <summary>
        /// Ziarno jakim pole zostalo zainicjalizowane
        /// <exception cref="NoSouchSeed">Wyrzuca wyjatek, jezeli nie ma takiego ziarna jakim probowana zainicjalizowac pole.</exception>
        /// </summary>
		public MapSeed.Value FieldSeed {
            get { return fieldSeed_; }
            set { //ustawienie odpowiednich wartosci zmiennych
                fieldSeed_ = value;
                IsFieldSeed = true;

				//przypisanie odopwiedniej tekstury i predkosci poruszania sie w zaleznosci od terenu
				if (fieldSeed_ == MapSeed.Value.Sand) {
					FieldTexture = MapTextures.SandTexture;
					MoveSpeed = 0.6;
				} else if (fieldSeed_ == MapSeed.Value.Grass) {
					FieldTexture = MapTextures.GrassTexture;
					MoveSpeed = 0.95;
				} else if (fieldSeed_ == MapSeed.Value.Rock) {
					FieldTexture = MapTextures.RockTexture;
					MoveSpeed = 0.0;
				} else {
					IsFieldSeed = false;
					throw new NoSouchSeed();
				}
            }
        }

        /// <summary>
        /// Mozliwosc dostania sie do sasiadow danego pola
        /// </summary>
        public FieldNeighvours Neighbour { get; }

        /// <summary>
        /// Pozycja danego pola na mapie
        /// </summary>
        public Vector2i MapPosition { get; }

        /// <summary>
        /// Pozycja danego pola na ekranie
        /// </summary>
        public Vector2f ScreenPosition {
			get { return Texture.Position; }
			set { Texture.Position = value; }
        }

        /// <summary>
        /// Zraca 'true' jesli na polu stoi jakas jednostka
        /// </summary>
        public bool IsSomethingOn => OnField.Count > 0;

		public bool IsConstructOn => (ConstructOn == null || ConstructOn.State != Construct.Status.Done);

		/// <summary>
		/// Zwraca jednostke stojaca na danym polu.
		/// <exception cref="FieldIsNotEmptyException">Jesli jest proba przypisania wartosc, a na polu jest juz jakas jednostka wyrzuci wyjatek</exception>
		/// </summary>
		public List<Beeing> OnField { get; } = new List<Beeing>();

		/// <summary>
		/// Zwraca wartosc 'true' jesli istnieje mozliwosc przejscia przez to pole.
		/// </summary>
		public bool IsAvaliable => MoveSpeed > 0.0 && (IsConstructOn && !(ConstructOn is Bed));

		public Construct ConstructOn { get; set; }

		#endregion Properities

        #region EventHandlers

        /// <summary>
        /// Funkcja wywoływana przy zmianie rozmiaru mapy (oddalenie, przyblizenie kamery)
        /// </summary>
        /// <param name="sender">Obiekt, ktory wywolal event</param>
        /// <param name="e">Argumenty z jakimi otrzymano event</param>
        internal void MapResized(object sender, MapResizedEventArgs e) {
            FieldSeed = FieldSeed;
	        foreach ( Beeing beeing in OnField ) {
				if ( beeing is Animal a ) {
					a.Texture = new Sprite(MapTextures.AnimalTexture);
				} else if ( beeing is Men m ) {
					if ( m.Colony != null ) {
						m.TextureNotSelected = new Sprite(MapTextures.MenTexture);
						m.TextureSelected = new Sprite(MapTextures.MenTextureSelected);
					} else {
						m.TextureNotSelected = new Sprite(MapTextures.BesigerTexture);
						m.TextureSelected = new Sprite(MapTextures.BesigerTexture);
					}
				}
			}

	        if ( ConstructOn != null && ConstructOn.BaseField == this ) {
				ConstructOn.SetTextureFromColor();
				ConstructOn.ScreenPosition = ScreenPosition;
	        }
        }

		/// <summary>
		/// Funkcja wywoływana przy poruszeniu mapy
		/// </summary>
		/// <param name="sender">Obiekt, ktory wywolal event</param>
		/// <param name="e">Argumenty z jakimi otrzymano event</param>
		internal void MapMoved(object sender, MapMovedEventArgs e) {
			ScreenPosition = new Vector2f(ScreenPosition.X + (float)e.dx, ScreenPosition.Y + (float)e.dy);

			if ( IsSomethingOn ) {
				foreach ( Beeing beeing in OnField ) {
					if ( beeing is Animal a ) { a.Texture = new Sprite(MapTextures.AnimalTexture); } else if ( beeing is Men m ) {
						if ( m.Colony != null ) {
							m.TextureNotSelected = new Sprite(MapTextures.MenTexture);
							m.TextureSelected = new Sprite(MapTextures.MenTextureSelected);
						} else {
							m.TextureNotSelected = new Sprite(MapTextures.BesigerTexture);
							m.TextureSelected = new Sprite(MapTextures.BesigerTexture);
						}
					}
				}

				if ( ConstructOn != null && ConstructOn.BaseField == this ) { ConstructOn.ScreenPosition = ScreenPosition; }
			}
		}

		#endregion EventHandlers

        #region EqualityFunc

        /// <summary>
        /// Przeladowany operator rownosci dwoch pol
        /// </summary>
        /// <param name="first">Pierwsze porownywane pole</param>
        /// <param name="other">Drugie porownywane pole</param>
        /// <returns>'true' jesli pola sa tym samym polem, w przeciwnym wypadku 'false'</returns>
        public static bool operator ==(MapField first, MapField other) => first?.MapPosition == other?.MapPosition;

	    /// <summary>
        /// Przeladowany operator nierownosci dwoch pol
        /// </summary>
        /// <param name="first">Pierwsze porownywane pole</param>
        /// <param name="other">Drugie porownywane pole</param>
        /// <returns>'true' jesli pola nie sa tym samym polem, w przeciwnym wypadku 'false'</returns>
		public static bool operator !=(MapField first, MapField other) => first?.MapPosition != other?.MapPosition;

	    /// <summary>
        /// Przeładowana funkcja z klasy object sprawdzająca rownosc pol.
        /// </summary>
        /// <returns>''true' jesli pola sa tym samym polem, w przeciwnym wypadku 'false'</returns>
        public bool Equals(MapField other) {
            return MapPosition.Equals(other.MapPosition);
        }

        /// <summary>
        /// Przeładowana funkcja z klasy object sprawdzająca rownosc pol.
        /// </summary>
        /// <returns>''true' jesli pola sa tym samym polem, w przeciwnym wypadku 'false'</returns>
		public override bool Equals(object obj) {
            if ( ReferenceEquals(null, obj) ) return false;
            if ( ReferenceEquals(this, obj) ) return true;
            if ( obj.GetType() != GetType() ) return false;

            return Equals((MapField)obj);
        }

        /// <summary>
        /// Przeładowana funkcja z klasy object zwracajaca Hash Code pola.
        /// </summary>
        /// <returns>Zwraca Hash Code pola</returns>
        public override int GetHashCode() { return MapPosition.GetHashCode(); }

        #endregion EqualityFunc

        #region Funcs

        /// <summary>
        /// Funkcja sprawdza czy pole znajduje sie w widzialnej czesci okna.
        /// </summary>
        /// <returns>'true' jesli sie znajduje, w przeciwnym wypadku 'false'</returns>
        public bool IsInsideWindows() {
            return (ScreenPosition.X >= Texture.Position.X) && (ScreenPosition.X <= Texture.Position.X + Texture.Texture.Size.X) &&
                   (ScreenPosition.Y >= Texture.Position.Y) && (ScreenPosition.Y <= Texture.Position.Y + Texture.Texture.Size.Y);
        }

        /// <summary>
        /// Przeciazona metoda ToString z klasy object.
        /// </summary>
        /// <returns>Zwraca string bedacy reprezentacja pola</returns>
        public override string ToString() => $"Pos [{MapPosition.X}, {MapPosition.Y}]\tVal {FieldSeed}";

		public static float Distance(MapField from, MapField to) {
			return (float)Round(Sqrt((from.MapPosition.X - to.MapPosition.X) *
									 (from.MapPosition.X - to.MapPosition.X) +
									 (from.MapPosition.Y - to.MapPosition.Y) *
									 (from.MapPosition.Y - to.MapPosition.Y)), 4);
		}

		#endregion Funcs

		#region PrivateVars

		private MapSeed.Value fieldSeed_;
        private readonly PracaInzynierska.Map.Map map_;
		private double moveSpeed_;

		#endregion PrivateVars
    }
}
