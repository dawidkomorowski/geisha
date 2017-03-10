using Geisha.Framework.Rendering;
using Geisha.Framework.Rendering.Gdi;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Geisha.Framework.Input;
using Geisha.Framework.Input.Wpf;

namespace Geisha.InitialProject
{
    public partial class Form1 : Form
    {
        private const double PlayerVelocity = 0.3;
        private const double Velocity = 0.005;
        private readonly ITexture _dot;
        private readonly IRenderer2D _renderer;
        private double _dotX;
        private readonly Stopwatch _stopwatch;
        private readonly InputProvider _inputProvider;
        private double _playerDotX;
        private double _playerDotY;

        public Form1()
        {
            InitializeComponent();

            _inputProvider = new InputProvider(new KeyMapper());

            var bmp = new Bitmap(640, 480);
            pictureBox1.Image = bmp;
            var renderingContextFactory = new RenderingContextFactory();
            RenderingContextFactory.Bitmap = bmp;
            _renderer = new Renderer2D(renderingContextFactory);

            _dot = null;
            using (Stream stream = new FileStream("Dot.png", FileMode.Open))
            {
                _dot = _renderer.CreateTexture(stream);
            }

            Application.Idle += OnTick;

            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
            double dt = _stopwatch.ElapsedMilliseconds;
            _stopwatch.Restart();

            HandleInput(dt);

            _dotX += Velocity * dt;

            _renderer.Clear();
            _renderer.Render(_dot, (int)(_playerDotX), (int)(_playerDotY));
            _renderer.Render(_dot, (int)((Math.Sin(_dotX) + 1) * 200), (int)((Math.Sin(_dotX + (Math.PI) / 2) + 1) * 200));
            _renderer.Render(_dot, (int)((Math.Sin(_dotX) + 1) * 50) + 400, (int)((Math.Sin(_dotX + 2 + (Math.PI) / 2) + 1) * 200));

            Refresh();
            Invalidate();

            const int maxFrametime = 16;
            var millisecondsTimeout = (int)(maxFrametime - _stopwatch.ElapsedMilliseconds);
            Thread.Sleep(millisecondsTimeout > 0 ? millisecondsTimeout : 0);
            Debug.WriteLine($"Frametime: {_stopwatch.ElapsedMilliseconds} FPS: {1000 / _stopwatch.ElapsedMilliseconds}");
        }

        private void HandleInput(double dt)
        {
            var hardwareInput = _inputProvider.Capture();
            if (hardwareInput.KeyboardInput[Key.Down]) _playerDotY += PlayerVelocity * dt;
            if (hardwareInput.KeyboardInput[Key.Up]) _playerDotY -= PlayerVelocity * dt;
            if (hardwareInput.KeyboardInput[Key.Left]) _playerDotX -= PlayerVelocity * dt;
            if (hardwareInput.KeyboardInput[Key.Right]) _playerDotX += PlayerVelocity * dt;
        }
    }
}
