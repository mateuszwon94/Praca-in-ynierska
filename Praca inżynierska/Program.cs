using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PracaInzynierska.Beeing;
using PracaInzynierska.Map;
using PracaInzynierska.UserInterface;
using PracaInzynierska.UserInterface.Controls;
using SFML;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static System.Console;
using PracaInzynierska.Textures;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Utils.Algorithm;
using static PracaInzynierska.Beeing.Men;
using static PracaInzynierska.Textures.GUITextures;
using static PracaInzynierska.Textures.MapTextures;

namespace PracaInzynierska {
	public static class Program {
		public static void Main(string[] args) {

			//Inicjalizacja okna
			window = new RenderWindow(new VideoMode(1280, 720, 32), "Praca inzynierska",Styles.Close);
			window.Closed += (o, e) => window.Close();
			window.KeyPressed += (o, e) => { if ( e.Code == Keyboard.Key.Escape ) { window.Close(); } };
			origWindowSize = window.Size;

			DisplayTitle();

			WriteLine("Title displayed!");

			//inicjalizacja mapy
			WriteLine("Start map creating!");
			const int mapSize = 50;
			map = new Map.Map(mapSize, new MapSeed((int)(mapSize / 5.0), (int)(mapSize / 10.0), (int)(mapSize / 15.0)));

			window.MouseMoved += map.Map_MouseMoved;
			window.MouseWheelScrolled += map.Map_MouseWheelScrolled;
			WriteLine("Map created!");

			WriteLine("Start creating colonist!");
			List<Men> colonists = new List<Men>() {
									  new Men() {
										  MoveSpeed = 1,
										  Location = map[5, 5],
										  TextureSelected = new Sprite(MenTextureSelected),
										  TextureNotSelected = new Sprite(MenTexture),
										  IsSelected = false
									  }
								  };


			foreach ( Men colonist in colonists ) {
				map.UpdateTime += colonist.UpdateTime;
				window.KeyPressed += colonist.Window_KeyPressed;
				window.KeyReleased += colonist.Window_KeyReleased;
				window.MouseButtonPressed += colonist.Window_MouseButtonPressed;
				window.MouseButtonReleased += colonist.Window_MouseButtonReleased;
			}
			WriteLine("Colonist created!");

			WriteLine("Start creating GUI!");
			//Tworzenie GUI
			gui = new GUI() {
					  new Button {
						  Name = "First Button",
						  IsActive = true,
						  ButtonTexture = new Sprite(NormalButtonTexture),
						  ButtonText = new Text("Zamknij!", font) {
										   CharacterSize = 20,
										   Color = Color.Black
									   },
						  Position = new Vector2f(20, window.Size.Y - 60),
						  MouseButtonPressedHandler = (s, e) => { if (Mouse.IsButtonPressed(Mouse.Button.Left)) window.Close(); }
					  }
				  };

			window.KeyPressed += gui.Window_KeyPressed;
			window.KeyReleased += gui.Window_KeyReleased;
			window.MouseButtonPressed += gui.Window_MouseButtonPressed;
			window.MouseButtonReleased += gui.Window_MouseButtonReleased;
			window.MouseMoved += gui.Window_MouseMoved;
			window.MouseWheelScrolled += gui.Window_MouseWheelScrolled;
			WriteLine("GUI created!");

			WriteLine("Start creating path!");
			WriteLine("Searching for start and end point!");
			//tymczasowe sprawdzenie wyznaczanie sciezki
			IList<MapField> path;
		    MapField start = null,
		             stop  = null;

            //Wyszukanie pierwszego dostepnego pola
            foreach ( MapField field in map ) {
                if ( field.IsAvaliable ) {
                    start = field;
                    break;
                }
            }

            //wyszukanie ostatniego dostepnego pola
            foreach ( MapField field in map.Reverse() ) {
                if ( field.IsAvaliable ) {
                    stop = field;
                    break;
                }
			}
			WriteLine("Sstart and end point found!");

			try { //próba wyznaczenia sciezki miedzy wyznaczonymi polami
                path = PathFinding.AStar(start, stop, PathFinding.Metric.EuclideanDistance);
			} catch ( FieldNotAvaliableException ) {
				WriteLine($"Field [{start.MapPosition.X}, {start.MapPosition.Y}] is avaliable = {start.IsAvaliable}");
                WriteLine($"Field [{stop.MapPosition.X}, {stop.MapPosition.Y}] is avaliable = {stop.IsAvaliable}");
                WriteLine("But path between this field dose not exists!");
				path = null;
			}
			WriteLine("Path created!");

			WriteLine("Start creating herd!");
			MapField mapField;
			int center = map.Size / 2;
			do {
				int x = rand.Next(center - center / 2, center + center / 2);
				int y = rand.Next(center - center / 2, center + center / 2);
				mapField = map[x, y];
			} while ( !mapField.IsAvaliable );
			WriteLine($"Start from - {mapField}");

			/*MapField goToMapField;
			do {
				int x = rand.Next(map.Size / 2);
				int y = rand.Next(map.Size / 2);
				goToMapField = map[x, y];
			} while ( !goToMapField.IsAvaliable );
			WriteLine($"End on - {goToMapField}");

			Animal an = new Animal() {
				MoveSpeed = 1,
				Location = mapField,
				Texture = new Sprite(AnimalTexture),
				GoToField = goToMapField,
			};
			map.UpdateTime += an.UpdateTime;*/

			Herd herd = new Herd(mapField, 5);

			map.UpdateTime += herd.UpdateTime;
			foreach ( Animal animal in herd ) { map.UpdateTime += animal.UpdateTime; }
			WriteLine("Herd created!");


			time.Start();

            //Główna petla gry
            while ( window.IsOpen ) {
				window.DispatchEvents();
				window.Clear();

				time.Stop();

				map.Update(time.Elapsed);

				time.Restart();

				window.Draw(map);

				if ( path != null ) {
					foreach ( Sprite val in path.Select(field => new Sprite(SelectedTexture) {
																	 Position = field.ScreenPosition
																 }) ) {
						window.Draw(val);
					}
				}

				foreach ( var colonist in colonists ) {
					window.Draw(colonist);
				}
				window.Draw(herd);
	            //window.Draw(an);

				window.Draw(gui);

				window.Display();
			}
		}

