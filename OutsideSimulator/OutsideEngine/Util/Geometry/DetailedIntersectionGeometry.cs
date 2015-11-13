using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SlimDX;

namespace OutsideEngine.Util.Geometry
{
    /// <summary>
    /// Extends IntersectionGeometry interface with additional methods
    ///  to determine the actual intersection points of various intersection
    ///  types.
    /// </summary>
    public interface DetailedIntersectionGeometry : IntersectionGeometry
    {
        /// <summary>
        /// True if the given geometry intersects with the ray specified
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="intersectionPoint">Point at which the intersection reported occurs - not modified if no intersection is present</param>
        /// <returns></returns>
        bool intersectsWith(Ray ray, ref Vector3 intersectionPoint);

        /// <summary>
        /// True if the given geometry intersects with a line
        /// </summary>
        /// <param name="line"></param>
        /// <param name="intersectionPoint">Point at which the intersection reported occurs - not modified if no intersection is present</param>
        /// <returns></returns>
        bool intersectsWith(Line line, ref Vector3 intersectionPoint);
    }
}
