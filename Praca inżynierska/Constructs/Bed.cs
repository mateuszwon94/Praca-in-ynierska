using PracaInzynierska.Map;
using SFML.Graphics;

namespace PracaInzynierska.Constructs {

	/// <summary>
	/// Klasa reprezentujaca lozko
	/// </summary>
	public class Bed : Construct {

		/// <inheritdoc />
		public Bed(uint x, uint y, MapField baseField, Color color) : base(x, y, baseField, color) { }

		/// <summary>
		/// Zmienna reprezentująca, czy łóżko jest zajęte czy nie
		/// </summary>
		public bool IsFree { get; set; } = true;
	}
}