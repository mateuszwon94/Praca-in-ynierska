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
using PracaInzynierska.Map;
using SFML.Window;
using static PracaInzynierska.Textures.MapTextures;

namespace PracaInzynierska.Utils {
	/// <summary>
	/// Klasa odpowiadajaca za kolonie i kolonistow
	/// </summary>
	public class Colony : Drawable, IEnumerable<Men>, IUpdateTime {
		/// <summary>
		/// Konstruktor koloni
		/// </summary>
		/// <param name="mapField">Pole wokol ktorego maja byc utworzeni pierwsi ludzie</param>
		/// <param name="map">Mapa na ktorej tworzona jest kolonia</param>
		/// <param name="window">Okno na ktorym yświetlana bedzie kolonia</param>
		public Colony(MapField mapField, Map.Map map, RenderWindow window)
		{
			colonists_ = new List<Men>();
			constructs_ = new List<Construct>();
			JobQueue = new FastPriorityQueue<Job>(5_000);

			map_ = map;
			window_ = window;
			centerField_ = mapField;
		}

		/// <summary>
		/// Funkcja wywoływana przy kazdym odswierzeniu okranu
		/// </summary>
		/// <param name="sender">Obiekt wysylajacy zdazenie</param>
		/// <param name="e">Argumenty zdarzenia</param>
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
				string stateSleep = MakeDecision(colonist.RestF, colonist.Laziness);
				string stateWork = MakeDecision(colonist.HP, colonist.Laziness);

				if ( stateSleep == "Sleep" ) {
					Bed bed = Constructs.Where(c => c is Bed)
										.Cast<Bed>()
										.FirstOrDefault(b => b.State == Construct.Status.Done && b.IsFree);

					colonist.Job = new SleepJob(colonist, bed);
				} else if ( stateWork == "Work" && JobQueue.Count > 0 ) {
					colonist.Job = JobQueue.Dequeue();
				} else if ( stateWork == "RestF" || colonist.HP != colonist.HP.MaxHP ) {
					colonist.Job = colonist.Rest;
				}

			}
		}

		/// <summary>
		/// Kolonisci w koloni
		/// </summary>
		public IList<Men> Colonist => colonists_;

		/// <summary>
		/// Budynki w koloni
		/// </summary>
		public IList<Construct> Constructs => constructs_;

		/// <summary>
		/// Funkcja dodajaca do kooni nowego czlona
		/// </summary>
		/// <param name="colonist">Kolonista jaki ma zostac dodany</param>
		public void AddColonist(Men colonist) {
			colonists_.Add(colonist);
			colonist.Colony = this;

			if ( colonist.Location == null ) {
				MapField field = null;
				while (field == null || !field.IsAvaliable ) { field = centerField_.Neighbour[rand_.Next(5), rand_.Next(5)]; }

				colonist.Location = field;
			}

			colonist.TextureSelected = new Sprite(MenTextureSelected);
			colonist.TextureNotSelected = new Sprite(MenTexture);

			// Dodanie do odpowiednich eventow
			map_.UpdateTimeEvent += colonist.UpdateTime;
			window_.KeyPressed += colonist.Window_KeyPressed;
			window_.KeyReleased += colonist.Window_KeyReleased;
			window_.MouseButtonPressed += colonist.Window_MouseButtonPressed;
			window_.MouseButtonReleased += colonist.Window_MouseButtonReleased;
		}

		/// <summary>
		/// Dodanie budynku do kolonii
		/// </summary>
		/// <param name="construct"></param>
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
									/*hp.Dying*/   { "RestF",    "RestF",       "RestF" },
									/*hp.Low*/     { "RestF",    "RestF",       "Work" },
									/*hp.Average*/ { "RestF",    "Work",       "Work" },
									/*hp.High*/    { "Work",    "Work",       "Work" },
									/*hp.Full*/    { "Work",    "Work",       "Work" },
											  };

			(float max, string state) = new FAM(hp, lazy, actions).MaxValue;

			return state;
		}

		private static string MakeDecision(FuzzyRest rest, FuzzyLaziness lazy)
		{
			string[,] actions = new string[,] {//  lazy.Lazy  lazy.Average  lazy.HardWorking
					   /*rest.Tired*/		 { "Sleep",   "Sleep",      "DoSth" },
					   /*rest.Normal*/        { "Sleep",   "DoSth",      "DoSth" },
					   /*rest.FullOfEnergy*/  { "DoSth",   "DoSth",      "DoSth" },
											  };

			(float max, string state) = new FAM(rest, lazy, actions).MaxValue;

			return state;
		}

		/// <summary>
		/// Kolejka zadań do wykonania
		/// </summary>
		public FastPriorityQueue<Job> JobQueue { get; private set; }

		private readonly List<Construct> constructs_;

		private readonly List<Men> colonists_;

		private readonly Map.Map map_;
		private readonly RenderWindow window_;
		private MapField centerField_;

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>Returns an enumerator that iterates through the collection.</summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		public IEnumerator<Men> GetEnumerator() => colonists_.GetEnumerator();

		private static readonly Random rand_ = new Random();
	}
}