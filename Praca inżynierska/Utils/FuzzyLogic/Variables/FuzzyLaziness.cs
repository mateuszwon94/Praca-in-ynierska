using System;
using System.ComponentModel;
using PracaInzynierska.Utils.FuzzyLogic.Func;

namespace PracaInzynierska.Utils.FuzzyLogic.Variables {
	public class FuzzyLaziness : FuzzyVariable {
		public FuzzyLaziness(float value) : base(value) {
			lazyFunc_ = new RightShoulderFunc(2d, 4d);
			avregeFunc_ = new TrapezoidFunc(2d, 4d, 6d, 8d);
			hardWorkingFunc_ = new LeftShoulderFunc(6d, 8d);
		}

		public override int StatesCount => (int)States.HardWorking - (int)States.Lazy + 1;

		public override void Fuzzify(string stateS) {
			if ( stateS == nameof(States.Lazy) ) {
				FuzzyValue = (float)lazyFunc_.Invoke(Value);
				State = nameof(States.Lazy);
			} else if ( stateS == nameof(States.Average) ) {
				FuzzyValue = (float)avregeFunc_.Invoke(Value);
				State = nameof(States.Average);
			} else if ( stateS == nameof(States.HardWorking) ) {
				FuzzyValue = (float)hardWorkingFunc_.Invoke(Value);
				State = nameof(States.HardWorking);
			} else throw new ArgumentException($"There is no state named {stateS}");
		}

		private readonly RightShoulderFunc lazyFunc_;
		private readonly TrapezoidFunc avregeFunc_;
		private readonly LeftShoulderFunc hardWorkingFunc_;

		public enum States {
			Lazy = 0,
			Average = 1,
			HardWorking = 2,
		}

		public override void Fuzzify(int stateNo) {
			if ( stateNo >= 0 && stateNo < StatesCount ) {
				Fuzzify(((States)stateNo).ToString());
			} else {
				throw new ArgumentException();
			}
		}
	}
}