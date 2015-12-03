using System.Runtime.InteropServices;

using SlimDX;

namespace OutsideEngine.Rendering.Lighting
{
    /// <summary>
    /// Represents the data for a DirectionalLight
    /// A directional light simulates a light source at some infinite distance
    ///  away with no attenuation, but with a definite source. A good example is the
    ///  sun - it illuminates the scene, but certainly has a source.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DirectionalLight
    {
        /// <summary>
        /// Ambient coloring quality of this light
        /// </summary>
        public Color4 Ambient;

        /// <summary>
        /// Diffuse coloring quality of this light
        /// </summary>
        public Color4 Diffuse;

        /// <summary>
        /// Specular coloring quality of this light
        /// </summary>
        public Color4 Specular;

        /// <summary>
        /// Direction in which this light is facing
        /// </summary>
        public Vector3 Direction;

        private float _padding;

        /// <summary>
        /// Size of a DirectionalLight in unmanaged memory (in bytes)
        /// </summary>
        public static int Stride = Marshal.SizeOf(typeof(DirectionalLight));
    }

    /// <summary>
    /// Represents the information associated with a point light. A point
    ///  light has a source within the bounds of the 3D scene being represented,
    ///  a degree of attenuation (i.e., fade out amount) and regular light
    ///  coloring qualities (specular, diffuse, ambient colors)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PointLight
    {
        /// <summary>
        /// Represents the ambient coloring term of this light
        /// </summary>
        public Color4 Ambient;
        
        /// <summary>
        /// Represents the diffuse coloring term of this light
        /// </summary>
        public Color4 Diffuse;
        
        /// <summary>
        /// Represents the specular coloring term of this light
        /// </summary>
        public Color4 Specular;

        /// <summary>
        /// Represents the position of this light. No information about space
        ///  is implied by this term, so the space must be maintained by the
        ///  code using this struct (in both the shader and host code)
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The range of this point light - the furthest unit away (in world units)
        ///  from this light at which a point may be illuminated (at least partially) 
        ///  by this light
        /// </summary>
        public float Range;

        /// <summary>
        /// Attenuation factor of this light, in X, Y, Z
        /// </summary>
        public Vector3 Attenuation;

        /// <summary>
        /// Padding - not used for anything, used only to maintain 16-byte aligned property
        /// </summary>
        public float Pad;

        /// <summary>
        /// Size, in bytes, of this structure (in memory).
        /// </summary>
        public static int Stride = Marshal.SizeOf(typeof(PointLight));
    }

    /// <summary>
    /// Represents the information required for a spot light. A spot light is a light source
    ///  which has a location, direction, and angle of illumination. A good example would be a
    ///  street lamp - it illuminates only downward, and only in a cone shaped area.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SpotLight
    {
        /// <summary>
        /// Ambient coloring quality of the light
        /// </summary>
        public Color4 Ambient;

        /// <summary>
        /// Diffuse coloring quality of the light
        /// </summary>
        public Color4 Diffuse;

        /// <summary>
        /// Specular coloring quality of the light
        /// </summary>
        public Color4 Specular;

        /// <summary>
        /// Position of the "bulb" in 3D space. The space is not specified here, and must
        ///  be inferred by the calling code (local, world, homogenous space, etc.)
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Range of the light - how far it is capable of illuminating (in the same space as
        ///  the position)
        /// </summary>
        public float Range;

        /// <summary>
        /// Direction of the light. From the source, this would form a ray through the center
        ///  of the cone of illumination formed by this light source.
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// Angle of illumination from the cone
        /// </summary>
        public float Spot;

        /// <summary>
        /// Attenuation (fade) factor of the ligth
        /// </summary>
        public Vector3 Attenuation;

        /// <summary>
        /// 4 bytes used for padding, to make this structure 16-byte aligned for use with DirectX
        /// </summary>
        public float Pad;

        /// <summary>
        /// Size of this structure in bytes.
        /// </summary>
        public static int Stride = Marshal.SizeOf(typeof(SpotLight));
    }

    /// <summary>
    /// Represents the information required for a material. Materials contain reflectivity
    ///  information of a surface, for ambient light, diffuse light, and specular light.
    /// There is also a reflect term, which [WHAT DOES IT DO I DON'T ACTUALLY KNOW]
    /// TODO: Figure out what reflect term does, add it to the documentation
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Material
    {
        /// <summary>
        /// Ambient color of this material. The color which is used for reflecting ambient light
        /// </summary>
        public Color4 Ambient;

        /// <summary>
        /// Diffuse color of this material. The color which is used for reflecting diffuse light.
        /// </summary>
        public Color4 Diffuse;

        /// <summary>
        /// Specular color of this material. The color which is used for reflecting specular light.
        /// </summary>
        public Color4 Specular;

        /// <summary>
        /// Something important.
        /// TODO: What is this again?
        /// </summary>
        public Color4 Reflect;

        /// <summary>
        /// Size of this structure in bytes, as held in memory
        /// </summary>
        public static int Stride = Marshal.SizeOf(typeof(Material));
    }
}
