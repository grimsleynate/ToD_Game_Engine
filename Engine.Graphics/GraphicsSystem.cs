using System.Diagnostics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Engine.Graphics
{
    /// <summary>
    /// Encapsulates window, device, swapchain, and per-fram logic.
    /// </summary>
    public class GraphicsSystem : IDisposable
    {
        private readonly Sdl2Window _window;
        private readonly GraphicsDevice _device;
        private readonly ResourceFactory _factory;
        private readonly Stopwatch _frameTimer = Stopwatch.StartNew();

        /// <summary>
        /// The Veldrid window.
        /// </summary>
        public Sdl2Window Window => _window;

        /// <summary>
        /// The graphics device (Vulkan, Direct3D11/12, OpenbGL, etc.).
        /// </summary>
        public GraphicsDevice Device => _device;


        /// <summary>
        /// Shortand to create resources (buffers, shaders, pipelines).
        /// </summary>
        public ResourceFactory Factory => _factory;

        /// <summary>
        /// The main swapchain and its frameboffer for rendering.
        /// </summary>
        public Swapchain MainSwapchain => _device.MainSwapchain;


        /// <summary>
        /// Initializes Veldrid, creating a window and graphics device.
        /// </summary>
        /// <param name="title">Window title.</param>
        /// <param name="width">Initial window width in pixels</param>
        /// <param name="height">Initial window height in pixels</param>
        /// <param name="syntToVBlank">Whether to wait for VSync</param>
        public GraphicsSystem(
            string title,
            uint width,
            uint height,
            bool syncToVBlank = true)
        {
            var windowCI = new WindowCreateInfo(50, 50, (int)width, (int)height, WindowState.Normal, title);
            var gdOptions = new GraphicsDeviceOptions(
                debug: true,
                swapchainDepthFormat: PixelFormat.R16_UNorm,
                swapchainSrgbFormat: true,
                syncToVerticalBlank: syncToVBlank,
                resourceBindingModel: ResourceBindingModel.Improved,
                preferDepthRangeZeroToOne: true,
                preferStandardClipSpaceYDirection: true);

            VeldridStartup.CreateWindowAndGraphicsDevice(
                windowCI,
                gdOptions,
                out _window,
                out _device);

            _factory = _device.ResourceFactory;
        }


        /// <summary>
        /// Pumps OS events and returns true if the window is still open.
        /// </summary>
        /// <returns>True if the window is still open</returns>
        public InputSnapshot PollEvents()
        {
            return _window.PumpEvents();
        }


        /// <summary>
        /// Call once per frame to get elapsed time (seconds) since last call.
        /// </summary>
        /// <returns></returns>
        public float GetDeltaSeconds()
        {
            var seconds = (float)_frameTimer.Elapsed.TotalSeconds;
            _frameTimer.Restart();
            return seconds;
        }


        /// <summary>
        /// Begins a new frame: create a CommandList, begins it, clears the back buffer.
        /// </summary>
        public CommandList BeginFrame()
        {
            var cl = _factory.CreateCommandList();
            cl.Begin();
            cl.SetFramebuffer(MainSwapchain.Framebuffer);
            cl.ClearColorTarget(0, RgbaFloat.Black);
            return cl;
        }


        /// <summary>
        /// Ends the frame: submits commands, presents backbuffer, disposes the CommandList.
        /// </summary>
        /// <param name="cl"></param>
        public void EndFrame(CommandList cl)
        {
            cl.End();
            _device.SubmitCommands(cl);
            _device.SwapBuffers(MainSwapchain);
            cl.Dispose();
        }


        public void Present()
        {
            _device.SwapBuffers(_device.MainSwapchain);
        }


        /// <summary>
        /// Releases all graphics resources and closes the window.
        /// </summary>
        public void Dispose()
        {
            _device.WaitForIdle();
            _device.Dispose();
            _window.Close();
        }
    }
}
