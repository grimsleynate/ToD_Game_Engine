// File: ToDEngine.Mesh/MeshLoader.cs
// Namespace: ToDEngine.Mesh

using System.Runtime.InteropServices;
using Veldrid;

namespace Engine.Mesh
{
    /// <summary>
    /// Loads raw mesh data into GPU buffers.
    /// </summary>
    public class MeshLoader
    {
        private readonly GraphicsDevice _device;
        private readonly ResourceFactory _factory;

        public MeshLoader(GraphicsDevice device)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _factory = device.ResourceFactory;
        }

        /// <summary>
        /// Creates a vertex buffer from an array of T-vertices.
        /// </summary>
        /// <typeparam name="T">Must be a struct matching your vertex layout.</typeparam>
        /// <param name="vertices">Vertex data.</param>
        /// <param name="vertexCount">Out: number of vertices.</param>
        /// <returns>A DeviceBuffer with vertex data uploaded.</returns>
        public DeviceBuffer CreateVertexBuffer<T>(T[] vertices, out uint vertexCount)
            where T : unmanaged
        {
            if (vertices == null || vertices.Length == 0)
                throw new ArgumentException("Must provide at least one vertex.", nameof(vertices));

            vertexCount = (uint)vertices.Length;
            // sizeof(T) * count
            uint sizeInBytes = (uint)(Marshal.SizeOf<T>() * vertices.Length);

            var desc = new BufferDescription(sizeInBytes, BufferUsage.VertexBuffer);
            var buffer = _factory.CreateBuffer(desc);

            // Upload data
            _device.UpdateBuffer(buffer, 0, vertices);
            return buffer;
        }

        /// <summary>
        /// Creates an index buffer from an array of indices (ushort or uint).
        /// </summary>
        /// <typeparam name="I">Must be a struct (ushort or uint) matching your index format.</typeparam>
        /// <param name="indices">Index data.</param>
        /// <param name="indexCount">Out: number of indices.</param>
        /// <returns>A DeviceBuffer with index data uploaded.</returns>
        public DeviceBuffer CreateIndexBuffer<I>(I[] indices, out uint indexCount)
            where I : unmanaged
        {
            if (indices == null || indices.Length == 0)
                throw new ArgumentException("Must provide at least one index.", nameof(indices));

            indexCount = (uint)indices.Length;
            uint sizeInBytes = (uint)(Marshal.SizeOf<I>() * indices.Length);

            var desc = new BufferDescription(sizeInBytes, BufferUsage.IndexBuffer);
            var buffer = _factory.CreateBuffer(desc);

            _device.UpdateBuffer(buffer, 0, indices);
            return buffer;
        }
    }
}