using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PracaInzynierska.Constructs;
using PracaInzynierska.Map;
using PracaInzynierska.UserInterface;
using PracaInzynierska.UserInterface.Controls;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static System.Console;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Utils.Algorithm;
using static PracaInzynierska.Textures.GUITextures;
using static PracaInzynierska.Textures.MapTextures;
using PracaInzynierska.Beeings;
using PracaInzynierska.Utils;
using static PracaInzynierska.Utils.Algorithm.PathFinding.Metric;
using PracaInzynierska.Utils.Diagnostic;
using static PracaInzynierska.Utils.Diagnostic.PathFinding;
using PracaInzynierska.Utils.FuzzyLogic.Variables;

namespace PracaInzynierska {

	public static class Program {
		public static void Main(string[] args) {

#if TEST
			PerformanceTests(25, 200, @"PerformanceTest.txt");
			QualityTests(25, 20, 10, @"QualityTest.txt");

			return;
#endif
			//Inicjalizacja okna
			window = new RenderWindow(new VideoMode(1280, 720, 32), "Praca inzynierska", Styles.Close);
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

			WriteLine("Start creating GUI!");
			//Tworzenie GUI
			gui = new GUI() { new Button {
											   Name = "Close Button",
											   IsActive = true,
											   ButtonTexture = new Sprite(NormalButtonTexture),
											   ButtonText = new Text("Zamknij!", font) {
																						   CharacterSize = 20,
																						   Color = Color.Black
																					   },
											   Position = new Vector2f(20, window.Size.Y - 60),
											   MouseButtonPressedHandler = (s, e) => {
																			   if ( Mouse.IsButtonPressed(Mouse.Button.Left) ) window.Close();
																		   }
										   },
								new BuildButton(font, window) {
													  Name = "Build Button",
													  IsActive = true,
													  ButtonTexture = new Sprite(NormalButtonTexture),
													  ButtonText = new Text("Buduj", font) {
																							   CharacterSize = 20,
																							   Color = Color.Black
																						   },
													  Position = new Vector2f(20, window.Size.Y - 120)
												  }
							};

			window.KeyPressed += gui.Window_KeyPressed;
			window.KeyReleased += gui.Window_KeyReleased;
			window.MouseButtonPressed += gui.Window_MouseButtonPressed;
			window.MouseButtonReleased += gui.Window_MouseButtonReleased;
			window.MouseMoved += gui.Window_MouseMoved;
			window.MouseWheelScrolled += gui.Window_MouseWheelScrolled;
			WriteLine("GUI created!");

			WriteLine("Start creating colony!");
			Colony colony = new Colony(map, window);
			map.UpdateTimeEvent += colony.UpdateTime;
			colony.AddColonist(new Men() {
											 Name = "Adam",
											 MoveSpeed = 5,
											 Location = map[10, 20],
											 TextureSelected = new Sprite(MenTextureSelected),
											 TextureNotSelected = new Sprite(MenTexture),
											 IsSelected = false,
											 HP = new FuzzyHP(50f, 50f),
											 Laziness = new FuzzyLaziness(2.5f),
											 Fatigue = new FuzzyFatigue(10f),
											 Strength = 5f,
											 Morale = new FuzzyMorale(5f),
											 Mining = 3f,
											 Constructing = 4f,
										 });
			colony.AddColonist(new Men() {
											 Name = "Adam",
											 MoveSpeed = 5,
											 Location = map[10, 23],
											 TextureSelected = new Sprite(MenTextureSelected),
											 TextureNotSelected = new Sprite(MenTexture),
											 IsSelected = false,
											 HP = new FuzzyHP(50f, 50f),
											 Laziness = new FuzzyLaziness(5f),
											 Fatigue = new FuzzyFatigue(8f),
											 Strength = 5f,
											 Morale = new FuzzyMorale(5f),
											 Mining = 10f,
											 Constructing = 4f,
										 });
			colony.AddColonist(new Men() {
											 Name = "Adam",
											 MoveSpeed = 5,
											 Location = map[13, 20],
											 TextureSelected = new Sprite(MenTextureSelected),
											 TextureNotSelected = new Sprite(MenTexture),
											 IsSelected = false,
											 HP = new FuzzyHP(50f, 50f),
											 Laziness = new FuzzyLaziness(7f),
											 Fatigue = new FuzzyFatigue(3f),
											 Strength = 7f,
											 Morale = new FuzzyMorale(5f),
											 Mining = 3f,
											 Constructing = 4f,
										 });
			/*colony.AddConstruct(new Construct(2, 3, map[3, 3], Color.Magenta) {
																				  MaxConstructPoints = 200
																			  });*/

			WriteLine("Colony created!");
			Besiegers besigers = new Besiegers(map, colony);
			map.UpdateTimeEvent += besigers.UpdateTime;
			WriteLine("Start creating besigers!");

			WriteLine("Besigers created!");

			/*WriteLine("Start creating herd!");
			MapField mapField;
			int center = map.Size / 2;
			do {
				int x = rand.Next(center - center / 2, center + center / 2);
				int y = rand.Next(center - center / 2, center + center / 2);
				mapField = map[x, y];
			} while ( !mapField.IsAvaliable );
			WriteLine($"Start from - {mapField}");

			Herd herd = new Herd(mapField, 5);

			map.UpdateTimeEvent += herd.UpdateTime;
			foreach ( Animal animal in herd ) { map.UpdateTimeEvent += animal.UpdateTime; }
			WriteLine("Herd created!");*/

			time.Start();

            //Główna petla gry
            while ( window.IsOpen && colony.Colonist.Count > 0) {
				window.DispatchEvents();
				window.Clear();

				time.Stop();

				map.Update(time.Elapsed);

				time.Restart();

				window.Draw(map);

	            //window.Draw(herd);

				window.Draw(besigers);

				window.Draw(colony);

				window.Draw(gui);

				window.Display();
			}

			WriteLine("PRZEGRAŁEŚ!");
			ReadKey();
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

		static void PerformanceTests(int mapSize, int iter, string fileName) {
			AStarDiagnostic diagnostic;
			using ( StreamWriter file = new StreamWriter(fileName) ) {
				for ( int i = 0 ; i < iter+1 ; i++ ) {
					if ( i == 105 ) continue;
					Write($"{i}\t");
					Map.Map map = new Map.Map(mapSize, new MapSeed((int)(mapSize / 2.5), (int)(mapSize / 5.0), (int)(mapSize / 7.5)), i);
					RenderTexture render = new RenderTexture((uint)(mapSize * MapField.ScreenSize),
															 (uint)(mapSize * MapField.ScreenSize), true) {
																											  Smooth = true,
																										  };
					render.Draw(map);
					render.Texture.CopyToImage().SaveToFile($@"map_{i}.png");

					MapField start = map.First(field => field.IsAvaliable);
					MapField stop = map.Reverse().First(field => field.IsAvaliable);

					Write("E\t");
					diagnostic = new AStarDiagnostic(new Stopwatch());
					diagnostic.Time.Start();
					try { AStar(start, stop, EuclideanDistance, ref diagnostic); } catch ( FieldNotAvaliableException ) { }
					diagnostic.Time.Stop();

					file.WriteLine($"{i}\tEucildesian\t{diagnostic.Time.Elapsed.TotalMilliseconds}\t{diagnostic.Iterations}\t{diagnostic.TotalCost}\t{diagnostic.TotalLength}");

					Write("M\t");
					diagnostic = new AStarDiagnostic(new Stopwatch());
					diagnostic.Time.Start();
					try { AStar(start, stop, ManhattanDistance, ref diagnostic); } catch ( FieldNotAvaliableException ) { }
					diagnostic.Time.Stop();

					file.WriteLine($"{i}\tManhattan\t{diagnostic.Time.Elapsed.TotalMilliseconds}\t{diagnostic.Iterations}\t{diagnostic.TotalCost}\t{diagnostic.TotalLength}");

					Write("N\t");
					diagnostic = new AStarDiagnostic(new Stopwatch());
					diagnostic.Time.Start();
					try { AStar(start, stop, NullDistance, ref diagnostic); } catch ( FieldNotAvaliableException ) { }
					diagnostic.Time.Stop();

					file.WriteLine($"{i}\tNull\t{diagnostic.Time.Elapsed.TotalMilliseconds}\t{diagnostic.Iterations}\t{diagnostic.TotalCost}\t{diagnostic.TotalLength}");
					WriteLine("END");
					file.Flush();

					GC.Collect();
				}
			}
		}

		static void QualityTests(int mapSize, int iter, int subIter, string fileName) {
			AStarDiagnostic diagnostic;
			using ( StreamWriter file = new StreamWriter(fileName) ) {
				for ( int i = 0 ; i < iter ; i++ ) {
					Map.Map map = new Map.Map(mapSize, new MapSeed((int)(mapSize / 2.5), (int)(mapSize / 5.0), (int)(mapSize / 7.5)), i);

					for ( int j = 0 ; j < subIter ; j++ ) {
						Write($"{i}\t{j}\t");
						MapField start = map[rand.Next(mapSize), rand.Next(mapSize)];
						MapField stop = map[rand.Next(mapSize), rand.Next(mapSize)];

						while ( !start.IsAvaliable ) { start = map[rand.Next(mapSize), rand.Next(mapSize)]; }
						while ( !stop.IsAvaliable || stop == start ) { stop = map[rand.Next(mapSize), rand.Next(mapSize)]; }

						Write("E\t");
						diagnostic = new AStarDiagnostic(new Stopwatch());
						diagnostic.Time.Start();
						try { AStar(start, stop, EuclideanDistance, ref diagnostic); } catch ( FieldNotAvaliableException ) { }
						diagnostic.Time.Stop();

						file.WriteLine($"{i}\tEucildesian\t{diagnostic.Time.Elapsed.TotalMilliseconds}\t{diagnostic.Iterations}\t{diagnostic.TotalCost}\t{diagnostic.TotalLength}");

						Write("M\t");
						diagnostic = new AStarDiagnostic(new Stopwatch());
						diagnostic.Time.Start();
						try { AStar(start, stop, ManhattanDistance, ref diagnostic); } catch ( FieldNotAvaliableException ) { }
						diagnostic.Time.Stop();

						file.WriteLine($"{i}\tManhattan\t{diagnostic.Time.Elapsed.TotalMilliseconds}\t{diagnostic.Iterations}\t{diagnostic.TotalCost}\t{diagnostic.TotalLength}");

						Write("N\t");
						diagnostic = new AStarDiagnostic(new Stopwatch());
						diagnostic.Time.Start();
						try { AStar(start, stop, NullDistance, ref diagnostic); } catch ( FieldNotAvaliableException ) { }
						diagnostic.Time.Stop();

						file.WriteLine($"{i}\tNull\t{diagnostic.Time.Elapsed.TotalMilliseconds}\t{diagnostic.Iterations}\t{diagnostic.TotalCost}\t{diagnostic.TotalLength}");
						WriteLine("END");

						file.Flush();
					}

					GC.Collect();
				}
			}
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