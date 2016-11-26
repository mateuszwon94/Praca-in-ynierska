using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using PracaInzynierska;

namespace PracaInzynierska.UserInterface {

	/// <summary>
	/// Klasa abstrakcyjna po ktorej dziedzicza wszystkie obiekty GUI
	/// </summary>
	public abstract class GUIElement : Drawable {

		#region Drawable

		/// <summary>
		/// Funkcja rysujaca element
		/// </summary>
		/// <param name="target">Cel, na ktorym jest rysowany</param>
		/// <param name="states">Stan</param>
		public void Draw(RenderTarget target, RenderStates states) {
			if ( IsActive ) {
				OnDraw(target, states);
			}
		}

		protected abstract void OnDraw(RenderTarget target, RenderStates states);

		#endregion

		#region Func

		/// <summary>
		/// Funkcja sprawdza czy podane koordynaty znajduja sie wewnatrz elementu
		/// </summary>
		/// <param name="x">Pozycja X na ekranie</param>
		/// <param name="y">Pozycja Y na ekranie</param>
		/// <returns>Zwraca true, jesli podane koordynaty znajduja sie wewnatrz obiektu, w przeciwnym wypadku false</returns>
		public bool InsideElement(int x, int y) {
			return (Position.X <= x) && (x < Position.X + ScreenSize.X) && (Position.Y <= y) && (y < Position.Y + ScreenSize.Y);
		}

		/// <summary>
		/// Funkcja sprawdza czy podane koordynaty znajduja sie wewnatrz elementu
		/// </summary>
		/// <param name="poition">Kordynaty na ekranie</param>
		/// <returns>Zwraca true, jesli podane koordynaty znajduja sie wewnatrz obiektu, w przeciwnym wypadku false</returns>
		public bool InsideElement(Vector2i poition) {
			return (Position.X <= poition.X) && (poition.X < Position.X + ScreenSize.X) && (Position.Y <= poition.Y) && (poition.Y < Position.Y + ScreenSize.Y);
		}

		#endregion

		#region Properities

		/// <summary>
		/// Nazwa elementu
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Pozycja na ekranie
		/// </summary>
		public abstract Vector2f Position { get; set; }

		/// <summary>
		/// Rozmiar elementu
		/// </summary>
		public abstract Vector2u ScreenSize { get; }

		/// <summary>
		/// Czy element jest aktywny
		/// </summary>
		public bool IsActive { get; set; }

		#endregion

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
		/// Event wywołyany przy przesunieciu mysza.
		/// </summary>
		public event EventHandler<MouseMoveEventArgs> MouseMoved;

		/// <summary>
		/// Event wywolywany przy przekreceniu scrollem.
		/// </summary>
		public event EventHandler<MouseWheelScrollEventArgs> MouseWheelScrolled;

		#endregion Events

		#region EventHandlers

		public EventHandler<KeyEventArgs> KeyPressedHandler {
			get { return null; }
			set { KeyPressed += value; }
		}

		public EventHandler<KeyEventArgs> KeyReleasedHandler {
			get { return null; }
			set { KeyReleased += value; }
		}

		public EventHandler<MouseButtonEventArgs> MouseButtonPressedHandler {
			get { return null; }
			set { MouseButtonPressed += value; }
		}

		public EventHandler<MouseButtonEventArgs> MouseButtonReleasedHandler {
			get { return null; }
			set { MouseButtonReleased += value; }
		}

		public EventHandler<MouseMoveEventArgs> MouseMovedHandler {
			get { return null; }
			set { MouseMoved += value; }
		}

		public EventHandler<MouseWheelScrollEventArgs> MouseWheelScrolledHandler {
			get { return null; }
			set { MouseWheelScrolled += value; }
		}

		internal virtual void GUI_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e) {
			EventHandler<MouseWheelScrollEventArgs> handler = MouseWheelScrolled;

			if ( (handler != null) && IsActive && InsideElement(Mouse.GetPosition()) ) {
				handler(this, e);
			}
		}

		internal virtual void GUI_MouseMoved(object sender, MouseMoveEventArgs e) {
			EventHandler<MouseMoveEventArgs> handler = MouseMoved;

			if ( (handler != null) && IsActive && InsideElement(e.X, e.Y) ) {
				handler(this, e);
			}
		}

		internal virtual void GUI_MouseButtonReleased(object sender, MouseButtonEventArgs e) {
			EventHandler<MouseButtonEventArgs> handler = MouseButtonReleased;

			if ( (handler != null) && IsActive && InsideElement(e.X, e.Y) ) {
				handler(this, e);
			}
		}

		internal virtual void GUI_MouseButtonPressed(object sender, MouseButtonEventArgs e) {
			EventHandler<MouseButtonEventArgs> handler = MouseButtonPressed;

			if ( (handler != null) && IsActive && InsideElement(e.X, e.Y) ) {
				handler(this, e);
			}
		}

		internal virtual void GUI_KeyReleased(object sender, KeyEventArgs e) {
			EventHandler<KeyEventArgs> handler = KeyReleased;

			if ( (handler != null) && IsActive ) {
				handler(this, e);
			}
		}

		internal virtual void GUI_KeyPressed(object sender, KeyEventArgs e) {
			EventHandler<KeyEventArgs> handler = KeyPressed;

			if ( (handler != null) && IsActive ) {
				handler(this, e);
			}
		}

		#endregion EventHandlers

	}
}
