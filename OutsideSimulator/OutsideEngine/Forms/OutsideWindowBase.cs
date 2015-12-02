using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SlimDX.DXGI;

using System.Windows.Forms;

using GameTimer = OutsideEngine.Timer.GameTimer;

using OutsideEngine.Util;

namespace OutsideEngine.Forms
{
    using Debug = System.Diagnostics.Debug;
    using Util = OutsideEngine.Util.Util;
    using SlimDX;
    using SlimDX.Direct3D11;

    /// <summary>
    /// Delegate type used for our WndProc wrapper, since apparently C# doesn't play very nicely with it
    /// </summary>
    /// <param name="m">The message sent via the message pump</param>
    /// <returns>True if the message was handled, false otherwise</returns>
    public delegate bool MyWndProc(ref Message m);

    /// <summary>
    /// Direct3D form - encapsulation only necessary wrap the WndProc
    /// </summary>
    public class D3DForm : Form
    {
        /// <summary>
        /// WndProc method - handle message pump interactions through this method
        /// </summary>
        public MyWndProc MyWndProc;

        /// <summary>
        /// Handle an incoming message from the Windows message pump (RESIZE, CLOSE, etc.)
        /// </summary>
        /// <param name="m">The message received from the Windows API</param>
        protected override void WndProc(ref Message m)
        {
            if (!(MyWndProc?.Invoke(ref m) ?? false))
            {
                base.WndProc(ref m);
            }
        }
    }

    /// <summary>
    /// Base for an OutsideEngine game. Exposes Windows events, performs initialization of all the
    ///  boring D3D stuff. Based heavily on Frank Luna's book, "3D Game Programming with DirectX 11"
    ///  and Eric Richard's tutorials available at http://www.richardssoftware.net/p/directx-11-tutorials.html
    /// </summary>
    public class OutsideWindowBase : DisposableClass
    {
        private bool _disposed;

        #region WinForms
        /// <summary>
        /// Windows form representing our main window
        /// </summary>
        public Form Window { get; protected set; }

        /// <summary>
        /// Pointer to the application instance. In C++, this would have been hInstance
        /// </summary>
        public IntPtr AppInst { get; protected set; }

        /// <summary>
        /// Width of the renderable area in our window
        /// </summary>
        protected int ClientWidth;

        /// <summary>
        /// Height of the renderable area in our window
        /// </summary>
        protected int ClientHeight;

        /// <summary>
        /// Aspect ratio. May be used for camera computations. Example: 4:3 would be 1.3333
        /// </summary>
        public float AspectRatio => (float)ClientWidth / ClientHeight;

        /// <summary>
        /// Caption of our application, displayed in the title bar (if one is present)
        /// </summary>
        protected string MainWindowCaption;

        /// <summary>
        /// True if the window is minimized, false otherwise
        /// </summary>
        protected bool Minimized;
        
        /// <summary>
        /// True if the window is maximized, false otherwise
        /// </summary>
        protected bool Maximized;

        /// <summary>
        /// True if the window is currently in the act of resizing
        /// </summary>
        protected bool Resizing;
        #endregion
        #region Timing
        /// <summary>
        /// Application timer for use with this OutsideEngine application
        /// </summary>
        protected GameTimer Timer;

        /// <summary>
        /// True if the application is paused, and not receiving timer updates
        /// </summary>
        protected bool AppPaused;
        #endregion
        #region Logical
        private bool _isD3DInitialized;
        private bool _running;
        private int _frameCount;
        private float _timeElapsed;
        #endregion
        #region Direct3D Members
        /// <summary>
        /// D3DDevice - represents the software interface to the hardware device (graphics card)
        /// </summary>
        protected Device Device;

        /// <summary>
        /// Context used for rendering - the immediate context. We're assuming no use of
        ///  D3D11 deferred contexts.
        /// </summary>
        protected DeviceContext ImmediateContext;

        /// <summary>
        /// COM reference to the swap chain. Holds the back buffer, used to present the scene
        /// </summary>
        protected SwapChain SwapChain;

