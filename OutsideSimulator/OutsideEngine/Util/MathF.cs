using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutsideEngine.Util
{
    /// <summary>
    /// Thin wrappers around double precision floating point math operations such as sin and cos
    ///  to define them for single precision floating point
    /// </summary>
    public static partial class MathF
    {
        public static readonly float Sqrt2 = (float)Math.Sqrt(2);
        public static readonly float PI = (float)Math.PI;

        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static float ToRadians(float degrees)
        {
            return PI * degrees / 180.0f;
        }

        /// <summary>
        /// Clamp a value to within a certain range - if provided something below said range,
        ///  return the minimum possible value, if above, provide maximum value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        /// <summary>
        /// Normalize an angle in radians to the range 0, 2*PI
        /// </summary>
        /// <param name="angle">Angle in radians to be normalized</param>
        /// <returns></returns>
        public static float NormalizeAngle(float angle)
        {
            while (angle < 0) angle += 2 * PI;
            while (angle >= 2 * PI) angle -= 2 * PI;
            return angle;
        }
    }
}
