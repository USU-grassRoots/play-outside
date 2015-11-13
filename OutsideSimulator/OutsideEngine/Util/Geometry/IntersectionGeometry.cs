using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SlimDX;

namespace OutsideEngine.Util.Geometry
{
    /// <summary>
    /// Interface for use in any gemoetry that may be intersected. There are a few useful
    ///  types against which we will certainly want to test intersection (point, line, ray)
    ///  and then also a general method to determine if the IntersectionGeometry object
    ///  intersects another one.
    /// </summary>
    public interface IntersectionGeometry
    {
        /// <summary>
        /// True if the given geometry intersects with the ray specified
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        bool intersectsWith(Ray ray);

        /// <summary>
        /// True if the given geometry contains a point specified
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        bool intersectsWith(Vector3 point);

        /// <summary>
        /// True if the given geometry intersects with a line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        bool intersectsWith(Line line);
    }
}
