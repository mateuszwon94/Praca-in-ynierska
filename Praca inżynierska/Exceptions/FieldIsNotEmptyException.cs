using System;
using System.Runtime.Serialization;

namespace PracaInzynierska.Exceptions {
	[Serializable]
	internal class FieldIsNotEmptyException : System.Exception {
		public FieldIsNotEmptyException() {
		}

		public FieldIsNotEmptyException(string message) : base(message) {
		}

		public FieldIsNotEmptyException(string message, System.Exception innerException) : base(message, innerException) {
		}

		protected FieldIsNotEmptyException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}