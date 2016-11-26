namespace PracaInzynierska.Beeings {
	using System;
	using Events;
	using Map;
	using SFML.Graphics;
	using SFML.System;

	/// <summary>
	/// Klasa odpowiadajaca za ludzi
	/// </summary>
	public class Men : Beeing {
		public Men() {
			MouseButtonReleased += (o, e) => {
				if ( IsSelected ) IsSelected = false;
				else IsSelected = true;
			};
		}

		/// <summary>
		/// Funkcja sprawdza czy podane koordynaty znajduja sie wewnatrz elementu
		/// </summary>
		/// <param name="x">Pozycja X na ekranie</param>
		/// <param name="y">Pozycja Y na ekranie</param>
		/// <returns>Zwraca true, jesli podane koordynaty znajduja sie wewnatrz obiektu, w przeciwnym wypadku false</returns>
		public override bool InsideElement(int x, int y) {
            if ( (Location.ScreenPosition.X <= x) && (x < Location.ScreenPosition.X + ScreenSize.X) && (Location.ScreenPosition.Y <= y) && (y < Location.ScreenPosition.Y + ScreenSize.Y) ) {
                Vector2f pos = new Vector2f(x, y) - Location.ScreenPosition;
                Vector2f center = Location.Center - Location.ScreenPosition;

                if ( Math.Pow(center.X - pos.X, 2) + Math.Pow(center.Y - pos.Y, 2) <= Math.Pow(MapField.ScreenSize, 2) ) return true;
            }
            return false;
        }

		/// <summary>
		/// Tekstura jaka ma być wyświetlana na ekranie
		/// </summary>
		public override Sprite Texture {
			get { return IsSelected ? TextureSelected : TextureNotSelected; }
			set {
				if ( IsSelected ) TextureSelected = value;
				else              TextureNotSelected = value;
			}
		}

		/// <summary>
		/// Tekstura zaznaczonego czlowieka
		/// </summary>
		public Sprite TextureSelected {
			get { return textureSelected_; }
			set {
				textureSelected_ = TransformTexture(value);
			}
		}

		/// <summary>
		/// tekstura niezaznaczonego czlowieka
		/// </summary>
		public Sprite TextureNotSelected {
			get { return textureNotSelected_; }
			set {
				textureNotSelected_ = TransformTexture(value);
			}
		}

		/// <summary>
		/// Zwraca pozucje na ekranie danego stworzenia
		/// </summary>
		public override Vector2f ScreenPosition {
			get { return Texture.Position; }
			set {
				if ( TextureNotSelected != null ) TextureNotSelected.Position = value;
				if ( TextureSelected != null ) TextureSelected.Position = value;
			}
		}

		/// <summary>
		/// Funkcja wywoływana przy kazdym odswierzeniu okranu
		/// </summary>
		/// <param name="sender">Obiekt wysylajacy zdazenie</param>
		/// <param name="e">Argumenty zdarzenia</param>
		public override void UpdateTime(object sender, UpdateEventArgs e) {
            if ( IsSelected ) { }
        }

		private Sprite textureSelected_;
		private Sprite textureNotSelected_;
	}
}
