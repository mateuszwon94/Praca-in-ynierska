using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace PracaInzynierska.Textures {
	static class LoadedTextures {
		/// <summary>
		/// Laduje wszystkie potrzebne tekstory do pamieci
		/// </summary>
		public static void LoadTextures() {
			MapTextures.GrassTexture = new Image(".\\Textures\\grass.jpg");
			MapTextures.SandTexture = new Image(".\\Textures\\sand.jpg");
			MapTextures.RockTexture = new Image(".\\Textures\\rock.jpg");
			MapTextures.DwarfTexture = new Image(".\\Textures\\K.png");

			GUITextures.ButtonTexture = new Image(".\\Textures\\button.png");
		}

		public static class MapTextures {
			public static Image GrassTexture;
			public static Image SandTexture;
			public static Image RockTexture;
			public static Image DwarfTexture;
		}

		public static class GUITextures {
			public static Image ButtonTexture;
		}
	}
}
