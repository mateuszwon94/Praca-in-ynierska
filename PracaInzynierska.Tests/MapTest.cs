// <copyright file="MapTest.cs">Copyright ©  2015</copyright>
using System;
using System.Collections.Generic;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PracaInzynierska.Map;

namespace PracaInzynierska.Map.Tests
{
    /// <summary>This class contains parameterized unit tests for Map</summary>
    [PexClass(typeof(global::PracaInzynierska.Map.Map))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class MapTest
    {
        /// <summary>Test stub for GetEnumerator()</summary>
        [PexMethod]
        public IEnumerator<MapField> GetEnumeratorTest([PexAssumeUnderTest]global::PracaInzynierska.Map.Map target)
        {
            IEnumerator<MapField> result = target.GetEnumerator();
            return result;
            // TODO: add assertions to method MapTest.GetEnumeratorTest(Map)
        }
    }
}
