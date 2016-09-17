using Geisha.Framework.Rendering;
using Geisha.Framework.Rendering.Gdi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Geisha.InitialProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Bitmap bmp = new Bitmap(640, 480);
            pictureBox1.Image = bmp;
            this.Size = bmp.Size;
            RenderingContext context = new RenderingContext(bmp);
        }
    }
}
