using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MaterialSurface
{
    public class ChoiceChip : RadioButton, IMaterialControl
    {
        public enum ChipStyle { Filled, Outlined };
        #region Variables
        protected readonly StringFormat textAlignment = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
        ChipStyle style = ChipStyle.Filled;
        Color primaryColor = Color.BlueViolet;
        #endregion
        #region Properties
        [Category("Appearance Material"), Description("")]
        public Color PrimaryColor
        {
            get
            {
                return primaryColor;
            }
            set
            {
                primaryColor = value;
                Invalidate();
            }
        }
        [Category("Appearance Material"), Description("")]
        public ChipStyle ChipType
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
                Invalidate();
            }
        }
        #endregion
        public MouseState MouseState { get; set; }
        public ChoiceChip()
        {
            SetStyle((ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);
            DoubleBuffered = true;

            MinimumSize = new Size(25, 24);
            AutoSize = false;
            Size = new Size(165, 30);
            Font = new Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
            MouseState = MouseState.OUT;
        }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            Graphics graphics = pevent.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            if (Parent is MaterialCard card)
            {
                graphics.Clear(card.CardColor);
            }
            else
            {
                graphics.Clear(Parent.BackColor);
            }

            GraphicsPath mainSketch = GraphicHelper.GetPillShape(0, 0, Width - 1, Height - 1, Height);


            switch (style)
            {
                case ChipStyle.Filled:
                    graphics.FillPath(new SolidBrush(Enabled ? Checked ? Color.FromArgb(100, primaryColor) : Color.WhiteSmoke :
                        Checked ? Color.FromArgb(220, Color.Gainsboro) : Color.WhiteSmoke),
                        mainSketch);
                    break;
                case ChipStyle.Outlined:
                    graphics.DrawPath(new Pen(new SolidBrush(Enabled ? Checked ? primaryColor : Color.DimGray : Color.Silver)),
                        mainSketch);
                    if (Checked)
                    {
                        graphics.FillPath(new SolidBrush(Color.FromArgb(100, Enabled ? primaryColor : Color.Gainsboro)), mainSketch);
                    }

                    break;
                default:
                    break;
            }
            if (MouseState == MouseState.HOVER)
            {
                graphics.FillPath(new SolidBrush(Color.FromArgb((Checked) ? 25 : 100, Color.Gray)), mainSketch);
            }

            if (Focused)
            {
                graphics.FillPath(new SolidBrush(Color.FromArgb(25, Color.Gray)), mainSketch);
            }

            graphics.DrawString(Text, Font, new SolidBrush((Enabled) ? Color.Black : Color.Gray),
                new RectangleF(0, 0, Width, Height), textAlignment);
            if (Checked && Enabled)
            {
                graphics.DrawString(Text, Font, new SolidBrush(Color.FromArgb(140, primaryColor)),
                new RectangleF(0, 0, Width, Height), textAlignment);
            }
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (AutoSize)
            {
                AutoSize = false;
            }

            SizeF textSize = this.CreateGraphics().MeasureString(Text, Font);
            Height = (int)(textSize.Height * 1.8f);
            Width = (int)textSize.Width + Height;
            Invalidate();
        }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Font = new Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
            Invalidate();
        }
        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);
            Invalidate();
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
            {
                MouseState = MouseState.OUT;
            }

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
    }
}
