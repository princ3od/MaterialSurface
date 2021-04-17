using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MaterialSurface
{
    public class TextButton : MaterialButton
    {
        #region Properties      
        [Category("Appearance Material"), Description("Effect type on click.")]
        public ET EffectType
        {
            get { return effectType; }
            set
            {
                effectType = value;
                defaultAniOpacity = 140;
                aniDec = 5;
                if (effectType == ET.Light)
                    primaryColor = Color.White;
                else if (effectType == ET.Dark)
                {
                    primaryColor = Color.DimGray;
                    defaultAniOpacity = 180;
                    aniDec = 6;
                }
                aniOpacity = defaultAniOpacity;
            }
        }
        [Category("Appearance Material"), Description("Heading icon (before text).")]
        public Image Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                if (icon != null)
                {
                    iconArea = new Rectangle(Width / 15, Height / 6, Height / 2 + Height / 5, Height / 2 + Height / 5);
                    drawArea.X = (iconArea.X + iconArea.Height + 4);
                    drawArea.Width = Width - (iconArea.X + iconArea.Height + 4);
                }
                else
                {
                    drawArea.X = 0;
                    drawArea.Width = Width;
                }
                this.Invalidate();
            }
        }
        #endregion
        public TextButton()
        {
            SetStyle((ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);
            DoubleBuffered = true;

            Size = new Size(160, 40);
            ForeColor = Color.BlueViolet;
            Font = new Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            MouseState = MouseState.OUT;

            drawArea = new Rectangle(1, 1, this.Width - 2, this.Height - 2);
            maxSize = Width + 2;
            incSize = maxSize / 12;

            animationDirector.Tick += OnAnimate;
        }
        protected override void OnAnimate(object sender, EventArgs e)
        {
            if (animationSize < maxSize * 2.2)
            {
                aniOpacity -= aniDec;
                aniOpacity = (aniOpacity < 0) ? 0 : aniOpacity;
                animationSize += incSize;
                this.Invalidate();
            }
            else
            {
                aniOpacity = defaultAniOpacity;
                animationSize = 0;
                Invalidate();
                animationDirector.Stop();
                base.OnClick(_eventArgs);
                base.OnMouseClick(_mouseClickArgs);
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            Graphics graphics = pevent.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            GraphicsPath mainSketch = new GraphicsPath();
            Region region = new Region();

            if (Parent is MaterialCard)
                graphics.Clear(((MaterialCard)Parent).CardColor);
            else
                graphics.Clear(Parent.BackColor);
            if (this.Enabled)
            {
                mainSketch = GraphicHelper.GetRoundedRectangle(1, 1, Width - 2, Height - 2, radius);
                region = new Region(mainSketch);
                graphics.SetClip(region, CombineMode.Replace);

                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.FillEllipse(new SolidBrush(Color.FromArgb(aniOpacity, primaryColor)),
                    mouseX - (animationSize / 2), mouseY - (animationSize / 2), animationSize, animationSize);

                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                if (MouseState == MouseState.HOVER || Focused && Enabled)
                    graphics.FillPath(new SolidBrush(Color.FromArgb(Convert.ToInt32(Math.Round(defaultAniOpacity * 0.3, 0)), primaryColor)), mainSketch);
            }
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            if (icon != null)
                graphics.DrawImage(icon, iconArea);
            if (this.Enabled)
                graphics.DrawString(Text, this.Font, new SolidBrush(this.ForeColor), drawArea, textAlignment);
            else
                graphics.DrawString(Text, this.Font, new SolidBrush(Color.FromArgb(125, Color.Gray)), drawArea, textAlignment);

            mainSketch.Dispose();
            region.Dispose();
        }
        protected override void OnClick(EventArgs e)
        {
            if (isPerformClick)
            {
                isPerformClick = false;
                base.OnClick(e);
            }
            else
            {
                _eventArgs = e;
                if (e == EventArgs.Empty)
                {
                    aniOpacity = defaultAniOpacity - 20;
                    mouseX = Width / 2;
                    mouseY = Height / 2;
                    animationSize = incSize * 2;
                    animationDirector.Start();
                }
            }
            Focus();
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            aniOpacity = defaultAniOpacity - 20;
            mouseX = e.X;
            mouseY = e.Y;
            animationSize = incSize * 2;
            animationDirector.Start();
            _mouseClickArgs = e;
            Focus();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Width > Height)
                maxSize = Width + 2;
            else
                maxSize = Height + 2;
            incSize = maxSize / 12;
            drawArea = new Rectangle(1, 1, this.Width - 2, this.Height - 2);
            if (icon != null)
            {
                iconArea = new Rectangle(Width / 15, Height / 6, Height / 2 + Height / 5, Height / 2 + Height / 5);
                drawArea.X += (iconArea.X + iconArea.Height + 4);
                drawArea.Width -= (iconArea.X + iconArea.Height + 4);
            }
            this.Invalidate();
        }
        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            if (EffectType == ET.Custom)
                primaryColor = ForeColor;
        }
    }
}