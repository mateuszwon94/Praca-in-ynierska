using System;
using System.Collections.Generic;
using System.Linq;
using PracaInzynierska.Beeings;
using PracaInzynierska.Constructs;
using PracaInzynierska.Utils;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static PracaInzynierska.Textures.GUITextures;

namespace PracaInzynierska.UserInterface.Controls {
	public class ColonySurface : GUIElement {
		public ColonySurface(Colony colony, Font font, Window window) {
			colony_ = colony;
			font_ = font;
			offset_ = 15f;



			texts_ = new List<List<Text>>(colony_.Colonist.Count) {
																	  new List<Text>(5) {
																						   new Text("Name", font_, 20),
																						   new Text("HP", font_, 20),
																						   new Text("Wypoczęcie", font_, 20),
																						   new Text("Morale", font_, 20),
																						   new Text("Praca", font_, 20)
																					   }
																  };

			foreach ( Men colonist in colony_.Colonist ) {
				texts_.Add(
						   new List<Text>(5) {
												 new Text($"{colonist.Name}", font_, 20),
												 new Text("50,00", font_, 20),
												 new Text("10,00", font_, 20),
												 new Text("-10,00", font_, 20),
												 new Text("Odpoczywa", font_, 20)
											 }
						  );
			}

			float maxWidth = 0f;
			float maxHeight = 0f;
			foreach ( FloatRect bounds in texts_.SelectMany(textsList => textsList)
												.Select(text => text.GetLocalBounds()) ) {
				if ( maxWidth < bounds.Width ) maxWidth = bounds.Width;
				if ( maxHeight < bounds.Height ) maxHeight = bounds.Height;
			}

			ScreenSize = new Vector2u((uint)((maxWidth + offset_ / 2) * 5 + offset_ * 2),
									  (uint)(maxHeight * texts_.Count + offset_ * 2 + offset_ / 2 * texts_.Count));

			Texture = new Sprite(ButtonTexture(ScreenSize.Y, ScreenSize.X, Color.White));
			Position = new Vector2f(window.Size.X - ScreenSize.X - offset_, window.Size.Y - ScreenSize.Y - offset_);

			float x = Position.X + offset_;
			float y = Position.Y + offset_;
			foreach ( List<Text> textsList in texts_ ) {

				foreach ( Text text in textsList ) {
					text.Color = Color.Black;

					text.Position = new Vector2f(x, y);

					x += maxWidth + offset_ / 2;
				}

				x = Position.X + offset_;
				y += maxHeight + offset_ / 2;
			}
		}

		protected override void OnDraw(RenderTarget target, RenderStates states) {
			UpdateTexts();
			target.Draw(Texture, states);
			foreach ( Text text in texts_.SelectMany(textsList => textsList) ) {
				target.Draw(text, states);
			}
		}

		private void UpdateTexts() {
			for ( int i = 0 ; i < colony_.Colonist.Count ; i++ ) {
				Men colonist = colony_.Colonist[i];

				texts_[i + 1][0].DisplayedString = $"{colonist.Name}";
				texts_[i + 1][1].DisplayedString = $"{colonist.HP.Value:F2}";
				texts_[i + 1][2].DisplayedString = $"{colonist.RestF.Value:F2}";
				texts_[i + 1][3].DisplayedString = $"{colonist.Morale.Value:F2}";

				string jobS = string.Empty;
				if (colonist.Job != null) {
					if ( colonist.Job.GetType() == typeof(Men.AttackJob) )                  { jobS += "Atakuje"; }
					else if ( colonist.Job.GetType() == typeof(Men.FleeJob) )               { jobS += "Ucieka"; }
					else if ( colonist.Job.GetType() == typeof(Men.RestJob) )               { jobS += "Odpoczywa"; }
					else if ( colonist.Job.GetType() == typeof(Construct.ConstructingJob) ) { jobS += "Buduje"; }
				}

				texts_[i + 1][4].DisplayedString = jobS;
			}
		}

		public Sprite Texture { get; private set; }

		/// <summary>
		/// Pozycja na ekranie
		/// </summary>
		public sealed override Vector2f Position {
			get { return Texture.Position; }
			set { Texture.Position = value; }
		}

		/// <summary>
		/// Rozmiar elementu
		/// </summary>
		public sealed override Vector2u ScreenSize { get; }

		private float offset_;

		private List<List<Text>> texts_;
		private Colony colony_;
		private Font font_;
	}
}