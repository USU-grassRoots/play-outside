using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

using static OutsideEngine.Util.MathF;

namespace OutsideEngine.Util.Geometry
{
    /// <summary>
    /// Represents a failed assignment to a value that has certain constraints.
    ///  Example: A length or radius must always be positive, throws an InvalidAssignmentException on attempt to set negative
    /// </summary>
    public class InvalidAssignmentException : Exception
    {
        /// <summary>
        /// The parameter which failed assignment
        /// </summary>
        public string ParamName { get; protected set; }

        /// <summary>
        /// Create an InvalidAssignmentException with the given parameter and (optional) message
        /// </summary>
        /// <param name="paramName">Parameter which caused the failed assignment</param>
        /// <param name="message"></param>
        public InvalidAssignmentException(string paramName, string message = "") : base(message)
        {
            ParamName = paramName;
        }
    }

    /// <summary>
    /// Represents a 3D sphere geometry, which occupies space and
    ///  therefore implements collision logic
    /// </summary>
    public class Sphere : Shape, DetailedIntersectionGeometry
    {
        #region Logical members
        /// <summary>
        /// The origin of the sphere in 3D space (the center)
        /// </summary>
        public Vector3 Origin { get; set; }

        /// <summary>
        /// Internal data representation of the radius of the sphere
        /// </summary>
        private float _radius;

        /// <summary>
        /// The radius of the sphere in 3D space (must be positive)
        /// </summary>
        public float Radius
        {
            get { return _radius; }
            set { if (value > 0.0f) _radius = value; else throw new InvalidAssignmentException(nameof(Radius),"Radius of a sphere must be greater than zero"); }
        }
        #endregion

        #region Creation
        /// <summary>
        /// Create a new sphere
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        public Sphere(Vector3 origin, float radius)
        {
            Origin = origin;
            Radius = radius;
        }

        /// <summary>
        /// Unit sphere with position [0, 0, 0] and radius 1
        /// </summary>
        public static Sphere UnitSphere = new Sphere(Vector3.Zero, 1.0f);
        #endregion

        #region DetailedIntersectionGeometry
        /// <summary>
        /// Test if the collision sphere contains the given point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool intersectsWith(Vector3 point) => ((Origin - point).LengthSquared() <= (Radius * Radius));

        /// <summary>
        /// Test if the collision sphere intersects the given line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool intersectsWith(Line line)
        {
            // If p0 is the origin of the sphere, and r is the radius, with p being the base point of the ray
            //  and d the unit direction of the ray...
            // An intersection point [x, y, z] will be found if (x - p0_x)^2 + (y - p0_y)^2 + (z - p0_z)^2 = r^2
            // The points [x, y, z] can be defined as some point p + n * d, for some number n.
            // If the number is positive we have an intersection at that point.
            // ((p_x + nd_x) - p0_x)^2 + ((p_y + nd_y) - p0_y)^2 + ((p_z + nd_z) - p0_z)^2 = r^2
            // Do some algebra and apply the quadratic formula
            Ray ray = line.getInnerRay();
            Vector3 d = ray.Direction;
            Vector3 p = ray.Position;
            Vector3 p0 = Origin;
            float r = Radius;

            float a = 1.0f; // Go figure - it turns out to be (d_x^2 + d_y^2 + d_z^2), the square magnitude of a unit vector... Which is one.
            float b = 2.0f * (d.X * (p.X - p0.X) + d.Y * (p.Y - p0.Y) + d.Z * (p.Z - p0.Z));
            float c = -(r * r) + p0.X * p0.X + p0.Y * p0.Y + p0.Z * p0.Z - 2.0f * (p.X * p0.X + p.Y * p0.Y + p.Z * p0.Z) + p.X * p.X + p.Y * p.Y + p.Z * p.Z;

            float sqrtnum = Sqrt(b * b - 4.0f * a * c);
            // No roots - not a collision
            if (float.IsNaN(sqrtnum))
            {
                return false;
            }

            float sol1 = (-b - sqrtnum) / (2.0f * a);
            float sol2 = (-b + sqrtnum) / (2.0f * a);

            return true;
        }

        /// <summary>
        /// Test if the collision sphere intersects the given ray
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public bool intersectsWith(Ray ray)
        {
            // TODO KAM: Try to use a more efficient ray/sphere collision method
            Vector3 foo = Vector3.Zero;
            return intersectsWith(ray, ref foo);
        }

