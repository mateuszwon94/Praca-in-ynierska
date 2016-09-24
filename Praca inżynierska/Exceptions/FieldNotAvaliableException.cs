using System;
using System.Runtime.Serialization;

namespace PracaInzynierska.Exceptions {
	[Serializable]
	internal class FieldNotAvaliableException : Exception {
		public FieldNotAvaliableException() {
		}

		public FieldNotAvaliableException(string message) : base(message) {
		}

		public FieldNotAvaliableException(string message, Exception innerException) : base(message, innerException) {
		}

		protected FieldNotAvaliableException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}