using System;
using System.ComponentModel;
using PracaInzynierska.Utils.FuzzyLogic.Func;

namespace PracaInzynierska.Utils.FuzzyLogic.Variables {
	public class FuzzyHP : FuzzyVariable {
		public FuzzyHP(float value) : base(value) { }

		public float MaxHP {
			get { return maxHp_; }
			set {
				maxHp_ = value;
				dyingFunc_  = new RightShoulderFunc(1f / 7f * value, 2f / 7f * value);
				lowFunc_    = new TraingularFunc(2f / 7f * value, 3f / 7f * value, 4f / 7f * value);
				avregeFunc_ = new TraingularFunc(3f / 7f * value, 4f / 7f * value, 5f / 7f * value);
				highFunc_   = new TraingularFunc(4f / 7f * value, 5f / 7f * value, 6f / 7f * value);
				fullFunc_   = new LeftShoulderFunc(5f / 7f * value, 6f / 7f * value);
			}
		}

		public override float Value {
			get { return value_; }
			set {
				if ( value >= MaxHP ) value_ = MaxHP;
				else value_ = value;
			}
		}
		public override int StatesCount => (int)States.Full - (int)States.Dying + 1;

		public override void Fuzzify(string stateS) {
			if ( stateS == nameof(States.Dying) ) {
				FuzzyValue = (float)dyingFunc_.Invoke(Value);
				State = nameof(States.Dying);
			} else if ( stateS == nameof(States.Low) ) {
				FuzzyValue = (float)lowFunc_.Invoke(Value);
				State = nameof(States.Low);
			} else if ( stateS == nameof(States.Avrege) ) {
				FuzzyValue = (float)avregeFunc_.Invoke(Value);
				State = nameof(States.Avrege);
			} else if ( stateS == nameof(States.High) ) {
				FuzzyValue = (float)highFunc_.Invoke(Value);
				State = nameof(States.High);
			} else if ( stateS == nameof(States.Full) ) {
				FuzzyValue = (float)fullFunc_.Invoke(Value);
				State = nameof(States.Full);
			} else throw new ArgumentException($"There is no state named {stateS}");
		}

		private RightShoulderFunc dyingFunc_;
		private TraingularFunc lowFunc_;
		private TraingularFunc avregeFunc_;
		private TraingularFunc highFunc_;
		private LeftShoulderFunc fullFunc_;

		public enum States {
			Dying = 0,
			Low = 1,
			Avrege = 2,
			High = 3,
			Full = 4
		}

		private float value_;
		private float maxHp_;
	}
}