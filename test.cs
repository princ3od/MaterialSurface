using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Material_Design_for_Winform
{
    public partial class test : UserControl
    {

        #region Variables
        Timer doAnimation = new Timer { Interval = 1 };

        StringFormat SF = new StringFormat();
        Rectangle R;

        int MouseX, MouseY, _shadowdepth, _radius = 5;

        float animationSize = 0,maxSize,incSize;

        bool Clicked = false;

        string _text = "Text Here";

        Color _backcolor = Color.RoyalBlue;
        #endregion
        #region Properties
        [Category("Material")]
        public int ShadowDepth
        { get { return _shadowdepth; } set { _shadowdepth = value; } }

        [Category("Material")]
        public int Radius
        { get { return _radius; } set { _radius = value;  } }

        [Category("Material")]
        public string String
        { get { return _text; } set { _text = value; } }

        [Category("Material")]
        public Color BackColor
        { get { return _backcolor; } set { _backcolor = value; } }
        #endregion
        #region Events
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Graphics gp = this.CreateGraphics();
            var BG = Drawer.drawRoundedRec(8, 8, Width - 16, Height - 16, _radius);
            Region region = new Region(BG);
            gp.SetClip(region, CombineMode.Replace);
            gp.FillRectangle(new SolidBrush(Color.FromArgb(40, Color.White)), 0, 0, this.Width, this.Height);
            gp.Dispose();
        }
        void OnMouseLeave(object s, EventArgs e)
        {
            base.OnMouseLeave(e);
            this.Invalidate();
        }
        void OnMouseDown(object s ,MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.Invalidate();
            if (Width > Height) maxSize = Width+2;
            else maxSize = Height+2;
            incSize = maxSize / 12;
            //MessageBox.Show(incSize.ToString());
                if (!Clicked)
                {
                    MouseX = e.X;
                    MouseY = e.Y;
                    doAnimation.Interval =  15 + Convert.ToInt32(maxSize/8) ;
                    doAnimation.Start();
                    Clicked = true;
                }
        }
        void OnMouseUp(object s, MouseEventArgs e)
        {
            base.OnMouseUp(e);
            doAnimation.Interval = 1;         
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            R = new Rectangle(10,10,this.Width - 20,this.Height -20);
        }
        #endregion
        public test()
        {
            SetStyle((ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);
            DoubleBuffered = true;

            Size = new Size(160, 50);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            BackColor = Color.RoyalBlue;

            SF.Alignment = StringAlignment.Center;
            SF.LineAlignment = StringAlignment.Center;

            doAnimation.Tick += doAnimation_Tick;
        }

        void OnPaint(object s, PaintEventArgs e)
        {
            R = new Rectangle(10,10, this.Width-20, this.Height-20);
            if (_radius <= 0) _radius = 2;
            var G = this.CreateGraphics();
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.Clear(Parent.BackColor);

            var BG = Drawer.drawRoundedRec(10,10,this.Width-20,this.Height-20,_radius);
            Region region = new Region(BG);

            G.FillPath(new SolidBrush(_backcolor), BG);

            G.SetClip(region, CombineMode.Replace);

            G.FillEllipse(new SolidBrush(Color.FromArgb(70, Color.White)), MouseX - (animationSize / 2), MouseY - (animationSize / 2), animationSize, animationSize);
            G.DrawString(_text, this.Font, new SolidBrush(this.ForeColor), R, SF);
        }
        void doAnimation_Tick(object sender, EventArgs e)
        {
            if (animationSize < maxSize * 2.7)
            {
                animationSize += incSize;                
                this.Invalidate();
            }
            else
            {
                //MessageBox.Show(incSize.ToString());
                animationSize = 0;
                this.Invalidate();
                doAnimation.Stop();
                Clicked = false;
            }
        }
    }
}
