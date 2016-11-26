using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PracaInzynierska.Map;
using PracaInzynierska.UserInterface;
using PracaInzynierska.UserInterface.Controls;
using SFML;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static System.Console;
using PracaInzynierska.Textures;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Utils.Algorithm;
using static PracaInzynierska.Beeings.Men;
using static PracaInzynierska.Textures.GUITextures;
using static PracaInzynierska.Textures.MapTextures;

namespace PracaInzynierska.Constructs {
	using Beeings;

	public class Construct : Drawable {
		public Construct(uint x, uint y, Color color) {
			Size = new Vector2u(x, y);
			color_ = color;
			Location = new MapFieldList(this);
			SetTextureFromColor();
		}

		public Construct(Vector2u size, Color color) {
			Size = size;
			color_ = color;
			Location = new MapFieldList(this);
			SetTextureFromColor();
		}

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
				if ( Status == State.Planned ) return PlannedTexture;
				if ( Status == State.UnderConstruction ) return UnderConstructionTexture;
				if ( Status == State.Done ) return DoneTexture;

				throw new ArgumentOutOfRangeException(nameof(Status), Status, "Status is out of range");
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
		public State Status { get; set; } = State.Planned;

		public int WorkUnit { get; set; } = -1;

		/// <summary>
		/// Zwraca czy zadane stworzenie jest zaznaczone
		/// </summary>
		public bool IsSelected { get; set; } = false;

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

				DoneTexture = DoneTexture;
				PlannedTexture = PlannedTexture;
				UnderConstructionTexture = UnderConstructionTexture;
			}
		}

		public Vector2u Size { get; private set; }

		/// <summary>
		/// Zwraca pozucje na ekranie danego stworzenia
		/// </summary>
		public Vector2f ScreenPosition { get; set; }

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

		public class MapFieldList {
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

			private readonly Construct parent_;
			private readonly List<MapField> list_ = new List<MapField>();
		}

		public enum State {
			Planned = 0,
			UnderConstruction = 1,
			Done = 2,
		}

		private Color color_;
		private Sprite plannedTexture_;
		private Sprite underConstructionTexture_;
		private Sprite doneTexture_;

		private void SetTextureFromColor() {
			PlannedTexture =
					new Sprite(GenerateConstructTexture((uint)MapField.ScreenSize * Size.X,
														(uint)MapField.ScreenSize * Size.Y,
														new Color(color_.R, color_.G, color_.B, 75)));

			UnderConstructionTexture =
					new Sprite(GenerateConstructTexture((uint)MapField.ScreenSize * Size.X,
														(uint)MapField.ScreenSize * Size.Y,
														new Color(color_.R, color_.G, color_.B, 200)));

			DoneTexture =
					new Sprite(GenerateConstructTexture((uint)MapField.ScreenSize * Size.X,
														(uint)MapField.ScreenSize * Size.Y,
														new Color(color_.R, color_.G, color_.B, 255)));
		}



		private Sprite TransformTexture(Sprite tex) {
			if ( BaseField != null ) tex.Position = BaseField.ScreenPosition;
			return tex;
		}
	}
}