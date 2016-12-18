using System;
using PracaInzynierska.Map;
using SFML.Graphics;
using SFML.System;
using PracaInzynierska.Exceptions;
using static PracaInzynierska.Textures.MapTextures;
using static PracaInzynierska.Utils.Math;

namespace PracaInzynierska.Constructs
{
	public partial class Construct : Drawable {
		public Construct(uint x, uint y, MapField baseField, Color color) {
			Size = new Vector2u(x, y);
			Location = new MapFieldList(this);
			BaseField = baseField;
			color_ = color;
			newColor_ = color;
			SetTextureFromColor();
			BuildJob = new ConstructingJob(this);
		}
		
		public ConstructingJob BuildJob { get; private set; }

		/// <summary>
		/// Funkcja sprawdza czy podane koordynaty znajduja sie wewnatrz elementu
		/// </summary>
		/// <param name="x">Pozycja X na ekranie</param>
		/// <param name="y">Pozycja Y na ekranie</param>
		/// <returns>Zwraca true, jesli podane koordynaty znajduja sie wewnatrz obiektu, w przeciwnym wypadku false</returns>
		public virtual bool InsideElement(int x, int y) {
			return (ScreenPosition.X <= x) && (x < ScreenPosition.X + ScreenSize.X) && (ScreenPosition.Y <= y) &&
				   (y < ScreenPosition.Y + ScreenSize.Y);
		}

		/// <summary>
		/// Funkcja sprawdza czy podane koordynaty znajduja sie wewnatrz elementu
		/// </summary>
		/// <param name="poition">Kordynaty na ekranie</param>
		/// <returns>Zwraca true, jesli podane koordynaty znajduja sie wewnatrz obiektu, w przeciwnym wypadku false</returns>
		public virtual bool InsideElement(Vector2i poition) {
			return (ScreenPosition.X <= poition.X) && (poition.X < ScreenPosition.X + ScreenSize.X) &&
				   (ScreenPosition.Y <= poition.Y) && (poition.Y < ScreenPosition.Y + ScreenSize.Y);
		}

		/// <summary>
		/// Tekstura jaka ma być wyświetlana na ekranie
		/// </summary>
		public Sprite Texture {
			get {
				if ( State == Status.Planned ) return PlannedTexture;
				if ( State == Status.UnderConstruction ) return UnderConstructionTexture;
				if ( State == Status.Done ) return DoneTexture;

				throw new ArgumentOutOfRangeException(nameof(State), State, "State is out of range");
			}
		}

		public Sprite PlannedTexture {
			get { return plannedTexture_; }
			private set { plannedTexture_ = TransformTexture(value); }
		}

		public Sprite UnderConstructionTexture {
			get { return underConstructionTexture_; }
			private set { underConstructionTexture_ = TransformTexture(value); }
		}

		public Sprite DoneTexture {
			get { return doneTexture_; }
			private set { doneTexture_ = TransformTexture(value); }
		}

		/// <summary>
		/// Funkcja transformujaca tekture tak, zeby jej punkt Origin byl w srodku
		/// </summary>
		/// <param name="tex">Tekstura ktora trzeba przetransformowac</param>
		/*protected virtual void TransformTexture(Sprite tex) {
			float x = Location.Select(l => l.ScreenPosition.X)
							  .Min();
			float y = Location.Select(l => l.ScreenPosition.Y)
							  .Min();
			tex.Position = new Vector2f(x, y);
		}*/
		public Status State {
			get {
				if ( ConstructPoints <= 0 ) return Status.Planned;
				if ( ConstructPoints >= MaxConstructPoints ) return Status.Done;

				return Status.UnderConstruction;
			}
		}

		public int WorkUnit { get; set; } = -1;

		/// <summary>
		/// Zwraca czy zadane stworzenie jest zaznaczone
		/// </summary>
		public bool IsSelected { get; set; } = false;

		public float ConstructPoints {
			get { return constructPoints_; }
			set {
				constructPoints_ = value;
				lastColor_ = newColor_;
				newColor_ = new Color(color_.R, color_.G, color_.B, (byte)Lerp(50d, 255d, value / MaxConstructPoints));
				if ( lastColor_?.A != newColor_?.A )
					UnderConstructionTexture =
							new Sprite(GenerateConstructTexture((uint)MapField.ScreenSize * Size.X,
																(uint)MapField.ScreenSize * Size.Y,
																(Color)newColor_));
			}
		}

		public int MaxConstructPoints { get; set; }
		/// <summary>
		/// Zwraca pole na mapie, na ktorym znajduje sie dane stworzenie
		/// </summary>
		public MapFieldList Location { get; }

		public MapField BaseField {
			get { return baseField_; }
			set {
				Location.Clear();
				for ( int x = 0 ; x < Size.X ; ++x ) {
					for ( int y = 0 ; y < Size.Y ; ++y ) {
						try {
							if ( !value.Neighbour[x, y].IsAvaliable ) throw new FieldNotAvaliableException();

							Location.Add(value.Neighbour[x, y]);
						} catch ( Exception ) {
							Location.Clear();
							throw;
						}
					}
				}

				baseField_ = value;
			}
		}

		public string Name { get; set; }

		public Vector2u Size { get; private set; }

		/// <summary>
		/// Zwraca pozucje na ekranie danego stworzenia
		/// </summary>
		public Vector2f ScreenPosition {
			get { return Texture.Position; }
			set {
				PlannedTexture.Position = value;
				UnderConstructionTexture.Position = value;
				DoneTexture.Position = value;
			}
		}

		/// <summary>
		/// Zwraca rozmiar tekstury
		/// </summary>
		public Vector2u ScreenSize => Texture.Texture.Size;

		/// <summary>
		/// Funkcja rysujaca teksture w zaleznosci czy jest on widoczny na ekranie czy nie
		/// </summary>
		/// <param name="target">Obiekt na ktorym ma byc narysowane stworzenie</param>
		/// <param name="states">Stan</param>
		public virtual void Draw(RenderTarget target, RenderStates states) {
			if ( (Texture.Position.X >= -Texture.Texture.Size.X) &&
				 (Texture.Position.X <= Program.window.Size.X) &&
				 (Texture.Position.Y >= -Texture.Texture.Size.Y) &&
				 (Texture.Position.Y <= Program.window.Size.Y) ) {
				target.Draw(Texture, states);
			}
		}

		protected static readonly Random rand_ = new Random();
		private MapField baseField_;

		public enum Status {
			Planned = 0,
			UnderConstruction = 1,
			Done = 2,
		}

		private Color color_;
		private Sprite plannedTexture_;
		private Sprite underConstructionTexture_;
		private Sprite doneTexture_;
		private float constructPoints_ = 0f;

		public void SetTextureFromColor() {
			PlannedTexture = UnderConstructionTexture =
									 new Sprite(GenerateConstructTexture((uint)MapField.ScreenSize * Size.X,
																		 (uint)MapField.ScreenSize * Size.Y,
																		 new Color(color_.R, color_.G, color_.B, 50)));

			DoneTexture =
					new Sprite(GenerateConstructTexture((uint)MapField.ScreenSize * Size.X,
														(uint)MapField.ScreenSize * Size.Y,
														new Color(color_.R, color_.G, color_.B, 255)));
		}

		private Color? lastColor_ = null;
		private Color? newColor_ = null;

		private Sprite TransformTexture(Sprite tex) {
			tex.Position = BaseField.ScreenPosition;
			return tex;
		}
	}
}