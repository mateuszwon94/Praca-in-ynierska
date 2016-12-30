using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using PracaInzynierska.Beeings;
using PracaInzynierska.Constructs;
using PracaInzynierska.Events;
using PracaInzynierska.Utils.FuzzyLogic;
using PracaInzynierska.Utils.FuzzyLogic.Variables;
using PracaInzynierska.Utils.Interfaces;
using PracaInzynierska.Utils.Jobs;
using SFML.Graphics;
using Priority_Queue;

namespace PracaInzynierska.Utils {
	public class Colony : Drawable, IEnumerable<Men>, IUpdateTime {

		public Colony(Map.Map map, RenderWindow window) {
			colonists_ = new List<Men>();
			constructs_ = new List<Construct>();
			JobQueue = new FastPriorityQueue<Job>(5_000);

			map_ = map;
			window_ = window;
		}

		public void UpdateTime(object sender, UpdateEventArgs e) {
			for ( int index = 0 ; index < colonists_.Count ;  ) {
				if ( colonists_[index].HP.Value > 0f ) {
					index++;
					continue;
				}

				colonists_[index].Die();
				colonists_.Remove(colonists_[index]);
			}

			foreach ( Men colonist in Colonist.Where(c => c.Job == null) ) {
				string stateSleep = MakeDecision(colonist.Fatigue, colonist.Laziness);
				string stateWork = MakeDecision(colonist.HP, colonist.Laziness);

				if ( stateSleep == "Sleep" ) {
					Bed bed = Constructs.Where(c => c is Bed)
										.Cast<Bed>()
										.FirstOrDefault(b => b.State == Construct.Status.Done && b.IsFree);

					colonist.Job = new SleepJob(colonist, bed);
				} else if ( stateWork == "Work" && JobQueue.Count > 0 ) {
					colonist.Job = JobQueue.Dequeue();
				} else if ( stateWork == "Rest" || colonist.HP != colonist.HP.MaxHP ) { colonist.Job = colonist.Rest; }

			}
		}

		public IList<Men> Colonist => colonists_;

		public IList<Construct> Constructs => constructs_;

		public void AddColonist(Men colonist) {
			colonists_.Add(colonist);
			colonist.Colony = this;
			colonist.MouseButtonReleased += (o, e) => colonist.IsSelected = !colonist.IsSelected;

			map_.UpdateTimeEvent += colonist.UpdateTime;
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

		private static string MakeDecision(FuzzyHP hp, FuzzyLaziness lazy) {
			string[,] actions = new string[,] { //   lazy.Lazy  lazy.Average  lazy.HardWorking
									/*hp.Dying*/   { "Rest",    "Rest",       "Rest" },
									/*hp.Low*/     { "Rest",    "Rest",       "Work" },
									/*hp.Average*/ { "Rest",    "Work",       "Work" },
									/*hp.High*/    { "Work",    "Work",       "Work" },
									/*hp.Full*/    { "Work",    "Work",       "Work" },
											  };

			(float max, string state) = new FAM(hp, lazy, actions).MaxValue;

			return state;
		}

		private static string MakeDecision(FuzzyFatigue fatigue, FuzzyLaziness lazy)
		{
			string[,] actions = new string[,] {//  lazy.Lazy  lazy.Average  lazy.HardWorking
					   /*fatigue.Tired*/		 { "Sleep",   "Sleep",      "DoSth" },
					   /*fatigue.Normal*/        { "Sleep",   "DoSth",      "DoSth" },
					   /*fatigue.FullOfEnergy*/  { "DoSth",   "DoSth",      "DoSth" },
											  };

			(float max, string state) = new FAM(fatigue, lazy, actions).MaxValue;

			return state;
		}

		public FastPriorityQueue<Job> JobQueue { get; private set; }

		private readonly List<Construct> constructs_;

		private readonly List<Men> colonists_;

		private readonly Map.Map map_;
		private readonly RenderWindow window_;

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>Returns an enumerator that iterates through the collection.</summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		public IEnumerator<Men> GetEnumerator() => colonists_.GetEnumerator();
	}
}