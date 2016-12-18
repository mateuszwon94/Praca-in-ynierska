using System.Collections.Generic;
using System.Linq;
using PracaInzynierska.Beeings;
using PracaInzynierska.Constructs;
using PracaInzynierska.Events;
using PracaInzynierska.Utils.FuzzyLogic.Variables;
using SFML.Graphics;
using Priority_Queue;

namespace PracaInzynierska.Utils {
	public class Colony : Drawable {

		public Colony(Map.Map map, RenderWindow window) {
			colonists_ = new List<Men>();
			constructs_ = new List<Construct>();
			JobQueue = new FastPriorityQueue<Job>(5_000);

			map_ = map;
			window_ = window;
		}

		public void UpdateTime(object sender, UpdateEventArgs e) {
			foreach ( Men colonist in colonists_.Where(colonist => colonist.Job == null) ) {
				if ( JobQueue.Count > 0 ) colonist.Job = JobQueue.Dequeue();
			}

		}

		public IEnumerator<Men> Colonist => colonists_.GetEnumerator();

		public IEnumerator<Construct> Constructs => constructs_.GetEnumerator();

		public void AddColonist(Men colonist) {
			colonists_.Add(colonist);
			colonist.Colony = this;

			map_.UpdateTime += colonist.UpdateTime;
			window_.KeyPressed += colonist.Window_KeyPressed;
			window_.KeyReleased += colonist.Window_KeyReleased;
			window_.MouseButtonPressed += colonist.Window_MouseButtonPressed;
			window_.MouseButtonReleased += colonist.Window_MouseButtonReleased;
		}

		public void AddConstruct(Construct construct) {
			constructs_.Add(construct);
			float priority = construct.BuildJob.Location.All(field => !field.IsAvaliable) ? 3f : 0.5f;
			JobQueue.Enqueue(construct.BuildJob, priority);
		}

		/// <summary>
		/// Draw the object to a render target
		/// This is a function that has to be implemented by the
		/// derived class to define how the drawable should be drawn.
		/// </summary>
		/// <param name="target">Render target to draw to</param>
		/// <param name="states">Current render states</param>
		public void Draw(RenderTarget target, RenderStates states) {
			foreach ( Construct construct in constructs_ ) { target.Draw(construct, states); }
			foreach ( Men colonist in colonists_ ) { target.Draw(colonist, states); }
		}

		public FastPriorityQueue<Job> JobQueue { get; private set; }

		private readonly List<Construct> constructs_;

		private readonly List<Men> colonists_;

		private Map.Map map_;
		private RenderWindow window_;
	}
}