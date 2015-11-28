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
	class MapField : Drawable {
		MapField(Image texture) {
			Size = 25;
			int WhereX = r.Next((int)(texture.Size.X - Size));
			int WhereY = r.Next((int)(texture.Size.Y - Size));

			Field = new Sprite(new Texture(texture, new IntRect(WhereX, WhereY, (int)Size, (int)Size)));
		}

		public ulong Size { get; private set; }
		public Sprite Field { get; private set; };

		private static Random r = new Random();

		public void Draw(RenderTarget target, RenderStates states) {
			target.Draw(Field);
		}
	}
}
