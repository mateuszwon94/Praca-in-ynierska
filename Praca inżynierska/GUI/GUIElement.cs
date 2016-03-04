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

		virtual internal protected void GUI_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e) { }

		virtual internal protected void GUI_MouseMoved(object sender, MouseMoveEventArgs e) { }

		virtual internal protected void GUI_MouseLeft(object sender, EventArgs e) { }

		virtual internal protected void GUI_MouseEntered(object sender, EventArgs e) { }

		virtual internal protected void GUI_MouseButtonReleased(object sender, MouseButtonEventArgs e) { }

		virtual internal protected void GUI_MouseButtonPressed(object sender, MouseButtonEventArgs e) { }

		virtual internal protected void GUI_KeyReleased(object sender, KeyEventArgs e) { }

		virtual internal protected void GUI_KeyPressed(object sender, KeyEventArgs e) { }

		abstract public void Draw(RenderTarget target, RenderStates states);
	}
}
