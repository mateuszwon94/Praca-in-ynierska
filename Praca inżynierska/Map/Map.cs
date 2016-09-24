using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PracaInzynierska.Events;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Textures;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace PracaInzynierska.Map {
	public sealed class Map : Drawable, IEnumerable<MapField> {

		#region Constructors

		/// <summary>
		/// Konstruktor tworzacy plansze o zadanym rozmiarze
		/// </summary>
		/// <param name="size">Rozmiar planszy</param>
		public Map(int size, MapSeed mapSeed) {
			//Wygenerowanie tekstur pol planszy
			MapTextures.GenerateAll((uint)MapField.Size);

			// Inicjalizacja rozmiaru i pol
			Size = size;

			MapSeed.Value[,] mapSeedValue = new MapSeed.Value[size, size];
			Vector2i[] posList = new Vector2i[mapSeed.Count];
			Random rand = new Random();

			//Inicjalizacja startowyxh ziaren na mapie
			Console.WriteLine("Initalizing seeds.");
			for ( int i = 0 ; i < mapSeed.Count ; ++i ) {
				try {
					Vector2i pos;
					do {
						pos.X = rand.Next(size);
						pos.Y = rand.Next(size);
					} while ( mapSeedValue[pos.X, pos.Y] != MapSeed.Value.None );
					posList[i] = pos;
					mapSeedValue[pos.X, pos.Y] = mapSeed.SeedFromIdx(i);
				} catch { Console.WriteLine("Error!"); }
			}
			Console.WriteLine("Start seeds initialized.");

			//Wygenerowanie tablicy ziaren na podstawie startowych ziaren
			Parallel.For(0, Size, i => {
				Parallel.For(0, Size, j => {
					try {
						double minDist = double.MaxValue;
						MapSeed.Value seedValue = MapSeed.Value.None;

						foreach ( var pos in posList ) {
							double dist = Math.Pow(pos.X - i, 2) + Math.Pow(pos.Y - j, 2);
							if ( dist < minDist ) {
								minDist = dist;
								seedValue = mapSeedValue[pos.X, pos.Y];
							}
						}

						mapSeedValue[i, j] = seedValue;
					} catch { Console.WriteLine("Error!"); }
				});
			});
			Console.WriteLine("Seeds initialized.");

			//Stworzenie pustego kontenera pol mapy
			Console.WriteLine("Initalizing map fields.");
			Grid = new List<List<MapField>>(Size);

			for ( int i = 0 ; i < size ; ++i ) {
				Grid.Add(new List<MapField>(Size));
				for ( int j = 0 ; j < size ; ++j ) {
					Grid[i].Add(null);
				}
			}

			//Inicjalizacja mapy na podsatawie wygenerowanej tablicy ziaren
			Parallel.For(0, Size, i => {
				Parallel.For(0, Size, j => {
					try {
						Grid[i][j] = new MapField(i, j, this, mapSeedValue[i, j]);
						MapMoved += Grid[i][j].MapMoved;
						MapResized += Grid[i][j].MapResized;
					} catch ( Exception ex ) { Console.WriteLine(ex.StackTrace); }
				});
			});
			Console.WriteLine("Map fields initialized.");
		}

		#endregion Constructors

		public void Update(TimeSpan t) {
			OnRaiseUpdateEvent(new UpdateEventArgs(t));
		}

		/// <summary>
		/// Funkcja rysujaca teksture
		/// </summary>
		/// <param name="target">Cel na ktorym jest rysowana</param>
		/// <param name="states">Stan</param>
		public void Draw(RenderTarget target, RenderStates states) {
			for ( int i = 0 ; i < Size ; ++i ) {
				for ( int j = 0 ; j < Size ; ++j ) {
					if ( this[i, j].IsFieldSeed && (this[i, j].Field.Position.X >= -MapField.Size) &&
						(this[i, j].Field.Position.X <= Program.window.Size.X) &&
						(this[i, j].Field.Position.Y >= -MapField.Size) &&
						(this[i, j].Field.Position.Y <= Program.window.Size.Y) ) {
						target.Draw(this[i, j].Field, states);
					}
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

		#region MapMoveing

		/// <summary>
		/// Event uruchamiany przy poruszaniu mapa
		/// </summary>
		public event EventHandler<MapMovedEventArgs> MapMoved;

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
				MoveMap(new MapMovedEventArgs(e.X - prevMousePos.Value.X, e.Y - prevMousePos.Value.Y));
			}

			prevMousePos = new Vector2f(e.X, e.Y);
		}

		private void MoveMap(MapMovedEventArgs e) {
			EventHandler<MapMovedEventArgs> handler = MapMoved;

			if ( (handler != null) && ((Math.Abs(e.dx) > 1E-10f) || (Math.Abs(e.dy) > 1E-10f)) ) {
				handler(this, e);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		private void OnRaiseUpdateEvent(UpdateEventArgs e) {
			UpdateTime?.Invoke(this, e);

			double dx = 0;
			double dy = 0;

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

			if ( Keyboard.IsKeyPressed(Keyboard.Key.LShift) || Keyboard.IsKeyPressed(Keyboard.Key.RShift) ) {
				dx *= 10;
				dy *= 10;
			}

			dx *= e.UpdateTime;
			dy *= e.UpdateTime;
			MoveMap(new MapMovedEventArgs(dx, dy));
		}

		#endregion MapMoveing

		#region MapResizing

		/// <summary>
		/// Event uruchamiany podczas skalowania mapy
		/// </summary>
		public event EventHandler<MapResizedEventArgs> MapResized;

		/// <summary>
		/// Funkcja wywoływana przy poruszaniu scrollem
		/// </summary>
		/// <param name="sender">Obiekt wysyłający zdarzenie</param>
		/// <param name="e">Parametry zdarzenia</param>
		internal void Map_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e) {
			int delta = (int)e.Delta;

			//Czy mapa nie jest za duża lub za mala żeby ją skalowac bardziej
			if ( ((MapField.Size == 20) && (delta < 0)) || ((MapField.Size == 30) && (delta > 0)) ) {
				return;
			}

			//Nowa delta, gdy stara przekroczylaby dopuszczalny wielkosc pol
			else if ( MapField.Size + delta < 20 ) {
				delta = MapField.Size - 20;
			} else if ( MapField.Size + delta > 30 ) {
				delta = 30 - MapField.Size;
			}

			if ( delta != 0 ) {
				MapField.Size += delta;

				MapTextures.GenerateAll((uint)MapField.Size);

				MapResized?.Invoke(this, new MapResizedEventArgs(delta));
			}
		}

		#endregion MapResizing

		internal void Map_Resized(object sender, SizeEventArgs e) {
			/*for (int i = 0; i < Size; ++i) {
				for (int j = 0; j < Size; ++j) {
					Grid[i][j].Field.Scale = new Vector2f((float)e.Height / Program.origWindowSize.X, (float)e.Width / Program.origWindowSize.Y);
				}
			}*/
		}

		public IEnumerator<MapField> GetEnumerator() {
			return new MapIterator(Grid);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		/// <summary>
		/// Rozmiar planszy
		/// </summary>
		public int Size { get; private set; }

		public event EventHandler<UpdateEventArgs> UpdateTime;


		private static Random r = new Random();
		private readonly List<List<MapField>> Grid;
		private Vector2f? prevMousePos;


		private static int count = 0;
		private static Mutex mutex = new Mutex();

		private class MapIterator : IEnumerator<MapField> {
			public MapIterator(List<List<MapField>> parent) {
				parent_ = parent;
			}

			public MapField Current => parent_[x][y];

			object IEnumerator.Current => Current;

			public void Dispose() { }

			public bool MoveNext() {
				if ( (x >= parent_.Count - 1) && (y >= parent_[x].Count - 1) ) return false;
				++y;
				if ( y == parent_[x].Count ) {
					y = 0;
					++x;
				}
				return true;
			}
		
			public void Reset() {
				x = 0;
				y = -1;
			}

			private readonly List<List<MapField>> parent_;
			private int x, y = -1;
		}
	}
}
