using System;
using System.Diagnostics;
using System.Drawing;
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

            Application.Idle += OnTick;

            RenderingContextFactory.Bitmap = new Bitmap(1280, 720);
            RenderArea.Image = RenderingContextFactory.Bitmap;

            FixWindowSize();

            var engineBootstrapper = new EngineBootstrapper();
            _engine = engineBootstrapper.StartNewEngine();
        }

        private void FixWindowSize()
        {
            int delta;

            Width = 0;
            Height = 0;

            while (RenderArea.Width < RenderArea.Image.Width)
            {
                delta = Width;
                Width++;
                delta -= Width;
                if (delta == 0) break;
            }

            while (RenderArea.Height < RenderArea.Image.Height)
            {
                delta = Height;
                Height++;
                delta -= Height;
                if (delta == 0) break;
            }

            Debug.WriteLine($"Window Width = {Width}");
            Debug.WriteLine($"Window Height = {Height}");

            Debug.WriteLine($"RenderArea Width = {RenderArea.Width}");
            Debug.WriteLine($"RenderArea Height = {RenderArea.Height}");
        }

        private void OnTick(object sender, EventArgs eventArgs)
        {
            PerformUpdate();
            Refresh();
            Invalidate();
        }

        private void PerformUpdate()
        {
            _engine.Update();
        }

        private void GameWindow_Load(object sender, EventArgs e)
        {
            Text = $"Geisha Engine {ProductVersion}";
        }
    }
}