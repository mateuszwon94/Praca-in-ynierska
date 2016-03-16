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
using System.Diagnostics;
using PracaInzynierska.Beeing;
using PracaInzynierska.GUI;
using PracaInzynierska.GUI.Controls;
using static PracaInzynierska.Textures.LoadedTextures;
using static PracaInzynierska.Textures.LoadedTextures.MapTextures;
using static PracaInzynierska.Textures.LoadedTextures.GUITextures;

namespace PracaInzynierska {
	class Program {
		static void Main(string[] args) {
			//Initialising window
			window = new RenderWindow(new VideoMode(1280, 720), "Praca inzynierska");

			window.Closed += (o, e) => window.Close();
			window.KeyPressed += Window_KeyPressed;
			origWindowSize = window.Size;

			font = new Font("times.ttf");
			DisplayTitle();

			//inicjalizacja tekstur
			LoadTextures();

			//inicjalizacja mapy
			int mapSize = 100;
			map = new Map(mapSize, new MapSeed((int)(mapSize / 5.0), (int)(mapSize / 10.0), (int)(mapSize / 15.0)));

			List<Men> units = new List<Men>();
			units.Add(new Men(1, map[0, 0], DwarfTexture));

			window.MouseMoved += map.Map_MouseMoved;
			window.Resized += map.Map_Resized;
			window.MouseWheelScrolled += map.Map_MouseWheelScrolled;

			gui = new GUIBase(window);
			gui.Add(new Button("First Button", true, new Vector2f(20, 20), ButtonTexture, new Text("Pierwszy!", font, 20)));
			gui[0].MouseButtonPressed += Program_MouseButtonPressed;
			((Button)gui[0]).ButtonText.Color = Color.Black;

			startTime = DateTime.Now;
			while (window.IsOpen) {
				window.DispatchEvents();
				window.Clear();

				stopTime = DateTime.Now;

				elapsed = stopTime - startTime;
				// elapsed2 = stopTime - startTime;
				map.Update(elapsed);

				startTime = DateTime.Now;
				
				window.Draw(map);
				foreach ( var unit in units	) {
					window.Draw(unit);
				}
				window.Draw(gui);

				window.Display();
			}
		}
		
		private static void Program_MouseButtonPressed(object sender, MouseButtonEventArgs e) {
			Console.WriteLine("Click!");
			window.Close();
		}

		private static void Window_KeyPressed(object sender, KeyEventArgs e) {
			if (e.Code == Keyboard.Key.Escape)
				window.Close();
		}

		private static void DisplayTitle() {
			Text grad = new Text("Praca inżynierska", font, 60);
			grad.Color = Color.White;
			grad.Style = Text.Styles.Bold;
			grad.Origin = new Vector2f(grad.GetGlobalBounds().Width / 2, grad.GetGlobalBounds().Height / 2);
			grad.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 335);

			Text author = new Text("Autor:", font, 20);
			author.Color = Color.White;
			author.Origin = new Vector2f(author.GetGlobalBounds().Width / 2, author.GetGlobalBounds().Height / 2);
			author.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 275);

