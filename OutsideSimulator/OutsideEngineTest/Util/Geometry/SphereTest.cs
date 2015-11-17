using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OutsideEngine.Util.Geometry;

using SlimDX;

using Ray = OutsideEngine.Util.Geometry.Ray;
using static OutsideEngine.Util.MathF;

namespace OutsideEngineTest.Util.Geometry
{
    [TestClass]
    public class SphereTest
    {
        [TestMethod]
        public void ConstructionTest()
        {
            Sphere sp = new Sphere(new Vector3(1.0f, 2.0f, 3.0f), 1.0f);

            Assert.AreEqual(new Vector3(1.0f, 2.0f, 3.0f), sp.Origin, "Sphere origin after constrution should be [1, 2, 3]");
            Assert.AreEqual(1.0f, sp.Radius);

            try
            {
                sp.Radius = -1.0f;
                Assert.Fail("Assignment of -1 to sphere radius should cause exception to be thrown");
            }
            catch (InvalidAssignmentException iax)
            {
                Assert.AreEqual(nameof(sp.Radius), iax.ParamName, "InvalidAssignmentException thrown with Radius < 0 should have ParamName of " + nameof(sp.Radius));
            }
            finally
            {
                Assert.AreEqual(1.0f, sp.Radius, "Radius of sphere should not change on invalid assignment");
            }
        }

        [TestMethod]
        public void RayIntersectionDetailedTest()
        {
            Sphere unitOriginSphere = new Sphere(Vector3.Zero, 1.0f);
            Sphere positiveXSphere = new Sphere(Vector3.UnitX * 10.0f, 5.0f);

            //
            // Unit intersection tests
            //
            // Test one: ray along positive x axis from negative x direction
            Ray tr1 = new Ray(Vector3.UnitX * -5.0f, Vector3.UnitX);
            Vector3 c1 = Vector3.Zero;
            Assert.IsTrue(unitOriginSphere.intersectsWith(tr1, ref c1), "Unit X ray from negative X origin should intersect unit origin sphere");
            Assert.AreEqual(Vector3.UnitX * -1.0f, c1, "Collision point should be at [-1, 0, 0]");

            // Test two: ray along negative y axis from positive y direction
            Ray tr2 = new Ray(Vector3.UnitY * 5.0f, Vector3.UnitY * -1.0f);
            Vector3 c2 = Vector3.Zero;
            Assert.IsTrue(unitOriginSphere.intersectsWith(tr2, ref c2), "Negative unit Y ray from positive Y origin should intersect unit origin sphere");
            Assert.AreEqual(Vector3.UnitY * 1.0f, c2, "Collision point should be at [0, 1, 0]");

            // Test three: Ray along positive z axis from negative z direction
            Ray tr3 = new Ray(Vector3.UnitZ * -5.0f, Vector3.UnitZ);
            Vector3 c3 = Vector3.Zero;
            Assert.IsTrue(unitOriginSphere.intersectsWith(tr3, ref c3), "Unit Z ray from negative Z origin should intersect unit origin sphere");
            Assert.AreEqual(Vector3.UnitZ * -1.0f, c3, "Collision point should be at [0, 0, -1]");

            //
            // Direct miss tests
            //
            // Near miss
            Ray tr4 = new Ray(new Vector3(0.0f, 5.1f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f));
            Vector3 c4 = Vector3.Zero;
            Assert.IsFalse(positiveXSphere.intersectsWith(tr4, ref c4), "A close miss of a ray with a sphere should yield no collision");
            Assert.AreEqual(Vector3.Zero, c4, "Output array should remain zero vector (assuming input zero vector) after a failed near miss collision");

            // Orthogonal miss
            Ray tr5 = new Ray(new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
            Vector3 c5 = Vector3.Zero;
            Assert.IsFalse(positiveXSphere.intersectsWith(tr5, ref c5), "An orthogonal miss of a ray with a sphere should yield no collision");
            Assert.AreEqual(Vector3.Zero, c5, "Output array should remain zero vector (assuming input zero vector) after a failed orthogonal collision");

            // Wrong direction miss
            Ray tr6 = new Ray(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(-1.0f, 0.0f, 0.0f));
            Vector3 c6 = Vector3.Zero;
            Assert.IsFalse(positiveXSphere.intersectsWith(tr6, ref c6), "An opposite direction miss of a ray with a sphere should yield no collision");
            Assert.AreEqual(Vector3.Zero, c6, "Output array should remain zero vector (assuming input zero vector) after a failed wrong direction collision");

            //
            // Barely hit test
            //
            Ray tr7 = new Ray(new Vector3(0.0f, 5.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f));
            Vector3 c7 = Vector3.Zero;
            Assert.IsTrue(positiveXSphere.intersectsWith(tr7, ref c7), "A collision with a sphere with a barely intersects point should yield a collision");
            Assert.AreEqual(new Vector3(10.0f, 5.0f, 0.0f), c7, "The collision point of a radius 5 sphere with origin [10, 0, 0] on a ray going along the y=5 axis should be [10, 5, 0])");

            //
            // Ray origin on edge of sphere test
            //
            Ray tr8 = new Ray(new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f));
            Vector3 c8 = Vector3.Zero;
            Assert.IsTrue(unitOriginSphere.intersectsWith(tr8, ref c8), "A collision with a sphere where the ray begins immediately on the sphere should yield a collision");
            Assert.AreEqual(new Vector3(-1.0f, 0.0f, 0.0f), c8, "The collision point of a ray begining on the sphere should be at the origin point of the sphere");

            //
            // Ray origin inside sphere test
            //
            Ray tr9 = new Ray(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f));
            Vector3 c9 = Vector3.Zero;
            Assert.IsTrue(unitOriginSphere.intersectsWith(tr9, ref c9), "A collision with a sphere from a ray based inside the sphere should yield a collision");
            Assert.AreEqual(Vector3.UnitX, c9, "The collision point of an intersection with a ray based inside a sphere should be on the closest edge");
        }

