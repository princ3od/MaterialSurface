using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MaterialSurface
{
    public class ContainedButton : MaterialButton
    {
        #region Variables     
        int shadowDepth = 3, shadowOpacity = 50;


        Color effectColor = Color.White;
        #endregion

        #region Properties     
        [Category("Appearance Material"), Description("Ripple animation color.")]
        public new Color PrimaryColor
        {
            get
            {
                return primaryColor;
            }
            set
            {
                if (DesignMode && effectType != ET.Custom)
                    return;
                primaryColor = value;
            }
        }
        [Category("Appearance Material"), Description("Effect type on click.")]
        public ET EffectType
        {
            get { return effectType; }
            set
            {
                if (value == ET.Custom)
                    return;
                effectType = value;
                defaultAniOpacity = 120;
                aniDec = 5;
                if (effectType == ET.Light)
                    effectColor = Color.White;
                else if (effectType == ET.Dark)
                {
                    effectColor = Color.DimGray;
                    defaultAniOpacity = 180;
                    aniDec = 6;
                }
                aniOpacity = defaultAniOpacity;
            }
        }

        [Category("Appearance Material"), Description("The depth of shadow.")]
        public int ShawdowDepth
        {
            get
            {
                return shadowDepth;
            }
            set
            {
                if (value >= 0 && value <= 5)
                {
                    shadowDepth = value;
                    Invalidate();
                }
            }
        }
        [Category("Appearance Material"), Description("Shadow opacity, yeah.")]
        public int ShawdowOpacity
        {
            get
            {
                return shadowOpacity;
            }
            set
            {
                if (value >= 40 && value <= 60)
                {
                    shadowOpacity = value;
                    Invalidate();
                }
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
                    iconArea = new Rectangle((Width - 6) / 15 + 6, (Height - 8) / 6 + 6, Height / 2 + Height / 5 - 12, Height / 2 + Height / 5 - 12);
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

        public ContainedButton()
        {
            SetStyle((ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);
            DoubleBuffered = true;

            Size = new Size(180, 50);
            ForeColor = Color.WhiteSmoke;
            Font = new Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            MouseState = MouseState.OUT;

            drawArea = new Rectangle(6, 6, this.Width - 12, this.Height - 12);
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
            Region region = new Region();

            if (Parent is MaterialCard)
                graphics.Clear(((MaterialCard)Parent).CardColor);
            else
                graphics.Clear(Parent.BackColor);
            GraphicsPath mainSketch;
            if (this.Enabled)
            {
                mainSketch = GraphicHelper.GetRoundedRectangle(6, 6, Width - 12, Height - 12, radius);
                region = new Region(mainSketch);
                /* -- Draw shadow -- */
                if (MouseState == MouseState.HOVER || Focused)
                {
                    if (MouseState == MouseState.HOVER || MouseState == MouseState.OUT)
                        for (int i = 0; i < shadowDepth; i++)
                        {
                            var shadowSketch = GraphicHelper.GetRoundedRectangle(6 - i, 6 - i + 2, Width - 12 + i * 2, Height - 12 + i * 2, radius + 2);
                            graphics.FillPath(new SolidBrush(Color.FromArgb(shadowOpacity + 10 - (5 * i), Color.Black)), shadowSketch);
                        }
                    else
                        for (int i = 0; i < shadowDepth + 1; i++)
                        {
                            var shadowSketch = GraphicHelper.GetRoundedRectangle(6 - i, 6 - i + 2, Width - 12 + i * 2, Height - 12 + i * 2, radius + 2);
                            graphics.FillPath(new SolidBrush(Color.FromArgb(shadowOpacity + 10 - (5 * i), Color.Black)), shadowSketch);
                        }
                }
                else
                    for (int i = 0; i < shadowDepth; i++)
                    {
                        var shadowSketch = GraphicHelper.GetRoundedRectangle(6 - i, 6 - i + 1, Width - 12 + i * 2, Height - 12 + i * 2, radius + 1);
                        graphics.FillPath(new SolidBrush(Color.FromArgb(shadowOpacity - (5 * i), Color.Black)), shadowSketch);
                    }
                /* ----------------- */
                graphics.FillPath(new SolidBrush(primaryColor), mainSketch);
                graphics.SetClip(region, CombineMode.Replace);

                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.FillEllipse(new SolidBrush(Color.FromArgb(aniOpacity, effectColor)),
                    mouseX - (animationSize / 2), mouseY - (animationSize / 2), animationSize, animationSize);

                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                if (MouseState == MouseState.HOVER || Focused && Enabled)
                    graphics.FillPath(new SolidBrush(Color.FromArgb(Convert.ToInt32(Math.Round(defaultAniOpacity * 0.3, 0)), effectColor)), mainSketch);
            }
            else
            {
                mainSketch = GraphicHelper.GetRoundedRectangle(6, 6, Width - 12, Height - 12, radius);
                graphics.FillPath(new SolidBrush(Color.FromArgb(95, Color.Gray)), mainSketch);
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
            //base.OnMouseClick(e);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            MouseState = MouseState.HOVER;
            Invalidate();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Width > Height)
                maxSize = Width + 2;
            else
                maxSize = Height + 2;
            incSize = maxSize / 12;
            drawArea = new Rectangle(6, 6, this.Width - 12, this.Height - 12);
            if (icon != null)
            {
                iconArea = new Rectangle((Width - 6) / 15 + 6, (Height - 6) / 6 + 6, Height / 2 + Height / 5 - 12, Height / 2 + Height / 5 - 12);
                drawArea.X += (iconArea.X + iconArea.Height + 4);
                drawArea.Width -= (iconArea.X + iconArea.Height + 4);
            }
            this.Invalidate();
        }
    }
}

