using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

using OutsideEngine.Util;
using OutsideEngine.Rendering.Lighting;
using SlimDX.Direct3D11;

namespace OutsideEngine.Rendering.Mesh
{
    /// <summary>
    /// Describes render modes used for rendering a mesh under the Outside Engine
    /// </summary>
    public enum RenderMode
    {
        /// <summary>
        /// Render to a normal map
        /// </summary>
        NormalMapped,

        /// <summary>
        /// Used to render in the basic format (to a regular buffer)
        /// </summary>
        Basic,

        /// <summary>
        /// Used to render to a displacement map
        /// </summary>
        DisplacementMapped,

        /// <summary>
        /// Used to render to a shadow map
        /// </summary>
        ShadowMap,

        /// <summary>
        /// Render to a normal depth map
        /// </summary>
        NormalDepthMap
    }

    /// <summary>
    /// Base class representing a MeshModel in the Outside Engine.
    ///  Stores information about the geometry, subsets, materials, diffuse maps, etc.
    /// <typeparam name="TVertexType">Type of vertices in use</typeparam>
    /// </summary>
    public abstract class MeshModel<TVertexType> : DisposableClass where TVertexType : struct
    {
        #region Geometry Data
        /// <summary>
        /// Mesh vertex / subset data (each model contains a mesh)
        /// </summary>
        public MeshGeometry ModelMesh { get; protected set; }

        /// <summary>
        /// Copy of the mesh subset data contained within our ModelMesh variable.
        ///  List of subsets (subcomponents) in our mesh
        ///  Example: A person mesh may contain subsets hat, head, torso, arm-r, arm-l...
        /// </summary>
        protected List<MeshGeometry.Subset> Subsets;

        /// <summary>
        /// Number of subsets present in this MeshModel
        /// </summary>
        public int SubsetCount => Subsets.Count;

        /// <summary>
        /// Index list for this MeshModel (32 bit unsigned integer, type R32_UInt)
        /// </summary>
        protected List<uint> Indices;

        /// <summary>
        /// Vertex list for this MeshModel (should be parallel with the ModelMesh data)
        /// </summary>
        protected List<TVertexType> Vertices;

        /// <summary>
        /// List of materials which are used by this MeshModel
        /// </summary>
        public List<Material> Materials { get; protected set; }

        /// <summary>
        /// List of shader resource views used for this material for diffuse maps.
        ///  A diffuse map is essentially a texture, in the classical sense (a picture of
        ///  wooden panels, for a crate).
        /// </summary>
        public List<ShaderResourceView> DiffuseMapSRV { get; protected set; }

        /// <summary>
        /// List of shader resource views used for this material for normal maps.
        ///  A normal map holds per-pixel normal information, which helps in per-pixel
        ///  lighting. Pre-baking lighting information this way allows per-pixel lighting
        ///  without having to compute normal information in the rasterization step
        ///  (which is liable to de-normalize normals anyways) 
        /// </summary>
        public List<ShaderResourceView> NormalMapSRV { get; protected set; }

        // public BoundingBox BoundingBox {get; protected set;}

        private bool _disposed;

        /// <summary>
        /// Cleanup Direct3D resources here, as well as any other managed
        ///  resources that use unmanaged resources, or raw unmanaged resources
        ///  used by the ModelMesh (in this case, just the meshGeometry object)
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    var meshGeometry = ModelMesh;
                    Util.Util.ReleaseCom(ref meshGeometry);
                    Indices.Clear();
                    Vertices.Clear();
                    Indices = null;
                    Vertices = null;
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Construction and State
        /// <summary>
        /// Construct the MeshModel base class by creating the list structures needed internally
        /// </summary>
        protected MeshModel()
        {
            Subsets = new List<MeshGeometry.Subset>();
            Vertices = new List<TVertexType>();
            Indices = new List<uint>();
            DiffuseMapSRV = new List<ShaderResourceView>();
            NormalMapSRV = new List<ShaderResourceView>();
            Materials = new List<Material>();
            ModelMesh = new MeshGeometry();
        }

        /// <summary>
        /// Initialize a mesh model from the information provided (list of vertices, list of indices)
        /// TODO: Do you actually need this to be abstract, if the TVertexType generic is being used?
        /// </summary>
        /// <param name="device">Direct3D device used for creation of assets</param>
        /// <param name="Vertices">List of vertices, fitting the required type</param>
        /// <param name="Indices">List of indices, fitting the required type</param>
        protected abstract void InitFromMeshData(Device device, List<TVertexType> Vertices, List<int> Indices);
        #endregion
    }
}
