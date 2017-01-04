using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PracaInzynierska.Events;
using PracaInzynierska.Map;
using PracaInzynierska.Utils.Algorithm;
using PracaInzynierska.Utils.FuzzyLogic.Variables;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using PracaInzynierska.Utils.Interfaces;
using static PracaInzynierska.Utils.Math;
using static PracaInzynierska.Utils.Algorithm.PathFinding.Metric;
using static System.Math;

namespace PracaInzynierska.Beeings {

	/// <summary>
	/// Podstawowa klasa odpowiedajaca za zyjace stworzenie
	/// </summary>
	public abstract class Beeing : Drawable, IUpdateTime {

		/// <summary>
		/// Funkcja wywoływana przy kazdym odswierzeniu okranu
		/// </summary>
		/// <param name="sender">Obiekt wysylajacy zdazenie</param>
		/// <param name="e">Argumenty zdarzenia</param>
		public abstract void UpdateTime(object sender, UpdateEventArgs e);

		public FuzzyHP HP { get; set; }
		public float Strength { get; set; }

		public virtual void Die() { }

		#region Events

		/// <summary>
		/// Event wywoływany przy nacisnieciu klawisza
		/// </summary>
		public event EventHandler<KeyEventArgs> KeyPressed;

		/// <summary>
		/// Event wywolywaniu przy puzczeniu klawisza.
		/// </summary>
		public event EventHandler<KeyEventArgs> KeyReleased;

		/// <summary>
		/// Event wywolywanu przy nacisniecku klawisa myszy.
		/// </summary>
		public event EventHandler<MouseButtonEventArgs> MouseButtonPressed;

		/// <summary>
		/// Event wywolywany przy pusszczeniu klawisza myszy.
		/// </summary>
		public event EventHandler<MouseButtonEventArgs> MouseButtonReleased;

		/// <summary>
		/// Funkcja wywoływana przez zdazrenie puszczenia klawisza myszy
		/// </summary>
		/// <param name="sender">obiekt wysylajacy zdarzenie</param>
		/// <param name="e">argumenty zdarzenia</param>
        public virtual void Window_MouseButtonReleased(object sender, MouseButtonEventArgs e) {
			if ( InsideElement(e.X, e.Y) ) {
				EventHandler<MouseButtonEventArgs> handler = MouseButtonReleased;
				handler?.Invoke(sender, e);
			}
		}

		/// <summary>
		/// Funkcja wywoływana przez zdazrenie wcisniecia klawisza myszy
		/// </summary>
		/// <param name="sender">obiekt wysylajacy zdarzenie</param>
		/// <param name="e">argumenty zdarzenia</param>
		public virtual void Window_MouseButtonPressed(object sender, MouseButtonEventArgs e) {
			if ( InsideElement(e.X, e.Y) ) {
				EventHandler<MouseButtonEventArgs> handler = MouseButtonPressed;
				handler?.Invoke(sender, e);
			}
			if ( IsSelected ) {

			}
		}

		/// <summary>
		/// Funkcja wywoływana przez zdazrenie puszczenia klawisza na klawiaturze
		/// </summary>
		/// <param name="sender">obiekt wysylajacy zdarzenie</param>
		/// <param name="e">argumenty zdarzenia</param>
		public virtual void Window_KeyReleased(object sender, KeyEventArgs e) {
			EventHandler<KeyEventArgs> handler = KeyReleased;
			handler?.Invoke(sender, e);
		}

		/// <summary>
		/// Funkcja wywoływana przez zdazrenie przycisniecia klawisza na klawiaturze
		/// </summary>
		/// <param name="sender">obiekt wysylajacy zdarzenie</param>
		/// <param name="e">argumenty zdarzenia</param>
		public virtual void Window_KeyPressed(object sender, KeyEventArgs e) {
			EventHandler<KeyEventArgs> handler = KeyPressed;
			handler?.Invoke(sender, e);
		}

		#endregion Events

		/// <summary>
		/// Funkcja sprawdza czy podane koordynaty znajduja sie wewnatrz elementu
		/// </summary>
		/// <param name="x">Pozycja X na ekranie</param>
		/// <param name="y">Pozycja Y na ekranie</param>
		/// <returns>Zwraca true, jesli podane koordynaty znajduja sie wewnatrz obiektu, w przeciwnym wypadku false</returns>
		public virtual bool InsideElement(int x, int y) {
			return (ScreenPosition.X <= x) && (x < ScreenPosition.X + ScreenSize.X) &&
				   (ScreenPosition.Y <= y) && (y < ScreenPosition.Y + ScreenSize.Y);
		}

		/// <summary>
		/// Funkcja sprawdza czy podane koordynaty znajduja sie wewnatrz elementu
		/// </summary>
		/// <param name="poition">Kordynaty na ekranie</param>
		/// <returns>Zwraca true, jesli podane koordynaty znajduja sie wewnatrz obiektu, w przeciwnym wypadku false</returns>
		public virtual bool InsideElement(Vector2i poition) {
			return (ScreenPosition.X <= poition.X) && (poition.X < ScreenPosition.X + ScreenSize.X) && (ScreenPosition.Y <= poition.Y) && (poition.Y < ScreenPosition.Y + ScreenSize.Y);
		}

