using System.Collections.Generic;
using PracaInzynierska.Exceptions;
using static System.Math;

namespace PracaInzynierska.Utils.FuzzyLogic.Variables {
	public abstract class FuzzyVariable {
		protected FuzzyVariable() { }

		public FuzzyVariable(float value) { Value = value; }

		public virtual float Value { get; set; }

		public float FuzzyValue {
			get {
				if ( State == null ) throw new VariableNotFuzzifiedException();

				State = null;
				return fuzzyVar_;
			}
			protected set { fuzzyVar_ = value; }
		}

		public string State { get; protected set; }

		public abstract int StatesCount { get; }

		public abstract void Fuzzify(string state);

		public abstract void Fuzzify(int stateNo);

		public static implicit operator float(FuzzyVariable fuzzy) => fuzzy.Value;

		public static float And(float first, float second) => Min(first, second);

		public static float Or(float first, float second) => Max(first, second);

		public static float Not(float val) => 1f - val;

		public static bool operator ==(FuzzyVariable one, FuzzyVariable two) {
			if ( ReferenceEquals(one, null) || ReferenceEquals(two, null) ) return false;

			return one.Value == two.Value;
		}

		public static bool operator !=(FuzzyVariable one, FuzzyVariable two) {
			if ( ReferenceEquals(one, null) || ReferenceEquals(two, null) ) return false;

			return one.Value != two.Value;
		}

		private float fuzzyVar_;
	}
}