using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OutsideEngine.Util;

namespace OutsideEngine.Rendering.Mesh
{
    using SlimDX.Direct3D11;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Encapsulate geometry loaded from a mesh
    /// </summary>
    public class MeshGeometry : DisposableClass
    {
        /// <summary>
        /// Represents information about a subset - the beginning vertex and
        ///  face offsets, as well as how many vertices/faces are in the subset
        /// </summary>
        public class Subset
        {
            /// <summary>
            /// Offset from the beginning of the vertex array at which this
            ///  subset begins
            /// </summary>
            public int VertexStart;

            /// <summary>
            /// Number of vertices in this mesh subset
            /// </summary>
            public int VertexCount;

            /// <summary>
            /// Offset from the beginning of the face array at which this subset begins
            /// </summary>
            public int FaceStart;

            /// <summary>
            /// Number of faces in this mesh subset
            /// </summary>
            public int FaceCount;
        }

        private Buffer _vertexBuffer;
        private Buffer _indexBuffer;

        private int _vertexStride;
        private List<Subset> _subsetTable;
        private bool _disposed;

        /// <summary>
        /// Cleanup DirectX resources (the buffers) used by this class
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Util.Util.ReleaseCom(ref _vertexBuffer);
                    Util.Util.ReleaseCom(ref _indexBuffer);
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Set the vertices in this model to those given in the list
        /// </summary>
        /// <typeparam name="TVertexType">Type of vertices that are being loaded into the buffer</typeparam>
        /// <param name="device">Direct3D device used for the creation of the vertex buffer</param>
        /// <param name="vertices">List of actual vertices for use in the vertex buffer</param>
        public void SetVertices<TVertexType>(Device device, List<TVertexType> vertices) where TVertexType : struct
        {
            // Release the old vertex buffer, and get the size of the new vertex stride
            Util.Util.ReleaseCom(ref _vertexBuffer);
            _vertexStride = Marshal.SizeOf(typeof(TVertexType));

            var vertexBufferDescription = new BufferDescription
            (
                _vertexStride * vertices.Count,
                ResourceUsage.Immutable,
                BindFlags.VertexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0
            );
            _vertexBuffer = new Buffer(device, new SlimDX.DataStream(vertices.ToArray(), false, false), vertexBufferDescription);
        }

        /// <summary>
        /// Set the indices that are used in this model
        /// </summary>
        /// <param name="device">Direct3D device for use in creation of the index buffer</param>
        /// <param name="indices">List of indices (in 16 bit integer format - save space, since models should be less than 32K points)</param>
        public void SetIndices(Device device, List<ushort> indices)
        {
            var indexBufferDesc = new BufferDescription
            (
                sizeof(ushort) * indices.Count,
                ResourceUsage.Immutable,
                BindFlags.IndexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0
            );
            _indexBuffer = new Buffer(device, new SlimDX.DataStream(indices.ToArray(), false, false), indexBufferDesc);
        }

        /// <summary>
        /// Set the subset table to be used for this model
        /// </summary>
        /// <param name="subsets">Subset table in use in this model</param>
        public void SetSubsetTable(List<Subset> subsets)
        {
            _subsetTable = subsets;
        }

        /// <summary>
        /// Draw the geometry to the provided Direct3D device context. Do not modify any shader state, input layout,
        ///  etc. This is beyond the scope of this class. Simply set the buffers, and draw from them. Only one subset
        ///  is drawn in this call. It is assumed by this method that the current primitive assembly mode is TriangleList
        /// </summary>
        /// <param name="context">Device context that is in use for drawing (usually Device.ImmediateContext)</param>
        /// <param name="subsetId">ID of the subset which we desire to draw</param>
        public void Draw(DeviceContext context, int subsetId)
        {
            const int offset = 0;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, _vertexStride, offset));
            context.InputAssembler.SetIndexBuffer(_indexBuffer, SlimDX.DXGI.Format.R16_UInt, 0);
            context.DrawIndexed(_subsetTable[subsetId].FaceCount * 3, _subsetTable[subsetId].FaceStart * 3, 0);
        }
    }
}
