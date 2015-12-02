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
        /// <summary>
        /// Constant square root of 2 (used in many calculations)
        /// </summary>
        public static readonly float Sqrt2 = (float)Math.Sqrt(2);

        /// <summary>
        /// Floating point value for PI
        /// </summary>
        public static readonly float PI = (float)Math.PI;

        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns> 
        public static float ToRadians(float degrees) => PI * degrees / 180.0f;

        /// <summary>
        /// Clamp a value to within a certain range - if provided something below said range,
        ///  return the minimum possible value, if above, provide maximum value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp(float value, float min, float max) => Math.Max(min, Math.Min(value, max)); 

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

        /// <summary>
        /// Compute floating point square root of X
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float Sqrt(float x) => (float)Math.Sqrt(x);

        /// <summary>
        /// From an Int32, return the low word (the bottom 16 bits) value
        /// </summary>
        /// <param name="i">Input 32 bit integer</param>
        /// <returns>Bottom 16 bits (2 bytes)</returns>
        public static int LowWord(this int i)
        {
            return i & 0xFFFF;
        }

        /// <summary>
        /// From an Int32, return the high word (top 16 bits) value
        /// </summary>
        /// <param name="i">Input 32 bit integer</param>
        /// <returns>Top 16 bits (2 bytes)</returns>
        public static int HighWord(this int i)
        {
            return (i >> 16) & 0xFFFF;
        }

        /// <summary>
        /// 32-bit floating point version of Sin (all it does is cast the double precision version)
        /// </summary>
        /// <param name="x">Input to the sin function</param>
        /// <returns>Output of the sin function</returns>
        public static float Sin(float x)
        {
            return (float)Math.Sin(x);
        }

        /// <summary>
        /// 32-bit floating point version of Cos (all it does is cast the double precision version)
        /// </summary>
        /// <param name="x">Input to the cos function</param>
        /// <returns>Output of the cos function</returns>
        public static float Cos(float x)
        {
            return (float)Math.Cos(x);
        }
    }
}