        /// <summary>
        /// Test if the collision sphere intersects the given line
        /// </summary>
        /// <param name="line">A 2D line in 3D space</param>
        /// <param name="intersectionPoint">The intersection point, if and only if there is an intersection</param>
        /// <returns>True if an intersection occurs, false otherwise</returns>
        public bool intersectsWith(Line line, ref Vector3 intersectionPoint)
        {
            // If p0 is the origin of the sphere, and r is the radius, with p being the base point of the ray
            //  and d the unit direction of the ray...
            // An intersection point [x, y, z] will be found if (x - p0_x)^2 + (y - p0_y)^2 + (z - p0_z)^2 = r^2
            // The points [x, y, z] can be defined as some point p + n * d, for some number n.
            // If the number is positive we have an intersection at that point.
            // ((p_x + nd_x) - p0_x)^2 + ((p_y + nd_y) - p0_y)^2 + ((p_z + nd_z) - p0_z)^2 = r^2
            // Do some algebra and apply the quadratic formula
            Ray ray = line.getInnerRay();
            Vector3 d = ray.Direction;
            Vector3 p = ray.Position;
            Vector3 p0 = Origin;
            float r = Radius;

            float a = 1.0f; // Go figure - it turns out to be (d_x^2 + d_y^2 + d_z^2), the square magnitude of a unit vector... Which is one.
            float b = 2.0f * (d.X * (p.X - p0.X) + d.Y * (p.Y - p0.Y) + d.Z * (p.Z - p0.Z));
            float c = -(r * r) + p0.X * p0.X + p0.Y * p0.Y + p0.Z * p0.Z - 2.0f * (p.X * p0.X + p.Y * p0.Y + p.Z * p0.Z) + p.X * p.X + p.Y * p.Y + p.Z * p.Z;

            float sqrtnum = Sqrt(b * b - 4.0f * a * c);
            // No roots - not a collision
            if (float.IsNaN(sqrtnum))
            {
                return false;
            }

            float sol1 = (-b - sqrtnum) / (2.0f * a);
            float sol2 = (-b + sqrtnum) / (2.0f * a);

            // There are two points... Select the ones closest to the control point of the line
            if (Math.Abs(sol1) < Math.Abs(sol2))
            {
                intersectionPoint = p + d * sol1;
            }
            else
            {
                intersectionPoint = p + d * sol2;
            }

            return true;
        }

        /// <summary>
        /// Test if a collision sphere intersects with the given ray
        /// </summary>
        /// <param name="ray">A ray in 3D space</param>
        /// <param name="intersectionPoint">The intersection point on the surface of the sphere, if one exists</param>
        /// <returns>True if an intersection occurs, false otherwise</returns>
        public bool intersectsWith(Ray ray, ref Vector3 intersectionPoint)
        {
            // If p0 is the origin of the sphere, and r is the radius, with p being the base point of the ray
            //  and d the unit direction of the ray...
            // An intersection point [x, y, z] will be found if (x - p0_x)^2 + (y - p0_y)^2 + (z - p0_z)^2 = r^2
            // The points [x, y, z] can be defined as some point p + n * d, for some number n.
            // If the number is positive we have an intersection at that point.
            // ((p_x + nd_x) - p0_x)^2 + ((p_y + nd_y) - p0_y)^2 + ((p_z + nd_z) - p0_z)^2 = r^2
            // Do some algebra and apply the quadratic formula
            Vector3 d = ray.Direction;
            Vector3 p = ray.Position;
            Vector3 p0 = Origin;
            float r = Radius;

            float a = 1.0f; // Go figure - it turns out to be (d_x^2 + d_y^2 + d_z^2), the square magnitude of a unit vector... Which is one.
            float b = 2.0f * (d.X * (p.X - p0.X) + d.Y * (p.Y - p0.Y) + d.Z * (p.Z - p0.Z));
            float c = -(r * r) + p0.X * p0.X + p0.Y * p0.Y + p0.Z * p0.Z - 2.0f * (p.X * p0.X + p.Y * p0.Y + p.Z * p0.Z) + p.X * p.X + p.Y * p.Y + p.Z * p.Z;

            float sqrtnum = Sqrt(b * b - 4.0f * a * c);
            // No roots - not a collision
            if (float.IsNaN(sqrtnum))
            {
                return false;
            }

            float sol1 = (-b - sqrtnum) / (2.0f * a);
            float sol2 = (-b + sqrtnum) / (2.0f * a);

            if (sol1 < 0.0f)
            {
                if (sol2 < 0.0f)
                {
                    return false;
                }
                else
                {
                    intersectionPoint = p + d * sol2;
                    return true;
                }
            }
            else
            {
                if (sol2 < 0.0f)
                {
                    intersectionPoint = p + d * sol1;
                    return true;
                }
                else
                {
                    intersectionPoint = p + d * Math.Min(sol1, sol2);
                    return true;
                }
            }
        }
        #endregion
    }
}