        /// <summary>
        /// Representation of the depth/stencil buffer in the swap chain
        /// </summary>
        protected Texture2D DepthStencilBuffer;

        /// <summary>
        /// The view to our render target, used to access the back buffer
        /// </summary>
        protected RenderTargetView RenderTargetView;

        /// <summary>
        /// The view to our depth/stencil buffer
        /// </summary>
        protected DepthStencilView DepthStencilView;

        /// <summary>
        /// Viewport of our application
        /// </summary>
        protected Viewport Viewport;

        /// <summary>
        /// Format used by the back buffer (DXGI type)
        /// </summary>
        protected Format BackBufferFormat;

        /// <summary>
        /// Format used by the depth stencil buffer (DXGI type)
        /// </summary>
        protected Format DepthStencilBufferFormat;

        /// <summary>
        /// The D3D DriverType we are going to use. Should be Hardware.
        ///  Specified in the creation of our device.
        /// </summary>
        protected DriverType DriverType;

        /// <summary>
        /// Sample description, used by the depth stencil description
        /// </summary>
        protected SampleDescription SampleDescription;

        /// <summary>
        /// Creation flags for the D3D device
        /// </summary>
        protected DeviceCreationFlags DeviceCreationFlags;
        #endregion
        #region Construction and Initialization
        /// <summary>
        /// Create our OutsideWindow app, but do not initialize Direct3D.
        /// </summary>
        /// <param name="hInstance">hInstance of the application</param>
        protected OutsideWindowBase(IntPtr hInstance)
        {
            AppInst = hInstance;
            MainWindowCaption = "Outside Engine";

            ClientWidth = 800;
            ClientHeight = 600;
            Window = null;
            AppPaused = false;
            Minimized = false;
            Maximized = false;
            Resizing = false;

            _isD3DInitialized = false;

            BackBufferFormat = Format.R8G8B8A8_UNorm;
            DepthStencilBufferFormat = Format.D24_UNorm_S8_UInt;
            DeviceCreationFlags = DeviceCreationFlags.None;
#if DEBUG
            //DeviceCreationFlags |= DeviceCreationFlags.Debug;
            //DeviceCreationFlags |= DeviceCreationFlags.SingleThreaded;
#endif

            SampleDescription = new SampleDescription(1, 0);

            DriverType = DriverType.Hardware;

            Device = null;
            ImmediateContext = null;
            SwapChain = null;
            DepthStencilView = null;
            DepthStencilBuffer = null;
            RenderTargetView = null;
            Viewport = new Viewport();
            Timer = new GameTimer();
        }

        /// <summary>
        /// Initialize this Direct3D window
        /// </summary>
        /// <returns>True if the initialization of the window was successful, false otherwise</returns>
        public virtual bool Init()
        {
            if (!InitMainWindow()) { return false; }

            if (!InitDirect3D()) { return false; }

            _running = true;
            return true;
        }

        /// <summary>
        /// Initialize the Windows... window. Attach Windows elements as well
        /// </summary>
        /// <returns>True if success, false otherwise</returns>
        protected bool InitMainWindow()
        {
            try
            {
                Window = new D3DForm
                {
                    Text = MainWindowCaption,
                    Name = "OutsideWndClassName",
                    FormBorderStyle = FormBorderStyle.Sizable,
                    ClientSize = new System.Drawing.Size(ClientWidth, ClientHeight),
                    StartPosition = FormStartPosition.CenterScreen,
                    MyWndProc = WndProc,
                    MinimumSize = new System.Drawing.Size(200, 200)
                };

                Window.MouseDown += OnMouseDown;
                Window.MouseUp += OnMouseUp;
                Window.MouseMove += OnMouseMove;
                Window.MouseWheel += OnMouseWheel;
                Window.ResizeBegin += (sender, args) =>
                {
                    AppPaused = true;
                    Resizing = true;
                    Timer.Stop();
                };
                Window.ResizeEnd += (sender, args) =>
                {
                    AppPaused = false;
                    Resizing = false;
                    Timer.Start();
                    OnResize();
                };

                Window.Show();
                Window.Update();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Outside Engine - Error");
                return false;
            }
        }

