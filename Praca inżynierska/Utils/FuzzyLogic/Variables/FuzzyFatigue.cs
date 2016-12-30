using System;
using System.ComponentModel;
using PracaInzynierska.Utils.FuzzyLogic.Func;

namespace PracaInzynierska.Utils.FuzzyLogic.Variables {
	public class FuzzyFatigue : FuzzyVariable {
		public FuzzyFatigue(float value) : base(value) {
			tiredFunc_ = new RightShoulderFunc(2d, 5d);
			avregeFunc_ = new TrapezoidFunc(2d, 5d, 6d, 8d);
			fullOfEnergyFunc_ = new LeftShoulderFunc(6d, 8d);
		}

		public override float Value {
			get { return value1_; }
			set { value1_ = value < 0f ? 0f : value; }
		}

		public override int StatesCount => (int)States.FullOfEnergy - (int)States.Tired + 1;

		public override void Fuzzify(string stateS) {
			if ( stateS == nameof(States.Tired) ) {
				FuzzyValue = (float)tiredFunc_.Invoke(Value);
				State = nameof(States.Tired);
			} else if ( stateS == nameof(States.Normal) ) {
				FuzzyValue = (float)avregeFunc_.Invoke(Value);
				State = nameof(States.Normal);
			} else if ( stateS == nameof(States.FullOfEnergy) ) {
				FuzzyValue = (float)fullOfEnergyFunc_.Invoke(Value);
				State = nameof(States.FullOfEnergy);
			} else throw new ArgumentException($"There is no state named {stateS}");
		}

		private readonly RightShoulderFunc tiredFunc_;
		private readonly TrapezoidFunc avregeFunc_;
		private readonly LeftShoulderFunc fullOfEnergyFunc_;
		private float value1_;

		public enum States {
			Tired = 0,
			Normal = 1,
			FullOfEnergy = 2,
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