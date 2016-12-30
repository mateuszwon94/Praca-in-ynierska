using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.System;

namespace PracaInzynierska.Textures {

	/// <summary>
	/// Klasa odpowiadajaca za generowanie i przechowywanie tekstor zwiazanych z plansza gry.
	/// </summary>
	public static class MapTextures {

		#region GeneratingFunc

		/// <summary>
		/// Funkcja generujaca Wszystkie tekstury potrzebne w grze.
		/// </summary>
		/// <param name="screenSize">Rozmiar pola planszy dla jakiego maja byc wygenerowane tekstury</param>
		public static void GenerateAll(uint screenSize) {
			GrassTexture = GenerateMapTexture(screenSize, new Color(0, 200, 0));
			SandTexture = GenerateMapTexture(screenSize, new Color(200, 200, 0));
			RockTexture = GenerateMapTexture(screenSize, new Color(127, 127, 127));
			SelectedTexture = GenerateMapTexture(screenSize, new Color(255, 255, 255, 127));
			OpenListTexture = GenerateMapTexture(screenSize, new Color(0, 0, 255, 127));
			CloseListTexture = GenerateMapTexture(screenSize, new Color(255, 0, 0, 127));
			CurrentFieldTexture = GenerateMapTexture(screenSize, new Color(Color.Magenta.R, Color.Magenta.G, Color.Magenta.B, 127));

			MenTexture = GenerateBeeingTexture(screenSize, Color.Cyan);
			MenTextureSelected = GenerateBeeingTexture(screenSize, Color.Magenta);
			BesigerTexture = GenerateBeeingTexture(screenSize, new Color(255, 0, 0));
			AnimalTexture = GenerateBeeingTexture(screenSize, new Color(75, 75, 255), 6);
		}

		/// <summary>
		/// Funkcja generujaca pole planszy.
		/// </summary>
		/// <param name="screenSize">Rozmiar pola</param>
		/// <param name="color">Color pola</param>
		/// <returns>Wygenerowana tekstura</returns>
		public static Texture GenerateMapTexture(uint screenSize, Color color) {
			RenderTexture renderTexture = new RenderTexture(screenSize, screenSize);
			renderTexture.Clear(Color.Transparent);
			RectangleShape box = new RectangleShape(new Vector2f(screenSize-1, screenSize-1)) {
									 Position = new Vector2f(1, 1),
									 FillColor = color,
									 OutlineThickness = 1,
									 OutlineColor = Color.Black,
								 };
			renderTexture.Draw(box);
			return new Texture(renderTexture.Texture) {
					   Smooth = true,
				   };
		}

		/// <summary>
		/// Funkcja generujaca teksture istot zywych na planszy, jako figure foremna.
		/// </summary>
		/// <param name="screenSize">Rozmiar pola planzy</param>
		/// <param name="color">Kolor istoty</param>
		/// <param name="pointCount">Liczba bokow wygenerowanej figory foremnej (domyslnie - 30)</param>
		/// <returns>Wygenerowana tekstura</returns>
		public static Texture GenerateBeeingTexture(uint screenSize, Color color, uint pointCount = 30) {
			RenderTexture renderTexture = new RenderTexture(screenSize, screenSize);
			renderTexture.Clear(Color.Transparent);
			CircleShape circle = new CircleShape((screenSize / 2) -1, pointCount) {
									 FillColor = color,
									 Position = new Vector2f(1, 1),
									 OutlineThickness = 1,
									 OutlineColor = Color.Black,
								 };
			renderTexture.Draw(circle);
			return new Texture(renderTexture.Texture) {
					   Smooth = true
				   };
		}

		public static Texture GenerateConstructTexture(uint screenSizeX, uint screenSizeY, Color color) {
			if ( !textures_.Keys.Contains(color) ) {
				RenderTexture renderTexture = new RenderTexture(screenSizeX, screenSizeY);
				renderTexture.Clear(Color.Transparent);
				RectangleShape rect = new RectangleShape(new Vector2f(screenSizeX, screenSizeY)) {
																									 FillColor = color,
																									 Position = new Vector2f(1, 1),
																									 OutlineThickness = 1,
																									 OutlineColor = Color.Black,
																								 };
				renderTexture.Draw(rect);
				textures_[color] = new Texture(renderTexture.Texture) {
																		  Smooth = true
																	  };
			}
			return textures_[color];
		}

		private static Dictionary<Color,Texture> textures_ = new Dictionary<Color, Texture>();

		#endregion

		#region TextureProps

		/// <summary>
		/// Tekstura pla trawiastego.
		/// </summary>
		public static Texture GrassTexture { get; private set; }

		/// <summary>
		/// Tekstura pola piaszczystego.
		/// </summary>
		public static Texture SandTexture { get; private set; }

		/// <summary>
		/// Tekstura pola skalistego.
		/// </summary>
		public static Texture RockTexture { get; private set; }

		/// <summary>
		/// Tekstura czlowieka.
		/// </summary>
		public static Texture MenTexture { get; private set; }

		/// <summary>
		/// Tekstura czlowieka zaznaczonego.
		/// </summary>
		public static Texture MenTextureSelected { get; private set; }

		/// <summary>
		/// Tekstura zaznaczonego pola.
		/// </summary>
		public static Texture SelectedTexture { get; private set; }

		/// <summary>
		/// Tekstura zwierzecia.
		/// </summary>
		public static Texture AnimalTexture { get; private set; }

		public static Texture OpenListTexture { get; private set; }

		public static Texture CloseListTexture { get; private set; }

		public static Texture CurrentFieldTexture { get; private set; }

		public static Texture BesigerTexture { get; private set; }

		#endregion

	}

	/// <summary>
	/// Klasa odpowiadajaca za generowanie i przechowywanie typowych tekstur GUI.
	/// </summary>
	public static class GUITextures {

		#region GeneratingFunc

		/// <summary>
		/// Funkcja generująca tekstory do wszystkich elementow GUI.
		/// </summary>
		/// <param name="width">Standardowa dlugosc elementu</param>
		/// <param name="height">Standardowa wysokosc elementu</param>
		public static void GenerateAll(uint width, uint height) {
			NormalButtonTexture = ButtonTexture(width, height, new Color(0, 200, 200));
		}

		/// <summary>
		/// Funkcja generuje teksture przycisku.
		/// </summary>
		/// <param name="width">Dlugosc przycisku</param>
		/// <param name="height">Wysokosc przycisku</param>
		/// <param name="color">Color przycisku</param>
		/// <returns>Wygenerowana tekstura</returns>
		public static Texture ButtonTexture(uint width, uint height, Color color) {
			RenderTexture renderTexture = new RenderTexture(height, width);
			renderTexture.Clear(Color.Transparent);
			RectangleShape fill = new RectangleShape(new Vector2f(height-4, width-4)) {
									  FillColor = color,
									  Position = new Vector2f(2, 2),
									  OutlineThickness = 2,
									  OutlineColor = Color.Black,
								  };
			renderTexture.Draw(fill);

			return new Texture(renderTexture.Texture) {
				Smooth = true
			};
		}

		#endregion

		#region TextureProps

		/// <summary>
		/// Tekstura Zwyklego przycisku
		/// </summary>
		public static Texture NormalButtonTexture { get; private set; }

		#endregion

	}
}
