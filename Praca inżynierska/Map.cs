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
using PracaInzynierska.Events;
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

			MapSeed.Value[,] mapSeedValue = new MapSeed.Value[size, size];

			Vector2i pos;
			List<Vector2i> posList = new List<Vector2i>();
			while ( mapSeed ) {
				do {
					pos = new Vector2i(r.Next(size), r.Next(size));
				} while ( mapSeedValue[pos.X, pos.Y] != MapSeed.Value.None );
				
				mapSeedValue[pos.X, pos.Y] = mapSeed.Next();
				posList.Add(pos);
			}

			for ( int i = 0 ; i < size ; i++ ) {
				for ( int j = 0 ; j < size ; j++ ) {

					double minDist = double.MaxValue;
					MapSeed.Value seedValue = MapSeed.Value.None;

					foreach ( var position in posList ) {
						double dist = Sqrt(Pow(position.X - i, 2) + Pow(position.Y - j, 2));
						if (dist < minDist) {
							minDist = dist;
							seedValue = mapSeedValue[position.X, position.Y];
						}
					}

					mapSeedValue[i, j] = seedValue;
				}
			}

			Grid = new List<List<MapField>>(Size);
			for ( int i = 0 ; i < Size ; ++i ) {
				Grid.Add(new List<MapField>(Size));
				for ( int j = 0 ; j < Size ; ++j ) {
					Grid[i].Add(new MapField(i, j, mapSeedValue[i, j]));
					MapMoved += Grid[i][j].MapMoved;
				}
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
		}

		private void MoveMap(MapMovedEventArgs e) {
			EventHandler<MapMovedEventArgs> handler = MapMoved;

			if ( handler != null && (e.dx != 0f || e.dy != 0f)) {
				handler(this, e);
			}
		}
		
		public void Update(TimeSpan t) {
			OnRaiseUpdateEvent(new UpdateEventArgs(t));
		}
				
		protected virtual void OnRaiseUpdateEvent(UpdateEventArgs e) {
			EventHandler<UpdateEventArgs> handler = UpdateTime;

			if ( handler != null ) {
				handler(this, e);
			}

			float dx = 0f;
			float dy = 0f;

			if ( Keyboard.IsKeyPressed(Keyboard.Key.Up) ) {
				dy += 50;
			}
			if ( Keyboard.IsKeyPressed(Keyboard.Key.Down) ) {
				dy -= 50;
			}

			if ( Keyboard.IsKeyPressed(Keyboard.Key.Left) ) {
				dx += 50;
			}
			if ( Keyboard.IsKeyPressed(Keyboard.Key.Right) ) {
				dx -= 50;
			}

			if (Keyboard.IsKeyPressed(Keyboard.Key.LShift) || Keyboard.IsKeyPressed(Keyboard.Key.RShift) ) {
				dx *= 10;
				dy *= 10;
			}

			dx *= e.UpdateTime;
			dy *= e.UpdateTime;
			MoveMap(new MapMovedEventArgs(dx, dy));
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

			if ( Mouse.IsButtonPressed(Mouse.Button.Middle) ) {
				Console.WriteLine("AAA");
				MoveMap(new MapMovedEventArgs(e.X - prevMousePos.Value.X, e.Y - prevMousePos.Value.Y));
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
		public event EventHandler<MapMovedEventArgs> MapMoved;

		private static Random r = new Random();
		private List<List<MapField>> Grid;
		private Vector2f? prevMousePos = null;


		private static int count = 0;
		private static Mutex mutex = new Mutex();
	}
}
