using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using PracaInzynierska;
using PracaInzynierska.Events;
using PracaInzynierska.Map;
using static System.Math;

namespace PracaInzynierska.Beeing {
	public class Men : Beeing {

		public Men() {
			MouseButtonReleased += (o, e) => {
				if ( IsSelected ) IsSelected = false;
				else IsSelected = true;
			};
		}
		
        public override bool InsideElement(int x, int y) {
            if ( (Location.ScreenPosition.X <= x) && (x < Location.ScreenPosition.X + Size.X) && (Location.ScreenPosition.Y <= y) && (y < Location.ScreenPosition.Y + Size.Y) ) {
                Vector2f pos = new Vector2f(x, y) - Location.ScreenPosition;
                Vector2f center = Location.Center - Location.ScreenPosition;

                if ( Pow(center.X - pos.X, 2) + Pow(center.Y - pos.Y, 2) <= Pow(MapField.Size, 2) ) return true;
            }
            return false;
        }

		public override Sprite Texture {
			get { return IsSelected ? TextureSelected : TextureNotSelected; }
			set {
				if ( IsSelected ) TextureSelected = value;
				else              TextureNotSelected = value;
			}
		}

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

		public override Vector2f ScreenPosition {
			get { return Texture.Position; }
			set {
				if ( TextureNotSelected != null ) TextureNotSelected.Position = value;
				if ( TextureSelected != null ) TextureSelected.Position = value;
			}
		}

		public override void UpdateTime(object sender, UpdateEventArgs e) {
            if ( IsSelected ) { }
        }

		private Sprite textureSelected_;
		private Sprite textureNotSelected_;
	}
}
