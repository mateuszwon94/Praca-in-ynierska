﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PracaInzynierska.Events;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Map;
using SFML.Graphics;
using SFML.System;
using PracaInzynierska.Utils.Interfaces;
using static PracaInzynierska.Textures.MapTextures;
using static PracaInzynierska.Beeings.Beeing;
using static System.Math;
using PracaInzynierska.Beeings;
using PracaInzynierska.Utils.FuzzyLogic.Variables;

namespace PracaInzynierska.Utils.Algorithm {

	/// <summary>
	/// Klasa zarzadzajaca pojedynczym stadem.
	/// </summary>
	public class Herd : Drawable, IEnumerable<Animal>, IUpdateTime {

		/// <summary>
		/// Konstruktor tworzacy stado
		/// </summary>
		/// <param name="location">Pozycja wokol ktorej ma byc stworzone stado</param>
		/// <param name="count">liczba czlonkow stada</param>
		public Herd(MapField location, uint count) {
			for ( uint i = 0 ; i < count ; ++i ) {
				MapField mapField;
				do { // wybieranie pozycji dla czlonka stada tak dlugo az uda sie znalezc dostepne miejsce
					mapField = location;
					int x = rand_.Next(-(int)(count / 2), (int)(count / 2) + 1);
					int y = rand_.Next(-(int)(count / 2), (int)(count / 2) + 1);

					try { mapField = mapField.Neighbour[x, y]; }
					catch ( NoSouchNeighbourException ) { mapField = null; }
				} while ( (mapField == null) || !mapField.IsAvaliable || mapField.IsSomethingOn );

				herd_.Add(new Animal() {
										   MoveSpeed = 1,
										   Location = mapField,
										   Texture = new Sprite(AnimalTexture),
										   HP = new FuzzyHP(30f, 50f),
					Strength = 6f,
									   });
			}
		}

		/// <summary>
		/// Funkcja rysujaca po kolei wszystkoch czlonkow stada
		/// </summary>
		/// <param name="target">Obiekt na ktorym ma byc narysowane stado</param>
		/// <param name="states">Stan</param>
		public void Draw(RenderTarget target, RenderStates states) {
			foreach ( Animal animal in this ) { animal.Draw(target, states); }
		}

		private List<Animal> herd_ = new List<Animal>();

		/// <summary>Returns an enumerator that iterates through the collection.</summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		public IEnumerator<Animal> GetEnumerator() { return herd_.GetEnumerator(); }

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

		public MapField Location {
			get {
			    Vector2f center = herd_.Aggregate(new Vector2f(0, 0),
			                                      (current, a) =>
			                                          current + new Vector2f(a.ExactPosition.X, a.ExactPosition.Y));

				center /= herd_.Count;

				return Program.map[(int)Round(center.X), (int)Round(center.Y)];
			}
		}

		/// <summary>
		/// Regula spojnosci w algorytmie stada. Sterowanie w kierunku uśrednionego położenia lokalnej grupy.
		/// Spójność daje zwierzeciu możliwość grupowania się ze zwierzetami z lokalnej grupy, czyli zapobiega „rozlatywaniu” się stad.
		/// </summary>
		/// <param name="animal">Zwierze dla ktorego ma byc obliczona ta regula</param>
		/// <returns>Wektor mowiacy w jakim kierunku na podstawie tej reguly ma sie poruzyc dane zwierze</returns>
		private Vector2f Cohesion(Animal animal) {
		    Vector2f centerOfMass = herd_.Where(a => a != animal)
		                                 .Aggregate(new Vector2f(0, 0),
		                                            (current, a) =>
		                                                current +
		                                                new Vector2f(a.ExactPosition.X, a.ExactPosition.Y));

			centerOfMass /= herd_.Count - 1;

			Vector2f animalPos = new Vector2f(animal.ExactPosition.X, animal.ExactPosition.Y);

			return centerOfMass - animalPos;
		}

		/// <summary>
		/// Regula rozdzielności w algorytmie stada. Sterowanie zapobiegające lokalnym zbiorowiskom.
		/// Rozdzielność daje zwierzeci możliwość utrzymania pożądanej odległości od innych zwierzat z lokalnej grupy, a tym samym zapobiega tworzeniu tłumu w jednym miejscu
		/// </summary>
		/// <param name="animal">Zwierze dla ktorego ma byc obliczona ta regula</param>
		/// <param name="minDistance">Minimalny dystans dla jakiego ma byc obliczana ta regula</param>
		/// <returns>Wektor mowiacy w jakim kierunku na podstawie tej reguly ma sie poruzyc dane zwierze</returns>
		private Vector2f Separation(Animal animal, float minDistance) {
			return herd_.Where(a => (a != animal) && (Distance(a, animal) < minDistance))
						.Aggregate(new Vector2f(0, 0),
								   (current, a) => current - new Vector2f(a.ExactPosition.X - animal.ExactPosition.X,
																		  a.ExactPosition.Y - animal.ExactPosition.Y));
		}

		/// <summary>
		/// Regula wyrownania w algorytmie stada. Sterowanie w kierunku uśrednionego celu lokalnej grupy.
		/// Wyrównywanie zapewnia zwierzeciu możliwość dostosowania swojego ruchu (tzn. zmiany kierunku ruchu lub prędkości) do innych zwierzat z jego lokalnej grupy
		/// </summary>
		/// <param name="animal">Zwierze dla ktorego ma byc obliczona ta regula</param>
		/// <returns>Wektor mowiacy w jakim kierunku na podstawie tej reguly ma sie poruzyc dane zwierze</returns>
		private Vector2f Alignment(Animal animal) {
			return herd_.Where(a => a != animal)
						.Aggregate(new Vector2f(0, 0),
								   (current, a) => current + a.MoveVector);
		}

		/// <summary>
		/// Funkcja wywoływana przy kazdym odswierzeniu okranu
		/// </summary>
		/// <param name="sender">Obiekt wysylajacy zdazenie</param>
		/// <param name="e">Argumenty zdarzenia</param>
		public void UpdateTime(object sender, UpdateEventArgs e) {
			if ( counter_ == 0 ) {
				counter_ = rand_.Next(1000, 3000);

				if ( rand_.Next(2) == 1 ) {
					pocX_ = 0;
					konX_ = 5;
				} else {
					pocX_ = -5;
					konX_ = 0;
				}

				if ( rand_.Next(2) == 1 ) {
					pocY_ = 0;
					konY_ = 5;
				} else {
					pocY_ = -5;
					konY_ = 0;
				}
			}

			// ALGORYTM STADA
			foreach ( Animal animal in herd_.Where(animal => !animal.IsMoveing) ) {
				// wyliczenie pozycji na jaka powinno poruzyc sie zwierze
			    Vector2f randVec = new Vector2f(rand_.Next(pocX_, konX_), rand_.Next(pocY_, konY_));
				Vector2f moveVector = Cohesion(animal) +
									  Separation(animal, 2f) +
									  Alignment(animal) +
									  randVec;

				MapField field;

				// wylicenie jakie to pole na mapie
				int x = (int)Round(moveVector.X);
				int y = (int)Round(moveVector.Y);
				try { field = animal.Location.Neighbour[x, y]; }
				catch ( NoSouchNeighbourException ) { continue; }

				// przekazanie tego pola do zwierzecia
				animal.GoToField = field;
			}

		    --counter_;
		}

	    private int pocX_,
	                pocY_,
	                konX_,
	                konY_;
	    private int counter_;

		private static readonly Random rand_ = new Random();
	}
}
