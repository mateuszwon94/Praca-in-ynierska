using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PracaInzynierska.Exceptions {
	class VariableNotFuzzifiedException : Exception {
		public VariableNotFuzzifiedException() { }

		public VariableNotFuzzifiedException(string message) : base(message) { }

		public VariableNotFuzzifiedException(string message, Exception innerException) : base(message, innerException) { }

		protected VariableNotFuzzifiedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
