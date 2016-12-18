namespace PracaInzynierska.Events {

	/// <summary>
	/// Typ argumentu eventu wywoływanego podczas skalowania mapy
	/// </summary>
	public class MapResizedEventArgs {

		/// <summary>
		/// Konstruktor obiektu
		/// </summary>
		/// <param name="delta">Powiekszenie w pikselach</param>
		public MapResizedEventArgs(int delta) {
			Delta = delta;
		}

		/// <summary>
		/// Powiekszenie o zadana liczbe pikseli
		/// </summary>
		public int Delta { get; private set; }
	}
}
