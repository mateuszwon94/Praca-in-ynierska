using System.Collections.Generic;
using PracaInzynierska.Exceptions;
using static System.Math;

namespace PracaInzynierska.Utils.FuzzyLogic.Variables {

	/// <summary>
	/// Klasa odpowiadająca za zmienną rozmytą
	/// </summary>
	public abstract class FuzzyVariable {

		/// <summary>
		/// domyślny konstruktor tworzący pusta zmienna
		/// </summary>
		protected FuzzyVariable() { }

		/// <summary>
		/// Konstruktor tworzacy zmienna o zadanej wartosci ostrej
		/// </summary>
		/// <param name="value">Wartość ostra zmiennej</param>
		public FuzzyVariable(float value) { Value = value; }

		/// <summary>
		/// Wartosc ostra zmiennej
		/// </summary>
		public virtual float Value { get; set; }

		/// <summary>
		/// Wartosc rozmyta zmiennej. Stopien przynależności do <see cref="State"/>
		/// </summary>
		public float FuzzyValue {
			get {
				if ( State == null ) throw new VariableNotFuzzifiedException();

				State = null;
				return fuzzyVar_;
			}
			protected set { fuzzyVar_ = value; }
		}

		/// <summary>
		/// Nazwa zbioru dla jakiego rozmyta jest zmienna
		/// </summary>
		public string State { get; protected set; }

		/// <summary>
		/// Liczba zbioró w danej zmiennej rozmytej
		/// </summary>
		public abstract int StatesCount { get; }

		/// <summary>
		/// Funkcja rozmywająca zmienną
		/// </summary>
		/// <param name="state">Nazwa zbioru dla jakiego nalezy rozmyć zmienną</param>
		public abstract void Fuzzify(string state);

		/// <summary>
		/// Funkcja rozmywająca zmienną
		/// </summary>
		/// <param name="stateNo">Numer zbioru dla jakiego należy rozmyć zmienną</param>
		public abstract void Fuzzify(int stateNo);

		/// <summary>
		/// Przeladowany operator float w celu ulatwienia poslugiwania sie zmienną ostra
		/// </summary>
		/// <param name="fuzzy">Zmienna rozmyta</param>
		/// <returns>Wartosc ostra zmiennej</returns>
		public static implicit operator float(FuzzyVariable fuzzy) => fuzzy.Value;

		/// <summary>
		/// Operator rozmyty AND
		/// </summary>
		/// <param name="first">Pierwsza wartosc rozmyta</param>
		/// <param name="second">Druga wartosc rozmyta</param>
		/// <returns>Mniejsza z wartosci</returns>
		public static float And(float first, float second) => Min(first, second);

		/// <summary>
		/// Operator rozmyty OR
		/// </summary>
		/// <param name="first">Pierwsza wartosc rozmyta</param>
		/// <param name="second">Druga wartosc rozmyta</param>
		/// <returns>ieksza z wartosci</returns>
		public static float Or(float first, float second) => Max(first, second);

		/// <summary>
		/// Operator rozmyty NOT
		/// </summary>
		/// <param name="val">Wartosc rozmyta</param>
		/// <returns>1-wartosc</returns>
		public static float Not(float val) => 1f - val;


		public static bool operator ==(FuzzyVariable one, FuzzyVariable two) {
			if ( ReferenceEquals(one, null) || ReferenceEquals(two, null) ) return false;

			return one.Value == two.Value;
		}

		/// <summary>
		/// Porownywanie dwóch zmiennych rozmytych oparte jest na ich wartosci ostrej
		/// </summary>
		/// <param name="one">Pierwsza zmienna rozmyta</param>
		/// <param name="two">Druga zmienna rozmyta</param>
		/// <returns>Czy zmienne są od siebie rozne</returns>
		public static bool operator !=(FuzzyVariable one, FuzzyVariable two) {
			if ( ReferenceEquals(one, null) || ReferenceEquals(two, null) ) return false;

			return one.Value != two.Value;
		}

		private float fuzzyVar_;
	}
}