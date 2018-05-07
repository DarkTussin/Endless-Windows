using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace EndlessWindowz
{
    public partial class EEForm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private Bitmap pbImg;
        private Bitmap scImg;
        private Graphics g;
        private bool alive = true;
        private int iWidth;
        private int iHeight;
        private int iLeft;
        private int iTop;
        private Stopwatch stoppy = new Stopwatch();
        private int scCalls = 0;
        private int lastCalls = 0;
        private float fontSize = 30.0f;
        private int drawDelay = 60;
        private Random randy = new Random();

        public EEForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            scImg = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            pbImg = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            g = Graphics.FromImage(scImg);
            pictureBox1.Image = pbImg;
            startThread();
        }

        private void startThread()
        {
            //stoppy.Start();
            new Thread(new ThreadStart(() =>
            {
                while(alive)
                {
                    takeScreenshot();
                    Thread.Sleep(drawDelay);
                }
            })).Start();
        }

        private void takeScreenshot()
        {
            this.Invoke((MethodInvoker)delegate
            {
                try
                {
                    g.CopyFromScreen(
                        Screen.FromControl(this).Bounds.X, //Not Screen.PrimaryScreen
                        Screen.FromControl(this).Bounds.Y,
                        0, 0,
                        scImg.Size,
                        CopyPixelOperation.SourceCopy);
                    pictureBox1.Image = scImg;
                }
                catch { /* Throw away error in case desktop is locked */ }
            });
        }

        private void drawCalls()
        {
            using (GraphicsPath p = new GraphicsPath())
            {
                p.AddString(lastCalls + "",
                    FontFamily.GenericSansSerif,
                    (int)FontStyle.Bold,
                    g.DpiY * fontSize / 72,
                    new Point(0, 0),
                    new StringFormat());
                g.DrawPath(Pens.Black, p);
                g.FillPath(Brushes.White, p);
            }
        }

        private void EEForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            alive = false;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                if (this.FormBorderStyle == FormBorderStyle.Sizable)
                    this.FormBorderStyle = FormBorderStyle.None;
                else
                    this.FormBorderStyle = FormBorderStyle.Sizable;
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if(this.FormBorderStyle == FormBorderStyle.Sizable)
            {
                iWidth = this.Width;
                iHeight = this.Height;
                iLeft = this.Left;
                iTop = this.Top;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Left = Screen.FromControl(this).WorkingArea.Left;
                this.Top = Screen.FromControl(this).WorkingArea.Top;
                this.Width = Screen.FromControl(this).Bounds.Width;
                this.Height = Screen.FromControl(this).Bounds.Height;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.Width = iWidth;
                this.Height = iHeight;
                this.Left = iLeft;
                this.Top = iTop;
            }
        }
    }
}
