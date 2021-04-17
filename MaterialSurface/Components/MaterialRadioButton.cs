using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MaterialSurface
{
    public class MaterialRadioButton : RadioButton, IMaterialControl
    {
        #region Variables

        readonly Timer animationDirector1 = new Timer { Interval = 16 };
        readonly Timer animationDirector2 = new Timer { Interval = 35 };

        Rectangle textArea;
        readonly StringFormat textAglignment = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        Color primaryColor = Color.BlueViolet;
        int aniAlpha = 0, aniSize, aniLocation, incLocation, incSize;

        #endregion
        #region  Properties

        [Category("Appearance Material"), Description("Check color.")]
        public Color PrimaryColor
        {
            get { return primaryColor; }
            set
            {
                primaryColor = value;
                Invalidate();
            }
        }
        #endregion
        public MouseState MouseState { get; set; }
        public MaterialRadioButton()
        {
            SetStyle((ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);
            DoubleBuffered = true;

            MinimumSize = new Size(25, 24);
            AutoSize = false;
            Size = new Size(165, 25);
            Font = new Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
            MouseState = MouseState.OUT;
            textArea = new Rectangle(Height - 2, 0, Width, Height - 1);
            aniSize = 0;
            aniLocation = Height / 2;
            incLocation = Height / 24;
            incSize = Height / 12;

            animationDirector1.Tick += OnAnimate1;
            animationDirector2.Tick += OnAnimate2;
        }
        private void OnAnimate1(object sender, EventArgs e)
        {
            if (Checked)
            {
                if (aniLocation > 8)
                {
                    aniLocation -= incLocation;
                    aniSize += incSize;
                }
                else
                {
                    if (aniSize < Height - 16)
                        aniSize += Height % 2;
                    animationDirector1.Stop();
                }
            }
            else
            {
                if (aniLocation < Convert.ToInt32(Math.Round(Height / 2.0, 0)))
                {
                    aniLocation += incLocation;
                    aniSize -= incSize;
                }
                else
                {
                    aniSize = 0;
                    aniLocation = Height / 2;
                    animationDirector1.Stop();
                }
            }
            Invalidate();
        }
        private void OnAnimate2(object sender, EventArgs e)
        {
            if (aniAlpha > 0)
                aniAlpha -= 5;
            else
            {
                animationDirector2.Stop();
            }
            if (!animationDirector1.Enabled)
                Invalidate();
        }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            Graphics graphics = pevent.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            if (Parent is IMaterialControl)
                graphics.Clear(((MaterialCard)Parent).CardColor);
            else
                graphics.Clear(Parent.BackColor);

            graphics.FillEllipse(new SolidBrush(Color.FromArgb(aniAlpha, Color.Gray)), 1, 1, Height - 2, Height - 2);

            if (Enabled)
            {
                if (Focused || MouseState == MouseState.HOVER)
                {
                    if (!Checked)
                        graphics.FillEllipse(new SolidBrush(Color.FromArgb(80, Color.Gray)), 1, 1, Height - 2, Height - 2);
                    else
                        graphics.FillEllipse(new SolidBrush(Color.FromArgb(80, primaryColor)), 1, 1, Height - 2, Height - 2);
                }
                if (!Checked && !animationDirector1.Enabled)
                    graphics.DrawEllipse(new Pen(new SolidBrush(Color.Gray), 1.8f), 6, 6, Height - 12, Height - 12);
                else
                    graphics.DrawEllipse(new Pen(new SolidBrush(PrimaryColor), 1.8f), 6, 6, Height - 12, Height - 12);
                graphics.FillEllipse(new SolidBrush(primaryColor), new Rectangle(aniLocation, aniLocation, aniSize, aniSize));
                graphics.DrawString(Text, Font, new SolidBrush(this.ForeColor), textArea, textAglignment);
            }
            else
            {
                graphics.DrawString(Text, Font, new SolidBrush(Color.Silver), textArea, textAglignment);
                graphics.DrawEllipse(new Pen(new SolidBrush(Color.Silver), 1.8f), 6, 6, Height - 12, Height - 12);
                graphics.FillEllipse(new SolidBrush(Color.Silver), new Rectangle(aniLocation, aniLocation, aniSize, aniSize));
            }
        }
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            aniAlpha = 110;
            animationDirector2.Start();
        }
        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);
            animationDirector1.Start();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (!Checked)
            {
                aniSize = 0;
                aniLocation = (int)(Height / 2);
            }
            else
            {
                aniSize = Height - 16;
                aniLocation = 8;
            }
            incLocation = Height / 24;
            incSize = Height / 12;
            textArea = new Rectangle(Height - 2, 0, Width, Height - 1);
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (!Checked)
            {
                aniSize = 0;
                aniLocation = (int)(Height / 2);
            }
            else
            {
                aniSize = Height - 16;
                aniLocation = 8;
            }
            incLocation = Height / 24;
            incSize = Height / 12;
            textArea = new Rectangle(Height - 2, 0, Width, Height - 1);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            MouseState = MouseState.HOVER;
            Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!Focused)
                MouseState = MouseState.OUT;
            Invalidate();
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            MouseState = MouseState.OUT;
            Invalidate();
        }
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }
    }
}
