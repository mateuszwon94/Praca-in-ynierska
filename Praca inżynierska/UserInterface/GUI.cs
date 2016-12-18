using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace PracaInzynierska.UserInterface {

	/// <summary>
	/// Glowna klasa GUI zachowujaca sie jak kontener dla jego elementow.
	/// </summary>
	public class GUI : List<GUIElement>, Drawable {

		#region Constructors

		/// <summary>
		/// Konstruktor odpowiadający List()
		/// </summary>
		/// <param name="window">Okno, z ktorego przechwytywane beda eventy</param>
		public GUI() {
			Textures.GUITextures.GenerateAll(40, 100);
		}

		/// <summary>
		/// Konstruktor odpowiadający List(int capacity)
		/// </summary>
		/// <param name="capacity">Ilosc elementow, dla jakich ma byc przy inicjalizacji zarezerwowana pamiec</param>
		/// <param name="window">Okno, z ktorego przechwytywane beda eventy</param>
		public GUI(int capacity) : base(capacity) {
			Textures.GUITextures.GenerateAll(40, 100);
		}

		/// <summary>
		/// Konstruktor odpowiadający List(IEnumerable collection)
		/// </summary>
		/// <param name="collection">Kolekcja na podstawie, ktorej ma byc zainicjalizowana lista</param>
		/// <param name="window">Okno, z ktorego przechwytywane beda eventy</param>
		public GUI(IEnumerable<GUIElement> collection) : base(collection) {
			Textures.GUITextures.GenerateAll(40, 100);
		}

		#endregion Constructors

		#region FuncFromList

		/// <summary>
		/// Dodaje element do GUI
		/// </summary>
		/// <param name="item">Element, ktory ma zostac dodany</param>
		public new void Add(GUIElement item) {
			base.Add(item);
			AddToEvents(item);
		}

		/// <summary>
		/// Dodaje elementy do GUI
		/// </summary>
		/// <param name="item">Kolekcja elementow, ktore maja zostac dodane</param>
		public new void AddRange(IEnumerable<GUIElement> collection) {
			base.AddRange(collection);
			foreach ( GUIElement item in collection ) {
				AddToEvents(item);
			}
		}

		/// <summary>
		/// Usuwa elementu z GUI
		/// </summary>
		public new void Clear() {
			foreach ( GUIElement item in this ) {
				Remove(item);
			}
		}

		/// <summary>
		/// Wstawia element na zadanej pozycji
		/// </summary>
		/// <param name="index">Pozycja, na ktora ma byc wstawiony element</param>
		/// <param name="item">Element, ktory ma zostac wstawiony</param>
		public new void Insert(int index, GUIElement item) {
			base.Insert(index, item);
			AddToEvents(item);
		}

		/// <summary>
		/// Wstawia elementy na zadanej pozycji
		/// </summary>
		/// <param name="index">Pozycja, od ktorej maja byc wstawiane elementy</param>
		/// <param name="item">Kolekcja elementow, ktore maja zostac wstawione</param>
		public new void InsertRange(int index, IEnumerable<GUIElement> collection) {
			base.InsertRange(index, collection);
			foreach ( GUIElement item in collection ) {
				AddToEvents(item);
			}
		}

		/// <summary>
		/// Usuwa element z GUI
		/// </summary>
		/// <param name="item">Element, ktory ma zostac usuniety</param>
		/// <returns>Zwraca true, jesli udalo sie usunac element, w przyiwnym wypadku false.</returns>
		public new bool Remove(GUIElement item) {
			if ( !base.Remove(item) ) return false;

			RemoveFromEvents(item);
			return true;
		}

		/// <summary>
		/// Usuwa elementy pasujace do wzorca
		/// </summary>
		/// <param name="match">Wzorzec do jakiego dopasowywane beda elementy</param>
		/// <returns>Liczba usunietych elementow</returns>
		public new int RemoveAll(Predicate<GUIElement> match) {
			List<GUIElement> items = FindAll(match);
			foreach ( GUIElement item in items ) {
				RemoveFromEvents(item);
			}
			return base.RemoveAll(match);
		}

		/// <summary>
		/// Usuwa element z zadanej pozycji
		/// </summary>
		/// <param name="index">Pozycja, z ktorej ma byc usuniety element</param>
		public new void RemoveAt(int index) {
			RemoveFromEvents(this[index]);
			base.RemoveAt(index);
		}

		/// <summary>
		/// Usuwa okreslona ilosc elementow od zadanej pozycji.
		/// </summary>
		/// <param name="index">Pozycja, od ktorej beda usuwane elementy</param>
		/// <param name="count">Ilosc usuwanych elementow</param>
		public new void RemoveRange(int index, int count) {
			for ( int i = index ; i < index + count ; ++i ) {
				RemoveFromEvents(this[i]);
			}
			base.RemoveRange(index, count);
		}

		#endregion FuncFromList

		#region Drawable

		/// <summary>
		/// Funkcja rysujaca GUI
		/// </summary>
		/// <param name="target">Cel na ktorym jest rysowane GUI</param>
		/// <param name="states">Stan</param>
		public void Draw(RenderTarget target, RenderStates states) {
			foreach ( GUIElement item in this ) {
				item.Draw(target, states);
			}
		}

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

		#endregion Events

		#region PrivateFunc

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

		#endregion
	}
}

