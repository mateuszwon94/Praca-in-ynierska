using PracaInzynierska.Exceptions;

namespace PracaInzynierska.Map {
    public struct MapSeed {
        #region Constructors

        /// <summary>
        /// Konstruktor obiektu
        /// </summary>
        /// <param name="sand">Liczba ziaren terenu piaskowego.</param>
        /// <param name="grass">Liczba ziaren terenu trawiastego.</param>
        /// <param name="rock">Liczba ziaren terenu kamiennego.</param>
        public MapSeed(int sand, int grass, int rock) {
            Sand = sand;
            Grass = grass;
            Rock = rock;
        }

        #endregion Constructors

        #region Indexers

        /// <summary>
        /// Indexer przeklada indeks na ziarno.
        /// </summary>
        /// <param name="idx">Index jaki ma byc przetworzony.</param>
        /// <returns>Odpowiadajace przetwarzanemu indeksowi ziarno</returns>
        public Value this[int idx] {
            get {
                if ( idx > Sand + Grass ) { return Value.Rock; }
                else if ( idx > Sand ) { return Value.Grass; }
                else { return Value.Sand; }
            }
        }

        /// <summary>
        /// Indexer przetwarzajacy ziarno na ilosc jego wystapien.
        /// </summary>
        /// <param name="e">Ziarno jakie ma byc przetworzone.</param>
        /// <returns>Ilosc wyatpien tego ziarna.</returns>
        public int this[Value e] {
            get {
                switch ( e ) {
                    case Value.Sand:
                        return Sand;
                    case Value.Grass:
                        return Grass;
                    case Value.Rock:
                        return Rock;
                    default:
                        throw new NoSouchSeed();
                }
            }
        }

        #endregion Indexers

        #region Properties

        /// <summary>
        /// Wlasciwosc okreslajaca ile ma byc wykorzystanych ziaren terenu piaskowego
        /// </summary>
        public int Sand { get; set; }

        /// <summary>
        /// Wlasciwosc okreslajaca ile ma byc wykorzystanych ziaren terenu trawiastego
        /// </summary>
        public int Grass { get; set; }

        /// <summary>
        /// Wlasciwosc okreslajaca ile ma byc wykorzystanych ziaren terenu skalistego
        /// </summary>
        public int Rock { get; set; }

        public int Count => Sand + Grass + Rock;

        #endregion Properties

        #region Enums

        /// <summary>
        /// Enum z dopuszcalnymi typami terenu
        /// </summary>
        public enum Value {
            None = 0,
            Sand,
            Grass,
            Rock
        }

        #endregion Enums
    }
}