		/// <summary>
		/// Tekstura jaka ma być wyświetlana na ekranie
		/// </summary>
		public abstract Sprite Texture { get; set; }

		/// <summary>
		/// Funkcja transformujaca tekture tak, zeby jej punkt Origin byl w srodku
		/// </summary>
		/// <param name="tex">Tekstura ktora trzeba przetransformowac</param>
		protected Sprite TransformTexture(Sprite tex) {
			tex.Origin = new Vector2f(tex.Texture.Size.X / 2f, tex.Texture.Size.Y / 2f);
			tex.Position = Location.Center;
			if ( GoToField != null ) {
				tex.Position += new Vector2f((float)Lerp(tex.Position.X, GoToField.Center.X, moved_),
											 (float)Lerp(tex.Position.Y, GoToField.Center.Y, moved_));
			}
			return tex;
		}

		/// <summary>
		/// Zwraca czy zadane stworzenie jest zaznaczone
		/// </summary>
		public bool IsSelected { get; set; }

		/// <summary>
		/// Zwraca predkosc poruzania się danego stworzenia
		/// </summary>
		public double MoveSpeed { get; set; } = 1.0;

		/// <summary>
		/// Zwraca pole na mapie, na ktorym znajduje sie dane stworzenie
		/// </summary>
		public MapField Location {
			get { return mapField_; }
			set {
				mapField_?.OnField.Remove(this);
				mapField_ = value;
				mapField_.OnField.Add(this);
			}
        }
        public Vector2f ExactPosition {
	        get {
		        if ( IsMoveing )
			        return new Vector2f((float)Lerp(Location.MapPosition.X, GoToField.MapPosition.X, moved_),
										(float)Lerp(Location.MapPosition.Y, GoToField.MapPosition.Y, moved_));
		        else return new Vector2f(Location.MapPosition.X, Location.MapPosition.Y);
	        }
        }

		/// <summary>
        /// Zwraca pozucje na ekranie danego stworzenia
        /// </summary>
        public abstract Vector2f ScreenPosition { get; set; }

		/// <summary>
		/// Zwraca rozmiar tekstury
		/// </summary>
		public Vector2u ScreenSize => Texture.Texture.Size;

		private MapField mapField_;

		/// <summary>
		/// Funkcja rysujaca teksture w zaleznosci czy jest on widoczny na ekranie czy nie
		/// </summary>
		/// <param name="target">Obiekt na ktorym ma byc narysowane stworzenie</param>
		/// <param name="states">Stan</param>
		public virtual void Draw(RenderTarget target, RenderStates states) {
			target.Draw(Texture, states);
		}

		/// <summary>
		/// Funkcja obliczajaca dystans miedzy dwoma stworzeniami
		/// </summary>
		/// <param name="from">Stworzenie od ktorego liczymy odleglosc</param>
		/// <param name="to">Stworzenie do ktorego liczymy odleglosc</param>
		/// <returns>Odleglosc miedzy stworzeniami</returns>
		public static float Distance(Beeing from, Beeing to) {
			return (float)Sqrt((from.ExactPosition.X - to.ExactPosition.X) * (from.ExactPosition.X - to.ExactPosition.X) +
		                       (from.ExactPosition.Y - to.ExactPosition.Y) * (from.ExactPosition.Y - to.ExactPosition.Y));
		}

		/// <summary>
		/// Pole do ktorego zwierze stara sie dostac
		/// </summary>
		public MapField GoToField { get; set; }

		/// <summary>
		/// Zraca czy zwierze obecnie sie porusza czy nie
		/// </summary>
		public bool IsMoveing { get; protected set; } = false;

		protected IList<MapField> path_;
		protected float moved_;
		protected bool isOnfield_ = true;

		protected void Go(double updateTime, bool onlyOne = false) {
			// ruszanie sie zwierzecia
			if ( (GoToField != null) ) {

				// Stworzenie sciezki i wybranie pola na ktore ma sie zwierze poruszyc
				if ( (isOnfield_ && (path_ == null)) || counter_ == 0 ) {
					try {
						path_ = PathFinding.AStar(Location, GoToField, heuristic_);
						counter_ = 5;
					} catch ( Exception ) {
						return;
					}
				}

				if ( !IsMoveing && path_ != null ) {
					path_.RemoveAt(0);
					GoToField = path_[0];
					--counter_;
				}

				if ( onlyOne ) { path_ = null; }

				IsMoveing = true;

				// obliczenie ile dotychczasowej drogi miedzy dwoma polami przebyla jednostka
				moved_ += (float)(updateTime * MoveSpeed * Location.MoveSpeed);

				// obliczenie na tej podstawei pozycji na mapie
				ScreenPosition = new Vector2f((float)Lerp(Location.Center.X, GoToField.Center.X, moved_),
											  (float)Lerp(Location.Center.Y, GoToField.Center.Y, moved_));

				// jesli przebyla cala droge zmiana jej lokacji
				if ( moved_ >= 1.0f ) { Location = GoToField; }
			}

			if ( GoToField == Location ) {
				if ( path_?.Count == 1 ) {
					path_ = null;
				}
				GoToField = null;
				IsMoveing = false;
				isOnfield_ = true;
				moved_ = 0.0f;
			}
		}

		protected heuristicFunc heuristic_;

		protected static readonly Random rand_ = new Random();
		private int counter_;
	}
}
