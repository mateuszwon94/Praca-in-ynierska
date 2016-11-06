using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Audio;
using SFML.System;
using SFML.Window;
using PracaInzynierska.Events;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Map;
using static System.Math;
using PracaInzynierska.Utils.Interfaces;

namespace PracaInzynierska.Beeing {

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
			return (ScreenPosition.X <= x) && (x < ScreenPosition.X + Size.X) && (ScreenPosition.Y <= y) && (y < ScreenPosition.Y + Size.Y);
		}

		/// <summary>
		/// Funkcja sprawdza czy podane koordynaty znajduja sie wewnatrz elementu
		/// </summary>
		/// <param name="poition">Kordynaty na ekranie</param>
		/// <returns>Zwraca true, jesli podane koordynaty znajduja sie wewnatrz obiektu, w przeciwnym wypadku false</returns>
		public virtual bool InsideElement(Vector2i poition) {
			return (ScreenPosition.X <= poition.X) && (poition.X < ScreenPosition.X + Size.X) && (ScreenPosition.Y <= poition.Y) && (poition.Y < ScreenPosition.Y + Size.Y);
		}

		/// <summary>
		/// Tekstura jaka ma być wyświetlana na ekranie
		/// </summary>
		public abstract Sprite Texture { get; set; }

		/// <summary>
		/// Funkcja transformujaca tekture tak, zeby jej punkt Origin byl w srodku
		/// </summary>
		/// <param name="tex">Tekstura ktora trzeba przetransformowac</param>
		protected virtual void TransformTexture(Sprite tex) {
			tex.Origin = new Vector2f(tex.Texture.Size.X / 2f, tex.Texture.Size.Y / 2f);
			tex.Position = Location.Center;
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
				value.OnField.Remove(this);
				mapField_ = value;
				mapField_.OnField.Add(this);
			}
		}

		/// <summary>
		/// Zwraca pozucje na ekranie danego stworzenia
		/// </summary>
		public abstract Vector2f ScreenPosition { get; set; }

		/// <summary>
		/// Zwraca rozmiar tekstury
		/// </summary>
		public Vector2u Size => Texture.Texture.Size;

		private MapField mapField_;

		/// <summary>
		/// Funkcja rysujaca teksture w zaleznosci czy jest on widoczny na ekranie czy nie
		/// </summary>
		/// <param name="target">Obiekt na ktorym ma byc narysowane stworzenie</param>
		/// <param name="states">Stan</param>
		public virtual void Draw(RenderTarget target, RenderStates states) {
			if ( (Texture.Position.X >= -Texture.Texture.Size.X) &&
				 (Texture.Position.X <= Program.window.Size.X) &&
				 (Texture.Position.Y >= -Texture.Texture.Size.Y) &&
				 (Texture.Position.Y <= Program.window.Size.Y) ) { target.Draw(Texture, states); }
		}

		/// <summary>
		/// Funkcja obliczajaca dystans miedzy dwoma stworzeniami
		/// </summary>
		/// <param name="from">Stworzenie od ktorego liczymy odleglosc</param>
		/// <param name="to">Stworzenie do ktorego liczymy odleglosc</param>
		/// <returns>Odleglosc miedzy stworzeniami</returns>
		public static float Distance(Beeing from, Beeing to) {
			return (float) Round(Sqrt((from.Location.MapPosition.X - to.Location.MapPosition.X) *
									  (from.Location.MapPosition.X - to.Location.MapPosition.X) +
									  (from.Location.MapPosition.Y - to.Location.MapPosition.Y) *
									  (from.Location.MapPosition.Y - to.Location.MapPosition.Y)), 4);
		}

		protected static readonly Random rand_ = new Random();
	}
}
