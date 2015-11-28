using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using static PracaInzynierska.LoadedTextures;

namespace PracaInzynierska {
	class Map {
		Map() {
			int cos = r.Next();
		}

		public MapField this[int i, int j] {
					get { return Grid[i][j]; }
			private	set { Grid[i][j] = value; }
		}

		private static Random r = new Random();
		private List<List<MapField>> Grid;
	}
}
