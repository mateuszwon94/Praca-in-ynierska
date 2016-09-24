using System;
using System.Runtime.Serialization;

namespace PracaInzynierska.Exceptions {
	[Serializable]
	class NoSouchNeighbourException : Exception {
		public NoSouchNeighbourException() {
		}

		public NoSouchNeighbourException(string message) : base(message) {
		}

		public NoSouchNeighbourException(string message, Exception innerException) : base(message, innerException) {
		}

		protected NoSouchNeighbourException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}