namespace PracaInzynierska.Events.Job {
	public class ConstructingJobEventArgs : JobEventArgs {
		public ConstructingJobEventArgs(float amount) { Amount = amount; }

		public float Amount { get; private set; }
	}
}