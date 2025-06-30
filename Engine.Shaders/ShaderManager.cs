// File: ToDEngine.Shaders/ShaderManager.cs
// Namespace: ToDEngine.Shaders

using System;
using System.IO;
using Veldrid;
using Veldrid.SPIRV;

namespace Engine.Shaders
{
    /// <summary>
    /// Loads SPIR-V shaders from disk and creates Veldrid Shader objects.
    /// </summary>
    public class ShaderManager : IDisposable
    {
        private readonly ResourceFactory _factory;
        private bool _disposed;

        /// <summary>
        /// Construct with the Veldrid ResourceFactory (from GraphicsDevice).
        /// </summary>
        public ShaderManager(ResourceFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>
        /// Loads a vertex + fragment shader pair using the default entrypoints ("VSMain"/"PSMain").
        /// </summary>
        /// <param name="vertPath">Path to the vertex SPIR-V (.spv) file.</param>
        /// <param name="fragPath">Path to the fragment SPIR-V (.spv) file.</param>
        /// <returns>An array of two shaders: [0] = vertex, [1] = fragment.</returns>
        public Shader[] LoadShaders(string vertPath, string fragPath)
        {
            return LoadShaders(
                (vertPath, ShaderStages.Vertex, "VSMain"),
                (fragPath, ShaderStages.Fragment, "PSMain")
            );
        }

        /// <summary>
        /// Loads any number of shaders by specifying path, stage, and entrypoint.
        /// </summary>
        /// <param name="shaderInfos">
        /// Tuples of (file path, shader stage, entrypoint name).
        /// </param>
        /// <returns>
        /// A Shader[] whose order matches <paramref name="shaderInfos"/>.
        /// </returns>
        public Shader[] LoadShaders(params (string Path, ShaderStages Stage, string EntryPoint)[] shaderInfos)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ShaderManager));
            if (shaderInfos == null || shaderInfos.Length == 0)
                throw new ArgumentException("Must supply at least one shader.", nameof(shaderInfos));

            // Read bytes & build descriptions
            ShaderDescription[] descs = new ShaderDescription[shaderInfos.Length];
            for (int i = 0; i < shaderInfos.Length; i++)
            {
                var info = shaderInfos[i];
                if (!File.Exists(info.Path))
                    throw new FileNotFoundException($"Shader file not found: {info.Path}", info.Path);

                byte[] bytes = File.ReadAllBytes(info.Path);
                descs[i] = new ShaderDescription(info.Stage, bytes, info.EntryPoint);
            }

            // Create and return Shader objects
            return _factory.CreateFromSpirv(descs[0], descs[1]);
        }

        /// <summary>
        /// Optionally, implement disposal if you manage resource lifetimes here.
        /// ResourceManager typically handles disposal of Shaders for you.
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
        }
    }
}