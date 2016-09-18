using Geisha.Framework.Rendering;
using Geisha.Framework.Rendering.Gdi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Geisha.InitialProject
{
    public partial class Form1 : Form
    {
        private readonly ITexture _dot;
        private readonly IRenderer2D _renderer;
        private double _dotX = 0;
        private readonly Stopwatch _stopwatch;

        public Form1()
        {
            InitializeComponent();

            Bitmap bmp = new Bitmap(640, 480);
            pictureBox1.Image = bmp;
            RenderingContext context = new RenderingContext(bmp);
            _renderer = new Renderer2D(context);

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
            Debug.WriteLine($"Time frame: {_stopwatch.ElapsedMilliseconds} FPS: {1000 / _stopwatch.ElapsedMilliseconds}");
            _stopwatch.Restart();
            _dotX += 0.005 * dt;
            _renderer.Clear();
            _renderer.Render(_dot, (int)((Math.Sin(_dotX) + 1) * 200), (int)((Math.Sin(_dotX + (Math.PI) / 2) + 1) * 200));
            _renderer.Render(_dot, (int)((Math.Sin(_dotX) + 1) * 50) + 400, (int)((Math.Sin(_dotX + 2 + (Math.PI) / 2) + 1) * 200));
            Refresh();
            Invalidate();
            //Thread.Sleep(7);
        }
    }
}
