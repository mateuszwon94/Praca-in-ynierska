using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace PracaInzynierska.Textures {
	
	public static class MapTextures {
		public static void GenerateAll(uint size) {
			GrassTexture = GenerateMapTexture(size, new Color(0, 200, 0));
			SandTexture = GenerateMapTexture(size, new Color(200, 200, 0));
			RockTexture = GenerateMapTexture(size, new Color(127, 127, 127));
			SelectedTexture = GenerateMapTexture(size, new Color(255, 255, 255));

			DwarfTexture = GenerateDwarfTexture(size, Color.Cyan);
			DwarfTextureSelected = GenerateDwarfTexture(size, Color.Magenta);
		}

		public static Texture GenerateMapTexture(uint size, Color color) {
			RenderTexture renderTexture = new RenderTexture(size, size);
			renderTexture.Clear();
			RectangleShape box = new RectangleShape(new Vector2f(size - 2, size - 2)) {
									 Position = new Vector2f(1, 1),
									 FillColor = color
								 };
			renderTexture.Draw(box);
			return new Texture(renderTexture.Texture) {
				Smooth = true,
			};
		}

		public static Texture GenerateDwarfTexture(uint size, Color color) {
			RenderTexture renderTexture = new RenderTexture(size, size);
			renderTexture.Clear(Color.Transparent);
			CircleShape bigger = new CircleShape(size /2) {
									 FillColor = Color.Black
								 };
			CircleShape circle = new CircleShape((size / 2) - 2) {
									 FillColor = color,
									 Origin = new Vector2f(0, 0),
									 Position = new Vector2f(2, 2)
								 };
			renderTexture.Draw(bigger);
			renderTexture.Draw(circle);
			return new Texture(renderTexture.Texture) {
					   Smooth = true,
				   };
		}

		public static Texture GrassTexture;
		public static Texture SandTexture;
		public static Texture RockTexture;
		public static Texture DwarfTexture;
		public static Texture DwarfTextureSelected;
		public static Texture SelectedTexture;
	}

	public static class GUITextures {
		public static void GenerateAll(uint width, uint height) {
			NormalButtonTexture = ButtonTexture(width, height, new Color(0, 200, 200));
		}

		public static Texture ButtonTexture(uint width, uint height, Color color) {
			RenderTexture renderTexture = new RenderTexture(height, width);
			renderTexture.Clear();
			RectangleShape fill = new RectangleShape(new Vector2f(height - 6, width - 6)) {
									  FillColor = color,
									  Position = new Vector2f(3, 3)
								  };
			renderTexture.Draw(fill);

			return new Texture(renderTexture.Texture) {
					   Smooth = true
				   };
		}

		public static Texture NormalButtonTexture;
	}
}
