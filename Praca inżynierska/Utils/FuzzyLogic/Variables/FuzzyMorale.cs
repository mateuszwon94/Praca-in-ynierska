using System;
using System.ComponentModel;
using PracaInzynierska.Utils.FuzzyLogic.Func;

namespace PracaInzynierska.Utils.FuzzyLogic.Variables {
	public class FuzzyMorale : FuzzyVariable {
		public FuzzyMorale(float value) : base(value) {
			brokenFunc_ = new RightShoulderFunc(-9, -5);
			lowFunc_ = new TrapezoidFunc(-9, -5, -3, -2);
			avregeFunc_ = new TrapezoidFunc(-3, -2, 2, 3);
			highFunc_ = new TrapezoidFunc(2, 3, 5, 9);
			fullFunc_ = new LeftShoulderFunc(5, 9);
		}

		public override int StatesCount => (int)States.Full - (int)States.Broken + 1;

		public override void Fuzzify(string stateS) {
			if ( stateS == nameof(States.Broken) ) {
				FuzzyValue = (float)brokenFunc_.Invoke(Value);
				State = nameof(States.Broken);
			} else if ( stateS == nameof(States.Low) ) {
				FuzzyValue = (float)lowFunc_.Invoke(Value);
				State = nameof(States.Low);
			} else if ( stateS == nameof(States.Average) ) {
				FuzzyValue = (float)avregeFunc_.Invoke(Value);
				State = nameof(States.Average);
			} else if ( stateS == nameof(States.High) ) {
				FuzzyValue = (float)highFunc_.Invoke(Value);
				State = nameof(States.High);
			} else if ( stateS == nameof(States.Full) ) {
				FuzzyValue = (float)fullFunc_.Invoke(Value);
				State = nameof(States.Full);
			} else throw new ArgumentException($"There is no state named {stateS}");
		}

		private RightShoulderFunc brokenFunc_;
		private TrapezoidFunc lowFunc_;
		private TrapezoidFunc avregeFunc_;
		private TrapezoidFunc highFunc_;
		private LeftShoulderFunc fullFunc_;

		public enum States {
			Broken = 0,
			Low = 1,
			Average = 2,
			High = 3,
			Full = 4
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