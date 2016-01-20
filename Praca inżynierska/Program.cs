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
using TGUI;
using static PracaInzynierska.LoadedTextures;

namespace PracaInzynierska {
	class Program {
		static void Main(string[] args) {
			//Initialising window
			window = new RenderWindow(new VideoMode(1280, 720), "Praca inzynierska");
			gui = new Gui(window);

            window.Closed += (o, e) => window.Close();
			window.KeyPressed += Window_KeyPressed;
			origWindowSize = window.Size;

			font = new SFML.Graphics.Font("times.ttf");
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
			map = new Map(mapSize, new MapSeed((int)(mapSize / 5.0), (int)(mapSize / 10.0), (int)(mapSize / 15.0)));

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

		public static SFML.Graphics.Font font { get; private set; }

		internal static RenderWindow window;
		internal static Gui gui;
		private static Map map;
		internal static Vector2u origWindowSize;
    }
}
