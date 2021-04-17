using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MaterialSurface
{
    public class MaterialCheckbox : CheckBox, IMaterialControl
    {
        #region Variables
        readonly Timer animationDirector1 = new Timer() { Interval = 1 };
        readonly Timer animationDirector2 = new Timer() { Interval = 1 };

        Rectangle textArea;
        readonly StringFormat textAglignment = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };

        Color borderColor = Color.Gray, primaryColor = Color.BlueViolet, markColor = Color.White;
        int aniAlpha, aniAlpha2 = 0, aniSize, aniLocation, incLocation, incSize;

        #endregion

        #region Properties
        [Category("Appearance Material"), Description("Color of the mark.")]
        public Color MarkColor
        {
            get { return markColor; }
            set { markColor = value; this.Invalidate(); }
        }
        [Category("Appearance Material"), Description("Color on checked.")]
        public Color PrimaryColor
        {
            get { return primaryColor; }
            set { primaryColor = value; this.Invalidate(); }
        }
        [Category("Appearance Material"), Description("Yub, as the name.")]
        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; this.Invalidate(); }
        }
        #endregion
        public MouseState MouseState { get; set; }
        public MaterialCheckbox()
        {
            SetStyle((ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);
            DoubleBuffered = true;

            MinimumSize = new Size(25, 24);
            AutoSize = false;
            Size = new Size(165, 25);
            Font = new Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
            MouseState = MouseState.OUT;
            aniSize = 0;
            aniLocation = Height / 2;
            incLocation = Height / 24;
            incSize = Height / 12;

            textArea = new Rectangle(Height, 0, Width - Height / 2, Height);

            animationDirector1.Tick += OnAnimate1;
            animationDirector2.Tick += OnAnimate2;
        }
        private void OnAnimate1(object sender, EventArgs e)
        {
            if (Checked)
            {
                if (aniAlpha < 240)
                    aniAlpha += 24;
                if (aniLocation > 0)
                {
                    aniLocation -= incLocation;
                    aniSize += incSize;
                }
                else
                {
                    aniAlpha = 240;
                    animationDirector1.Stop();
                }
            }
            else
            {
                if (aniAlpha > 0)
                    aniAlpha -= 24;
                if (aniLocation < Convert.ToInt32(Math.Round(Height / 2.0, 0)))
                {
                    aniLocation += incLocation;
                    aniSize -= incSize;
                }
                else
                {
                    aniSize = 0;
                    aniLocation = Height / 2;
                    aniAlpha = 0;
                    animationDirector1.Stop();
                }
            }
            Invalidate();
        }
        private void OnAnimate2(object sender, EventArgs e)
        {
            if (aniAlpha2 > 0)
                aniAlpha2 -= 5;
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

            GraphicsPath sketch = GraphicHelper.GetRoundedRectangle(5, 5, Height - 10, Height - 10, 1);
            graphics.FillEllipse(new SolidBrush(Color.FromArgb(aniAlpha2, Color.Gray)), 0, 0, Height, Height);

            Region region = new Region(sketch);

            if (Enabled)
            {
                if (Focused || MouseState == MouseState.HOVER)
                {
                    if (!Checked)
                        graphics.FillEllipse(new SolidBrush(Color.FromArgb(90, Color.Gray)), 0, 0, Height, Height);
                    else
                        graphics.FillEllipse(new SolidBrush(Color.FromArgb(90, primaryColor)), 0, 0, Height, Height);
                }
                if (!Checked && !animationDirector1.Enabled)
                    graphics.DrawPath(new Pen(new SolidBrush(borderColor), 2), sketch);
                else
                    graphics.DrawPath(new Pen(new SolidBrush(primaryColor), 2), sketch);
                graphics.SetClip(region, CombineMode.Replace);
                graphics.FillRectangle(new SolidBrush(primaryColor), aniLocation, aniLocation, aniSize, aniSize);

                graphics.ResetClip();
                if (Checked)
                {
                    switch (CheckState)
                    {
                        case CheckState.Checked:
                            GraphicHelper.DrawSymbolMark(graphics, new Pen(Color.FromArgb(aniAlpha, markColor), 2), 0, 0, Height, Height, 8);
                            break;
                        case CheckState.Indeterminate:
                            graphics.DrawLine(new Pen(Color.FromArgb(aniAlpha, markColor), 2), 5 + (int)Height / 12, (int)(Height / 2), Height - 7, (int)(Height / 2));
                            break;
                        default:
                            break;
                    }
                }
                graphics.DrawString(Text, Font, new SolidBrush(this.ForeColor), textArea, textAglignment);
            }
            else
            {
                graphics.DrawPath(new Pen(new SolidBrush(Color.Silver), 2), sketch);
                graphics.SetClip(region, CombineMode.Replace);
                graphics.FillRectangle(new SolidBrush(Color.Silver), aniLocation, aniLocation, aniSize, aniSize);

                graphics.ResetClip();
                if (Checked)
                {
                    switch (CheckState)
                    {
                        case CheckState.Checked:
                            GraphicHelper.DrawSymbolMark(graphics, new Pen(Color.White, 2), 0, 0, Height, Height, 7);
                            break;
                        case CheckState.Indeterminate:
                            graphics.DrawLine(new Pen(Color.White, 2), 5 + (int)Height / 12, (int)(Height / 2), Height - 7, (int)(Height / 2));
                            break;
                        default:
                            break;
                    }
                }
                graphics.DrawString(Text, Font, new SolidBrush(Color.Silver), textArea, textAglignment);
            }

            sketch.Dispose();
            region.Dispose();
        }
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            aniAlpha2 = 200;
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
                aniSize = Height - 10;
                aniLocation = 5;
            }
            incLocation = Height / 24;
            incSize = Height / 12;
            textArea = new Rectangle(Height, 0, Width - Height / 2, Height);
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
