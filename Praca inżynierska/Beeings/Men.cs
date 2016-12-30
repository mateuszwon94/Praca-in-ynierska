using System;
using System.Collections.Generic;
using System.Linq;
using PracaInzynierska.Events;
using PracaInzynierska.Events.Job;
using PracaInzynierska.Map;
using PracaInzynierska.Utils;
using PracaInzynierska.Utils.FuzzyLogic.Variables;
using PracaInzynierska.Utils.Jobs;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static System.Math;
using static PracaInzynierska.Utils.Algorithm.PathFinding.Metric;

namespace PracaInzynierska.Beeings
{

	/// <summary>
	/// Klasa odpowiadajaca za ludzi
	/// </summary>
	public partial class Men : Beeing {
		public Men() : base() {
			heuristic_ = EuclideanDistance;

			Rest = new RestJob(this);
			AttackThis = new AttackJob(this);
		}

		public string Name { get; set; }

		public Job Job { get; set; }

		public RestJob Rest { get; private set; }

		public float Constructing { get; set; }
		public float Mining { get; set; }

		public FuzzyFatigue Fatigue { get; set; }

		public FuzzyLaziness Laziness { get; set; }

		public FuzzyMorale Morale { get; set; }

		public override void Die() {
			if ( Colony != null && Job != null ) { Colony.JobQueue.Enqueue(Job, Job.Location.All(field => !field.IsAvaliable) ? 3f : 0.5f); }
		}

		/// <summary>
		/// Funkcja sprawdza czy podane koordynaty znajduja sie wewnatrz elementu
		/// </summary>
		/// <param name="x">Pozycja X na ekranie</param>
		/// <param name="y">Pozycja Y na ekranie</param>
		/// <returns>Zwraca true, jesli podane koordynaty znajduja sie wewnatrz obiektu, w przeciwnym wypadku false</returns>
		public override bool InsideElement(int x, int y) {
			if ( (Location.ScreenPosition.X <= x) && (x < Location.ScreenPosition.X + ScreenSize.X) &&
				 (Location.ScreenPosition.Y <= y) && (y < Location.ScreenPosition.Y + ScreenSize.Y) ) {
				Vector2f pos = new Vector2f(x, y) - Location.ScreenPosition;
				Vector2f center = Location.Center - Location.ScreenPosition;

				if ( Pow(center.X - pos.X, 2) + Pow(center.Y - pos.Y, 2) <= Pow(MapField.ScreenSize, 2) ) return true;
			}

			return false;
		}

		/// <summary>
		/// Tekstura jaka ma być wyświetlana na ekranie
		/// </summary>
		public override Sprite Texture {
			get { return IsSelected ? TextureSelected : TextureNotSelected; }
			set {
				if ( IsSelected ) TextureSelected = value;
				else TextureNotSelected = value;
			}
		}

		/// <summary>
		/// Tekstura zaznaczonego czlowieka
		/// </summary>
		public Sprite TextureSelected {
			get { return textureSelected_; }
			set { textureSelected_ = TransformTexture(value); }
		}

		/// <summary>
		/// tekstura niezaznaczonego czlowieka
		/// </summary>
		public Sprite TextureNotSelected {
			get { return textureNotSelected_; }
			set { textureNotSelected_ = TransformTexture(value); }
		}

		/// <summary>
		/// Zwraca pozucje na ekranie danego stworzenia
		/// </summary>
		public override Vector2f ScreenPosition {
			get { return Texture.Position; }
			set {
				if ( TextureNotSelected != null ) TextureNotSelected.Position = value;
				if ( TextureSelected != null ) TextureSelected.Position = value;
			}
		}

		public Colony Colony { get; set; }

		/// <summary>
		/// Funkcja wywoływana przy kazdym odswierzeniu okranu
		/// </summary>
		/// <param name="sender">Obiekt wysylajacy zdazenie</param>
		/// <param name="e">Argumenty zdarzenia</param>
		public override void UpdateTime(object sender, UpdateEventArgs e) {
			//if ( IsSelected ) { }
			if ( Job != null ) {
				if ( Job.Location.All(field => !field.IsAvaliable) ) {
					Colony.JobQueue.Enqueue(Job, 3f);
					Job = null;
				} else if ( Job.State == Job.Status.Done ) {
					Job = null;
					Console.WriteLine("Job DONE!");
				} else if ( !Job.IsInLocation(Location) ) {
					if ( GoToField == null ) GoToField = Closest(Job.Location);
					Go(e.UpdateTime);
				} else {
					Job.Work(this, new JobEventArgs((float)e.UpdateTime));
				}
			}
		}

		private MapField Closest(IEnumerable<MapField> listFields) {
			MapField closestField = null;
			float minDist = float.PositiveInfinity;
			foreach ( MapField field in listFields ) {
				float cur = MapField.Distance(Location, field);
				if ( cur < minDist ) {
					minDist = cur;
					closestField = field;
				}
			}

			return closestField;
		}

		/// <summary>
		/// Funkcja wywoływana przez zdazrenie puszczenia klawisza myszy
		/// </summary>
		/// <param name="sender">obiekt wysylajacy zdarzenie</param>
		/// <param name="e">argumenty zdarzenia</param>
		public override void Window_MouseButtonReleased(object sender, MouseButtonEventArgs e) {
			base.Window_MouseButtonReleased(sender, e);

			if ( IsSelected && e.Button == Mouse.Button.Right ) {
				MapField mapField = Program.map[0, 0];

				int X = (int)(e.X + mapField.ScreenPosition.X) / MapField.ScreenSize,
					Y = (int)(e.Y + mapField.ScreenPosition.Y) / MapField.ScreenSize;

				if ( Program.map[X, Y].ConstructOn != null ) { Job = Program.map[X, Y].ConstructOn.BuildJob; }
			}
		}

		public AttackJob AttackThis { get; private set; } 

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString() => $"{Name} [HP - {HP}]";

		private Sprite textureSelected_;
		private Sprite textureNotSelected_;
	}
}
