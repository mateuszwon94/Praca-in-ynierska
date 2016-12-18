using PracaInzynierska.Exceptions;
using static System.Math;

namespace PracaInzynierska.Utils.FuzzyLogic.Variables {
	public abstract class FuzzyVariable {
		public FuzzyVariable(float value) { Value = value; }

		public virtual float Value {
			get { return value_; }
			set {
				value_ = value;
				Fuzzify(State);
			}
		}

		public float FuzzyValue {
			get {
				if ( State == null ) throw new VariableNotFuzzifiedException();
				else return fuzzyVar_;
			}
			protected set { fuzzyVar_ = value; }
		}

		public string State { get; protected set; }

		public abstract int StatesCount { get; }

		public static implicit operator float(FuzzyVariable fuzzy) { return fuzzy.Value; }

		public abstract void Fuzzify(string state);

		public static float And(float first, float second) => Min(first, second);

		public static float Or(float first, float second) => Max(first, second);

		public static float Not(float val) => 1f - val;

		private float fuzzyVar_;
		private float value_;
	}
}