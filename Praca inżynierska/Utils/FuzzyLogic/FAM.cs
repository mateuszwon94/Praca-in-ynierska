using System;
using System.Collections.Generic;
using System.Linq;
using PracaInzynierska.Utils.FuzzyLogic.Variables;

namespace PracaInzynierska.Utils.FuzzyLogic {
	public class FAM {
		public FAM(int row, int column) {
			list_ = new List<List<ValueTuple<float, string>>>(row);
			for ( int i = 0 ; i < row ; ++i ) {
				list_.Add(new List<ValueTuple<float, string>>(column));
				for ( int j = 0 ; j < column ; ++j ) {
					list_[i].Add((0f, null));
				}
			}
		}

		public (float val, string action) GetMaxValue() {
			(float v, string a) value = ( float.MinValue, string.Empty );
			foreach ( (float v, string a) pair in list_.SelectMany(list => list.Where(pair => pair.Item1 >= value.v)) ) {
				value = pair;
			}

			return value;
		}

		public (float val, string action) this[int i, int j] {
			get { return list_[i][j]; }
			set { list_[i][j] = value; }
		}

		private List<List<(float val, string action)>> list_;
	}
}