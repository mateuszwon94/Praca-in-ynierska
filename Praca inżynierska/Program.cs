using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;

namespace Praca_inżynierska {
	class Program {
		static void Main(string[] args) {
			RenderWindow window = new RenderWindow(new VideoMode(800, 600), "Praca inzynierska");
            RectangleShape rec = new RectangleShape(new Vector2f(10, 10));
			window.Closed += (o, e) => window.Close();

			while (window.IsOpen) {
				window.DispatchEvents();
				window.Clear();
				window.Draw(rec);
				window.Display();
			}
		}
	}
}
