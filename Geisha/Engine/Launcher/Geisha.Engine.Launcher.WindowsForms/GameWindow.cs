using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Geisha.Engine.Core;
using Geisha.Framework.Rendering.Gdi;

namespace Geisha.Engine.Launcher.WindowsForms
{
    public partial class GameWindow : Form
    {
        private readonly IEngine _engine;

        public GameWindow()
        {
            InitializeComponent();

            SetupWindow(1280, 720);

            var engineBootstrapper = new EngineBootstrapper();
            _engine = engineBootstrapper.StartNewEngine();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            _engine.Update();

            e.Graphics.CompositingMode = CompositingMode.SourceCopy;
            e.Graphics.SmoothingMode = SmoothingMode.None;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(RenderingContextFactory.Bitmap, Point.Empty);
            Invalidate();
        }

        private void SetupWindow(int width, int height)
        {
            ClientSize = new Size(width, height);

            RenderingContextFactory.Bitmap = new Bitmap(width, height);

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.DoubleBuffer, false);
            SetStyle(ControlStyles.Opaque, true);

            Debug.WriteLine($"Window Width = {Width}");
            Debug.WriteLine($"Window Height = {Height}");
        }

        private void GameWindow_Load(object sender, EventArgs e)
        {
            Text = $"Geisha Engine {ProductVersion}";
        }
    }
}