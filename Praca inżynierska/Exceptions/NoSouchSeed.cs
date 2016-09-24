using System;
using System.Runtime.Serialization;

namespace PracaInzynierska.Exceptions {
	[Serializable]
	internal class NoSouchSeed : Exception {
		public NoSouchSeed() {
		}

		public NoSouchSeed(string message) : base(message) {
		}

		public NoSouchSeed(string message, Exception innerException) : base(message, innerException) {
		}

		protected NoSouchSeed(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}