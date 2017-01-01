using System;
using System.Collections.Generic;
using System.Linq;
using PracaInzynierska.Utils.FuzzyLogic.Variables;

namespace PracaInzynierska.Utils.FuzzyLogic {
	public class FAM {
		public FAM(FuzzyVariable row, FuzzyVariable column, string[,] actions) {
			list_ = new List<List<ValueTuple<float, string>>>(row.StatesCount);
			for ( int i = 0 ; i < row.StatesCount; ++i ) {
				list_.Add(new List<ValueTuple<float, string>>(column.StatesCount));
				for ( int j = 0 ; j < column.StatesCount ; ++j ) {
					column.Fuzzify(j);
					row.Fuzzify(i);
					list_[i].Add((FuzzyVariable.And(row.FuzzyValue, column.FuzzyValue), actions[i, j]));
				}
			}
		}

		public (float val, string action) MaxValue {
			get {
				(float v, string a) value = ( float.MinValue, string.Empty );
				foreach ( (float val, string action) pair in list_.SelectMany(list => list) ) {
					if (pair.val > value.v)
					   value = pair;
				}

				return value;
			}
		}

		public (float val, string action) this[int i, int j] {
			get { return list_[i][j]; }
			set { list_[i][j] = value; }
		}

		private List<List<(float val, string action)>> list_;
	}
}