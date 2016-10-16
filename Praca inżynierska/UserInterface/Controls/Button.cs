using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Audio;
using SFML.System;
using SFML.Window;
using static PracaInzynierska.Textures.GUITextures;

namespace PracaInzynierska.UserInterface.Controls {

	/// <summary>
	/// Klasa reprezentujaca przycisk
	/// </summary>
	public class Button : GUIElement {
		#region Properities

		/// <summary>
		/// Obiekt slozacy do rysowania pola
		/// </summary>
		public Sprite ButtonTexture { get; set; }

		/// <summary>
		/// Pozycja na ekranie
		/// </summary>
		public override Vector2f Position {
			get { return ButtonTexture.Position; }
			set {
				ButtonTexture.Position = value;
				CenterButtonTextRect();
			}
		}

		/// <summary>
		/// Rozmiar elementu
		/// </summary>
		public override Vector2u Size => ButtonTexture.Texture.Size;

		/// <summary>
		/// Tekst jaki wyswietlany jest na przycisku
		/// </summary>
		public Text ButtonText { get; set; }

		#endregion

		#region EventFunc

		internal override void GUI_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e) { }

		#endregion

		#region PrivateFunc

		private void CenterButtonTextRect() {
			FloatRect buttonTextRect = ButtonText.GetLocalBounds();
			ButtonText.Origin = new Vector2f(buttonTextRect.Width / 2, buttonTextRect.Height / 2);
			ButtonText.Position = new Vector2f(ButtonTexture.Position.X + ButtonTexture.Texture.Size.X / 2, ButtonTexture.Position.Y + ButtonTexture.Texture.Size.Y / 2);
		}

		protected override void OnDraw(RenderTarget target, RenderStates states) {
			target.Draw(ButtonTexture);
			target.Draw(ButtonText);
		} 
		#endregion

	}
}
