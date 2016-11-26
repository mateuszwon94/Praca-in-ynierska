namespace PracaInzynierska.Beeings {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Events;
	using Map;
	using SFML.Graphics;
	using SFML.System;
	using Utils.Algorithm;
	using static Utils.Math.Math;

	/// <summary>
	/// Klasa odpowiadajaca za zwierzeta
	/// </summary>
	public class Animal : Beeing {

		/// <summary>
		/// Funkcja wywoływana przy kazdym odswierzeniu okranu
		/// </summary>
		/// <param name="sender">Obiekt wysylajacy zdazenie</param>
		/// <param name="e">Argumenty zdarzenia</param>
		public override void UpdateTime(object sender, UpdateEventArgs e) {

			// pewne opóźnienie, zeby zwierzeta nie biegaly po planszy a czasem przystawaly
			if ( !IsMoveing && (counter_ == 0) && (rand_.NextDouble() <= 0.25) ) { counter_ = (uint)rand_.Next(50, 150); }
			if ( counter_ != 0 ) { --counter_; }

			// ruszanie sie zwierzecia
			if ( (GoToField != null) && (counter_ == 0)) {

				// Stworzenie sciezki i wybranie pola na ktore ma sie zwierze poruszyc
				if ( !Location.Neighbour.Contains(GoToField) ) {
					if ( isOnfield_ && (path_ == null) ) {
						try {
							path_ = PathFinding.AStar(Location, GoToField, PathFinding.Metric.ManhattanDistance);
							GoToField = path_[1];
							path_ = null;
						} catch ( Exception ) {
							return;
						}
					}
				}

				IsMoveing = true;

				// obliczenie ile dotychczasowej drogi miedzy dwoma polami przebyla jednostka
				moved_ += (float)(e.UpdateTime * MoveSpeed * Location.MoveSpeed);

				// obliczenie na tej podstawei pozycji na mapie
				ScreenPosition = new Vector2f((float)Lerp(Location.Center.X, GoToField.Center.X, moved_),
											  (float)Lerp(Location.Center.Y, GoToField.Center.Y, moved_));

				// jesli przebyla cala droge zmiana jej lokacji
				if ( moved_ >= 1.0f ) { Location = GoToField; }
			}

			if ( GoToField == Location ) {
				path_ = null;
				GoToField = null;
				IsMoveing = false;
				isOnfield_ = true;
				moved_ = 0.0f;
			}

		}

		/// <summary>
		/// Funkcja transformujaca tekture tak, zeby jej punkt Origin byl w srodku
		/// </summary>
		/// <param name="tex">Tekstura ktora trzeba przetransformowac</param>
		protected override Sprite TransformTexture(Sprite tex) {
			base.TransformTexture(tex);
			if ( GoToField != null ) {
				tex.Position += new Vector2f((float)Lerp(tex.Position.X, GoToField.Center.X, moved_),
											 (float)Lerp(tex.Position.Y, GoToField.Center.Y, moved_));
			}
			return tex;
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

		/// <summary>
		/// Pole do ktorego zwierze stara sie dostac
		/// </summary>
		public MapField GoToField { get; set; }

		/// <summary>
		/// Zraca czy zwierze obecnie sie porusza czy nie
		/// </summary>
		public bool IsMoveing { get; private set; } = false;

		private IList<MapField> path_;

		private float moved_;

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

		private bool isOnfield_ = true;
		private uint counter_;
	}
}
