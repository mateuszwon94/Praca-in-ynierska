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
	class GUIBase : List<GUIElement>, Drawable {

		public GUIBase(RenderWindow window) : base() {
			AddEvents(window);
		}

		public GUIBase(int capacity, RenderWindow window) : base(capacity) {
			AddEvents(window);
		}

		public GUIBase(IEnumerable<GUIElement> collection, RenderWindow window) : base(collection) {
			AddEvents(window);
		}
		
		private void AddEvents(RenderWindow window) {
			window.KeyPressed += Window_KeyPressed;
			window.KeyReleased += Window_KeyReleased;
			window.MouseButtonPressed += Window_MouseButtonPressed;
			window.MouseButtonReleased += Window_MouseButtonReleased;
			window.MouseMoved += Window_MouseMoved;
			window.MouseWheelScrolled += Window_MouseWheelScrolled;
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

		new public void Add(GUIElement item) {
			base.Add(item);
			AddToEvents(item);
		}

		new public void AddRange(IEnumerable<GUIElement> collection) {
			base.AddRange(collection);
			foreach (var item in collection) {
				AddToEvents(item);
			}
		}

		new public void Clear() {
			foreach	(var item in this) {
				Remove(item);
			}
		}

		new public void Insert(int index, GUIElement item) {
			base.Insert(index, item);
			AddToEvents(item);
		}

		new public void InsertRange(int index, IEnumerable<GUIElement> collection) {
			base.InsertRange(index, collection);
			foreach ( var item in collection ) {
				AddToEvents(item);
			}
		}

		new public bool Remove(GUIElement item) {
			if ( base.Remove(item) ) {
				RemoveFromEvents(item);
				return true;
			}
			return false;
		}

		new public int RemoveAll(Predicate<GUIElement> match) {
			List<GUIElement> items = FindAll(match);
			foreach ( var item in items ) {
				RemoveFromEvents(item);
			}
			return base.RemoveAll(match);
		}

		new public void RemoveAt(int index) {
			RemoveFromEvents(this[index]);
			base.RemoveAt(index);
		}

		new public void RemoveRange(int index, int count) {
			for (int i = index ; i < index+count ; ++i ) {
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


		private void Window_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e) {
			EventHandler<MouseWheelScrollEventArgs> handler = MouseWheelScrolled;

			if ( handler != null ) {
				handler(this, e);
			}
		}


		private void Window_MouseMoved(object sender, MouseMoveEventArgs e) {
			EventHandler<MouseMoveEventArgs> handler = MouseMoved;

			if ( handler != null ) {
				handler(this, e);
			}
		}

		private void Window_MouseButtonReleased(object sender, MouseButtonEventArgs e) {
			EventHandler<MouseButtonEventArgs> handler = MouseButtonReleased;

			if ( handler != null ) {
				handler(this, e);
			}
		}

		private void Window_MouseButtonPressed(object sender, MouseButtonEventArgs e) {
			EventHandler<MouseButtonEventArgs> handler = MouseButtonPressed;

			if ( handler != null ) {
				handler(this, e);
			}
		}

		private void Window_KeyReleased(object sender, KeyEventArgs e) {
			EventHandler<KeyEventArgs> handler = KeyReleased;

			if ( handler != null ) {
				handler(this, e);
			}
		}

		private void Window_KeyPressed(object sender, KeyEventArgs e) {
			EventHandler<KeyEventArgs> handler = KeyPressed;

			if ( handler != null ) {
				handler(this, e);
			}
		}
		
	}
}
