using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using PracaInzynierska.Beeings;
using PracaInzynierska.Events;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Map;
using PracaInzynierska.Utils.FuzzyLogic;
using PracaInzynierska.Utils.FuzzyLogic.Variables;
using PracaInzynierska.Utils.Interfaces;
using SFML.Graphics;
using static PracaInzynierska.Textures.MapTextures;

namespace PracaInzynierska.Utils {
	public class Besiegers : Drawable, IUpdateTime {
		public Besiegers(Map.Map map, Colony colony) {
			map_ = map;
			colony_ = colony;
			counter_ = new TimeSpan(0, rand_.Next(2), rand_.Next(60));
			besigers_ = new List<Men>();
		}

		public void RemoveBesiger(Men besiger) {
			besigersToRemove_.Add(besiger);
			map_.UpdateTimeEvent += besiger.UpdateTime;
		}

		/// <summary>
		/// Draw the object to a render target
		/// This is a function that has to be implemented by the
		/// derived class to define how the drawable should be drawn.
		/// </summary>
		/// <param name="target">Render target to draw to</param>
		/// <param name="states">Current render states</param>
		public void Draw(RenderTarget target, RenderStates states) {
			foreach ( Men besiger in besigers_ ) { besiger.Draw(target, states); }
		}

		public void UpdateTime(object sender, UpdateEventArgs e) {
			Console.WriteLine($"{State} \t {counter_:g} \t \t {preparationCounter_:g}");

			for ( int index = 0 ; index < besigers_.Count ; ) {
				if ( besigers_[index].HP.Value > 0f ) {
					index++;
					continue;
				}

				--menStillNotFleeing_;
				besigers_[index].Die();
				besigers_.Remove(besigers_[index]);

				foreach (Men other in besigers_) { other.Morale.Value -= 5f; }
			}


			if ( besigersToRemove_.Count != 0 ) {
				foreach ( Men besiger in besigersToRemove_ ) {
					besiger.Die();
					besigers_.Remove(besiger);
				}

				besigersToRemove_.Clear();
				State = Status.None;
			}

			if ( State == Status.Spawning ) {
				menStillNotFleeing_ = menAttacking_ = rand_.Next(3, 11);
				MapField location = null;
				while ( location == null || !location.IsAvaliable ||
						colony_.Any(colonist => MapField.Distance(location, colonist.Location) <= 0.4f * map_.Size) ) {
					location = map_[rand_.Next(map_.Size), rand_.Next(map_.Size)];
				}

				for ( int i = 0 ; i < menAttacking_; i++ ) {
					MapField currentField = null;
					while ( currentField == null || !currentField.IsAvaliable ) {
						try { currentField = location.Neighbour[rand_.Next(-7, 7), rand_.Next(-7, 7)]; }
						catch (NoSouchNeighbourException ) { }
					}

					besigers_.Add(new Men() {
												Name = "Adam",
												MoveSpeed = 5,
												Location = currentField,
												TextureSelected = new Sprite(BesigerTexture),
												TextureNotSelected = new Sprite(BesigerTexture),
												IsSelected = false,
												HP = new FuzzyHP(50f, 50f),
												Laziness = new FuzzyLaziness((float)rand_.NextDouble() * 10f),
												Fatigue = new FuzzyFatigue((float)rand_.NextDouble() * 10f),
												Strength = (float)rand_.NextDouble() * 10f,
												Morale = new FuzzyMorale(7.5f),
												Mining = (float)rand_.NextDouble() * 10f,
												Constructing = (float)rand_.NextDouble() * 10f,
											});
				}

				foreach ( Men besiger in besigers_ ) { map_.UpdateTimeEvent += besiger.UpdateTime; }

				State = Status.Preparation;
			} else if ( State == Status.Preparation )
			{
				if (preparationCounter_ == null) preparationCounter_ = new TimeSpan(0, 0, rand_.Next(60));
				preparationCounter_ -= e.Elapsed;

				if ( preparationCounter_ <= TimeSpan.Zero) {
					preparationCounter_ = null;
					State = Status.Attack;
				}
			} else if ( State == Status.Attack ) {
				if (menStillNotFleeing_ <= 2) {
					State = Status.Flee;
					return;
				}

				foreach ( Men besiger in besigers_ ) {
					string stateFleeing = MakeDecision(besiger.HP, besiger.Morale);

					switch ( stateFleeing ) {
						case "Flee":
							if ( besiger.Job is Men.FleeJob ) continue;

							besiger.Job = new Men.FleeJob(besiger, map_, colony_, this);
							--menStillNotFleeing_;
							foreach ( Men other in besigers_.Where(b => b != besiger) ) { other.Morale.Value -= 2f; }

							break;
						case "Atack":
							if ( besiger.Job is Men.AttackJob ) continue;

							besiger.Job = colony_.Colonist[rand_.Next(colony_.Colonist.Count)].AttackThis;
							break;
					}
				}
			}
			else if ( State == Status.Flee ) {
				foreach ( Men besiger in besigers_.Where(besiger => besiger.Job == null || besiger.Job.GetType() != typeof(Men.FleeJob)) ) {
					besiger.Job = new Men.FleeJob(besiger, map_, colony_, this);
				}
			} else if ( counter_ <= TimeSpan.Zero ) {
				State = Status.Spawning;
				counter_ = new TimeSpan(0, rand_.Next(2), rand_.Next(60));
			} else if ( State == Status.None ) counter_ -= e.Elapsed;
		}

		private static string MakeDecision(FuzzyHP hp, FuzzyMorale morale) {
			string[,] actions = new string[,] { //  morale.broken  morale.low  morale.average  morale.high  morale.full
								 /*hp.Dying*/     { "Flee",        "Flee",     "Flee",         "Flee",      "Flee"},
								 /*hp.Low*/       { "Flee",        "Flee",     "Flee",         "Flee",      "Atack"} ,
								 /*hp.Average*/   { "Flee",        "Flee",     "Flee",         "Atack",     "Atack"},
								 /*hp.High*/      { "Flee",        "Flee",     "Atack",        "Atack",     "Atack"},
								 /*hp.Full*/      { "Flee",        "Atack",    "Atack",        "Atack",     "Atack"},
											  };

			(float max, string state) = new FAM(hp, morale, actions).MaxValue;

			return state;
		}

		public Status State { get; private set; } = Status.None;

		public enum Status {
			None = 0,
			Spawning = 1,
			Preparation  = 2,
			Attack = 3,
			Flee = 4
		}

		private readonly Map.Map map_;
		private readonly Colony colony_;
		private TimeSpan counter_;
		private TimeSpan? preparationCounter_;
		private static Random rand_ = new Random();
		private readonly List<Men> besigers_;
		private readonly List<Men> besigersToRemove_ = new List<Men>();
		private int menAttacking_;
		private int menStillNotFleeing_;
	}
}