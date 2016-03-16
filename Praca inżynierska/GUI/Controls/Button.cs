using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Audio;
using SFML.System;
using SFML.Window;
using static PracaInzynierska.Textures.LoadedTextures.GUITextures;

namespace PracaInzynierska.GUI.Controls {
	class Button : GUIElement {

		public Button(string name, Vector2f position, Image buttonImage) : base(name) {
			ButtonTexture = new Sprite(new Texture(buttonImage));
			ButtonTexture.Position = position;
			ButtonText.DisplayedString = "";
			centerButtonText();
		}

		public Button(string name, bool isActive, Vector2f position, Image buttonImage) : base(name, isActive) {
			ButtonTexture = new Sprite(new Texture(buttonImage));
			ButtonTexture.Position = position;
			ButtonText.DisplayedString = "";
			centerButtonText();
		}

		public Button(string name, Vector2f position, Image buttonImage, Text buttonText) : base(name) {
			ButtonTexture = new Sprite(new Texture(buttonImage));
			ButtonTexture.Position = position;
			ButtonText = buttonText;
			centerButtonText();
		}

		public Button(string name, bool isActive, Vector2f position, Image buttonImage, Text buttonText) : base(name, isActive) {
			ButtonTexture = new Sprite(new Texture(buttonImage));
			ButtonTexture.Position = position;
			ButtonText = buttonText;
			centerButtonText();
		}

		public Button(string name, Vector2f position, Image buttonImage, string buttonText, Font buttonTextFont, Color buttonTextColor, uint buttonTextCharacterSize = 12, Text.Styles buttonTextStyle = Text.Styles.Regular) : base(name) {
			ButtonTexture = new Sprite(new Texture(buttonImage));
			ButtonTexture.Position = position;
			ButtonText = new Text(buttonText, buttonTextFont, buttonTextCharacterSize);
			ButtonText.Color = buttonTextColor;
			ButtonText.Style = buttonTextStyle;
			centerButtonText();
		}

		public Button(string name, bool isActive, Vector2f position, Image buttonImage, string buttonText, Font buttonTextFont, Color buttonTextColor, uint buttonTextCharacterSize = 12, Text.Styles buttonTextStyle = Text.Styles.Regular) : base(name, isActive) {
			ButtonTexture = new Sprite(new Texture(buttonImage));
			ButtonTexture.Position = position;
			ButtonText = new Text(buttonText, buttonTextFont, buttonTextCharacterSize);
			ButtonText.Color = buttonTextColor;
			ButtonText.Font = buttonTextFont;
			centerButtonText();
		}

		private void centerButtonText() {
			FloatRect buttonTextRect = ButtonText.GetLocalBounds();
			ButtonText.Origin = new Vector2f(buttonTextRect.Width / 2, buttonTextRect.Height / 2);
			ButtonText.Position = new Vector2f(ButtonTexture.Position.X + ButtonTexture.Texture.Size.X / 2, ButtonTexture.Position.Y + ButtonTexture.Texture.Size.Y / 2);
		}

		override protected void OnDraw(RenderTarget target, RenderStates states) {
			target.Draw(ButtonTexture);
			target.Draw(ButtonText);
		}
		
		/// <summary>
		/// Obiekt slozacy do rysowania pola
		/// </summary>
		public Sprite ButtonTexture { get; internal set; }

		override public Vector2f Position {
			get {
				return ButtonTexture.Position;
			}
			set {
				ButtonTexture.Position = value;
				centerButtonText();
			}
		}

		override public Vector2u Size {
			get {
				return ButtonTexture.Texture.Size;
			}
		}

		public Text ButtonText { get; set; }
	}
}
