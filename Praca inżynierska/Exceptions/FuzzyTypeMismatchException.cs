using System;
using System.Runtime.Serialization;

namespace PracaInzynierska.Exceptions {
	public class FuzzyTypeMismatchException : Exception {
		public FuzzyTypeMismatchException() { }

		public FuzzyTypeMismatchException(string message) : base(message) { }

		public FuzzyTypeMismatchException(string message, Exception innerException) : base(message, innerException) { }

		protected FuzzyTypeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}