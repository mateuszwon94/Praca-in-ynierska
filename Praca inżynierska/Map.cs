using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using static PracaInzynierska.Textures.LoadedTextures;
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
			for ( int i = 0 ; i < Size ; ++i ) {
				Grid.Add(new List<MapField>(Size));
				for ( int j = 0 ; j < Size ; ++j )
					Grid[i].Add(new MapField(i, j));
			}

			Parallel.For(0, Size, i => {
				Parallel.For(0, Size, j => {
					for ( int k = -1 ; k <= 1 ; ++k ) {
						for ( int l = -1 ; l <= 1 ; ++l ) {
							if ( l == 0 && k == 0 )
								this[i, j].Neighbour[k, l] = null;
							else {
								try {
									this[i, j].Neighbour[k, l] = this[i + k, j + l];
								} catch ( ArgumentOutOfRangeException ) {
									this[i, j].Neighbour[k, l] = null;
								}
							}
						}
					}
				});
			});

			//List<MapField> fields = new List<MapField>();
			List<Thread> threads = new List<Thread>();
			// Inicjalizacja losowych miejsc na planszy jako ziaren do ulozenia terenu
			List<Vector2i> positions = new List<Vector2i>();
			for ( MapSeed.Value i = 0 ; i < (MapSeed.Value)MapSeed.MaxValue() ; ++i ) {
				int howMany = 0;
				int max = mapSeed[i];

				while ( howMany < max ) {
					int posX = r.Next(Size);
					int posY = r.Next(Size);
					int howManyAdditions = 0;
					if ( !this[posX, posY].IsFieldSeed ) {
						this[posX, posY].FieldSeed = i;
						positions.Add(new Vector2i(posX, posY));
						threads.Add(new Thread(() => { GenerateTerrain(this[posX, posY]); }));
						++howMany;
					} else
						continue;

					while ( howManyAdditions <= max / 2 ) {
						int posXAdd = r.Next(-Size / 10, Size / 10);
						int posYAdd = r.Next(-Size / 10, Size / 10);
						try {
							if ( !this[posX + posXAdd, posY + posYAdd].IsFieldSeed ) {
								this[posX + posXAdd, posY + posYAdd].FieldSeed = i;
								positions.Add(new Vector2i(posX + posXAdd, posY + posYAdd));
								threads.Add(new Thread(() => { GenerateTerrain(this[posX + posXAdd, posY + posYAdd]); }));
								++howManyAdditions;
							}
						} catch ( ArgumentOutOfRangeException ) { }
					}
				}
			}

			Parallel.For(0, Size, i => {
				Parallel.For(0, Size, j => {
					if ( !this[i, j].IsFieldSeed ) {
						// szukanie najblizszego ziarna
						Vector2i nearestSeed = new Vector2i(0, 0);
						double minDist = -1;
						foreach ( var seed in positions ) {
							double dist = Sqrt(Pow(i * i - seed.X * seed.X, 2) + Pow(j * j - seed.Y * seed.Y, 2));
							if ( minDist == -1 || dist < minDist ) {
								minDist = dist;
								nearestSeed = seed;
							}
						}

						//inicjalizacja odpowiedniego pola odpowiednia tekstura
						this[i, j].FieldSeed = this[nearestSeed.X, nearestSeed.Y].FieldSeed;
					}
				});
            });
		}

		public void Update(TimeSpan t) {
			OnRaiseUpdateEvent(new UpdateEventArgs(t));
		}
		
		
		protected virtual void OnRaiseUpdateEvent(UpdateEventArgs e) {
			EventHandler<UpdateEventArgs> handler = UpdateTime;
			
			if ( handler != null ) {
				handler(this, e);
			}

		}


		private void GenerateTerrain(MapField field) {
			lock ( mutex ) {
				if ((++count) % 100 == 0)
					Console.WriteLine(count);
			}

			//List<MapField> fields = new List<MapField>();
			List<Thread> threads = new List<Thread>();
			for (int k = -1 ; k <=1 ;++k ) {
				for ( int l = -1 ; l <= 1 ; ++l ) {
					if ( k != 0 && l != 0 ) {
						try {
							if ( field.Neighbour[k, l] != null && !field.Neighbour[k, l].IsFieldSeed ) {
								field.Neighbour[k, l].FieldSeed = field.FieldSeed;
								threads.Add(new	Thread(() => { GenerateTerrain(field.Neighbour[k, l]); }));
							}
						} catch ( ArgumentOutOfRangeException ) {
						} catch ( IndexOutOfRangeException ) { }

					}
				}
			}

			if ( threads.Count == 0 )
				return;

			threads.OrderBy(item => r.Next());

			foreach ( var thread in threads ) 
				thread.Start();

			foreach ( var thread in threads ) 
				thread.Join();
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
			for ( int i = 0 ; i < Size ; ++i ) {
				for ( int j = 0 ; j < Size ; ++j ) {
					if ( this[i, j].IsFieldSeed && this[i, j].Field.Position.X >= -this[i, j].Size &&
						this[i, j].Field.Position.X <= Program.window.Size.X &&
						this[i, j].Field.Position.Y >= -this[i, j].Size &&
						this[i, j].Field.Position.Y <= Program.window.Size.Y )
						target.Draw(this[i, j].Field);
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
			private set { Grid[i][j] = value; }
		}

		/// <summary>
		/// Poruszanie mapa
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void Map_MouseMoved(object sender, MouseMoveEventArgs e) {
			if ( prevMousePos == null ) {
				prevMousePos = new Vector2f(e.X, e.Y);
				return;
			}
			float dx = e.X - prevMousePos.Value.X;
			float dy = e.Y - prevMousePos.Value.Y;

			if ( Mouse.IsButtonPressed(Mouse.Button.Middle) ) {
				for ( int i = 0 ; i < Size ; ++i ) {
					for ( int j = 0 ; j < Size ; ++j ) {
						if ( this[i, j].IsFieldSeed ) {
							float x = (Grid[i][j].Field.Position.X + dx);
							float y = (Grid[i][j].Field.Position.Y + dy);
							Grid[i][j].Field.Position = new Vector2f(x, y);
						}
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

		/// <summary>
		/// Event odpowiadający za zachowanie wszystkiego.
		/// </summary>
		public event EventHandler<UpdateEventArgs> UpdateTime;

		private static Random r = new Random();
		private List<List<MapField>> Grid;
		private Vector2f? prevMousePos = null;


		private static int count = 0;
		private static Mutex mutex = new Mutex();
	}
}
