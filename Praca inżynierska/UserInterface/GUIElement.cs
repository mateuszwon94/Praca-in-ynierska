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
	public abstract class GUIElement : Drawable {
		
		public void Draw(RenderTarget target, RenderStates states) {
			if ( IsActive ) {
				OnDraw(target, states);
			}
		}

		protected abstract void OnDraw(RenderTarget target, RenderStates states);

		public bool InsideElement(int x, int y) {
			return (Position.X <= x) && (x < Position.X + Size.X) && (Position.Y <= y) && (y < Position.Y + Size.Y);
		}

		public bool InsideElement(Vector2i poition) {
			return (Position.X <= poition.X) && (poition.X < Position.X + Size.X) && (Position.Y <= poition.Y) && (poition.Y < Position.Y + Size.Y);
		}

		public string Name { get; set; }
		
		public abstract Vector2f Position { get; set; }

		public abstract Vector2u Size { get; }

		public bool IsActive { get; set; }
		
		public event EventHandler<KeyEventArgs> KeyPressed;
		public event EventHandler<KeyEventArgs> KeyReleased;
		public event EventHandler<MouseButtonEventArgs> MouseButtonPressed;
		public event EventHandler<MouseButtonEventArgs> MouseButtonReleased;
		public event EventHandler<MouseMoveEventArgs> MouseMoved;
		public event EventHandler<MouseWheelScrollEventArgs> MouseWheelScrolled;

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
