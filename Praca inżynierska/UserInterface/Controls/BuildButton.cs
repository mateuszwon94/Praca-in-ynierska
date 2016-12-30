using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static PracaInzynierska.Textures.GUITextures;

namespace PracaInzynierska.UserInterface.Controls {
	class BuildButton : Button {

		public BuildButton(Font font, RenderWindow window) : base() {
			MouseButtonPressed += BuildButton_MouseButtonPressed;
			new List<Button>() {
								   new Button() {
													Name = "Dig Button",
													IsActive = true,
													ButtonTexture = new Sprite(NormalButtonTexture),
													ButtonText = new Text("Buduj", font) {
																							 CharacterSize = 20,
																							 Color = Color.Black
																						 },
													Position = new Vector2f(20, window.Size.Y - 120)
												}
							   };
		}

		private void BuildButton_MouseButtonPressed(object sender, MouseButtonEventArgs e) {
			if ( IsOn ) {

			}
		}

		public bool IsOn { get; private set; }

		private List<Button> subButtons_;
	}
}
