using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Material_Design_for_Winform
{
    public partial class MaterialCheckBox : CheckBox
    {
        #region Variables
        public event EventHandler CheckedChanged;
        Timer doAnimation = new Timer() { Interval = 16 };
        Timer doAnimation2 = new Timer() { Interval = 15 };
        Point[] Mark_Points = new Point[3];
        int[] com = new int[2];

        Rectangle R;
        StringFormat SF = new StringFormat();
        Color _borderColor = Color.Gray;

        bool firstLoad = true;
        bool isMouseIn = false;
        int aniAlpha, aniAlpha2 = 0, aniSize, aniLocation;
        Color _focusColor = Color.Teal, _markColor = Color.White;
        private bool added = false;
        #endregion
        #region Properties
        [Category("Appearance Material")]
        public Color MarkColor
        {
            get { return _markColor; }
            set { _markColor = value; this.Invalidate(); }
        }
        [Category("Appearance Material")]
        public Color CheckedColor
        {
            get { return _focusColor; }
            set { _focusColor = value; this.Invalidate(); }
        }
        [Category("Appearance Material")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; this.Invalidate(); }
        }
        #endregion
        #region Events
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (!Checked)
            {
                aniSize = 0;
                aniLocation = Helper.Round(Height / 2);
            }
            else
            {
                aniSize = Height - 10;
                aniLocation = 5;
            }

            R = new Rectangle(Height + 1, 0, Width * 2, Height - 1);
            Mark_Points[0] = new Point(5 + (int)Height / 12, Height / 2 - 1);
            Mark_Points[1] = new Point((Height - 2) / 2, Height - 9);
            Mark_Points[2] = new Point(Height - 5 - (int)Height / 12, 8);
        }
        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);
            //firstLoad = false;
            aniAlpha2 = 200;
            doAnimation.Start();
            doAnimation2.Start();
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.Invalidate();
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            isMouseIn = false;
            this.Invalidate();
        }
        protected override void OnMouseEnter(EventArgs eventargs)
        {
            base.OnMouseEnter(eventargs);
            isMouseIn = true;
            this.Invalidate();
        }
        protected override void OnMouseLeave(EventArgs eventargs)
        {
            base.OnMouseLeave(eventargs);
            if (!this.Focused)
                isMouseIn = false;
            this.Invalidate();
        }
        #endregion
        public MaterialCheckBox()
        {
            SetStyle((ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);
            DoubleBuffered = true;

            this.AutoSize = false;
            Size = new Size(165, 25);
            Font = new Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
            aniSize = 0;
            aniLocation = Height / 2;
            firstLoad = true;

            R = new Rectangle(Height, 0, Width, Height - 1);
            Mark_Points[0] = new Point(5 + (int)Height / 12, Height / 2 - 1);
            Mark_Points[1] = new Point((Height - 2) / 2, Height - 9);
            Mark_Points[2] = new Point(Height - 7, 8);
            SF.Alignment = StringAlignment.Near;
            SF.LineAlignment = StringAlignment.Far;
            doAnimation.Tick += doAnimation_Tick;
            doAnimation2.Tick += doAnimation2_Tick;
        }


        void doAnimation2_Tick(object sender, EventArgs e)
        {
            if (aniAlpha2 > 0)
                aniAlpha2 -= 5;
            else
            {
                doAnimation2.Stop();
            }
            if (!doAnimation.Enabled)
                this.Invalidate();
        }

        void doAnimation_Tick(object sender, EventArgs e)
        {
            if (Checked)
            {
                if (aniAlpha < 240)
                    aniAlpha += 24;
                if (aniLocation > 0)
                {
                    aniLocation -= 1;
                    aniSize += 2;
                }
                else
                {
                    doAnimation.Stop();
                    aniAlpha = 240;
                }
            }
            else
            {
                if (aniAlpha > 0)
                    aniAlpha -= 24;
                if (aniLocation < Helper.Round(Height / 2))
                {
                    aniLocation += 1;
                    aniSize -= 2;
                }
                else
                {
                    doAnimation.Stop();
                    aniSize = 0;
                    aniAlpha = 0;
                }
            }
            if (!doAnimation.Enabled && CheckedChanged != null)
                this.Invoke(CheckedChanged);
            this.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            var g = pevent.Graphics;
            if (Parent is MaterialInterface)
            {
                MaterialInterface mt = (MaterialInterface)Parent;
                g.Clear(mt.MainColor);
            }
            else
                g.Clear(Parent.BackColor);
            if (AutoSize)
                AutoSize = false;
            var gp = Helper.drawRoundedRec(5, 5, Height - 10, Height - 10, 1);
            g.SmoothingMode = SmoothingMode.HighQuality;

            g.FillEllipse(new SolidBrush(Color.FromArgb(aniAlpha2, Color.Gray)), 0, 0, Height, Height);
            Region region = new Region(gp);

            if (Enabled)
            {
                if (this.Focused || isMouseIn)
                    g.FillEllipse(new SolidBrush(Color.FromArgb(70, Color.Gray)), 0, 0, Height, Height);
                if (!Checked && !doAnimation.Enabled)
                    g.DrawPath(new Pen(new SolidBrush(_borderColor), 2), gp);
                else
                    g.DrawPath(new Pen(new SolidBrush(_focusColor), 2), gp);
                g.SetClip(region, CombineMode.Replace);
                g.FillRectangle(new SolidBrush(_focusColor), aniLocation, aniLocation, aniSize, aniSize);

                g.ResetClip();
                if (Checked)
                    g.DrawLines(new Pen(Color.FromArgb(aniAlpha, _markColor), 2), Mark_Points);
                else if (!doAnimation.Enabled)
                    g.DrawLines(new Pen(Color.FromArgb(aniAlpha, _markColor), 2), Mark_Points);
                g.DrawString(Text, Font, new SolidBrush(this.ForeColor), R, SF);
            }
            else
            {
                g.DrawPath(new Pen(new SolidBrush(Color.Silver), 2), gp);
                g.SetClip(region, CombineMode.Replace);
                g.FillRectangle(new SolidBrush(Color.Silver), aniLocation, aniLocation, aniSize, aniSize);

                g.ResetClip();
                if (Checked)
                    g.DrawLines(new Pen(Color.FromArgb(aniAlpha, Color.White), 2), Mark_Points);
                g.DrawString(Text, Font, new SolidBrush(Color.Silver), R, SF);
            }
        }
    }
}
