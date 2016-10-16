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

namespace PracaInzynierska.Beeing {
	public abstract class Beeing : Drawable {

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

        public virtual void Window_MouseButtonReleased(object sender, MouseButtonEventArgs e) {
			if ( InsideElement(e.X, e.Y) ) {
				EventHandler<MouseButtonEventArgs> handler = MouseButtonReleased;
				handler?.Invoke(sender, e);
			}
		}

		public virtual void Window_MouseButtonPressed(object sender, MouseButtonEventArgs e) {
			if ( InsideElement(e.X, e.Y) ) {
				EventHandler<MouseButtonEventArgs> handler = MouseButtonPressed;
				handler?.Invoke(sender, e);
			}
		}

		public virtual void Window_KeyReleased(object sender, KeyEventArgs e) {
			EventHandler<KeyEventArgs> handler = KeyReleased;
			handler?.Invoke(sender, e);
		}

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

		public abstract Sprite Texture { get; set; } 
		
		protected virtual void TransformTexture(Sprite tex) {
			tex.Origin = new Vector2f(tex.Texture.Size.X / 2f, tex.Texture.Size.Y / 2f);
			tex.Position = Location.Center;
		}

		public bool IsSelected { get; set; }

		public double MoveSpeed { get; set; } = 1.0;

		public MapField Location {
			get { return mapField_; }
			set {
				if ( value.IsUnitOn ) { throw new FieldIsNotEmptyException(); }
				else {
					mapField_ = value;
					mapField_.UnitOn = this;
				}
			}
		}
		
		public abstract Vector2f ScreenPosition { get; set; }

		public Vector2i MapPosition => mapField_.MapPosition;

		public Vector2u Size => Texture.Texture.Size;

		private MapField mapField_;

		public virtual void Draw(RenderTarget target, RenderStates states) {
			if ( (Texture.Position.X >= -Texture.Texture.Size.X) &&
				 (Texture.Position.X <= Program.window.Size.X) &&
				 (Texture.Position.Y >= -Texture.Texture.Size.Y) &&
				 (Texture.Position.Y <= Program.window.Size.Y) ) { target.Draw(Texture, states); }
		}

		public static float Distance(Beeing from, Beeing to) {
			return (float) Round(Sqrt((from.MapPosition.X - to.MapPosition.X) * (from.MapPosition.X - to.MapPosition.X) +
				   					  (from.MapPosition.Y - to.MapPosition.Y) * (from.MapPosition.Y - to.MapPosition.Y)), 4);
		}
	}
}
