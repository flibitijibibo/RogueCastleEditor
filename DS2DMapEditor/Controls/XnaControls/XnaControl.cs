using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DS2DEngine;
using Microsoft.Xna.Framework.Content;

namespace RogueCastleEditor
{
    public abstract class XnaControl : GraphicsDeviceControl, IControl
    {
         //XNA necessary game variables. Do not change.
        public GraphicsDevice GraphicsDevice;
        protected Camera2D m_camera;
        // We use a Stopwatch to emulate GameTime.
        protected Stopwatch watch = new Stopwatch();
        protected Texture2D m_genericTexture;
        protected ContentManager Content;
        protected GameServiceContainer services;

        public MainWindow ControllerRef { get; set; }

        public XnaControl()
        {
            LoadContent  += loadContent;
            RenderXna += xnaControl_RenderXna;
            HwndMouseMove += Mouse_MouseMove;
            HwndLButtonDown += LeftButton_MouseDown;
            HwndLButtonUp += LeftButton_MouseUp;
            HwndRButtonDown += RightButton_MouseDown;
            HwndRButtonUp += RightButton_MouseUp;
            HwndMouseWheel += MiddleButton_Scroll;
        }

        public virtual void Initialize() { }

         /// <summary>
        /// Invoked after either control has created its graphics device.
        /// </summary>
        protected virtual void loadContent(object sender, GraphicsDeviceEventArgs e)
        {
            GraphicsDevice = e.GraphicsDevice;
            m_camera = new Camera2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            // Creating a generic texture for use.
            m_genericTexture = new Texture2D(GraphicsDevice, 1, 1);
            Int32[] pixel = { 0xFFFFFF }; // White. 0xFF is Red, 0xFF0000 is Blue
            m_genericTexture.SetData<Int32>(pixel, 0, m_genericTexture.Width * m_genericTexture.Height);

            // Start elapsed game time.
            watch.Start();

            // Register the service, so components like ContentManager can find it.
            services = new GameServiceContainer();
            services.AddService(typeof(IGraphicsDeviceService), GraphicsService);
            //services.AddService<IGraphicsDeviceService>(this.GraphicsService);
            Content = new ContentManager(services, "RogueCastleEditorContent");
        }

        /// <summary>
        /// Invoked when our first control is ready to render.
        /// </summary>
        private void xnaControl_RenderXna(object sender, GraphicsDeviceEventArgs e)
        {
            Update(watch);
            Draw(watch);
        }

        protected abstract void Update(Stopwatch gameTime);

        protected abstract void Draw(Stopwatch gameTime);

        protected abstract void LeftButton_MouseDown(object sender, HwndMouseEventArgs e);

        protected abstract void LeftButton_MouseUp(object sender, HwndMouseEventArgs e);

        protected abstract void RightButton_MouseDown(object sender, HwndMouseEventArgs e);

        protected abstract void RightButton_MouseUp(object sender, HwndMouseEventArgs e);

        protected abstract void MiddleButton_Scroll(object sender, HwndMouseEventArgs e);

        protected abstract void Mouse_MouseMove(object sender, HwndMouseEventArgs e);

        public Camera2D Camera
        {
            get { return m_camera; }
        }
    }
}
