namespace PracaInzynierska.Utils.FuzzyLogic.Func {
	public class RightShoulderFunc {
		public RightShoulderFunc(double x0, double x1) {
			x0_ = x0;
			x1_ = x1;
			(a_, b_) = Math.LinearFactors(x0, 1, x1, 0);
		}

		public double Invoke(double x) {
			if ( x <= x0_ ) return 1.0;
			else if ( x >= x1_ ) return 0.0;
			else return a_ * x + b_;
		}

		private readonly double x0_;
		private readonly double x1_;
		private readonly double a_;
		private readonly double b_;
	}
}