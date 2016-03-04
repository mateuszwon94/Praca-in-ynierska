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

		abstract public void Draw(RenderTarget target, RenderStates states);

		public event EventHandler<KeyEventArgs> KeyPressed;
		public event EventHandler<KeyEventArgs> KeyReleased;
		public event EventHandler<MouseButtonEventArgs> MouseButtonPressed;
		public event EventHandler<MouseButtonEventArgs> MouseButtonReleased;
		public event EventHandler<EventArgs> MouseEntered;
		public event EventHandler<EventArgs> MouseLeft;
		public event EventHandler<MouseMoveEventArgs> MouseMoved;
		public event EventHandler<MouseWheelScrollEventArgs> MouseWheelScrolled;

		internal void GUI_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e) {
			EventHandler<MouseWheelScrollEventArgs> handler = MouseWheelScrolled;

			if ( handler != null ) {
				handler(this, e);
			}
		}
		
		internal void GUI_MouseMoved(object sender, MouseMoveEventArgs e) {
			EventHandler<MouseMoveEventArgs> handler = MouseMoved;

			if ( handler != null ) {
				handler(this, e);
			}
		}

		internal void GUI_MouseLeft(object sender, EventArgs e) {
			EventHandler<EventArgs> handler = MouseLeft;

			if ( handler != null ) {
				handler(this, e);
			}
		}

		internal void GUI_MouseEntered(object sender, EventArgs e) {
			EventHandler<EventArgs> handler = MouseEntered;
			
			if ( handler != null ) {
				handler(this, e);
			}
		}

		internal void GUI_MouseButtonReleased(object sender, MouseButtonEventArgs e) {
			EventHandler<MouseButtonEventArgs> handler = MouseButtonReleased;

			if ( handler != null ) {
				handler(this, e);
			}
		}

		internal void GUI_MouseButtonPressed(object sender, MouseButtonEventArgs e) {
			EventHandler<MouseButtonEventArgs> handler = MouseButtonPressed;

			if ( handler != null ) {
				handler(this, e);
			}
		}

		internal void GUI_KeyReleased(object sender, KeyEventArgs e) {
			EventHandler<KeyEventArgs> handler = KeyReleased;

			if ( handler != null ) {
				handler(this, e);
			}
		}

		internal void GUI_KeyPressed(object sender, KeyEventArgs e) {
			EventHandler<KeyEventArgs> handler = KeyPressed;

			if ( handler != null ) {
				handler(this, e);
			}
		}

	}
}
