using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OutsideEngine.Util
{
    /// <summary>
    /// Handles utility methods for memeory, COM management, etc.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Release a COM object and set its reference equal to null
        ///  This is for safety - so it isn't attempted to be used with unmanaged resources
        /// </summary>
        /// <typeparam name="T">Type of COM object</typeparam>
        /// <param name="x">COM reference</param>
        public static void ReleaseCom<T>(ref T x) where T : class, IDisposable
        {
            x?.Dispose();
            x = null;
        }

        /// <summary>
        /// Convert an ASSIMP Vector3D into a SlimDX Vector3
        /// </summary>
        /// <param name="v">ASSIMP Vector3D</param>
        /// <returns>SlimDX Vector3</returns>
        public static SlimDX.Vector3 ToVector3(this Assimp.Vector3D v)
        {
            return new SlimDX.Vector3(v.X, v.Y, v.Z);
        }

        /// <summary>
        /// Convert an ASSIMP Vector2D into a SlimDX Vector2
        /// </summary>
        /// <param name="v">ASSIMP Vector3D</param>
        /// <returns>SlimDX Vector3</returns>
        public static SlimDX.Vector2 ToVector2(this Assimp.Vector2D v)
        {
            return new SlimDX.Vector2(v.X, v.Y);
        }

        /// <summary>
        /// Convert an ASSIMP material into an OutsideEngine material
        /// </summary>
        /// <param name="m">ASSIMP material</param>
        /// <returns>OutsideEngine material</returns>
        public static OutsideEngine.Rendering.Lighting.Material ToMaterial(this Assimp.Material m)
        {
            var ret = new OutsideEngine.Rendering.Lighting.Material
            {
                Ambient = new SlimDX.Color4(m.ColorAmbient.A, m.ColorAmbient.R, m.ColorAmbient.G, m.ColorAmbient.B),
                Diffuse = new SlimDX.Color4(m.ColorDiffuse.A, m.ColorDiffuse.R, m.ColorDiffuse.G, m.ColorDiffuse.B),
                Specular = new SlimDX.Color4(m.Shininess, m.ColorSpecular.R, m.ColorSpecular.G, m.ColorSpecular.B),
                Reflect = new SlimDX.Color4(m.ColorReflective.A, m.ColorReflective.R,m.ColorReflective.G, m.ColorReflective.B)
            };

            if (ret.Ambient == new SlimDX.Color4(0, 0, 0, 0))
            {
                ret.Ambient = System.Drawing.Color.Gray;
            }

            if (ret.Diffuse == new SlimDX.Color4(0, 0, 0, 0) || ret.Diffuse == System.Drawing.Color.Black)
            {
                ret.Diffuse = System.Drawing.Color.White;
            }

            if (m.ColorSpecular == new Assimp.Color4D(0, 0, 0, 0) || m.ColorSpecular == new Assimp.Color4D(0, 0, 0))
            {
                ret.Specular = new SlimDX.Color4(ret.Specular.Alpha, 0.5f, 0.5f, 0.5f);
            }

            return ret;
        }

        /// <summary>
        /// Get the array of bytes which is represented by the managed object provided
        /// Equivalent to the C++ reinterpret_cast as a char pointer operation
        /// </summary>
        /// <param name="o">Object to be serialized into binary</param>
        /// <returns>The byte array representing the given memory</returns>
        public static byte[] GetArray(object o)
        {
            var len = Marshal.SizeOf(o);
            var _unmanagedStaging = new byte[len];
            var ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(o, ptr, true);
            Marshal.Copy(ptr, _unmanagedStaging, 0, len);
            Marshal.FreeHGlobal(ptr);
            return _unmanagedStaging;
        }
    }
}
