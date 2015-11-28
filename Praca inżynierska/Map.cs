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
		public Map(int size) {
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
				int howMany = 0;
				while (howMany > Size % 10) {
					int posX = r.Next(Size);
					int posY = r.Next(Size);
					if (Grid[posX][posY] == null) {
						Image tx = null;
						switch (i) {
							case 0:
								tx = RockTexture;
								break;
							case 1:
								tx = SandTexture;
								break;
							case 2:
								tx = GrassTexture;
								break;
						}
						Grid[posX][posY] = new MapField(tx);
						positions.Add(new Vector2i(posX, posY));
						Console.Write(posX);
						Console.Write(" - ");
						Console.WriteLine(posY);
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
						Grid[i][j] = new MapField(Grid[nearestSeed.X][nearestSeed.Y].FieldTexture);
                    }

					Grid[i][j].Field.Position = new Vector2f(i * Grid[i][j].Size, j * Grid[i][j].Size);
				}
			}
		}

		/// <summary>
		/// Funkcja rysujaca teksture
		/// </summary>
		/// <param name="target">Cel na ktorym jest rysowana</param>
		/// <param name="states">Stan</param>
		public void Draw(RenderTarget target, RenderStates states) {
			for (int i = 0; i < Size; ++i) {
				for (int j = 0; j < Size; ++j) {
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
		/// Rozmiar planszy
		/// </summary>
		public int Size { get; private set; }

		private static Random r = new Random();
		private List<List<MapField>> Grid;

	}
}
