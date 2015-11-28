using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using static PracaInzynierska.LoadedTextures;

namespace PracaInzynierska {
	class Program {
		static void Main(string[] args) {
			init();
			/*
            Sprite sand = new Sprite(new Texture(SandTexture));
			sand.Scale = new Vector2f(0.2f, 0.2f);

			Sprite grass = new Sprite(new Texture(GrassTexture));
			grass.Scale = new Vector2f(0.2f, 0.2f);
			grass.Position = new Vector2f(0.2f * 512, 0.2f * 512);

			Sprite rock = new Sprite(new Texture(RockTexture), new IntRect(0, 0, 512, 512));
			rock.Scale = new Vector2f(0.2f, 0.2f);
			rock.Position = new Vector2f(0.2f * 512, 0f);
			*/
			while (window.IsOpen) {
				window.DispatchEvents();
				window.Clear();
				/*window.Draw(sand);
				window.Draw(grass);
				window.Draw(rock);*/
				window.Draw(map);
				window.Display();
			}
		}

		/// <summary>
		/// Funkcja inicjalizujaca podstawowe elementy
		/// </summary>
		static void init() {
			//Initialising window
			window = new RenderWindow(new VideoMode(800, 600), "Praca inzynierska");
			window.Closed += (o, e) => window.Close();
			window.KeyPressed += KeyPressed;
			
			//initialising textures
			LoadTextures();

			//inicjalizacja mapy
			map = new Map(30);
        }

		private static void KeyPressed(object sender, KeyEventArgs e) {
			if (e.Code == Keyboard.Key.Escape) 
				window.Close();
		}

		static RenderWindow window;
		static Map map;
    }
}
