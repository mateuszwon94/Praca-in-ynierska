using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace PracaInzynierska {
	static class LoadedTextures {
		/// <summary>
		/// Laduje wszystkie potrzebne tekstory do pamieci
		/// </summary>
		public static void LoadTextures() {
			GrassTexture = new Image(".\\Textures\\grass.jpg");
			SandTexture  = new Image(".\\Textures\\sand.jpg");
			RockTexture  = new Image(".\\Textures\\rock.jpg");
		}

		public static Image GrassTexture;
		public static Image SandTexture;
		public static Image RockTexture;
	}
}
