using Microsoft.VisualStudio.TestTools.UnitTesting;
using PracaInzynierska.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using PracaInzynierska.UserInterface.Controls;
using SFML.Graphics;
using SFML.System;

namespace PracaInzynierska.UserInterface.Tests {
	[TestClass()]
	public class GUIElementTests {
		[TestMethod()]
		public void InsideElementTest() {
			Button button = new Button {
								IsActive = true,
								ButtonTexture = new Sprite(new Texture(20, 20)),
								Position = new Vector2f(20, 20)
							};

			Assert.AreEqual(button.InsideElement(30, 30), true);
			Assert.AreEqual(button.InsideElement(0, 0), false);
			Assert.AreEqual(button.InsideElement(20, 20), true);
			Assert.AreEqual(button.InsideElement(40, 40), false);
		}
	}
}