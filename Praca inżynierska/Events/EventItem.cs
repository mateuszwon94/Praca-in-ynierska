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

namespace PracaInzynierska.Events {

	/// <summary>
	/// Interfejs zawierajacy wszystkie Eventy obsługi myszy i klawiatury.
	/// </summary>
	public interface IEventItem {
        event EventHandler<KeyEventArgs> KeyPressed;
        event EventHandler<KeyEventArgs> KeyReleased;
        event EventHandler<MouseButtonEventArgs> MouseButtonPressed;
        event EventHandler<MouseButtonEventArgs> MouseButtonReleased;
        event EventHandler<MouseMoveEventArgs> MouseMoved;
        event EventHandler<MouseWheelScrollEventArgs> MouseWheelScrolled;

        void AddEvents(RenderWindow window);

        void Window_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e);
        void Window_MouseMoved(object sender, MouseMoveEventArgs e);
        void Window_MouseButtonReleased(object sender, MouseButtonEventArgs e);
        void Window_MouseButtonPressed(object sender, MouseButtonEventArgs e);
        void Window_KeyReleased(object sender, KeyEventArgs e);
        void Window_KeyPressed(object sender, KeyEventArgs e);
    }
}
