using PracaInzynierska.Map;
using SFML.Graphics;

namespace PracaInzynierska.Constructs {
	public class Bed : Construct {
		public Bed(uint x, uint y, MapField baseField, Color color) : base(x, y, baseField, color) { }

		public bool IsFree { get; set; } = true;
	}
}