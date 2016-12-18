using System;
using System.Collections.Generic;
using System.Linq;
using PracaInzynierska.Events;
using PracaInzynierska.Map;
using SFML.Graphics;
using SFML.System;
using PracaInzynierska.Utils.Algorithm;
using static PracaInzynierska.Utils.Math;
using static PracaInzynierska.Utils.Algorithm.PathFinding.Metric;

namespace PracaInzynierska.Beeings {

	/// <summary>
	/// Klasa odpowiadajaca za zwierzeta
	/// </summary>
	public class Animal : Beeing {

		public Animal() : base() {
			heuristic_ = ManhattanDistance;
		}

		/// <summary>
		/// Funkcja wywoływana przy kazdym odswierzeniu okranu
		/// </summary>
		/// <param name="sender">Obiekt wysylajacy zdazenie</param>
		/// <param name="e">Argumenty zdarzenia</param>
		public override void UpdateTime(object sender, UpdateEventArgs e) {

			// pewne opóźnienie, zeby zwierzeta nie biegaly po planszy a czasem przystawaly
			if ( !IsMoveing && (counter_ == 0) && (rand_.NextDouble() <= 0.25) ) { counter_ = (uint)rand_.Next(50, 150); }
			if ( counter_ != 0 ) { --counter_; }

			if ( (counter_ == 0)) Go(e.UpdateTime, true);

		}
		
		/// <summary>
		/// Tekstura jaka ma być wyświetlana na ekranie
		/// </summary>
		public override Sprite Texture {
			get { return texture_; }
			set {
				texture_ = TransformTexture(value);
			}
		}

		/// <summary>
		/// Zwraca pozucje na ekranie danego stworzenia
		/// </summary>
		public override Vector2f ScreenPosition {
			get { return Texture.Position; }
			set { Texture.Position = value; }
		}

		private Sprite texture_;

		/// <summary>
		/// Przeladowany operator rownosci dwoch zwierzat
		/// </summary>
		/// <param name="first">Pierwsze zwierze</param>
		/// <param name="other">Drugie zwierze</param>
		/// <returns>Zwraca czy zwierzeta sa tym samym zwierzeciem</returns>
		public static bool operator ==(Animal first, Animal other) {
			if ( (first is null) || (other is null) ) { return false; }

			return ReferenceEquals(first, other);
		}


		/// <summary>
		/// Przeladowany operator nierownosci dwoch zwierzat
		/// </summary>
		/// <param name="first">Pierwsze zwierze</param>
		/// <param name="other">Drugie zwierze</param>
		/// <returns>Zwraca czy zwierzeta nie sa tym samym zwierzeciem</returns>
		public static bool operator !=(Animal first, Animal other) {
			if ( (first is null) || (other is null) ) return false;

			return !ReferenceEquals(first, other);
		}

		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <param name="obj">The object to compare with the current object. </param>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object other) {
			return Equals((Animal)other);
		}

		protected bool Equals(Animal other) {
			if ( ReferenceEquals(null, other) ) return false;
			if ( ReferenceEquals(this, other) ) return true;
			return false;
		}

		/// <summary>
		/// Zwraca wektor opisujacy to w jakim kierunku porusza sie dane zwierze
		/// </summary>
		public Vector2f MoveVector {
			get {
				if ( !IsMoveing || (GoToField  == null)) { return new Vector2f(0, 0); }

				Vector2i vec = Location.MapPosition - GoToField.MapPosition;
				return new Vector2f(vec.X, vec.Y);
			}
		}

		private uint counter_;
	}
}
