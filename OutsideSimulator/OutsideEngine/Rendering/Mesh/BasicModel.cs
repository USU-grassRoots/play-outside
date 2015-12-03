using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using OutsideEngine.Util;
using OutsideEngine.Resource;
using OutsideEngine.Rendering.Shaders;
using OutsideEngine.Rendering.Lighting;

using Assimp;
using SlimDX;
using SlimDX.Direct3D11;

namespace OutsideEngine.Rendering.Mesh
{
    /// <summary>
    /// Represents a basic model, as loaded from an ASSIMP resource
    /// </summary>
    public class BasicModel : DisposableClass
    {
        private bool _disposed;

        private readonly List<MeshGeometry.Subset> _subsets;
        private readonly List<BasicEffectVertex> _vertices;
        private readonly List<ushort> _indices;
        private MeshGeometry _modelMesh;

        /// <summary>
        /// The materials included in this basic model
        /// </summary>
        public List<Lighting.Material> Materials { get; private set; }

        /// <summary>
        /// List of diffuse texture maps used by this model
        /// </summary>
        public List<ShaderResourceView> DiffuseMapSRV { get; private set; }

        /// <summary>
        /// List of normal map resources used by this model
        /// </summary>
        public List<ShaderResourceView> NormalMapSRV { get; private set; }

        /// <summary>
        /// Accessor for our model mesh
        /// </summary>
        public MeshGeometry ModelMesh => _modelMesh;

        /// <summary>
        /// Number of subsets contained by this basic mesh
        /// </summary>
        public int SubsetCount => _subsets.Count;

        /// <summary>
        /// Dispose of our BasicModel instance, removing all COM objects and
        ///  otherwise unmanaged resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Util.Util.ReleaseCom(ref _modelMesh);
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Initialize our basic model
        /// </summary>
        /// <param name="device">Direct3D device for use</param>
        /// <param name="texMgr">Texture manager from which to load texture data</param>
        /// <param name="filename">Filename of the ASSIMP resource we would like to load (see ASSIMP documentation for supported formats)</param>
        /// <param name="texturePath">Texture path - base path for textures used by the ASSIMP model</param>
        public BasicModel(Device device, TextureManager texMgr, string filename, string texturePath)
        {
            _subsets = new List<MeshGeometry.Subset>();
            _vertices = new List<BasicEffectVertex>();
            _indices = new List<ushort>();
            DiffuseMapSRV = new List<ShaderResourceView>();
            NormalMapSRV = new List<ShaderResourceView>();
            Materials = new List<Lighting.Material>();
            _modelMesh = new MeshGeometry();

            var importer = new AssimpContext();
            if (!importer.IsImportFormatSupported(Path.GetExtension(filename)))
            {
                throw new ArgumentException($"Model format {Path.GetExtension(filename)} is not supported! Cannot load {filename}", "Outside Engine");
            }
            var model = importer.ImportFile(filename, PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.CalculateTangentSpace);
#if DEBUG
            var logStream = new ConsoleLogStream();
            logStream.Attach();
#endif

            foreach (var mesh in model.Meshes)
            {
                //
                // Vertex processing
                //
                var verts = new List<BasicEffectVertex>();
                var subset = new MeshGeometry.Subset
                {
                    VertexCount = mesh.VertexCount,
                    VertexStart = _vertices.Count,
                    FaceStart = _indices.Count / 3,
                    FaceCount = mesh.FaceCount
                };
                _subsets.Add(subset);
             
                // TODO KAM: Process bounding box corners

                for (var i = 0; i < mesh.VertexCount; ++i)
                {
                    Vector3 pos = mesh.HasVertices ? mesh.Vertices[i].ToVector3() : new Vector3();

                    var norml = mesh.HasNormals ? mesh.Normals[i].ToVector3() : new Vector3();
                    var texC = mesh.HasTextureCoords(0) ? mesh.TextureCoordinateChannels[0][i].ToVector3() : new Vector3();
                    var tan = mesh.HasTangentBasis ? mesh.Tangents[i].ToVector3() : new Vector3();
                    var v = new BasicEffectVertex(pos, norml, new Vector2(texC.X, texC.Y));
                    verts.Add(v);
                }

                _vertices.AddRange(verts);

                var indices = mesh.GetIndices().Select(i => (ushort)(i + (uint)subset.VertexStart)).ToList();
                _indices.AddRange(indices);

                //
                // Material processing
                //
                var mat = model.Materials[mesh.MaterialIndex];
                var material = mat.ToMaterial();

                Materials.Add(material);
                TextureSlot diffuseSlot;
                mat.GetMaterialTexture(TextureType.Diffuse, 0, out diffuseSlot);
                var diffusePath = diffuseSlot.FilePath;
                if (Path.GetExtension(diffusePath) == ".tga")
                {
                    throw new InvalidDataException("Cannot use TGA files for textures with DirectX. Sorry about that.");
                }

                if (!string.IsNullOrEmpty(diffusePath))
                {
                    DiffuseMapSRV.Add(texMgr.GetTexture(Path.Combine(texturePath, diffusePath)));
                }

                TextureSlot normalSlot;
                mat.GetMaterialTexture(TextureType.Normals, 0, out normalSlot);
                var normalPath = normalSlot.FilePath;
                if (!string.IsNullOrEmpty(normalPath))
                {
                    NormalMapSRV.Add(texMgr.GetTexture(Path.Combine(texturePath, normalPath)));
                }
                else
                {
                    var normalExt = Path.GetExtension(diffusePath);
                    normalPath = Path.GetFileNameWithoutExtension(diffusePath) + "_nmap" + normalExt;
                    NormalMapSRV.Add(texMgr.GetTexture(Path.Combine(texturePath, normalPath)));
                }
            }

            _modelMesh.SetSubsetTable(_subsets);
            _modelMesh.SetVertices(device, _vertices);
            _modelMesh.SetIndices(device, _indices);
        }
    }

    // TODO KAM: You need to re-vamp this. Like, with Dirtyables and whatnot.

    /// <summary>
    /// Single instance of a basic model. The same basic model shouldn't be
    ///  loaded more than once into memory, but multiple instances can be used
    /// </summary>
    public struct BasicModelInstance
    {
        /// <summary>
        /// Model data
        /// </summary>
        public BasicModel Model;

        /// <summary>
        /// World transformation matrix to be used on this instance of the model
        /// </summary>
        public Matrix World;

        /// <summary>
        /// Draw this instance of the BasicModel mesh
        /// </summary>
        /// <param name="context">Rendering context to use for the draw calls</param>
        /// <param name="effect">BasicEffect to use for the draw call</param>
        /// <param name="viewProj">ViewProj combined matrix</param>
        public void Draw(DeviceContext context, BasicEffect effect, Matrix viewProj)
        {
            var world = World;
            var worldInverseTranspose = MathF.InverseTranspose(world);
            var worldViewProj = world * viewProj;

            effect.SetWorld(World);
            effect.SetWorldInvTranspose(worldInverseTranspose);
            effect.SetWorldViewProj(worldViewProj);
            effect.SetTextureTransform(Matrix.Identity);

            for (int i = 0; i < Model.SubsetCount; ++i)
            {
                effect.SetMaterial(Model.Materials[i]);
                // TODO KAM: Remove this shim
                if (Model.DiffuseMapSRV.Count > 0)
                {
                    effect.SetDiffuseMap(Model.DiffuseMapSRV[i]);
                }

                effect.ColorTech.GetPassByIndex(0).Apply(context);
                Model.ModelMesh.Draw(context, i);
            }
        }
    }
}