        [TestMethod]
        public void RayIntersectionTest()
        {
            Sphere unitOriginSphere = new Sphere(Vector3.Zero, 1.0f);
            Sphere positiveXSphere = new Sphere(Vector3.UnitX * 10.0f, 5.0f);

            //
            // Unit intersection tests
            //
            // Test one: ray along positive x axis from negative x direction
            Ray tr1 = new Ray(Vector3.UnitX * -5.0f, Vector3.UnitX);
            Assert.IsTrue(unitOriginSphere.intersectsWith(tr1), "Unit X ray from negative X origin should intersect unit origin sphere");

            // Test two: ray along negative y axis from positive y direction
            Ray tr2 = new Ray(Vector3.UnitY * 5.0f, Vector3.UnitY * -1.0f);
            Assert.IsTrue(unitOriginSphere.intersectsWith(tr2), "Negative unit Y ray from positive Y origin should intersect unit origin sphere");

            // Test three: Ray along positive z axis from negative z direction
            Ray tr3 = new Ray(Vector3.UnitZ * -5.0f, Vector3.UnitZ);
            Assert.IsTrue(unitOriginSphere.intersectsWith(tr3), "Unit Z ray from negative Z origin should intersect unit origin sphere");

            //
            // Direct miss tests
            //
            // Near miss
            Ray tr4 = new Ray(new Vector3(0.0f, 5.1f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f));
            Assert.IsFalse(positiveXSphere.intersectsWith(tr4), "A close miss of a ray with a sphere should yield no collision");

            // Orthogonal miss
            Ray tr5 = new Ray(new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
            Assert.IsFalse(positiveXSphere.intersectsWith(tr5), "An orthogonal miss of a ray with a sphere should yield no collision");

            // Wrong direction miss
            Ray tr6 = new Ray(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(-1.0f, 0.0f, 0.0f));
            Assert.IsFalse(positiveXSphere.intersectsWith(tr6), "An opposite direction miss of a ray with a sphere should yield no collision");

            //
            // Barely hit test
            //
            Ray tr7 = new Ray(new Vector3(0.0f, 5.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f));
            Assert.IsTrue(positiveXSphere.intersectsWith(tr7), "A collision with a sphere with a barely intersects point should yield a collision");

            //
            // Ray origin on edge of sphere test
            //
            Ray tr8 = new Ray(new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f));
            Assert.IsTrue(unitOriginSphere.intersectsWith(tr8), "A collision with a sphere where the ray begins immediately on the sphere should yield a collision");

            //
            // Ray origin inside sphere test
            //
            Ray tr9 = new Ray(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f));
            Assert.IsTrue(unitOriginSphere.intersectsWith(tr9), "A collision with a sphere from a ray based inside the sphere should yield a collision");
        }

        [TestMethod]
        public void PointIntersectionTest()
        {
            Sphere unitOriginSphere = new Sphere(Vector3.Zero, 1.0f);
            Sphere outerSphere = new Sphere(new Vector3(20.0f, 50.0f, 100.0f), 25.0f);

            // Origin test
            Assert.IsTrue(unitOriginSphere.intersectsWith(Vector3.Zero), "Unit origin sphere should contain origin point");
            Assert.IsTrue(outerSphere.intersectsWith(new Vector3(20.0f, 50.0f, 100.0f)), "Sphere placed at [20, 50, 100] with radius 25 should contain point [20, 50, 100]");

            // Small inner values tests
            Assert.IsTrue(unitOriginSphere.intersectsWith(new Vector3(0.1f, 0.2f, 0.3f)), "Unit origin sphere should contain [0.1, 0.2, 0.3]");
            Assert.IsTrue(outerSphere.intersectsWith(new Vector3(44.0f, 50.0f, 101.0f)), "Sphere placed at [20, 50, 100] with radius 25 should contain point [44, 50, 101]");

            // Close miss tests
            Assert.IsFalse(unitOriginSphere.intersectsWith(new Vector3(1.1f, 0.0f, 0.0f)), "Unit origin sphere should not contain [1.1, 0, 0]");
            Assert.IsFalse(outerSphere.intersectsWith(new Vector3(45.1f, 50.0f, 100.0f)), "Sphere placed at [20, 50, 100] with radius 25 should not contain point [45.1, 50, 100]");

            // Far miss tests
            Assert.IsFalse(unitOriginSphere.intersectsWith(new Vector3(100.0f, 100.0f, 213.0f)), "Unit origin sphere should not contain [100, 100, 213]");
            Assert.IsFalse(outerSphere.intersectsWith(Vector3.Zero), "Sphere placed at [20, 50, 100] with radius 25 should not contain point [0, 0, 0]");

            // Edge tests
            Assert.IsTrue(unitOriginSphere.intersectsWith(Vector3.UnitZ), "Unit origin sphere should contain [0, 0, 1]");
            Assert.IsTrue(outerSphere.intersectsWith(new Vector3(45.0f, 50.0f, 100.0f)), "Sphere placed at [20, 50, 100] with radius 25 should contain point [45, 50, 100]");
        }

        [TestMethod]
        public void LineIntersectionTest()
        {
            Sphere unitSphere = new Sphere(Vector3.Zero, 1.0f);
            Sphere outSphere = new Sphere(new Vector3(1.0f, 2.0f, 3.0f), 3.0f);

            // Intersection with external line
            Assert.IsTrue(unitSphere.intersectsWith(new Line(new Vector3(-20.0f, 0.0f, 0.0f), new Vector3(-10.0f, 0.0f, 0.0f))),
                "Unit sphere should intersect with line along x axis");
            Assert.IsTrue(outSphere.intersectsWith(new Line(new Vector3(-0.1f, -0.2f, -0.3f), new Vector3(-0.2f, -0.4f, -0.6f))),
                "Sphere at [1, 2, 3] with radius 3 should intersect with line going through points [-0.1, -0.2, -0.3] and [-0.2, -0.4, -0.6]");

            // Close miss
            Assert.IsFalse(unitSphere.intersectsWith(new Line(new Vector3(-20.0f, 1.1f, 0.0f), new Vector3(-10.0f, 1.1f, 0.0f))),
                "Unit sphere should not intersect with line going along [-20, 1.1, 0] to [-10, 1.1, 0]");

            // Edge touch
            Assert.IsTrue(unitSphere.intersectsWith(new Line(new Vector3(-20.0f, 1.0f, 0.0f), new Vector3(-10.0f, 1.0f, 0.0f))),
                "Unit sphere should intersect with line going through [-20, 1, 0] to [-10, 1, 0]");

            // Control point within sphere
            Assert.IsTrue(unitSphere.intersectsWith(new Line(Vector3.Zero, new Vector3(1.0f, 0.0f, 0.0f))),
                "Unit sphere should intersect with line with control point on origin");
        }

        [TestMethod]
        public void LineIntersectionDetailedTest()
        {
            Sphere unitSphere = new Sphere(Vector3.Zero, 1.0f);
            Sphere outSphere = new Sphere(new Vector3(1.0f, 1.0f, 0.0f), 1.0f);

            // Intersection with external line
            Vector3 outVector = Vector3.Zero;
            Assert.IsTrue(unitSphere.intersectsWith(new Line(new Vector3(-20.0f, 0.0f, 0.0f), new Vector3(-10.0f, 0.0f, 0.0f)), ref outVector),
                "Unit sphere should intersect with line along x axis");
            Assert.AreEqual(new Vector3(-1.0f, 0.0f, 0.0f), outVector, "Unit sphere intersection should be [-1, 0, 0]");
            outVector = Vector3.Zero;

            Assert.IsTrue(outSphere.intersectsWith(new Line(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.5f, 0.5f, 0.0f)), ref outVector),
                "Sphere at [1, 1, 0] with radius 1 should intersect with line going through [0, 0, 0] and [0.5, 0.5, 0]");
            Assert.AreEqual(new Vector3(1.0f - Sqrt2 / 2.0f, 1.0f - Sqrt2 / 2.0f, 0.0f), outVector,
                "Sphere at [1, 1, 0] with radius 1 should intersect with line going through [0, 0, 0] and [0.5, 0.5, 0] at point [1 - 1/sqrt(2), 1 - 1/sqrt(2), 0]");
            outVector = Vector3.Zero;

            // Close miss
            Assert.IsFalse(unitSphere.intersectsWith(new Line(new Vector3(-20.0f, 1.1f, 0.0f), new Vector3(-10.0f, 1.1f, 0.0f)), ref outVector),
                "Unit sphere should not intersect with line going along [-20, 1.1, 0] to [-10, 1.1, 0]");
            Assert.AreEqual(Vector3.Zero, outVector, "Collision point should be unchanged [0, 0, 0] if collision did not occur");
            outVector = Vector3.Zero;

            // Edge touch
            Assert.IsTrue(unitSphere.intersectsWith(new Line(new Vector3(-20.0f, 1.0f, 0.0f), new Vector3(-10.0f, 1.0f, 0.0f)), ref outVector),
                "Unit sphere should intersect with line going through [-20, 1, 0] to [-10, 1, 0]");
            Assert.AreEqual(new Vector3(0.0f, 1.0f, 0.0f), outVector, "Collision point with unit sphere and line crossing [0, 1, 0] should be [0, 1, 0]");
            outVector = Vector3.Zero;

            // Control point within sphere
            Assert.IsTrue(unitSphere.intersectsWith(new Line(new Vector3(0.1f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f)), ref outVector),
                "Unit sphere should intersect with line with control point on origin");
            Assert.AreEqual(new Vector3(1.0f, 0.0f, 0.0f), outVector, "Unit sphere should intersect at [1, 0, 0] if first collision point is inside sphere");
            outVector = Vector3.Zero;
        }
    }
}
