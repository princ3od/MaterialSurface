using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MaterialSurface
{
    public class MaterialComboBox : ComboBox, IMaterialControl
    {
        #region Variables
        const string TEXT_TO_MEASURE = "DUONG BINH TRONG";
        readonly StringFormat textAglignment = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };


        readonly Timer animationDirector = new Timer { Interval = 1 };

        Color primaryColor = Color.BlueViolet;
        Font floatingLabelFont, hintFont;

        int firstDot, middleDot, lastDot, incSize, curIndex = -1;
        int topPadding, hintLocation, incLocation;
        float incFontSize = 0.315f;
        string hintText = "";
        bool autoSize = false;

        readonly Point[] trianglePoints = new Point[3];

        BoxType comboBoxType = BoxType.Normal;
        #endregion
        #region  Properties

        [Category("Appearance Material"), Description("Primary color.")]
        public Color PrimaryColor
        {
            get { return primaryColor; }
            set
            {
                primaryColor = value;
                Invalidate();
            }
        }
        [Category("Appearance Material"), Description("Hint-text is displayed when textfield is empty.")]
        public string HintText
        {
            get { return hintText; }
            set
            {
                hintText = value;
                if (autoSize)
                    ReCalculateWidth(hintText);
                Invalidate();
            }
        }
        [Category("Appearance Material"), Description("")]
        public BoxType ComboBoxType
        {
            get { return comboBoxType; }
            set
            {
                comboBoxType = value;
                Invalidate();
            }
        }
        [Category("Appearance Material"), Description("")]
        public bool AutoSizing
        {
            get { return autoSize; }
            set
            {
                autoSize = value;
                ReCalculateWidth();
            }
        }

        #endregion
        public MouseState MouseState { get; set; }
        public MaterialComboBox()
        {
            SetStyle((ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);
            DoubleBuffered = true;

            Font = new Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular);
            MouseState = MouseState.OUT;

            incSize = Width / 24;
            incFontSize = (this.Font.Size * 0.25f) / 8;
            firstDot = Width / 2;
            middleDot = firstDot;
            lastDot = firstDot;

            DrawMode = DrawMode.OwnerDrawVariable;
            DropDownStyle = ComboBoxStyle.DropDownList;
            DropDownWidth = Width;
            MaxDropDownItems = 4;

            hintLocation = Height / 2 + topPadding / 2 - 2;
            floatingLabelFont = new Font(this.Font.FontFamily, this.Font.Size * 0.75f, this.Font.Style);
            topPadding = (int)floatingLabelFont.Size * 2;
            ItemHeight = (int)this.CreateGraphics().MeasureString(TEXT_TO_MEASURE, this.Font).Height + topPadding;

            trianglePoints[0] = new Point(Width - 8 - 10, Height / 2 + topPadding / 2 - 4 - 2);
            trianglePoints[1] = new Point(Width - 8 - 5, Height / 2 + topPadding / 2 + 2 - 2);
            trianglePoints[2] = new Point(Width - 8, Height / 2 + topPadding / 2 - 4 - 2);
            animationDirector.Tick += OnAnimate;
        }

        private void OnAnimate(object sender, EventArgs e)
        {
            if (Focused)
            {
                if (firstDot > 0)
                {
                    if (hintFont.Size - incFontSize > floatingLabelFont.Size)
                        hintFont = new Font(hintFont.FontFamily, hintFont.Size - incFontSize, hintFont.Style);
                    if (hintLocation - incLocation > topPadding / 2)
                        hintLocation -= incLocation;
                    firstDot -= incSize;
                    lastDot += incSize;
                }
                else
                {
                    hintFont = floatingLabelFont;
                    hintLocation = topPadding / 2;
                    animationDirector.Stop();
                }
            }
            else
            {
                if (firstDot < middleDot)
                {
                    if ((SelectedItem == null) && hintFont.Size + incFontSize < this.Font.Size)
                        hintFont = new Font(hintFont.FontFamily, hintFont.Size + incFontSize, hintFont.Style);
                    if ((SelectedItem == null) && hintLocation + incLocation < Height / 2 + topPadding / 2 - 2)
                        hintLocation += incLocation;
                    firstDot += incSize;
                    lastDot -= incSize;
                }
                else
                {
                    if (SelectedItem == null)
                    {
                        hintFont = this.Font;
                        hintLocation = Height / 2 + topPadding / 2 - 2;
                    }
                    firstDot = middleDot;
                    lastDot = middleDot;

                    animationDirector.Stop();
                }
            }
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            Pen pen = new Pen(new SolidBrush(Color.FromArgb(180, Color.Gray)), 1.5f);
            GraphicsPath mainSketch = new GraphicsPath();
            Color ParentColor;
            float[] dashValues = { 4, 2 };
            pen.DashPattern = dashValues;

            if (Parent is IMaterialControl)
                ParentColor = ((MaterialCard)Parent).CardColor;
            else
                ParentColor = Parent.BackColor;

            graphics.Clear(ParentColor);
            if (Enabled)
            {
                switch (comboBoxType)
                {
                    case BoxType.Normal:
                        graphics.DrawLine(new Pen(new SolidBrush((MouseState == MouseState.OUT) ? Color.Gray : primaryColor),
                            (MouseState == MouseState.OUT) ? 1.3f : 1.6f),
                            0, Height - 4, Width, Height - 4);
                        break;
                    case BoxType.Outlined:
                        mainSketch = GraphicHelper.GetRoundedRectangle(2, topPadding / 2, Width - 4, Height - topPadding / 2 - 2, 4);
                        if (Focused)
                            graphics.DrawPath(new Pen(primaryColor, 2.2f), mainSketch);
                        else
                            graphics.DrawPath(new Pen((MouseState == MouseState.OUT) ? Color.DimGray : primaryColor,
                                (MouseState == MouseState.OUT) ? 1.3f : 1.6f), mainSketch);
                        break;
                    case BoxType.Filled:
                        mainSketch = GraphicHelper.GetRoundedRectangle(0, 0, Width, Height - 2, 4);
                        graphics.FillPath(new SolidBrush(Color.FromArgb(Focused ? 255 : 200, Color.WhiteSmoke)), mainSketch);
                        graphics.DrawLine(new Pen(new SolidBrush((MouseState == MouseState.OUT) ? Color.Gray : primaryColor),
                            (MouseState == MouseState.OUT) ? 1.3f : 1.6f),
                            0, Height - 4, Width, Height - 4);

                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (comboBoxType)
                {
                    case BoxType.Normal:
                        graphics.DrawLine(pen, 0, Height - 4, Width, Height - 4);
                        break;
                    case BoxType.Outlined:
                        mainSketch = GraphicHelper.GetRoundedRectangle(2, topPadding / 2, Width - 4, Height - topPadding / 2 - 2, 4);
                        graphics.DrawPath(pen, mainSketch);
                        break;
                    case BoxType.Filled:
                        mainSketch = GraphicHelper.GetRoundedRectangle(0, 0, Width, Height - 2, 4);
                        graphics.FillPath(new SolidBrush(Color.WhiteSmoke), mainSketch);
                        graphics.DrawLine(pen, 0, Height - 4, Width, Height - 4);
                        break;
                    default:
                        break;
                }
            }
            if (comboBoxType == BoxType.Outlined)
            {
                SizeF hintRec = this.CreateGraphics().MeasureString(hintText, hintFont);
                graphics.FillRectangle(new SolidBrush(ParentColor), 7, hintLocation / 2, hintRec.Width, hintRec.Height);
            }
            graphics.DrawString(hintText, hintFont, new SolidBrush(Focused ? primaryColor : (SelectedItem != null) ? Color.Gray : Color.Silver),
                8, hintLocation, textAglignment);

            if (SelectedItem != null)
                graphics.DrawString(SelectedItem.ToString(), this.Font, new SolidBrush(ForeColor),
                    8, Height / 2 + topPadding / 2 - 2, textAglignment);

            GraphicsPath triangle = new GraphicsPath();
            triangle.AddLine(trianglePoints[0], trianglePoints[1]);
            triangle.AddLine(trianglePoints[1], trianglePoints[2]);
            triangle.AddLine(trianglePoints[0], trianglePoints[2]);
            triangle.CloseFigure();
            graphics.FillPath(new SolidBrush((Focused || MouseState == MouseState.HOVER) ? primaryColor : Color.Gray), triangle);
            if (comboBoxType != BoxType.Outlined)
            {
                graphics.DrawLine(new Pen(new SolidBrush(primaryColor), 2.6f), firstDot, Height - 4, middleDot, Height - 4);
                graphics.DrawLine(new Pen(new SolidBrush(primaryColor), 2.6f), middleDot, Height - 4, lastDot, Height - 4);
            }

            mainSketch.Dispose();
            pen.Dispose();
        }
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            if (e.Index < 0 || e.Index > Items.Count || !Focused)
                return;

            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);

            if (e.State.HasFlag(DrawItemState.Focus) || curIndex == e.Index) // Focus == hover
            {
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(75, Color.Gray)), e.Bounds);
            }
            if (curIndex == e.Index)
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Gray)), e.Bounds);


            graphics.DrawString(" " + Items[e.Index].ToString(), this.Font, new SolidBrush(Color.Black), e.Bounds, textAglignment);

        }
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (autoSize)
                ReCalculateWidth();
        }
        protected override void OnDropDownStyleChanged(EventArgs e)
        {
            base.OnDropDownStyleChanged(e);
            DropDownStyle = ComboBoxStyle.DropDownList;
        }
        protected override void OnDropDown(EventArgs e)
        {
            base.OnDropDown(e);
            curIndex = SelectedIndex;
            int temp = trianglePoints[1].Y;
            trianglePoints[1].Y = (!DroppedDown) ? trianglePoints[0].Y : trianglePoints[1].Y;
            trianglePoints[0].Y = trianglePoints[2].Y = (!DroppedDown) ? temp : trianglePoints[0].Y;
        }
        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            int temp = trianglePoints[1].Y;
            trianglePoints[1].Y = (!DroppedDown) ? trianglePoints[0].Y : trianglePoints[1].Y;
            trianglePoints[0].Y = trianglePoints[2].Y = (!DroppedDown) ? temp : trianglePoints[0].Y;
            Invalidate();
        }
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            if (animationDirector.Enabled)
                return;
            if (SelectedIndex > -1)
            {
                hintFont = floatingLabelFont;
                hintLocation = topPadding / 2;
            }
            else
            {
                hintFont = this.Font;
                hintLocation = Height / 2 + topPadding / 2 - 2;
            }
            Invalidate();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            incSize = Width / 24;
            firstDot = Width / 2;
            middleDot = firstDot;
            lastDot = firstDot;
            if (SelectedItem == null)
                hintLocation = Height / 2 + topPadding / 2 - 2;
            trianglePoints[0] = new Point(Width - 8 - 10, Height / 2 + topPadding / 2 - 4 - 2);
            trianglePoints[1] = new Point(Width - 8 - 5, Height / 2 + topPadding / 2 + 2 - 2);
            trianglePoints[2] = new Point(Width - 8, Height / 2 + topPadding / 2 - 4 - 2);
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            floatingLabelFont = new Font(this.Font.FontFamily, this.Font.Size * 0.75f, this.Font.Style);
            hintFont = (SelectedItem != null) ? floatingLabelFont : this.Font;
            incFontSize = (this.Font.Size * 0.25f) / 8;
            topPadding = (int)floatingLabelFont.Size * 2;
            incLocation = topPadding / 8;
            ItemHeight = (int)this.CreateGraphics().MeasureString(TEXT_TO_MEASURE, this.Font).Height + topPadding;
            Invalidate();
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            MouseState = MouseState.HOVER;
            firstDot -= incSize * 3;
            lastDot += incSize * 3;
            animationDirector.Start();
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            MouseState = MouseState.OUT;
            animationDirector.Start();
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
        void ReCalculateWidth()
        {
            int width = 150;
            SizeF textSize = this.CreateGraphics().MeasureString(hintText, Font);
            int w = (int)textSize.Width + 30;
            if (w > width)
                width = w;
            foreach (var item in this.Items)
            {
                textSize = this.CreateGraphics().MeasureString(item.ToString(), Font);
                w = (int)textSize.Width + 30;
                if (w > width)
                    width = w;
            }
            Width = width;
            Invalidate();
        }
        void ReCalculateWidth(string text)
        {
            SizeF textSize = this.CreateGraphics().MeasureString(text, Font);
            int w = (int)textSize.Width + 30;
            if (w > Width)
                Width = w;
            else
                ReCalculateWidth();
            Invalidate();
        }
    }
}
