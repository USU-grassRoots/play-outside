using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OutsideEngine;
using Ray = OutsideEngine.Util.Geometry.Ray;

using SlimDX;

namespace OutsideEngineTest.Util.Geometry
{
    [TestClass]
    public class RayTest
    {
        [TestMethod]
        public void RegularConstruction()
        {
            Ray ray = new Ray(new Vector3(4.0f, 5.0f, 6.0f), new Vector3(1.0f, 2.0f, 3.0f));
            Assert.AreEqual(4.0f, ray.Position.X, 0.000001f, "X component of ray position should be (hard-coded) initial value");
            Assert.AreEqual(5.0f, ray.Position.Y, 0.000001f, "Y component of ray position should be (hard-coded) initial value");
            Assert.AreEqual(6.0f, ray.Position.Z, 0.000001f, "Z component of ray position should be (hard-coded) initial value");

            var magnitude = Math.Sqrt(1.0 * 1.0 + 2.0 * 2.0 + 3.0 * 3.0);
            var xExpected = 1.0 / magnitude;
            var yExpected = 2.0 / magnitude;
            var zExpected = 3.0 / magnitude;

            Assert.AreEqual(xExpected, ray.Direction.X, 0.000001f, "X component of ray direction should be normalized from (hard-coded) initial value");
            Assert.AreEqual(yExpected, ray.Direction.Y, 0.000001f, "Y component of ray direction should be normalized from (hard-coded) initial value");
            Assert.AreEqual(zExpected, ray.Direction.Z, 0.000001f, "Z component of ray direction should be normalized from (hard-coded) initial value");
        }

        [TestMethod]
        public void Normalization()
        {
            try
            {
                Ray test = new Ray(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f));
                Assert.Fail("Exception should be thrown when providing a ray with no direction");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(Exception), "TODO: Write a better test (make sure exception is of expected type)");
            }

            Ray okayBetterTest = new Ray(Vector3.Zero, Vector3.UnitX);
            Assert.AreEqual(1.0f, okayBetterTest.Direction.X, 0.000001f, "Normalization of unit X vector should be unit X vector (X component should be 1)");
            Assert.AreEqual(0.0f, okayBetterTest.Direction.Y, 0.000001f, "Normalization of unit X vector should be unit X vector (Y component should be 0)");
            Assert.AreEqual(0.0f, okayBetterTest.Direction.Z, 0.000001f, "Normalization of unit unit X vector should be unit X vector (Z component should be 0)");

            okayBetterTest.Direction = Vector3.UnitY * 2.0f;
            Assert.AreEqual(0.0f, okayBetterTest.Direction.X, 0.0000001f, "Normalization of two times unit Y vector should be unit Y vector (X component should be 0)");
            Assert.AreEqual(1.0f, okayBetterTest.Direction.Y, 0.0000001f, "Normalization of two times unit Y vector should be unit Y vector (Y component should be 1)");
            Assert.AreEqual(0.0f, okayBetterTest.Direction.Z, 0.0000001f, "Normalization of two times unit Y vector should be unit Y vector (Z component should be 0)");

            okayBetterTest.Direction = new Vector3(1.0f, 1.0f, 1.0f);
            var eachSize = 1.0f / Math.Sqrt(1.0f * 1.0f * 3.0f);
            Assert.AreEqual(eachSize, okayBetterTest.Direction.X, 0.0000001f, "Normalization of vector with ones component in each direction should have 1/sqrt(3) magnitude on X component");
            Assert.AreEqual(eachSize, okayBetterTest.Direction.Y, 0.0000001f, "Normalization of vector with ones component in each direction should have 1/sqrt(3) magnitude on Y component");
            Assert.AreEqual(eachSize, okayBetterTest.Direction.Z, 0.0000001f, "Normalization of vector with ones component in each direction should have 1/sqrt(3) magnitude on Z component");

            okayBetterTest.Direction = new Vector3(1.0f, 0.0f, 1.0f);
            eachSize = 1.0f / Math.Sqrt(1.0f * 1.0f * 2.0f);
            Assert.AreEqual(eachSize, okayBetterTest.Direction.X, 0.0000001f, "Normalization of vector with ones component in (X, Z) should have 1/sqrt(2) magnitude on X component");
            Assert.AreEqual(0.0f, okayBetterTest.Direction.Y, 0.0000001f, "Normalization of vector with ones component in (X, Z) should have 0 magnitude on Y component");
            Assert.AreEqual(eachSize, okayBetterTest.Direction.Z, 0.0000001f, "Normalization of vector with ones component in (X, Z) should have 1/sqrt(2) magnitude on Z component");
        }
    }
}
