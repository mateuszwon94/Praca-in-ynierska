using System;
using System.Collections;
using System.Collections.Generic;
using PracaInzynierska.Exceptions;

namespace PracaInzynierska.Map {

    /// <summary>
    /// Klasa reprezentujaca pole mapy
    /// </summary>
	public partial class MapField {

        /// <summary>
        /// Klasa umożliwiajaca dostanie sie do sasiadow danego pola
        /// </summary>
		public class FieldNeighvours : IEnumerable<MapField> {
            #region Constructors

            /// <summary>
            /// Konstruktor obiektu
            /// </summary>
            /// <param name="parent">Pole, do ktorego sasiadow ma na umozliwic dostep ta instancja</param>
            internal FieldNeighvours(MapField parent) { parent_ = parent; }

            #endregion Constructors

            #region Indexer

            /// <summary>
            /// Indexer umozliwiajacy dostanie sie do konkretnego sasiada.
            /// </summary>
            /// <param name="x">Wzgledna pozycja x sasiada.</param>
            /// <param name="y">Wzgledna pozycja y sasiada.</param>
            /// <exception cref="NoSouchNeighbourException">Zwraca wyjatek, jesli sasiad nie istnieje badz probujemy odwolac sie "za daleko".</exception>
            /// <returns>Pole sasiada.</returns>
            public MapField this[int x, int y] {
                get {
                    if ( ((x < -1) || (x > 1)) || ((y < -1) || (y > 1)) ) { throw new NoSouchNeighbourException(); }

                    try {
                        return parent_.map_[parent_.MapPosition.X + x, parent_.MapPosition.Y + y];
                    } catch {
                        throw new NoSouchNeighbourException();
                    }
                }
            }

            #endregion Indexer

            #region Iterators

            /// <summary>
            /// Funkcja generujaca odwolania do kolejnych istniejacych sasiadow poczawszy od [-1, -1]
            /// </summary>
            /// <returns>Iterator po sasiadach</returns>
            public IEnumerator<MapField> GetEnumerator() {
                for ( sbyte x = -1 ; x <= 1 ; ++x ) {
                    for ( sbyte y = -1 ; y <= 1 ; ++y ) {
                        try {
                            MapField mf = this[x, y];
                        } catch ( NoSouchNeighbourException ) {
                            continue;
                        }

                        if ( (x == 0) && (y == 0) ) continue;

                        yield return this[x, y];
                    }
                }
            }

            /// <summary>
            /// Funkcja generujaca odwolania do kolejnych istniejacych sasiadow poczawszy od [-1, -1]
            /// </summary>
            /// <returns>Iterator po sasiadach</returns>
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

            #endregion Iterators

            #region PrivateVars

            private readonly MapField parent_;

            #endregion PrivateVars
        }
    }
}