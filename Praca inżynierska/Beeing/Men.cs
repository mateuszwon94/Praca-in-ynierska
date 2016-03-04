using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Audio;
using SFML.System;
using SFML.Window;
using PracaInzynierska;

namespace PracaInzynierska.Beeing {
	class Men : Drawable {

		public Men(double moveSpeed, MapField location, Image texture) {
			MoveSpeed = moveSpeed;
			Location = location;
			Location.UnitOn = this;
			Texture = new Sprite(new Texture(texture));
			Texture.Texture.Smooth = true;
			Texture.Position = Location.Field.Position;

			Program.map.UpdateTime += UpdateTime;
			Program.window.MouseMoved += Window_MouseMoved;
		}

		private void UpdateTime(object sender, UpdateEventArgs e) {
			this.Position = new Vector2f(Position.X + 5f * e.UpdateTime, Position.Y + 5f * e.UpdateTime);
			Console.WriteLine("Jestem na pozycji ({0}, {1}).", Location.Position.X, Location.Position.Y);
		}

		public Sprite Texture { get; private set; }

		public double MoveSpeed { get; private set; }

		public MapField Location { get; private set; }

		public Vector2f Position {
			get { return Texture.Position; }
			private set {
				Texture.Position = value;
			}
		}

		public void Draw(RenderTarget target, RenderStates states) {
			if ( this.Texture.Position.X >= -Texture.Texture.Size.X &&
					this.Texture.Position.X <= Program.window.Size.X &&
					this.Texture.Position.Y >= -Texture.Texture.Size.Y &&
					this.Texture.Position.Y <= Program.window.Size.Y ) {
				target.Draw(Texture);
			}
		}

		private void Window_MouseMoved(object sender, MouseMoveEventArgs e) {
			if ( prevMousePos == null ) {
				prevMousePos = new Vector2f(e.X, e.Y);
				return;
			}
			float dx = e.X - prevMousePos.Value.X;
			float dy = e.Y - prevMousePos.Value.Y;

			if ( Mouse.IsButtonPressed(Mouse.Button.Middle) ) {
				if ( Texture != null ) {
					float x = (this.Texture.Position.X + dx);
					float y = (this.Texture.Position.Y + dy);
					this.Texture.Position = new Vector2f(x, y);
				}
			}

			prevMousePos = new Vector2f(e.X, e.Y);
		}

		private Vector2f? prevMousePos = null;
	}
}
