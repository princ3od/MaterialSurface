using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MaterialSurface
{
    public class MaterialCard : Panel, IMaterialControl
    {
        public enum CardStyle { Elevated, Outlined }
        #region Variables

        int radius = 8, shadowOpacity = 50, shadowDepth = 2;
        bool mouseInteract = false;

        CardStyle cardStyle = CardStyle.Elevated;
        Color cardColor = Color.White;
        #endregion

        #region Properties
        [Category("Appearance Material"), Description("Effect type on click.")]
        public Color CardColor
        {
            get { return cardColor; }
            set
            {
                cardColor = value;
                Invalidate();
            }
        }
        [Category("Appearance Material"), Description("Effect type on click.")]
        public CardStyle Style
        {
            get { return cardStyle; }
            set
            {
                cardStyle = value;
                switch (cardStyle)
                {
                    case CardStyle.Elevated:
                        Margin = new Padding(5);
                        break;
                    case CardStyle.Outlined:
                        Margin = new Padding(4);
                        break;
                    default:
                        break;
                }
                Invalidate();
            }
        }
        [Category("Appearance Material"), Description("Rounded ratio.")]
        public int Radius
        {
            get
            {
                return radius;
            }
            set
            {

                radius = value;
                Invalidate();
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
        [Category("Appearance Material"), Description("Toggle card animation on mouse enter/leave.")]
        public bool MouseInteract
        {
            get { return mouseInteract; }
            set
            {
                mouseInteract = value;
            }
        }
        #endregion
        public MouseState MouseState { get; set; }
        public MaterialCard()
        {
            SetStyle((ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);
            DoubleBuffered = true;

            Margin = new Padding(5);
            BorderStyle = BorderStyle.None;
            Size = new Size(300, 100);
            Font = new Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            MouseState = MouseState.OUT;

        }
        protected override void InitLayout()
        {
            base.InitLayout();
            BackColor = Parent.BackColor;
        }
        protected override void OnParentBackColorChanged(EventArgs e)
        {
            base.OnParentBackColorChanged(e);
            BackColor = Parent.BackColor;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            GraphicsPath mainSketch = new GraphicsPath();
            switch (cardStyle)
            {
                case CardStyle.Elevated:
                    mainSketch = GraphicHelper.GetRoundedRectangle(5, 5, Width - 10, Height - 10, radius);
                    graphics.Clear(Parent.BackColor);
                    /* -- Draw shadow -- */
                    if ((MouseState == MouseState.HOVER || Focused) && mouseInteract)
                        for (int i = 0; i < shadowDepth + 1; i++)
                        {
                            var shadowSketch = GraphicHelper.GetRoundedRectangle(5 - i, 5 - i + 2, Width - 10 + i * 2, Height - 10 + i * 2, radius);
                            graphics.FillPath(new SolidBrush(Color.FromArgb(shadowOpacity + 10 - (5 * i), Color.Black)), shadowSketch);
                        }
                    else
                        for (int i = 0; i < shadowDepth; i++)
                        {
                            var shadowSketch = GraphicHelper.GetRoundedRectangle(5 - i, 5 - i + 1, Width - 10 + i * 2, Height - 10 + i * 2, radius);
                            graphics.FillPath(new SolidBrush(Color.FromArgb(shadowOpacity - (5 * i), Color.Black)), shadowSketch);
                        }
                    /* ----------------- */
                    graphics.FillPath(new SolidBrush(cardColor), mainSketch);
                    break;
                case CardStyle.Outlined:
                    mainSketch = GraphicHelper.GetRoundedRectangle(4, 4, Width - 8, Height - 8, radius + 1);
                    graphics.DrawPath(new Pen(Color.Gray, 1.4f), mainSketch);
                    if ((MouseState == MouseState.HOVER || Focused) && mouseInteract)
                        for (int i = 0; i < 4; i++)
                        {
                            var shadowSketch = GraphicHelper.GetRoundedRectangle(5 - i, 5 - i + 2, Width - 10 + i * 2, Height - 10 + i * 2, radius);
                            graphics.FillPath(new SolidBrush(Color.FromArgb(shadowOpacity - 20 - (5 * i), Color.Black)), shadowSketch);
                        }
                    graphics.FillPath(new SolidBrush(cardColor), mainSketch);
                    break;
                default:
                    break;
            }
            mainSketch.Dispose();
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
            MouseState = MouseState.OUT;
            Invalidate();
        }
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            e.Control.MouseEnter += (s, ev) =>
            {
                MouseState = MouseState.HOVER;
                Invalidate();
            };
        }
    }
}
