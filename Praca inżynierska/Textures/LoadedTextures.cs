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
	
	/// <summary>
	/// Klasa odpowiadajaca za generowanie i przechowywanie tekstor zwiazanych z plansza gry.
	/// </summary>
	public static class MapTextures {

		#region GeneratingFunc

		/// <summary>
		/// Funkcja generujaca Wszystkie tekstury potrzebne w grze.
		/// </summary>
		/// <param name="size">Rozmiar pola planszy dla jakiego maja byc wygenerowane tekstury</param>
		public static void GenerateAll(uint size) {
			GrassTexture = GenerateMapTexture(size, new Color(0, 200, 0));
			SandTexture = GenerateMapTexture(size, new Color(200, 200, 0));
			RockTexture = GenerateMapTexture(size, new Color(127, 127, 127));
			SelectedTexture = GenerateMapTexture(size, new Color(255, 255, 255));

			MenTexture = GenerateBeeingTexture(size, Color.Cyan);
			MenTextureSelected = GenerateBeeingTexture(size, Color.Magenta);
			AnimalTexture = GenerateBeeingTexture(size, new Color(75, 75, 255), 6);
		}

		/// <summary>
		/// Funkcja generujaca pole planszy.
		/// </summary>
		/// <param name="size">Rozmiar pola</param>
		/// <param name="color">Color pola</param>
		/// <returns>Wygenerowana tekstura</returns>
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

		/// <summary>
		/// Funkcja generujaca teksture istot zywych na planszy, jako figure foremna.
		/// </summary>
		/// <param name="size">Rozmiar pola planzy</param>
		/// <param name="color">Kolor istoty</param>
		/// <param name="pointCount">Liczba bokow wygenerowanej figory foremnej (domyslnie - 30)</param>
		/// <returns>Wygenerowana tekstura</returns>
		public static Texture GenerateBeeingTexture(uint size, Color color, uint pointCount = 30) {
			RenderTexture renderTexture = new RenderTexture(size, size);
			renderTexture.Clear(Color.Transparent);
			CircleShape bigger = new CircleShape(size / 2, pointCount) { FillColor = Color.Black };
			CircleShape circle = new CircleShape((size / 2) - 2, pointCount) {
									 FillColor = color,
									 Origin = new Vector2f(0, 0),
									 Position = new Vector2f(2, 2)
								 };
			renderTexture.Draw(bigger);
			renderTexture.Draw(circle);
			return new Texture(renderTexture.Texture) { Smooth = true };
		}

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

		#endregion

		#region TextureProps

		/// <summary>
		/// Tekstura Zwyklego przycisku
		/// </summary>
		public static Texture NormalButtonTexture { get; private set; } 

		#endregion

	}
}