        /// <summary>
        /// Initialize Direct3D device, context and swap chain
        /// </summary>
        /// <returns>True on success, false otherwise</returns>
        protected bool InitDirect3D()
        {
            try
            {
                Device = new Device(DriverType, DeviceCreationFlags);
            }
            catch (Exception ex)
            {
                MessageBox.Show("D3D11Device creation failed:\n" + ex.Message + "\n" + ex.StackTrace, "Outside Engine - Error");
                return false;
            }

            ImmediateContext = Device.ImmediateContext;
            if (Device.FeatureLevel != FeatureLevel.Level_11_0)
            {
                Console.WriteLine("Direct3D Feature Level 11 unsupported.\nSupported feature level: " + Enum.GetName(Device.FeatureLevel.GetType(), Device.FeatureLevel));
                return false;
            }

            try
            {
                Format format = BackBufferFormat;
                var sd = new SwapChainDescription
                {
                    ModeDescription = new ModeDescription(ClientWidth, ClientHeight, new Rational(60, 1), format)
                    {
                        ScanlineOrdering = DisplayModeScanlineOrdering.Unspecified,
                        Scaling = DisplayModeScaling.Unspecified
                    },
                    SampleDescription = SampleDescription,
                    Usage = Usage.RenderTargetOutput,
                    BufferCount = 1,
                    OutputHandle = Window.Handle,
                    IsWindowed = true,
                    SwapEffect = SwapEffect.Discard,
                    Flags = SwapChainFlags.None
                };

                SwapChain = new SwapChain(Device.Factory, Device, sd);
            }
            catch (Exception ex)
            {
                MessageBox.Show("SwapChain creation failed\n" + ex.Message + "\n" + ex.StackTrace, "Outside Engine - Error");
                return false;
            }

            _isD3DInitialized = true;

            OnResize();

            return true;
        }
        #endregion
        #region Windows message handling
        private const int WM_ACTIVATE = 0x06;
        private const int WM_SIZE = 0x05;
        private const int WM_DESTROY = 0x02;

        private bool WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_ACTIVATE:
                    if (m.WParam.ToInt32().LowWord() ==0 )
                    {
                        AppPaused = true;
                        Timer.Stop();
                    }
                    else
                    {
                        AppPaused = false;
                        Timer.Start();
                    }
                    return true;
                case WM_SIZE:
                    ClientWidth = m.LParam.ToInt32().LowWord();
                    ClientHeight = m.LParam.ToInt32().HighWord();
                    if (m.WParam.ToInt32() == 1) // SIZE_MINIMIZED
                    {
                        AppPaused = true;
                        Minimized = true;
                        Maximized = false;
                    }
                    else if (m.WParam.ToInt32() == 2) // SIZE_MAXIMIZED
                    {
                        AppPaused = false;
                        Minimized = false;
                        Maximized = true;
                        OnResize();
                    }
                    else if (m.WParam.ToInt32() == 0) // SIZE_RESTORED
                    {
                        if (Minimized)
                        {
                            AppPaused = false;
                            Minimized = false;
                            OnResize();
                        }
                        else if (Maximized)
                        {
                            AppPaused = false;
                            Maximized = false;
                            OnResize();
                        }
                        else if (!Resizing)
                        {
                            OnResize();
                        }
                    }
                    return true;
                case WM_DESTROY:
                    _running = false;
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Method called when the mouse wheel state is changed.
        /// </summary>
        /// <param name="sender">Object reporting the mouse wheel state change</param>
        /// <param name="e">EventArgs containing information about the event</param>
        protected virtual void OnMouseWheel(object sender, MouseEventArgs e) { }

        /// <summary>
        /// Method called when the mouse position is changed.
        /// </summary>
        /// <param name="sender">Object reporting the mouse wheel state change</param>
        /// <param name="e">EventArgs containing information about the event</param>
        protected virtual void OnMouseMove(object sender, MouseEventArgs e) { }

        /// <summary>
        /// Method called when the mouse button is released.
        /// </summary>
        /// <param name="sender">Object reporting the mouse wheel state change</param>
        /// <param name="e">EventArgs containing information about the event</param>
        protected virtual void OnMouseUp(object sender, MouseEventArgs e) { }