			Text who = new Text("Mateusz Winiarski", font, 20);
			who.Color = Color.White;
			who.Style = Text.Styles.Italic | Text.Styles.Bold;
			who.Origin = new Vector2f(who.GetGlobalBounds().Width / 2, who.GetGlobalBounds().Height / 2);
			who.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 250);

			Text AGH1 = new Text("Akademia Górniczo-Hutnicza", font, 25);
			AGH1.Color = Color.White;
			AGH1.Origin = new Vector2f(AGH1.GetGlobalBounds().Width / 2, AGH1.GetGlobalBounds().Height / 2);
			AGH1.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 200);

			Text AGH2 = new Text("im. Stanisława Staszica w Krakowie", font, 25);
			AGH2.Color = Color.White;
			AGH2.Origin = new Vector2f(AGH2.GetGlobalBounds().Width / 2, AGH2.GetGlobalBounds().Height / 2);
			AGH2.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 175);

			Text WFiIS = new Text("Wydział Fizyki i Informatyki Stosowanej", font, 25);
			WFiIS.Color = Color.White;
			WFiIS.Origin = new Vector2f(WFiIS.GetGlobalBounds().Width / 2, WFiIS.GetGlobalBounds().Height / 2);
			WFiIS.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 125);

			Text IS = new Text("Informatyka Stosowana", font, 25);
			IS.Color = Color.White;
			IS.Origin = new Vector2f(IS.GetGlobalBounds().Width / 2, IS.GetGlobalBounds().Height / 2);
			IS.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 100);

			Text topic1 = new Text("Sztuczna inteligencja w grach komputerowych", font, 35);
			Text topic2 = new Text("na przykładzie logiki rozmytej, algorytmu stada", font, 35);
			Text topic3 = new Text("i problemu najkrótszej ścieżki w grze 2D.", font, 35);
			topic1.Color = Color.White;
			topic2.Color = Color.White;
			topic3.Color = Color.White;
			topic1.Origin = new Vector2f(topic1.GetGlobalBounds().Width / 2, topic1.GetGlobalBounds().Height / 2);
			topic2.Origin = new Vector2f(topic2.GetGlobalBounds().Width / 2, topic2.GetGlobalBounds().Height / 2);
			topic3.Origin = new Vector2f(topic3.GetGlobalBounds().Width / 2, topic3.GetGlobalBounds().Height / 2);
			topic1.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 45);
			topic2.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 10);
			topic3.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 + 25);

			Text prom = new Text("Promotor:", font, 20);
			prom.Color = Color.White;
			prom.Origin = new Vector2f(prom.GetGlobalBounds().Width / 2, prom.GetGlobalBounds().Height / 2);
			prom.Position = new Vector2f(window.Size.X / 2 - 250, window.Size.Y / 2 + 120);

			Text promName = new Text("dr inż. Janusz Malinowski", font, 20);
			promName.Color = Color.White;
			promName.Style = Text.Styles.Italic;
			promName.Origin = new Vector2f(promName.GetGlobalBounds().Width / 2, promName.GetGlobalBounds().Height / 2);
			promName.Position = new Vector2f(window.Size.X / 2 - 250, window.Size.Y / 2 + 145);

			Text rec = new Text("Recenzent:", font, 20);
			rec.Color = Color.White;
			rec.Origin = new Vector2f(rec.GetGlobalBounds().Width / 2, rec.GetGlobalBounds().Height / 2);
			rec.Position = new Vector2f(window.Size.X / 2 + 250, window.Size.Y / 2 + 120);

			Text recName = new Text("Unknown", font, 20);
			recName.Color = Color.White;
			recName.Style = Text.Styles.Italic;
			recName.Origin = new Vector2f(recName.GetGlobalBounds().Width / 2, recName.GetGlobalBounds().Height / 2);
			recName.Position = new Vector2f(window.Size.X / 2 + 250, window.Size.Y / 2 + 145);

			Text wait = new Text("Proszę czekać. Trwa generowanie mapy.", font, 30);
			wait.Color = Color.Red;
			wait.Style = Text.Styles.Bold;
			wait.Origin = new Vector2f(wait.GetGlobalBounds().Width / 2, wait.GetGlobalBounds().Height / 2);
			wait.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 + 220);

			Text time = new Text("Może to mi troszkę zająć. Cierpliwości :)", font, 20);
			time.Color = Color.Red;
			time.Style = Text.Styles.Bold | Text.Styles.Italic;
			time.Origin = new Vector2f(time.GetGlobalBounds().Width / 2, time.GetGlobalBounds().Height / 2);
			time.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 + 255);

			window.Clear();
			window.Draw(grad);
			window.Draw(author);
			window.Draw(who);
			window.Draw(AGH1);
			window.Draw(AGH2);
			window.Draw(WFiIS);
			window.Draw(IS);
			window.Draw(topic1);
			window.Draw(topic2);
			window.Draw(topic3);
			window.Draw(prom);
			window.Draw(promName);
			window.Draw(rec);
			window.Draw(recName);
			window.Draw(wait);
			window.Draw(time);
			window.Display();
		}

		public static SFML.Graphics.Font font { get; private set; }

		internal static RenderWindow window;
		internal static Map map;
		internal static Vector2u origWindowSize;
		internal static GUIBase gui;

		private static DateTime startTime;
		private static DateTime stopTime;
		private static TimeSpan elapsed;
	}
}
