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

namespace PracaInzynierska.GUI {
	abstract class GUIElement : Drawable {

		public GUIElement(string name) {
			IsActive = false;
			Name = name;
		}

		public GUIElement(string name, bool isActive) {
			IsActive = isActive;
			Name = name;
		}

		public void Draw(RenderTarget target, RenderStates states) {
			if ( IsActive ) {
				OnDraw(target, states);
			}
		}

		public string Name { get; private set; }

		protected abstract void OnDraw(RenderTarget target, RenderStates states);
		
		public abstract Vector2f Position { get; set; }

		public abstract Vector2u Size { get; }

		public bool IsActive { get; private set; }

		public bool InsideElement(int X, int Y) {
			if ( Position.X <= X && X < Position.X + Size.X && Position.Y <= Y && Y < Position.Y + Size.Y )
				return true;
			return false;
		}

		public bool InsideElement(Vector2i poition) {
			if ( Position.X <= poition.X && poition.X < Position.X + Size.X && Position.Y <= poition.Y && poition.Y < Position.Y + Size.Y )
				return true;
			return false;
		}

		public event EventHandler<KeyEventArgs> KeyPressed;
		public event EventHandler<KeyEventArgs> KeyReleased;
		public event EventHandler<MouseButtonEventArgs> MouseButtonPressed;
		public event EventHandler<MouseButtonEventArgs> MouseButtonReleased;
		public event EventHandler<MouseMoveEventArgs> MouseMoved;
		public event EventHandler<MouseWheelScrollEventArgs> MouseWheelScrolled;

		internal void GUI_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e) {
			EventHandler<MouseWheelScrollEventArgs> handler = MouseWheelScrolled;
			
			if ( handler != null && IsActive && InsideElement(Mouse.GetPosition()) ) {
				handler(this, e);
			}
		}
		
		internal void GUI_MouseMoved(object sender, MouseMoveEventArgs e) {
			EventHandler<MouseMoveEventArgs> handler = MouseMoved;

			if ( handler != null && IsActive && InsideElement(e.X, e.Y) ) {
				handler(this, e);
			}
		}

		internal void GUI_MouseButtonReleased(object sender, MouseButtonEventArgs e) {
			EventHandler<MouseButtonEventArgs> handler = MouseButtonReleased;

			if ( handler != null && IsActive && InsideElement(e.X, e.Y) ) {
				handler(this, e);
			}
		}

		internal void GUI_MouseButtonPressed(object sender, MouseButtonEventArgs e) {
			EventHandler<MouseButtonEventArgs> handler = MouseButtonPressed;

			if ( handler != null && IsActive && InsideElement(e.X, e.Y) ) {
				handler(this, e);
			}
		}

		internal void GUI_KeyReleased(object sender, KeyEventArgs e) {
			EventHandler<KeyEventArgs> handler = KeyReleased;

			if ( handler != null && IsActive ) {
				handler(this, e);
			}
		}

		internal void GUI_KeyPressed(object sender, KeyEventArgs e) {
			EventHandler<KeyEventArgs> handler = KeyPressed;

			if ( handler != null && IsActive ) {
				handler(this, e);
			}
		}

	}
}
