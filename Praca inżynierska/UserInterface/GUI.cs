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
using PracaInzynierska.Events;

namespace PracaInzynierska.UserInterface {
	public class GUI : List<GUIElement>, IEventItem, Drawable {

		public GUI(RenderWindow window) {
			Textures.GUITextures.GenerateAll(40, 100);
			AddEvents(window);
		}

		public GUI(int capacity, RenderWindow window) : base(capacity) {
			Textures.GUITextures.GenerateAll(40, 100);
			AddEvents(window);
		}

		public GUI(IEnumerable<GUIElement> collection, RenderWindow window) : base(collection) {
			Textures.GUITextures.GenerateAll(40, 100);
			AddEvents(window);
		}
		
		private void AddToEvents(GUIElement item) {
			KeyPressed += item.GUI_KeyPressed;
			KeyReleased += item.GUI_KeyReleased;
			MouseButtonPressed += item.GUI_MouseButtonPressed;
			MouseButtonReleased += item.GUI_MouseButtonReleased;
			MouseMoved += item.GUI_MouseMoved;
			MouseWheelScrolled += item.GUI_MouseWheelScrolled;
		}

		private void RemoveFromEvents(GUIElement item) {
			KeyPressed -= item.GUI_KeyPressed;
			KeyReleased -= item.GUI_KeyReleased;
			MouseButtonPressed -= item.GUI_MouseButtonPressed;
			MouseButtonReleased -= item.GUI_MouseButtonReleased;
			MouseMoved -= item.GUI_MouseMoved;
			MouseWheelScrolled -= item.GUI_MouseWheelScrolled;
		}

		public new void Add(GUIElement item) {
			base.Add(item);
			AddToEvents(item);
		}

		public new void AddRange(IEnumerable<GUIElement> collection) {
			base.AddRange(collection);
			foreach (var item in collection) {
				AddToEvents(item);
			}
		}

		public new void Clear() {
			foreach	(var item in this) {
				Remove(item);
			}
		}

		public new void Insert(int index, GUIElement item) {
			base.Insert(index, item);
			AddToEvents(item);
		}

		public new void InsertRange(int index, IEnumerable<GUIElement> collection) {
			base.InsertRange(index, collection);
			foreach ( var item in collection ) {
				AddToEvents(item);
			}
		}

		public new bool Remove(GUIElement item) {
			if ( !base.Remove(item) ) return false;

			RemoveFromEvents(item);
			return true;
		}

		public new int RemoveAll(Predicate<GUIElement> match) {
			List<GUIElement> items = FindAll(match);
			foreach ( var item in items ) {
				RemoveFromEvents(item);
			}
			return base.RemoveAll(match);
		}

		public new void RemoveAt(int index) {
			RemoveFromEvents(this[index]);
			base.RemoveAt(index);
		}

		public new void RemoveRange(int index, int count) {
			for (int i = index ; i < index + count ; ++i ) {
				RemoveFromEvents(this[i]);
			}
			base.RemoveRange(index, count);
		}

		public void Draw(RenderTarget target, RenderStates states) {
			foreach ( var item in this ) {
				item.Draw(target, states);
			}
		}

		public event EventHandler<KeyEventArgs> KeyPressed;
		public event EventHandler<KeyEventArgs> KeyReleased;
		public event EventHandler<MouseButtonEventArgs> MouseButtonPressed;
		public event EventHandler<MouseButtonEventArgs> MouseButtonReleased;
		public event EventHandler<MouseMoveEventArgs> MouseMoved;
		public event EventHandler<MouseWheelScrollEventArgs> MouseWheelScrolled;


		public void Window_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e) {
			EventHandler<MouseWheelScrollEventArgs> handler = MouseWheelScrolled;
			handler?.Invoke(sender, e);
		}

		public void Window_MouseMoved(object sender, MouseMoveEventArgs e) {
			EventHandler<MouseMoveEventArgs> handler = MouseMoved;
			handler?.Invoke(sender, e);
		}

		public void Window_MouseButtonReleased(object sender, MouseButtonEventArgs e) {
			EventHandler<MouseButtonEventArgs> handler = MouseButtonReleased;
			handler?.Invoke(sender, e);
		}

		public void Window_MouseButtonPressed(object sender, MouseButtonEventArgs e) {
			EventHandler<MouseButtonEventArgs> handler = MouseButtonPressed;
			handler?.Invoke(sender, e);
		}

		public void Window_KeyReleased(object sender, KeyEventArgs e) {
			EventHandler<KeyEventArgs> handler = KeyReleased;
			handler?.Invoke(sender, e);
		}

		public void Window_KeyPressed(object sender, KeyEventArgs e) {
			EventHandler<KeyEventArgs> handler = KeyPressed;
			handler?.Invoke(sender, e);
		}

		public void AddEvents(RenderWindow window) {
			window.KeyPressed += Window_KeyPressed;
			window.KeyReleased += Window_KeyReleased;
			window.MouseButtonPressed += Window_MouseButtonPressed;
			window.MouseButtonReleased += Window_MouseButtonReleased;
			window.MouseMoved += Window_MouseMoved;
			window.MouseWheelScrolled += Window_MouseWheelScrolled;
		}
	}
}

