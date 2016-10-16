using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracaInzynierska.Events;
using PracaInzynierska.Map;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static PracaInzynierska.Utils.Algorithm.PathFinding;
using static PracaInzynierska.Utils.Algorithm.PathFinding.Metric;
using static PracaInzynierska.Utils.Math.Math;

namespace PracaInzynierska.Beeing {
	public class Animal : Beeing {

		public override void UpdateTime(object sender, UpdateEventArgs e) {

			if ( GoToField != null ) {
				IsMoveing = true;

				MapField cur;
				if ( Location.Neighbour.Contains(GoToField) ) { cur = GoToField; }
				else {
					if ( path_ == null ) { path_ = AStar(Location, GoToField, ManhattanDistance); }

					do { cur = path_.RemoveAndGet(0); } while ( cur == Location );
				}

				Vector2f moved = new Vector2f((float)Lerp(ScreenPosition.X, cur.Center.X, e.UpdateTime),
											  (float)Lerp(ScreenPosition.Y, cur.Center.Y, e.UpdateTime)) * (float)MoveSpeed;

				moved_ += moved;
				ScreenPosition += moved;

				if ( ScreenPosition == cur.Center ) {
					moved_ = new Vector2f(0, 0);
					
				}
			}

			if ( GoToField == Location ) {
				GoToField = null;
				IsMoveing = false;
			}

		}

		protected override void TransformTexture(Sprite tex) {
			base.TransformTexture(tex);
			tex.Position += moved_;
		}

		public override Sprite Texture {
			get { return texture_; }
			set {
				texture_ = value;
				TransformTexture(texture_);
			}
		}

		public override Vector2f ScreenPosition {
			get { return Texture.Position; }
			set { Texture.Position = value; }
		}

		public MapField GoToField { get; set; }

		public bool IsMoveing { get; private set; } = false;

		private IList<MapField> path_;

		private Vector2f moved_ = new Vector2f(0, 0);

		private Sprite texture_;

		public static bool operator ==(Animal first, Animal other) {
			if ( (first is null) || (other is null) ) return false;

			return ReferenceEquals(first, other);
		}

		public static bool operator !=(Animal first, Animal other) {
			if ( (first == null) || (other == null) ) return false;

			return !ReferenceEquals(first, other);
		}
		
		public bool Equals(MapField other) { return ReferenceEquals(this, other); }
		
		public override bool Equals(object other) {
			return Equals((Animal)other);
		}

		protected bool Equals(Animal other) {
			if ( ReferenceEquals(null, other) ) return false;
			if ( ReferenceEquals(this, other) ) return true;
			return false;
		}

		public override int GetHashCode() { return Location.GetHashCode(); }
	}
}
