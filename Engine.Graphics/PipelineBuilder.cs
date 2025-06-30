// File: ToDEngine.Graphics/PipelineBuilder.cs
// Namespace: ToDEngine.Graphics

using Veldrid;

namespace Engine.Graphics
{
    /// <summary>
    /// Helper class to build and create GraphicsPipeline instances with sane defaults.
    /// </summary>
    public class PipelineBuilder
    {
        private readonly GraphicsDevice _device;
        private readonly ResourceFactory _factory;

        /// <summary>
        /// Initializes a new instance that will build pipelines on the provided device.
        /// </summary>
        public PipelineBuilder(GraphicsDevice device)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _factory = device.ResourceFactory;
        }

        /// <summary>
        /// Creates a simple graphics pipeline.
        /// </summary>
        /// <param name="shaders">
        /// An array containing exactly two shaders: 
        /// [0] is the vertex shader, [1] is the fragment shader.
        /// </param>
        /// <param name="outputDesc">
        /// The framebuffer's OutputDescription, usually from Swapchain.Framebuffer.OutputDescription.
        /// </param>
        /// <param name="vertexLayouts">
        /// One or more vertex-layout descriptions matching your shader inputs.
        /// </param>
        /// <param name="primitiveTopology">TriangleList by default.</param>
        /// <param name="cullMode">Back‐face culling by default.</param>
        /// <param name="depthTest">Enable depth‐testing & writing?</param>
        /// <param name="blend">Enable simple alpha blending?</param>
        /// <returns>A new GraphicsPipeline.</returns>
        public Pipeline Create(
            Shader[] shaders,
            OutputDescription outputDesc,
            VertexLayoutDescription[] vertexLayouts,

            // Optional settings with defaults:
            PrimitiveTopology primitiveTopology = PrimitiveTopology.TriangleList,
            FaceCullMode cullMode = FaceCullMode.Back,
            bool depthTest = false,
            bool blend = false)
        {
            if (shaders == null || shaders.Length != 2)
                throw new ArgumentException("Expect exactly [vertex, fragment] shaders", nameof(shaders));
            if (vertexLayouts == null || vertexLayouts.Length == 0)
                throw new ArgumentException("Must supply at least one vertex layout", nameof(vertexLayouts));

            // 1) Blend state
            var blendState = blend
                ? BlendStateDescription.SingleAlphaBlend
                : new BlendStateDescription();

            // 2) Depth‐stencil
            var depthDesc = depthTest
                ? DepthStencilStateDescription.DepthOnlyLessEqual
                : DepthStencilStateDescription.Disabled;

            // 3) Rasterizer
            var rasterDesc = new RasterizerStateDescription(
                cullMode: cullMode,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);

            // 4) Shader set (vertex + fragment)
            var shaderSet = new ShaderSetDescription(vertexLayouts, shaders);

            // 5) Pipeline description
            var pipelineDesc = new GraphicsPipelineDescription(
                blendState,
                depthDesc,
                rasterDesc,
                primitiveTopology,
                shaderSet,
                Array.Empty<ResourceLayout>(),
                outputDesc);

            // 6) Create & return
            return _factory.CreateGraphicsPipeline(ref pipelineDesc);
        }
    }
}