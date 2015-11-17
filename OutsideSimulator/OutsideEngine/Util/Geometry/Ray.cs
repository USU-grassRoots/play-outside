using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SlimDX;

namespace OutsideEngine.Util.Geometry
{
    /// <summary>
    /// Defines a one dimensional ray in three dimensional space.
    /// A ray is composed of a point and direction.
    /// Rays are frequently used in testing intersection of geometry, though they may
    ///  also be used for lighting formulas, etc.
    /// 
    /// The direction of a ray is normalized. The assumption is made that the direction
    ///  will be set less frequently than it will be queried, so normalization occurs on
    ///  setting the direction.
    /// </summary>
    public struct Ray
    {
        /// <summary>
        /// The position of the ray in local 3D space
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The cached value for a vector
        /// </summary>
        private Vector3 _direction;

        /// <summary>
        /// The normalized direction in which the ray points. Automatically normalizes on setting
        /// </summary>
        public Vector3 Direction
        {
            get
            { return _direction; }
            set
            {
                value.Normalize();
                _direction = value;
            }
        }

        /// <summary>
        /// Constructor for a ray. Requires presence of both position and direction.
        /// </summary>
        /// <param name="position">Position in local 3D space of the ray</param>
        /// <param name="direction">Direction in local 3D space of the ray. Will be normalized before being stored.</param>
        public Ray(Vector3 position, Vector3 direction) : this()
        {
            Position = position;
            Direction = direction;
        }
    }
}
