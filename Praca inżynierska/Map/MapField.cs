using System.Collections;
using System.Collections.Generic;
using PracaInzynierska.Beeing;
using PracaInzynierska.Events;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Textures;
using SFML.Graphics;
using SFML.System;

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
            if ( IsFieldSeed && IsInsideWindows() ) {
                target.Draw(Field, states);
            }
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
		public double MoveSpeed { get; private set; }

        /// <summary>
        /// Rozmiar pola
        /// </summary>
        public static int Size { get; set; } = 25;

        /// <summary>
        /// Obiekt slozacy do rysowania pola
        /// </summary>
        public Sprite Field { get; internal set; }

        /// <summary>
        /// Zwraca 'true' jesli polu zostalo przypisane juz ziarno
        /// </summary>
        public bool IsFieldSeed { get; private set; } = false;

        /// <summary>
        /// Tekstura sluzaca do rysowania pola
        /// </summary>
        public Texture FieldTexture {
            get { return Field.Texture; }
            internal set {
                Field = new Sprite(value) {
                    Position = new Vector2f(MapPosition.X * Size, MapPosition.Y * Size)
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

                switch ( fieldSeed_ ) { //przypisanie odopwiedniej tekstury i predkosci poruszania sie w zaleznosci od terenu
                    case MapSeed.Value.Sand:
                        FieldTexture = MapTextures.SandTexture;
                        MoveSpeed = 0.7;
                        break;
                    case MapSeed.Value.Grass:
                        FieldTexture = MapTextures.GrassTexture;
                        MoveSpeed = 0.9;
                        break;
                    case MapSeed.Value.Rock:
                        FieldTexture = MapTextures.RockTexture;
                        MoveSpeed = 0.0;
                        break;
                    default:
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
			get { return Field.Position; }
			set { Field.Position = value; }
        }

        /// <summary>
        /// Zraca 'true' jesli na polu stoi jakas jednostka
        /// </summary>
        public bool IsUnitOn => UnitOn != null;

        /// <summary>
        /// Zwraca jednostke stojaca na danym polu.
        /// <exception cref="FieldIsNotEmptyException">Jesli jest proba przypisania wartosc, a na polu jest juz jakas jednostka wyrzuci wyjatek</exception>
        /// </summary>
        public Beeing.Beeing UnitOn {
            get { return unitOn_; }
            set {
                if ( IsUnitOn && (value != null) ) throw new FieldIsNotEmptyException();
                unitOn_ = value;
            }
        }

        /// <summary>
        /// Zwraca wartosc 'true' jesli istnieje mozliwosc przejscia przez to pole.
        /// </summary>
        public bool IsAvaliable => MoveSpeed > 0.0;

        #endregion Properities

        #region EventHandlers

        /// <summary>
        /// Funkcja wywoływana przy zmianie rozmiaru mapy (oddalenie, przyblizenie kamery)
        /// </summary>
        /// <param name="sender">Obiekt, ktory wywolal event</param>
        /// <param name="e">Argumenty z jakimi otrzymano event</param>
        internal void MapResized(object sender, MapResizedEventArgs e) {
            FieldSeed = FieldSeed;
	        if (UnitOn is Animal a) {
				a.Texture = new Sprite(MapTextures.AnimalTexture);
	        } else if (UnitOn is Men m) {
				m.TextureNotSelected = new Sprite(MapTextures.MenTexture);
				m.TextureSelected = new Sprite(MapTextures.MenTextureSelected);
	        }
        }

        /// <summary>
        /// Funkcja wywoływana przy poruszeniu mapy
        /// </summary>
        /// <param name="sender">Obiekt, ktory wywolal event</param>
        /// <param name="e">Argumenty z jakimi otrzymano event</param>
		internal void MapMoved(object sender, MapMovedEventArgs e) {
            ScreenPosition = new Vector2f(ScreenPosition.X + (float)e.dx, ScreenPosition.Y + (float)e.dy);

            if ( IsUnitOn ) UnitOn.ScreenPosition = Center;
        }

        #endregion EventHandlers

        #region EqualityFunc

        /// <summary>
        /// Przeladowany operator rownosci dwoch pol
        /// </summary>
        /// <param name="first">Pierwsze porownywane pole</param>
        /// <param name="other">Drugie porownywane pole</param>
        /// <returns>'true' jesli pola sa tym samym polem, w przeciwnym wypadku 'false'</returns>
        public static bool operator ==(MapField first, MapField other) {
            return first?.MapPosition == other?.MapPosition;
        }

        /// <summary>
        /// Przeladowany operator nierownosci dwoch pol
        /// </summary>
        /// <param name="first">Pierwsze porownywane pole</param>
        /// <param name="other">Drugie porownywane pole</param>
        /// <returns>'true' jesli pola nie sa tym samym polem, w przeciwnym wypadku 'false'</returns>
		public static bool operator !=(MapField first, MapField other) { return !(first == other); }

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
            return (ScreenPosition.X >= Field.Position.X) && (ScreenPosition.X <= Field.Position.X + Field.Texture.Size.X) &&
                   (ScreenPosition.Y >= Field.Position.Y) && (ScreenPosition.Y <= Field.Position.Y + Field.Texture.Size.Y);
        }

        /// <summary>
        /// Przeciazona metoda ToString z klasy object.
        /// </summary>
        /// <returns>Zwraca string bedacy reprezentacja pola</returns>
        public override string ToString() => $"Pos [{MapPosition.X}, {MapPosition.Y}]\tVal {FieldSeed}";

        #endregion Funcs

        #region PrivateVars

        private MapSeed.Value fieldSeed_;
        private readonly PracaInzynierska.Map.Map map_;
        private Beeing.Beeing unitOn_;

        #endregion PrivateVars
    }
}
