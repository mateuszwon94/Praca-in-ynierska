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
using static System.Math;

namespace PracaInzynierska {
	class Map : Drawable {
		/// <summary>
		/// Konstruktor tworzacy plansze o zadanym rozmiarze
		/// </summary>
		/// <param name="size">Rozmiar planszy</param>
		public Map(int size, MapSeed mapSeed) {
			// Inicjalizacja rozmiaru i pol
			Size = size;
			Grid = new List<List<MapField>>(Size);
			for (int i = 0; i < Size; ++i) {
				Grid.Add(new List<MapField>(Size));
				for (int j = 0; j < Size; ++j)
					Grid[i].Add(null);
			}

			// Inicjalizacja losowych miejsc na planszy jako ziaren do ulozenia terenu
			List<Vector2i> positions = new List<Vector2i>();
			for (int i = 0; i < 3; ++i) {
				Image tx;
                int howMany = 0;
				int max;
				switch (i) {
					case 0:
						tx = SandTexture;
						max = mapSeed[MapSeedValues.Sand];
						break;
					case 1:
						tx = GrassTexture;
						max = mapSeed[MapSeedValues.Grass];
						break;
					case 2:
						tx = RockTexture;
						max = mapSeed[MapSeedValues.Rock];
						break;
					default:
						tx = null;
						max = 0;
						break;
				}
				
				while (howMany < max) {
					int posX = r.Next(Size);
					int posY = r.Next(Size);
					if (Grid[posX][posY] == null) {
						Grid[posX][posY] = new MapField(tx);
						positions.Add(new Vector2i(posX, posY));
						++howMany;
                    }
				}
			}

			// inicjalizacja reszty terenu
			for (int i = 0; i < Size; ++i) {
				for (int j = 0; j < Size; ++j) {
					if (Grid[i][j] == null) {
						// szukanie najblizszego ziarna
						Vector2i nearestSeed = new Vector2i(0, 0);
						double minDist = -1;
						foreach (var seed in positions) {
							double dist = Sqrt(Pow(i * i - seed.X * seed.X, 2) + Pow(j * j - seed.Y * seed.Y, 2));
							if (minDist == -1 || dist < minDist) {
								minDist = dist;
								nearestSeed = seed;
                            }
						}

						//inicjalizacja odpowiedniego pola odpowiednia tekstura
						Grid[i][j] = new MapField(Grid[nearestSeed.X][nearestSeed.Y].FieldImage);
                    }

					Grid[i][j].Field.Position = new Vector2f(i * Grid[i][j].Size, j * Grid[i][j].Size);
				}
			}
		}

		internal void Map_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e) {
			/*
			for (int i = 0; i < Size; ++i) {
				for (int j = 0; j < Size; ++j) {
					Grid[i][j].Field.Scale = new Vector2f(Grid[i][j].Field.Scale.X + e.Delta, 
														  Grid[i][j].Field.Scale.Y + e.Delta);

				}
			}*/
        }

		

		/// <summary>
		/// Funkcja rysujaca teksture
		/// </summary>
		/// <param name="target">Cel na ktorym jest rysowana</param>
		/// <param name="states">Stan</param>
		public void Draw(RenderTarget target, RenderStates states) {
			for (int i = 0; i < Size; ++i) {
				for (int j = 0; j < Size; ++j) {
					if (Grid[i][j].Field.Position.X >= -Grid[i][j].Size && 
						Grid[i][j].Field.Position.X <= Program.window.Size.X && 
						Grid[i][j].Field.Position.Y >= -Grid[i][j].Size && 
						Grid[i][j].Field.Position.Y <= Program.window.Size.Y)
						target.Draw(Grid[i][j].Field);
				}
			}
		}

		/// <summary>
		/// Mozliwosc dostania sie do odpowiedniego pola na planszy
		/// </summary>
		/// <param name="i">Pierwsza wspolrzedna</param>
		/// <param name="j">Druga wspolrzedna</param>
		/// <returns></returns>
		public MapField this[int i, int j] {
					get { return Grid[i][j]; }
			private	set { Grid[i][j] = value; }
		}
		
		/// <summary>
		/// Poruszanie mapa
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void Map_MouseMoved(object sender, MouseMoveEventArgs e) {
			if (prevMousePos == null) {
				prevMousePos = new Vector2f(e.X, e.Y);
				return;
			}
			float dx = e.X - prevMousePos.Value.X;
			float dy = e.Y - prevMousePos.Value.Y;

			if (Mouse.IsButtonPressed(Mouse.Button.Middle)) {
				for (int i = 0; i < Size; ++i) {
					for (int j = 0; j < Size; ++j) {
						float x = (Grid[i][j].Field.Position.X + dx);
						float y = (Grid[i][j].Field.Position.Y + dy);
						Grid[i][j].Field.Position = new Vector2f(x, y);
					}
				}
			}

			prevMousePos = new Vector2f(e.X, e.Y);
		}

		internal void Map_Resized(object sender, SizeEventArgs e) {
			/*for (int i = 0; i < Size; ++i) {
				for (int j = 0; j < Size; ++j) {
					Grid[i][j].Field.Scale = new Vector2f((float)e.Height / Program.origWindowSize.X, (float)e.Width / Program.origWindowSize.Y);
				}
			}*/
		}

		/// <summary>
		/// Rozmiar planszy
		/// </summary>
		public int Size { get; private set; }

		private static Random r = new Random();
		private List<List<MapField>> Grid;
		private Vector2f? prevMousePos = null;
	}
}
