using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PracaInzynierska.Events;
using PracaInzynierska.Textures;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace PracaInzynierska.Map {

    /// <summary>
    /// Clasa reprezentujaca mape
    /// </summary>
	public sealed class Map : Drawable, IEnumerable<MapField> {
		#region Constructors

	    /// <summary>
	    /// Konstruktor tworzacy plansze o zadanym rozmiarze
	    /// </summary>
	    /// <param name="size">Rozmiar planszy</param>
	    /// <param name="mapSeed">Okresla proporcje w występowaniu terenu</param>
	    public Map(int size, MapSeed mapSeed) {
			//Wygenerowanie tekstur pol planszy
			MapTextures.GenerateAll((uint)MapField.ScreenSize);

			// Inicjalizacja rozmiaru i pol
			Size = size;

			MapSeed.Value[,] mapSeedValue = new MapSeed.Value[size, size];
			Vector2i[] posList = new Vector2i[mapSeed.Count];

			//Inicjalizacja startowyxh ziaren na mapie
			Console.WriteLine("Initalizing seeds.");
			for ( int i = 0 ; i < mapSeed.Count ; ++i ) {
				try {
                    ref Vector2i pos = ref posList[i];
					do {
						pos.X = rand.Next(size);
						pos.Y = rand.Next(size);
					} while ( mapSeedValue[pos.X, pos.Y] != MapSeed.Value.None );
					mapSeedValue[pos.X, pos.Y] = mapSeed[i];
				} catch { Console.WriteLine("Error!"); }
			}
			Console.WriteLine("Start seeds initialized.");

			//Wygenerowanie tablicy ziaren na podstawie startowych ziaren
			Parallel.For(0, Size, i => {
				Parallel.For(0, Size, j => {
					try {
						double minDist = double.MaxValue;
						MapSeed.Value seedValue = MapSeed.Value.None;

						foreach ( Vector2i pos in posList ) {
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

        #region Events

        /// <summary>
        /// Event uruchamiany przy odswierzaniu mapy
        /// </summary>
        public event EventHandler<UpdateEventArgs> UpdateTime;

        /// <summary>
        /// Funkcja wywołująca event odswiezania mapy
        /// </summary>
        /// <param name="t">Czas jaki uplynal od ostatniego odswiezenia</param>
		public void Update(TimeSpan t) {
            OnRaiseUpdateEvent(new UpdateEventArgs(t));
        }

        #endregion Events

        #region Drawable

        /// <summary>
        /// Funkcja rysujaca mape
        /// </summary>
        /// <param name="target">Cel, na ktorym jest rysowana</param>
        /// <param name="states">Stan</param>
        public void Draw(RenderTarget target, RenderStates states) {
	       // List<Drawable> drawables = new List<Drawable>(Size * Size * 2);
	        foreach ( MapField field in this.Where(field => field.IsFieldSeed && field.IsInsideWindows()) ) {
		        target.Draw(field, states);
		        /*drawables.Insert(0, field);
		        drawables.AddRange(field.OnField);

		        if ( field.ConstructOn != null && field.ConstructOn.BaseField == field) {
			        drawables.Add(field.ConstructOn);
		        }*/
	        }

	       // foreach ( Drawable drawable in drawables ) { target.Draw(drawable, states); }
        }

        #endregion Drawable

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

			if ( ((Math.Abs(e.dx) > 1E-10f) || (Math.Abs(e.dy) > 1E-10f)) ) {
				handler?.Invoke(this, e);
			}
		}

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
			if ( ((MapField.ScreenSize == 20) && (delta < 0)) || ((MapField.ScreenSize == 30) && (delta > 0)) ) {
				return;
			}

			//Nowa delta, gdy stara przekroczylaby dopuszczalny wielkosc pol
			if ( MapField.ScreenSize + delta < 20 ) {
				delta = MapField.ScreenSize - 20;
			} else if ( MapField.ScreenSize + delta > 30 ) {
				delta = 30 - MapField.ScreenSize;
			}

		    if ( delta == 0 ) return;

		    MapField.ScreenSize += delta;

		    MapTextures.GenerateAll((uint)MapField.ScreenSize);

		    MapResized?.Invoke(this, new MapResizedEventArgs(delta));
		}

        #endregion MapResizing

        #region Iterators

        /// <summary>
        /// Funkcja generujaca odwolania do kolejnych pol na mapie zaczynajac od lewego gornego rogu
        /// </summary>
        /// <returns>Iterator po polach na mapie</returns>
        public IEnumerator<MapField> GetEnumerator() {
            for ( int i = 0 ; i < Size ; ++i ) {
                for ( int j = 0 ; j < Size ; ++j ) { yield return this[i, j]; }
            }
        }

        /// <summary>
        /// Funkcja generujaca odwolania do kolejnych pol na mapie zaczynajac od lewego gornego rogu
        /// </summary>
        /// <returns>Iterator po polach na mapie</returns>
	    IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

	    /// <summary>
	    /// Funkcja generujaca odwolania do kolejnych pol na mapie zaczynajac od prawego dolnego rogu
	    /// </summary>
	    /// <returns>Iterator po polach na mapie</returns>
	    public IEnumerable<MapField> Reverse() {
	        for ( int i = Size - 1 ; i >= 0 ; --i ) {
	            for ( int j = Size - 1 ; j >= 0 ; --j ) { yield return this[i, j]; }
	        }
	    }

        #endregion Iterators

        #region Properities

        /// <summary>
        /// Mozliwosc dostania sie do odpowiedniego pola na planszy
        /// </summary>
        /// <param name="i">Pierwsza wspolrzedna</param>
        /// <param name="j">Druga wspolrzedna</param>
        /// <returns>Pole o zadanych wspolrzednych</returns>
        public MapField this[int i, int j] {
            get { return Grid[i][j]; }
            private set { Grid[i][j] = value; }
        }

        /// <summary>
        /// Rozmiar planszy
        /// </summary>
        public int Size { get; private set; }

        #endregion Properities

        #region PrivateVars

        private static Random rand = new Random(1000);
        private readonly List<List<MapField>> Grid;
        private Vector2f? prevMousePos;

        #endregion PrivateVars
    }
}
