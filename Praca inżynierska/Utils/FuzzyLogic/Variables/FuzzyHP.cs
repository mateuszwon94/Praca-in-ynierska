using System;
using System.ComponentModel;
using PracaInzynierska.Utils.FuzzyLogic.Func;

namespace PracaInzynierska.Utils.FuzzyLogic.Variables {
	public class FuzzyHP : FuzzyVariable {
		public FuzzyHP(float value, float maxHP) {
			MaxHP = maxHP;
			Value = value;
		}

		public float MaxHP {
			get { return maxHp_; }
			set {
				maxHp_ = value;
				dyingFunc_ = new RightShoulderFunc(1f / 6f * value, 2f / 6f * value);
				lowFunc_ = new TraingularFunc(1f / 6f * value, 2f / 6f * value, 3f / 6f * value);
				avregeFunc_ = new TraingularFunc(2f / 6f * value, 3f / 6f * value, 4f / 6f * value);
				highFunc_ = new TraingularFunc(3f / 6f * value, 4f / 6f * value, 5f / 6f * value);
				fullFunc_ = new LeftShoulderFunc(4f / 6f * value, 5f / 6f * value);
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

		private RightShoulderFunc dyingFunc_;
		private TraingularFunc lowFunc_;
		private TraingularFunc avregeFunc_;
		private TraingularFunc highFunc_;
		private LeftShoulderFunc fullFunc_;

		public enum States {
			Dying = 0,
			Low = 1,
			Average = 2,
			High = 3,
			Full = 4
		}

		private float value_;
		private float maxHp_;

		public override void Fuzzify(int stateNo) {
			if ( stateNo >= 0 && stateNo < StatesCount ) {
				Fuzzify(((States)stateNo).ToString());
			} else {
				throw new ArgumentException();
			}
		}
	}
}