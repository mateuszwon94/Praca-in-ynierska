using PracaInzynierska.Map;
using SFML.Graphics;

namespace PracaInzynierska.Constructs {
	public static class ConstructGenerator {

		public static (Construct horizontal, Construct wertical) Wall(MapField baseField) {
			return (new Construct(1, 1, baseField, new Color(128, 128, 128)) {
																				 MaxConstructPoints = 300,
																				 Name = "Wall",
																			 },
					new Construct(1, 1, baseField, new Color(128, 128, 128)) {
																				 MaxConstructPoints = 300,
																				 Name = "Wall",
					});
		}

		public static (Construct horizontal, Construct wertical) Bed(MapField baseField) {
			return (new Construct(2, 1, baseField, Color.Blue) {
																   MaxConstructPoints = 250,
																   Name = "Bed",
															   },
					new Construct(1, 2, baseField, Color.Blue) {
																   MaxConstructPoints = 250,
																   Name = "Bed",
															   });

		}


		public static (Construct horizontal, Construct wertical) Table(MapField baseField) {
			return (new Construct(2, 2, baseField, new Color(60, 60, 0)) {
																			MaxConstructPoints = 250,
																			Name = "Bed",
																		},
					new Construct(2, 2, baseField, new Color(60, 60, 0)) {
						MaxConstructPoints = 250,
						Name = "Bed",
					});
		}

		public static (Construct horizontal, Construct wertical) FoodMaker(MapField baseField) {
			return (new Construct(3, 2, baseField, new Color(35, 175, 75)) {
																			  MaxConstructPoints = 250,
																			  Name = "Bed",
																		  },
					new Construct(2, 3, baseField, new Color(35, 175, 75)) {
						MaxConstructPoints = 250,
						Name = "Bed",
					});
		}
	}
}