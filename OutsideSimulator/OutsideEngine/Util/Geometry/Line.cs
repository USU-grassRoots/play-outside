using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SlimDX;

namespace OutsideEngine.Util.Geometry
{
    /// <summary>
    /// Defines a one dimensional line in three dimensional space.
    /// A line may be defined by any two points.
    /// Lines are frequently used in testing intersection of geometry, though they may have
    ///  strictly logical applications as well.
    /// </summary>
    public struct Line
    {
        /// <summary>
        /// First point which defines this line
        /// </summary>
        public Vector3 Point1;

        /// <summary>
        /// Second point which defines this line
        /// </summary>
        public Vector3 Point2;

        /// <summary>
        /// Get a ray from the first point that points in the direction of the second point
        /// </summary>
        /// <returns></returns>
        public Ray getInnerRay()
        {
            return new Ray(Point1, Point1 - Point2);
        }

        /// <summary>
        /// Get a ray from the first point that points in the direction opposite the second point
        /// </summary>
        /// <returns></returns>
        public Ray getOuterRay()
        {
            return new Ray(Point1, Point2 - Point1);
        }

        /// <summary>
        /// Switch which point is considered Point1 and which is considered Point2
        /// May be useful if the ordering is important (such as for the getInnerRay and getOuterRay methods)
        /// </summary>
        public void SwapPoints()
        {
            Vector3 tmp = Point1;
            Point1 = Point2;
            Point1 = tmp;
        }

        /// <summary>
        /// Creates a line between the two points specified.
        /// </summary>
        /// <param name="anchorPoint">Primary point on the line (in computing rays, this is the anchor)</param>
        /// <param name="otherPoint">Secondary point on the line</param>
        public Line(Vector3 anchorPoint, Vector3 otherPoint)
        {
            Point1 = anchorPoint;
            Point2 = otherPoint;
        }
    }
}
