using Engine.Graphics;
using Engine.Mesh;
using Engine.Shaders;
using System.Numerics;
using Veldrid;

namespace ToDGame.App
{
    public class Game : IDisposable
    {
        private readonly GraphicsSystem _graphics;
        private readonly ShaderManager _shaderManager;
        private readonly PipelineBuilder _pipelineBuilder;
        private readonly MeshLoader _meshLoader;
        private readonly Engine.Core.ResourceManager _resourceManager;

        private CommandList _cl;
        private Pipeline _trianglePipeline;
        private DeviceBuffer _vertexBuffer;
        private uint _vertexCount;

        private bool _running;

        public Game()
        {
            //Instantiate core systems
            _resourceManager = new Engine.Core.ResourceManager();
            _graphics = new GraphicsSystem("Tides of Dominion", 800, 600);
            _shaderManager = new Engine.Shaders.ShaderManager(_graphics.Factory);
            _pipelineBuilder = new PipelineBuilder(_graphics.Device);
            _meshLoader = new MeshLoader(_graphics.Device);
        }

        public void Run()
        {
            Initialize();
            _running = true;

            while (_running && _graphics.Window.Exists)
            {
                InputSnapshot input = _graphics.PollEvents();

                if (!_graphics.Window.Exists)
                {
                    _running = false;
                    break;
                }

                if (input.KeyEvents.Any(ke => ke.Key == Key.Escape && ke.Down))
                {
                    _running = false;
                    break;
                }

                float dt = _graphics.GetDeltaSeconds();
                Update(dt, input);
                Draw();
                _graphics.Present();
            }

            Shutdown();
        }


        private void Initialize()
        {
            //Load and compile shaders
            var shaders = _resourceManager.GetOrCreate("basic_shaders", () =>
                    _shaderManager.LoadShaders(AppContext.BaseDirectory + "Shaders/MyShader.vertex.spv", AppContext.BaseDirectory + "Shaders/MyShader.fragment.spv"));

            //Build a graphics pipeline (also cached)
            _trianglePipeline = _resourceManager.GetOrCreate("triangle_pipeline", () =>
                _pipelineBuilder.Create(
                    shaders,
                    _graphics.MainSwapchain.Framebuffer.OutputDescription,
                    new[] { new VertexLayoutDescription(
                        0,
                        new VertexElementDescription(
                            "Position",
                            VertexElementSemantic.Position,
                            VertexElementFormat.Float2 
                            ),
                        new VertexElementDescription(
                            "Color",
                            VertexElementSemantic.Color,
                            VertexElementFormat.Float3
                            )
                    )}
                )
            );

            //Create a vertex buffer
            _vertexBuffer = _resourceManager.GetOrCreate("triangle_vb", () =>
            {
                var verts = new[]
                {
                    new VertexPositionColor(new Vector2(0f, 0.5f), new Vector3(1f, 0f, 0f)),
                    new VertexPositionColor(new Vector2(0.5f, -0.5f), new Vector3(0f, 1f, 0f)),
                    new VertexPositionColor(new Vector2(-0.5f, -0.5f), new Vector3(0f, 0f, 1f))
                };
                return _meshLoader.CreateVertexBuffer(verts, out _vertexCount);
            });
        }


        private void Update(float deltaSeconds, InputSnapshot input)
        {
            if (!_graphics.Window.Exists)
                _running = false;
        }


        private void Draw()
        {
            _cl = _graphics.BeginFrame();

            _cl.SetPipeline(_trianglePipeline);
            _cl.SetVertexBuffer(0, _vertexBuffer);
            _cl.Draw(_vertexCount);

            _graphics.EndFrame(_cl);
        }


        private void Shutdown()
        {
            _resourceManager.Dispose();
            _graphics.Dispose();
        }


        public void Dispose() => Shutdown();
    }

    //Helper struct matching the vertex layout above
    public struct VertexPositionColor
    {
        public Vector2 Position;
        public Vector3 Color;
        public VertexPositionColor(Vector2 position, Vector3 color)
        {
            Position = position;
            Color = color;
        }
    }
}
