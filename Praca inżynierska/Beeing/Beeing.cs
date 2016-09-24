using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Audio;
using SFML.System;
using SFML.Window;
using PracaInzynierska.Events;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Map;

namespace PracaInzynierska.Beeing {
	public abstract class Beeing : Drawable {

		public Beeing(Window window, Map.Map map) {
			AddEvents(window);
			map.UpdateTime += UpdateTime;

			MouseButtonReleased += (o, e) => {
							   if ( IsSelected ) IsSelected = false;
							   else IsSelected = true;
						   };
		}

		protected virtual void UpdateTime(object sender, UpdateEventArgs e) {
			
		}

		#region Events
		public event EventHandler<KeyEventArgs> KeyPressed;
		public event EventHandler<KeyEventArgs> KeyReleased;
		public event EventHandler<MouseButtonEventArgs> MouseButtonPressed;
		public event EventHandler<MouseButtonEventArgs> MouseButtonReleased;

		public EventHandler<KeyEventArgs> KeyPressedHandler {
			get { return null; }
			set { KeyPressed += value; }
		}

		public EventHandler<KeyEventArgs> KeyReleasedHandler {
			get { return null; }
			set { KeyReleased += value; }
		}

		public EventHandler<MouseButtonEventArgs> MouseButtonPressedHandler {
			get { return null; }
			set { MouseButtonPressed += value; }
		}

		public EventHandler<MouseButtonEventArgs> MouseButtonReleasedHandler {
			get { return null; }
			set { MouseButtonReleased += value; }
		}

		public void Window_MouseButtonReleased(object sender, MouseButtonEventArgs e) {
			if ( InsideElement(e.X, e.Y) ) {
				EventHandler<MouseButtonEventArgs> handler = MouseButtonReleased;
				handler?.Invoke(sender, e);
			}
		}

		public virtual void Window_MouseButtonPressed(object sender, MouseButtonEventArgs e) {
			if ( InsideElement(e.X, e.Y) ) {
				EventHandler<MouseButtonEventArgs> handler = MouseButtonPressed;
				handler?.Invoke(sender, e);
			}
		}

		public virtual void Window_KeyReleased(object sender, KeyEventArgs e) {
			EventHandler<KeyEventArgs> handler = KeyReleased;
			handler?.Invoke(sender, e);
		}

		public virtual void Window_KeyPressed(object sender, KeyEventArgs e) {
			EventHandler<KeyEventArgs> handler = KeyPressed;
			handler?.Invoke(sender, e);
		}

		private void AddEvents(Window window) {
			window.KeyPressed += Window_KeyPressed;
			window.KeyReleased += Window_KeyReleased;
			window.MouseButtonPressed += Window_MouseButtonPressed;
			window.MouseButtonReleased += Window_MouseButtonReleased;
		} 
		#endregion Events

		public virtual bool InsideElement(int x, int y) {
			return (Position.X <= x) && (x < Position.X + Size.X) && (Position.Y <= y) && (y < Position.Y + Size.Y);
		}

		public virtual bool InsideElement(Vector2i poition) {
			return (Position.X <= poition.X) && (poition.X < Position.X + Size.X) && (Position.Y <= poition.Y) && (poition.Y < Position.Y + Size.Y);
		}

		public Sprite Texture => IsSelected ? TextureSelected : TextureNotSelected;

		public Sprite TextureSelected {
			get { return textureSelected_; }
			set {
				textureSelected_ = value;
				TransformTexture(textureSelected_);
			}
		}

		public Sprite TextureNotSelected {
			get { return textureNotSelected_; }
			set {
				textureNotSelected_ = value;
				TransformTexture(textureNotSelected_);
			}
		}

		private void TransformTexture(Sprite tex) {
			tex.Origin = new Vector2f(tex.Texture.Size.X / 2f, tex.Texture.Size.Y / 2f);
			tex.Position = Location.Center;
		}

		public bool IsSelected { get; set; }

		public double MoveSpeed { get; set; } = 1.0;

		public MapField Location {
			get { return mapField_; }
			set {
				mapField_ = value;
				try { mapField_.UnitOn = this; }
				catch ( FieldIsNotEmptyException ex ) {
					mapField_ = null;
					throw ex;
				}
			}
		}
		
		public Vector2f Position {
			get { return Texture.Position; }
			set {
				TextureNotSelected.Position = value;
				TextureSelected.Position = value;
			}
		}

		public Vector2u Size => Texture.Texture.Size;

		private MapField mapField_;
		private Sprite textureSelected_;
		private Sprite textureNotSelected_;

		public abstract void Draw(RenderTarget target, RenderStates states);
	}
}
