using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracaInzynierska.Map.Tests;
using PracaInzynierska.UserInterface.Tests;

namespace PracaInzynierska.Tests {
    public class TestMain {
        public static void Main(string[] args) {
            MapTests mapTests = new MapTests();

            mapTests.GetEnumeratorTest();
            mapTests.GoReverseTest();


            GUIElementTests guiElementTests = new GUIElementTests();

            guiElementTests.InsideElementTest();
        }
    }
}
