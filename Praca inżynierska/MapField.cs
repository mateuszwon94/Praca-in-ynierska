using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static PracaInzynierska.LoadedTextures;

namespace PracaInzynierska {
	class MapField : Drawable {
		/// <summary>
		/// Konstruktor tworzacy pojedyncze pole na planszy
		/// </summary>
		/// <param name="texture">Obraz z jakiego ma zostac wylosowana tekstora</param>
		public MapField(Image texture) {
			Size = 20;
			FieldImage = texture;

			int WhereX = r.Next((int)(texture.Size.X - Size));
			int WhereY = r.Next((int)(texture.Size.Y - Size));

			Field = new Sprite(new Texture(texture, new IntRect(WhereX, WhereY, (int)Size, (int)Size)));
			Field.Texture.Smooth = true;
		}

		/// <summary>
		/// Funkcja rysujaca teksture
		/// </summary>
		/// <param name="target">Cel na ktorym jest rysowana</param>
		/// <param name="states">Stan</param>
		public void Draw(RenderTarget target, RenderStates states) {
			target.Draw(Field);
		}
		
		/// <summary>
		/// Rozmiar pola
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		/// Obiekt slozacy do rysowania pola
		/// </summary>
		public Sprite Field { get; internal set; }

		/// <summary>
		/// Tekstura ktora posluzyla do zainicjalizowania tego pola
		/// </summary>
		public Image FieldImage { get; private set; }

		private static Random r = new Random();
	}
}
