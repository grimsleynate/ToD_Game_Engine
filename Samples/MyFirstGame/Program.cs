using System;

namespace ToDGame.App
{
    class Program
    {
        static void Main(string[] args)
        {
            //Wrap Game up in a using to call Dispose() when Run() exits
            using (var game = new Game())
            {
                try
                {
                    game.Run();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }
    }
}







//struct Vertex
//{
//    public Vector2 Position;
//    public Vector3 Color;
//    public const uint SizeInBytes = sizeof(float) * (2 + 3); 
//    public Vertex(Vector2 p, Vector3 c) { Position = p; Color = c; }
//}

//class Program
//{
//    static void Main()
//    {
//        string exeDir = AppContext.BaseDirectory;                    // ...\bin\Debug\net9.0\
//        string projectDir = Path.GetFullPath(Path.Combine(exeDir, "..", "..", ".."));
//        // now projectDir points at ...\MyFirstGame\  

//        string shaderDir = Path.Combine(projectDir, "Shaders");
//        string vsPath = Path.Combine(shaderDir, "MyShader.vert.spv");
//        string fsPath = Path.Combine(shaderDir, "MyShader.frag.spv");

//        // Optional: sanity‐check
//        if (!File.Exists(vsPath) || !File.Exists(fsPath))
//            throw new FileNotFoundException(
//              $"Shader files not found:\n  {vsPath}\n  {fsPath}"
//            );

//        // Load them
//        var vsBytes = File.ReadAllBytes(vsPath);
//        var fsBytes = File.ReadAllBytes(fsPath);

//        // 1) Create window and device
//        var windowCI = new WindowCreateInfo(100, 100, 800, 600, WindowState.Normal, "Hello Triangle");
//        var window = VeldridStartup.CreateWindow(ref windowCI);
//        var gdOpts = new GraphicsDeviceOptions(false, null, true, ResourceBindingModel.Improved);
//        var device = VeldridStartup.CreateGraphicsDevice(window, gdOpts);
//        var factory = device.ResourceFactory;

//        var vsDesc = new ShaderDescription(ShaderStages.Vertex, vsBytes, "main");
//        var fsDesc = new ShaderDescription(ShaderStages.Fragment, fsBytes, "main");

//        Shader[] shaders = factory.CreateFromSpirv(vsDesc, fsDesc);

//        // 3) Vertex buffer for a single triangle
//        Vertex[] verts =
//        {
//            new Vertex(new Vector2(0.0f, 0.8f), new Vector3(1, 0, 0)),
//            new Vertex(new Vector2(0.8f, -0.8f), new Vector3(0, 1, 0)),
//            new Vertex(new Vector2(-0.8f, -0.8f), new Vector3(0, 0, 1)),
//        };
//        var vb = factory.CreateBuffer(new BufferDescription(Vertex.SizeInBytes * (uint)verts.Length, BufferUsage.VertexBuffer));
//        device.UpdateBuffer(vb, 0, verts);

//        // 4) Describe the triangle's vertex layout
//        var vertexLayout = new VertexLayoutDescription(
//            stride: Vertex.SizeInBytes,
//            elements: new[] { 
//                new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2, 0),
//                new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3, sizeof(float)*2)
//            }
//        );

//        // 5) Create a pipeline with no resource layouts
//        var pipelineDesc = new GraphicsPipelineDescription
//        {
//            BlendState = BlendStateDescription.SingleOverrideBlend,
//            DepthStencilState = DepthStencilStateDescription.Disabled,
//            RasterizerState = new RasterizerStateDescription(
//                                   FaceCullMode.None, PolygonFillMode.Solid, FrontFace.Clockwise, true, false),
//            PrimitiveTopology = PrimitiveTopology.TriangleList,
//            ShaderSet = new ShaderSetDescription(
//                vertexLayouts: new[] { vertexLayout },
//                shaders: shaders
//            ),
//            ResourceLayouts = Array.Empty<ResourceLayout>(),  // <— no uniforms, so zero layouts
//            Outputs = device.SwapchainFramebuffer.OutputDescription
//        };
//        var pipeline = factory.CreateGraphicsPipeline(pipelineDesc);

//        // 6) Render loop
//        var cl = factory.CreateCommandList();
//        while (window.Exists)
//        {
//            window.PumpEvents();
//            cl.Begin();
//            cl.SetFramebuffer(device.SwapchainFramebuffer);
//            cl.ClearColorTarget(0, RgbaFloat.CornflowerBlue);
//            cl.SetPipeline(pipeline);
//            cl.SetVertexBuffer(0, vb);
//            cl.Draw(3);
//            cl.End();
//            device.SubmitCommands(cl);
//            device.SwapBuffers();
//        }

//        // 7) Cleanup
//        cl.Dispose();
//        pipeline.Dispose();
//        vb.Dispose();
//        foreach (var s in shaders) s.Dispose();
//        device.Dispose();
//    }
//}