        /// <summary>
        /// Method called when the mouse button is pressed.
        /// </summary>
        /// <param name="sender">Object reporting the mouse wheel state change</param>
        /// <param name="e">EventArgs containing information about the event</param>
        protected virtual void OnMouseDown(object sender, MouseEventArgs e) { }
        #endregion
        #region Event Handling
        /// <summary>
        /// Method to be invoked every time the window is resized
        /// </summary>
        public virtual void OnResize()
        {
            if (!_isD3DInitialized) { return; }
            Debug.Assert(ImmediateContext != null);
            Debug.Assert(Device != null);
            Debug.Assert(SwapChain != null);

            Util.ReleaseCom(ref RenderTargetView);
            Util.ReleaseCom(ref DepthStencilView);
            Util.ReleaseCom(ref DepthStencilBuffer);

            using (var resource = Resource.FromSwapChain<Texture2D>(SwapChain, 0))
            {
                RenderTargetView = new RenderTargetView(Device, resource);
                RenderTargetView.Resource.DebugName = "Main Render Target";
            }

            var depthStencilDesc = new Texture2DDescription
            {
                Width = ClientWidth,
                Height = ClientHeight,
                MipLevels = 1,
                ArraySize = 1,
                Format = DepthStencilBufferFormat,
                SampleDescription = SampleDescription,
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };
            DepthStencilBuffer = new Texture2D(Device, depthStencilDesc) { DebugName = "DepthStencilBuffer" };
            DepthStencilView = new DepthStencilView(Device, DepthStencilBuffer);

            ImmediateContext.OutputMerger.SetTargets(DepthStencilView, RenderTargetView);

            Viewport = new Viewport(0, 0, ClientWidth, ClientHeight, 0.0f, 1.0f);

            ImmediateContext.Rasterizer.SetViewports(Viewport);

            _frameCount = 0;
            _timeElapsed = 0.0f;
        }

        /// <summary>
        /// This method is called between frames, and here is handled any world updating
        ///  for our application. This method is empty, derived classes should override it
        /// </summary>
        /// <param name="dt">The amount of time that has passed (in seconds)</param>
        public virtual void UpdateScene(float dt) { }
        #endregion
        #region Game Loop
        /// <summary>
        /// Start the application, initialize the game loop
        /// </summary>
        public void Run()
        {
            Timer.Reset();
            Timer.FrameTime = 1.0f / 60.0f;
            while (_running)
            {
                Application.DoEvents();
                Timer.Tick();

                if (!AppPaused)
                {
                    CalculateFrameRateStats();
                    UpdateScene(Timer.DeltaTime);
                    DrawScene();
                }
                else
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
            Dispose();
        }

        /// <summary>
        /// Calculate framerate measurements, add them to the main window bar
        /// </summary>
        protected void CalculateFrameRateStats()
        {
            ++_frameCount;
            if ((Timer.TotalTime - _timeElapsed) >= 1.0f)
            {
                var fps = (float)_frameCount;
                var mspf = 1000.0f / fps;

                string s = $"{MainWindowCaption}   FPS: {fps}  Frame Time: {mspf} ms";
                Window.Text = s;
                _frameCount = 0;
                _timeElapsed += 1.0f;
            }
        }

        /// <summary>
        /// Perform any actions necessary to draw a frame. Direct3D per-frame calls should
        ///  be placed in here.
        /// </summary>
        protected virtual void DrawScene() { }
        #endregion
        #region Safety (disposal)
        /// <summary>
        /// Release all COM objects and otherwise unmanaged memory
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Util.ReleaseCom(ref RenderTargetView);
                    Util.ReleaseCom(ref DepthStencilView);
                    Util.ReleaseCom(ref DepthStencilBuffer);

                    ImmediateContext?.ClearState();
                    SwapChain?.SetFullScreenState(false, null);

                    Util.ReleaseCom(ref SwapChain);
                    Util.ReleaseCom(ref ImmediateContext);
                    Util.ReleaseCom(ref Device);
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
