using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SlimDX;

using Ray = OutsideEngine.Util.Geometry.Ray;

using static OutsideEngine.Util.MathF;

using OutsideEngine.Util.Geometry;

namespace OutsideEngineTest.Util.Geometry
{
    [TestClass]
    public class LineTest
    {
        [TestMethod]
        public void RegularConstruction()
        {
            Line line = new Line(Vector3.UnitY, Vector3.UnitX);
            Assert.AreEqual(0.0f, line.Point1.X, 0.000001f, "X component of primary point component should be zero");
            Assert.AreEqual(1.0f, line.Point1.Y, 0.000001f, "Y component of primary point component should be one");
            Assert.AreEqual(0.0f, line.Point1.Z, 0.000001f, "Z component of primary point component should be zero");

            Assert.AreEqual(1.0f, line.Point2.X, 0.000001f, "X component of secondary point component should be one");
            Assert.AreEqual(0.0f, line.Point2.Y, 0.000001f, "Y component of secondary point component should be zero");
            Assert.AreEqual(0.0f, line.Point2.Z, 0.000001f, "Z component of secondary point component should be zero");
        }

        [TestMethod]
        public void InnerRayComputation()
        {
            Vector3 v1 = new Vector3(1.0f, 2.0f, 3.0f);
            Vector3 v2 = new Vector3(2.0f, -2.0f, 10.0f);
            Vector3 diff = v1 - v2;
            diff.Normalize();

            Line line = new Line(v1, v2);
            Ray innerRay = line.getInnerRay();
            Ray outerRay = line.getOuterRay();

            float magnitude = diff.Length();

            Assert.AreEqual(diff, innerRay.Direction, "Inner ray direction should be in difference of two vectors");
            Assert.AreEqual(diff * -1.0f, outerRay.Direction, "Inner ray direction should be in difference of two vectors");
            Assert.AreEqual(v1, innerRay.Position, "Inner ray position should be primary point");
            Assert.AreEqual(v1, outerRay.Position, "Outer ray position should be primary point");
        }
    }
}