		/// <summary>
		/// Funkcja wyświetla ekran tytułowy
		/// </summary>
		private static void DisplayTitle() {
			Text grad = new Text("Praca inżynierska", font, 60) {
							Color = Color.White,
							Style = Text.Styles.Bold
						};
			grad.Origin = new Vector2f(grad.GetGlobalBounds().Width / 2, grad.GetGlobalBounds().Height / 2);
			grad.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 335);

			Text author = new Text("Autor:", font, 20) {
							  Color = Color.White
						  };
			author.Origin = new Vector2f(author.GetGlobalBounds().Width / 2, author.GetGlobalBounds().Height / 2);
			author.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 275);

			Text who = new Text("Mateusz Winiarski", font, 20) {
						   Color = Color.White,
						   Style = Text.Styles.Italic | Text.Styles.Bold
					   };
			who.Origin = new Vector2f(who.GetGlobalBounds().Width / 2, who.GetGlobalBounds().Height / 2);
			who.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 250);

			Text AGH1 = new Text("Akademia Górniczo-Hutnicza", font, 25) {
							Color = Color.White
						};
			AGH1.Origin = new Vector2f(AGH1.GetGlobalBounds().Width / 2, AGH1.GetGlobalBounds().Height / 2);
			AGH1.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 200);

			Text AGH2 = new Text("im. Stanisława Staszica w Krakowie", font, 25) {
							Color = Color.White
						};
			AGH2.Origin = new Vector2f(AGH2.GetGlobalBounds().Width / 2, AGH2.GetGlobalBounds().Height / 2);
			AGH2.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 175);

			Text WFiIS = new Text("Wydział Fizyki i Informatyki Stosowanej", font, 25) {
							 Color = Color.White
						 };
			WFiIS.Origin = new Vector2f(WFiIS.GetGlobalBounds().Width / 2, WFiIS.GetGlobalBounds().Height / 2);
			WFiIS.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 125);

			Text IS = new Text("Informatyka Stosowana", font, 25) {
						  Color = Color.White
					  };
			IS.Origin = new Vector2f(IS.GetGlobalBounds().Width / 2, IS.GetGlobalBounds().Height / 2);
			IS.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 100);

			Text topic1 = new Text("Sztuczna inteligencja w grach komputerowych", font, 35) {
							  Color = Color.White
						  };
			topic1.Origin = new Vector2f(topic1.GetGlobalBounds().Width / 2, topic1.GetGlobalBounds().Height / 2);
			topic1.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 45);

			Text topic2 = new Text("na przykładzie logiki rozmytej, algorytmu stada", font, 35) {
							  Color = Color.White
						  };
			topic2.Origin = new Vector2f(topic2.GetGlobalBounds().Width / 2, topic2.GetGlobalBounds().Height / 2);
			topic2.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 - 10);

			Text topic3 = new Text("i problemu najkrótszej ścieżki w grze 2D.", font, 35) {
							  Color = Color.White
						  };
			topic3.Origin = new Vector2f(topic3.GetGlobalBounds().Width / 2, topic3.GetGlobalBounds().Height / 2);
			topic3.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 + 25);

			Text prom = new Text("Promotor:", font, 20) {
							Color = Color.White
						};
			prom.Origin = new Vector2f(prom.GetGlobalBounds().Width / 2, prom.GetGlobalBounds().Height / 2);
			prom.Position = new Vector2f(window.Size.X / 2 - 250, window.Size.Y / 2 + 120);

			Text promName = new Text("dr inż. Janusz Malinowski", font, 20) {
								Color = Color.White,
								Style = Text.Styles.Italic
							};
			promName.Origin = new Vector2f(promName.GetGlobalBounds().Width / 2, promName.GetGlobalBounds().Height / 2);
			promName.Position = new Vector2f(window.Size.X / 2 - 250, window.Size.Y / 2 + 145);

			Text rec = new Text("Recenzent:", font, 20) {
						   Color = Color.White
					   };
			rec.Origin = new Vector2f(rec.GetGlobalBounds().Width / 2, rec.GetGlobalBounds().Height / 2);
			rec.Position = new Vector2f(window.Size.X / 2 + 250, window.Size.Y / 2 + 120);

			Text recName = new Text("Unknown", font, 20) {
							   Color = Color.White,
							   Style = Text.Styles.Italic
						   };
			recName.Origin = new Vector2f(recName.GetGlobalBounds().Width / 2, recName.GetGlobalBounds().Height / 2);
			recName.Position = new Vector2f(window.Size.X / 2 + 250, window.Size.Y / 2 + 145);

			Text wait = new Text("Proszę czekać. Trwa generowanie mapy.", font, 30) {
							Color = Color.Red,
							Style = Text.Styles.Bold
						};
			wait.Origin = new Vector2f(wait.GetGlobalBounds().Width / 2, wait.GetGlobalBounds().Height / 2);
			wait.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2 + 220);

			Text time = new Text("Może to mi troszkę zająć. Cierpliwości :)", font, 20) {
							Color = Color.Red,
							Style = Text.Styles.Bold | Text.Styles.Italic
						};
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

		/// <summary>
		/// Podstawowa czcionka
		/// </summary>
		internal static Font font { get; } = new Font("times.ttf");

        internal static RenderWindow window;
		internal static Vector2u origWindowSize;
		internal static Map.Map map;
		internal static GUI gui;

		public static Random rand = new Random();

		//Zmienne do odmierzania czasu, ktory uplynal
		private static readonly Stopwatch time = new Stopwatch();
	}
}