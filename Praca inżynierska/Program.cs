using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static PracaInzynierska.LoadedTextures;

namespace PracaInzynierska {
	class Program {
		static void Main(string[] args) {
			//Initialising window
			window.Closed += (o, e) => window.Close();
			window.KeyPressed += Window_KeyPressed;
			origWindowSize = window.Size;

			font = new Font("times.ttf");
			Text grad = new Text("Praca inżynierska", font);
			Text who = new Text("Mateusz Winiarski", font);
			Text topic = new Text("Sztuczna inteligencja w grach komputerowych\nna przykladzie logiki rozmytej, algorytmu stada\ni problemu najkrótszej ścieżki w grze 2D.", font);
			
			window.Clear();
			// window.Draw(grad);
			window.Display();
			
			//inicjalizacja tekstur
			LoadTextures();

			//inicjalizacja mapy
			int mapSize = 100;
			map = new Map(mapSize, new MapSeed(mapSize / 5, mapSize / 10, mapSize / 15));

			window.MouseMoved += map.Map_MouseMoved;
			window.Resized += map.Map_Resized;
			window.MouseWheelScrolled += map.Map_MouseWheelScrolled;

			while (window.IsOpen) {
				window.DispatchEvents();
				window.Clear();
				window.Draw(map);
				window.Display();
			}
		}

		private static void Window_KeyPressed(object sender, KeyEventArgs e) {
			if (e.Code == Keyboard.Key.Escape)
				window.Close();
		}

		public static Font font { get; private set; }

		internal static RenderWindow window = new RenderWindow(new VideoMode(1280, 720), "Praca inzynierska");
		private static Map map;
		internal static Vector2u origWindowSize;
    }
}
