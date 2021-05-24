using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MaterialSurface
{
    [DefaultEvent("TextChanged")]
    public class MaterialTextfield : Control, IMaterialControl
    {
        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);
        #region Variables
        int LINE_SPACE = 8;
        #region Menu
        ContextMenuStrip contextMenuStrip1 = new ContextMenuStrip();
        ToolStripMenuItem copyToolStripMenuItem = new ToolStripMenuItem();
        ToolStripMenuItem pasteToolStripMenuItem = new ToolStripMenuItem();
        ToolStripMenuItem cutToolStripMenuItem = new ToolStripMenuItem();
        ToolStripSeparator toolStripSeparator1 = new ToolStripSeparator();
        ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem();
        ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem();
        ToolStripSeparator toolStripSeparator2 = new ToolStripSeparator();
        #endregion
        readonly Timer animationDirector = new Timer { Interval = 1 };
        TextBox mainTextbox = new TextBox();
        TextBox lbHint = new TextBox();
        TextBox lbFloating = new TextBox();

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;
        public event KeyPressEventHandler KeyPress;

        public event EventHandler GotFocus;
        public event EventHandler LostFocus;
        public event EventHandler Click;
        public event EventHandler MouseHover;
        public event EventHandler MouseEnter;
        public event EventHandler MouseLeave;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseMove;

        readonly StringFormat textAglignment = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
        readonly StringFormat countingTextAglignment = new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Near };

        bool passChar = false, multiline = false, autoCol = true, hideCaret = false, countText = false;
        Color primaryColor = Color.BlueViolet;

        int firstDot, middleDot, lastDot, incSize;

        Font floatingLabelFont;

        int topPadding, hintLocation, incLocation, fixOnMultilin = 10;
        readonly int leftPadding = 12;
        float incFontSize = 0.315f;

        string helperText = "", errorText = "";
        bool handleError = false;
        public bool hasError = false;

        public enum TextfieldStyle { None, HasFloatingLabel, HintAsFloatingLabel };
        TextfieldStyle style = TextfieldStyle.None;

        BoxType textFieldType = BoxType.Normal;

        #endregion
        #region Properties


        [Category("Additonal Material"), Description("Show counting-text label. The limit is MaxLength.")]
        public bool CountText
        {
            get => countText; set
            {
                if (!handleError)
                    handleError = true;
                countText = value;
                Invalidate(true);
            }
        }

        [Category("Additonal Material"), Description("")]
        public string HelperText
        {
            get => helperText;
            set
            {
                helperText = value;
                if (!string.IsNullOrEmpty(helperText) && !handleError)
                    handleError = true;
                Invalidate(true);
            }
        }
        [Category("Additonal Material"), Description("Textfield can inform Error, show counting-text, show heleper-text or not.")]
        public bool HandleError
        {
            get => handleError;
            set
            {
                if (!string.IsNullOrEmpty(helperText) || countText && !value)
                    return;
                handleError = value;
                OnSizeChanged(EventArgs.Empty);
            }
        }
        [Category("Appearance Material"), Description("Only display when Style property is HasFloatingLabel.")]
        public string FloatingLabelText
        {
            get { return lbFloating.Text; }
            set
            {
                lbFloating.Text = value;
                lbFloating.Width = (int)this.CreateGraphics().MeasureString(lbFloating.Text, lbFloating.Font).Width;
            }
        }
        [Category("Appearance Material"), Description("Hint-text is displayed when textfield is empty.")]
        public string HintText
        {
            get { return lbHint.Text; }
            set
            {
                lbHint.Text = value;
                lbHint.Width = (int)this.CreateGraphics().MeasureString(lbHint.Text, lbHint.Font).Width;
                if (lbHint.Width > mainTextbox.Width)
                    lbHint.Width = mainTextbox.Width;
            }
        }
        [Category("Appearance Material"), Description("TextField type.")]
        public BoxType FieldType
        {
            get
            {
                return textFieldType;
            }
            set
            {
                textFieldType = value;
                if (textFieldType == BoxType.Filled)
                {
                    autoCol = false;
                    this.BackColor = Color.WhiteSmoke;
                    mainTextbox.BackColor = Color.WhiteSmoke;
                    lbHint.BackColor = Color.WhiteSmoke;
                    lbFloating.BackColor = Color.WhiteSmoke;
                }
                else
                {
                    autoCol = true;
                    mainTextbox.BackColor = this.BackColor;
                    lbHint.BackColor = this.BackColor;
                    lbFloating.BackColor = this.BackColor;
                }
                Invalidate(true);
            }
        }
        [Category("Appearance Material"), Description("TextField style.")]
        public TextfieldStyle Style
        {
            get { return style; }
            set
            {
                style = value;
                if (style == TextfieldStyle.HasFloatingLabel)
                {
                    lbFloating.BackColor = this.BackColor;
                    this.Controls.Add(lbFloating);
                    lbHint.Font = Font;
                    lbHint.ForeColor = Color.Silver;
                    if (multiline)
                    {
                        lbHint.Location = new Point(fixOnMultilin + leftPadding, mainTextbox.Location.Y);
                        lbFloating.Location = new Point(fixOnMultilin + leftPadding, 0);
                    }
                    else
                        lbHint.Location = new Point(1 + leftPadding, mainTextbox.Location.Y);
                    if (Text.Length > 0)
                        lbHint.SendToBack();
                    lbHint.Width = (int)this.CreateGraphics().MeasureString(lbHint.Text, lbHint.Font).Width;
                    if (lbHint.Width > mainTextbox.Width)
                        lbHint.Width = mainTextbox.Width;
                }
                if (style == TextfieldStyle.None)
                {
                    if (this.Contains(lbFloating))
                        this.Controls.Remove(lbFloating);
                    lbHint.Font = Font;
                    lbHint.ForeColor = Color.Silver;
                    if (!multiline)
                        lbHint.Location = new Point(1 + leftPadding, mainTextbox.Location.Y);
                    if (Text.Length > 0)
                        lbHint.SendToBack();
                }
                if (style == TextfieldStyle.HintAsFloatingLabel)
                {
                    if (this.Contains(lbFloating))
                        this.Controls.Remove(lbFloating);
                    if (Text.Length > 0)
                    {
                        hintLocation = 0;
                        lbHint.ForeColor = Color.Gray;
                        lbHint.Font = floatingLabelFont;
                        lbHint.Location = new Point(fixOnMultilin + leftPadding, 0);
                        lbHint.Width = (int)this.CreateGraphics().MeasureString(lbHint.Text, lbHint.Font).Width;
                        if (lbHint.Width > mainTextbox.Width)
                            lbHint.Width = mainTextbox.Width;
                    }
                    else
                    {
                        hintLocation = topPadding;
                        lbHint.Location = new Point(fixOnMultilin + leftPadding, mainTextbox.Location.Y);
                        lbHint.BringToFront();
                    }
                }

            }
        }
        [Category("Appearance Material"), Description("Textfield color when getting focus.")]
        public Color PrimaryColor
        {
            get { return primaryColor; }
            set
            {
                primaryColor = value;
                Invalidate(true);
                if (style == TextfieldStyle.HintAsFloatingLabel && lbHint.ForeColor != Color.Silver && !DesignMode)
                    lbHint.ForeColor = primaryColor;
                else if (style == TextfieldStyle.HasFloatingLabel && lbFloating.ForeColor != Color.Gray && !DesignMode)
                    lbFloating.ForeColor = primaryColor;
            }
        }
        [Category("Appearance Material"), Description("Textfield backcolor scale with parent back color.")]
        public bool AutoScaleColor
        {
            get { return autoCol; }
            set { autoCol = value; this.Invalidate(true); }
        }
        [Category("Appearance Material"), Description("Hide or show the text caret.")]
        public bool ShowCaret
        {
            get { return !hideCaret; }
            set { hideCaret = !value; this.Focus(); }
        }
        [Category("Behavior")]
        public int MaxLength
        {
            get { return mainTextbox.MaxLength; }
            set { mainTextbox.MaxLength = value; Invalidate(true); }
        }
        [Category("Behavior")]
        public char PasswordChar
        {
            get { return mainTextbox.PasswordChar; }
            set { mainTextbox.PasswordChar = value; }
        }
        [Category("Behavior")]
        public bool ReadOnly
        {
            get { return mainTextbox.ReadOnly; }
            set { mainTextbox.ReadOnly = value; }
        }
        [Category("Behavior")]
        public bool HideSelection
        {
            get { return mainTextbox.HideSelection; }
            set { mainTextbox.HideSelection = value; }
        }
        public bool ShortcutsEnable
        {
            get { return mainTextbox.ShortcutsEnabled; }
            set { mainTextbox.ShortcutsEnabled = value; }
        }
        [Category("Behavior")]
        public bool Multiline
        {
            get
            { return multiline; }
            set
            {
                multiline = value;
                mainTextbox.Multiline = value;
                if (multiline)
                {
                    fixOnMultilin = topPadding / 4 + 1;
                    lbHint.Location = new Point(fixOnMultilin + leftPadding, mainTextbox.Location.Y);
                    lbFloating.Location = new Point(fixOnMultilin + leftPadding, 0);
                }
                else
                {
                    fixOnMultilin = 0;
                    lbHint.Location = new Point(fixOnMultilin + leftPadding, mainTextbox.Location.Y);
                    lbFloating.Location = new Point(1 + leftPadding, 0);
                    this.Height = mainTextbox.Height + LINE_SPACE + topPadding;
                    mainTextbox.Location = new Point(leftPadding, topPadding);
                }
                this.Invalidate(true);
            }
        }
        [Category("Behavior")]
        public bool UseSystemPasswordChar
        {
            get { return passChar; }
            set
            {
                passChar = value;
                mainTextbox.UseSystemPasswordChar = value;
                Invalidate(true);
            }
        }
        #endregion

        public MouseState MouseState { get; set; }

        public MaterialTextfield()
        {
            SetStyle((ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
            BackColor = Color.White;

            incSize = Width / 32;
            incFontSize = (this.Font.Size * 0.25f) / 8;
            firstDot = Width / 2;
            middleDot = firstDot;
            lastDot = firstDot;

            floatingLabelFont = new Font(this.Font.FontFamily, this.Font.Size * 0.75f, this.Font.Style);
            topPadding = (int)floatingLabelFont.Size * 2;
            Size = new Size(200, (int)Font.Size * 2 + LINE_SPACE + topPadding);
            OnCreate();
            animationDirector.Tick += OnAnimate;

            MouseState = MouseState.OUT;
            this.Invalidate(true);
        }
        private void OnAnimate(object sender, EventArgs e)
        {
            if (this.IsDisposed)
            {
                animationDirector.Stop();
                return;
            }    
            if (mainTextbox.Focused)
            {
                if (firstDot > leftPadding + fixOnMultilin + incSize || (firstDot > 0 && textFieldType == BoxType.Filled))
                {
                    if (style == TextfieldStyle.HintAsFloatingLabel)
                    {
                        if (hintLocation >= 2)
                            hintLocation -= incLocation;
                        if (lbHint.Font.Size - incSize > floatingLabelFont.Size)
                            lbHint.Font = new Font(this.Font.FontFamily, lbHint.Font.Size - incFontSize, this.Font.Style);
                        else
                            lbHint.Font = floatingLabelFont;
                        lbHint.Width = (int)this.CreateGraphics().MeasureString(lbHint.Text, lbHint.Font).Width;
                        if (lbHint.Width > mainTextbox.Width)
                            lbHint.Width = mainTextbox.Width;
                    }
                    firstDot -= incSize;
                    lastDot += incSize;
                }
                else
                {
                    firstDot = (textFieldType == BoxType.Normal) ? leftPadding + fixOnMultilin - 2 :
                        (textFieldType == BoxType.Filled) ? 0 : fixOnMultilin - 2;
                    lastDot = (textFieldType == BoxType.Normal) ? Width - fixOnMultilin * 2 - leftPadding :
                        (textFieldType == BoxType.Filled) ? Width : Width - fixOnMultilin * 2 + 5;
                    animationDirector.Stop();
                }
            }
            else
            {
                if (firstDot < middleDot)
                {
                    if (style == TextfieldStyle.HintAsFloatingLabel && Text.Length == 0)
                    {
                        if (hintLocation < topPadding)
                            hintLocation += incLocation;
                        if (lbHint.Font.Size + incFontSize < this.Font.Size)
                            lbHint.Font = new Font(this.Font.FontFamily, lbHint.Font.Size + incFontSize, this.Font.Style);
                        else
                            lbHint.Font = this.Font;
                        lbHint.Width = (int)this.CreateGraphics().MeasureString(lbHint.Text, lbHint.Font).Width;
                        if (lbHint.Width > mainTextbox.Width)
                            lbHint.Width = mainTextbox.Width;
                    }
                    firstDot += incSize;
                    lastDot -= incSize;
                }
                else
                {
                    firstDot = middleDot;
                    lastDot = middleDot;

                    animationDirector.Stop();
                }
            }
            this.Invalidate(true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            Pen pen = new Pen(new SolidBrush(Color.FromArgb(180, Color.Gray)), 1.5f);
            GraphicsPath mainSketch = new GraphicsPath();

            float[] dashValues = { 4, 2 };
            pen.DashPattern = dashValues;
            if (Parent is MaterialCard && autoCol)
            {
                MaterialCard mt = (MaterialCard)Parent;
                this.BackColor = mt.CardColor;
            }
            else if (autoCol && BackColor != Parent.BackColor)
                BackColor = Parent.BackColor;
            if (!Focused)
            {
                if (!this.Enabled)
                {
                    switch (textFieldType)
                    {
                        case BoxType.Normal:
                            graphics.DrawLine(pen, 0 + fixOnMultilin - 2 + leftPadding, mainTextbox.Height + LINE_SPACE - 2 + topPadding,
                                Width - fixOnMultilin * 2 - leftPadding, mainTextbox.Height + LINE_SPACE - 2 + topPadding);
                            break;
                        case BoxType.Outlined:
                            mainSketch = GraphicHelper.GetRoundedRectangle(2, topPadding / 2, Width - 4,
                                Height - topPadding / 2 - ((handleError) ? topPadding : 0) - 2, 4);
                            graphics.DrawPath(pen, mainSketch);
                            break;
                        case BoxType.Filled:
                            mainSketch = GraphicHelper.GetRoundedRectangle(0, 0, Width, Height - 2, 4);
                            graphics.FillPath(new SolidBrush(Color.WhiteSmoke), mainSketch);
                            graphics.DrawLine(pen, 0, mainTextbox.Height + LINE_SPACE - 2 + topPadding, Width, mainTextbox.Height + LINE_SPACE - 2 + topPadding);
                            if (handleError)
                            {
                                Color bgColor;
                                if (Parent is MaterialCard)
                                    bgColor = ((MaterialCard)Parent).CardColor;
                                else
                                    bgColor = Parent.BackColor;
                                graphics.FillRectangle(new SolidBrush(bgColor), -1, mainTextbox.Height + LINE_SPACE - 2 + topPadding + 1, Width + 1, Height);
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (textFieldType)
                    {
                        case BoxType.Normal:
                            if (!mainTextbox.Focused || animationDirector.Enabled)
                                graphics.DrawLine(new Pen(new SolidBrush((hasError) ? Color.Red : (MouseState == MouseState.OUT) ? Color.Gray : primaryColor),
                                   (MouseState == MouseState.OUT) ? 1.3f : 1.6f),
                                    0 + fixOnMultilin - 2 + leftPadding, mainTextbox.Height + LINE_SPACE - 2 + topPadding,
                                    Width - fixOnMultilin * 2 - leftPadding, mainTextbox.Height + LINE_SPACE - 2 + topPadding);
                            break;
                        case BoxType.Outlined:
                            mainSketch = GraphicHelper.GetRoundedRectangle(2, topPadding / 2, Width - 4,
                                Height - topPadding / 2 - ((handleError) ? topPadding : 0) - 2, 4);
                            if (mainTextbox.Focused)
                                graphics.DrawPath(new Pen((hasError) ? Color.Red : primaryColor, 2.2f), mainSketch);
                            else
                                graphics.DrawPath(new Pen((hasError) ? Color.Red : Color.DimGray, (MouseState == MouseState.OUT) ? 1.3f : 1.6f), mainSketch);
                            break;
                        case BoxType.Filled:
                            mainSketch = GraphicHelper.GetRoundedRectangle(0, 0, Width, Height - 2, 4);
                            graphics.FillPath(new SolidBrush(Color.FromArgb(Focused ? 255 : 200, Color.WhiteSmoke)), mainSketch);
                            if (!mainTextbox.Focused || animationDirector.Enabled)
                                graphics.DrawLine(new Pen(new SolidBrush((hasError) ? Color.Red : Color.Gray), (MouseState == MouseState.OUT) ? 1.3f : 1.6f),
                                    0, mainTextbox.Height + LINE_SPACE - 2 + topPadding, Width, mainTextbox.Height + LINE_SPACE - 2 + topPadding);
                            if (handleError)
                            {
                                Color bgColor;
                                if (Parent is MaterialCard)
                                    bgColor = ((MaterialCard)Parent).CardColor;
                                else
                                    bgColor = Parent.BackColor;
                                graphics.FillRectangle(new SolidBrush(bgColor), -1, mainTextbox.Height + LINE_SPACE - 2 + topPadding + 1, Width + 1, Height);
                            }
                            break;
                        default:
                            break;
                    }
                    if (hasError)
                        graphics.DrawString(errorText, floatingLabelFont, new SolidBrush(Color.Red),
                            new Rectangle(mainTextbox.Location.X, mainTextbox.Height + LINE_SPACE - 2 + topPadding + 1,
                            Width, Height), textAglignment);

                }
                if ((!hasError || !Enabled) && !string.IsNullOrEmpty(helperText))
                    graphics.DrawString(helperText, floatingLabelFont, new SolidBrush(Color.Gray),
                        new Rectangle(mainTextbox.Location.X, mainTextbox.Height + LINE_SPACE - 2 + topPadding + 1,
                        Width, Height), textAglignment);
                if (countText)
                    graphics.DrawString(string.Format("{0}/{1}", mainTextbox.Text.Length.ToString(), mainTextbox.MaxLength.ToString()),
                        floatingLabelFont, new SolidBrush((hasError && Enabled) ? Color.Red : Color.Gray),
                           new Rectangle(mainTextbox.Location.X - fixOnMultilin - 1, mainTextbox.Height + LINE_SPACE - 2 + topPadding + 1,
                           Width - (mainTextbox.Location.X + fixOnMultilin + leftPadding), Height), countingTextAglignment);
            }

            //Draw animation
            if (textFieldType != BoxType.Outlined)
            {
                graphics.DrawLine(new Pen(new SolidBrush((hasError) ? Color.Red : primaryColor), 2.6f),
                    firstDot, mainTextbox.Height + LINE_SPACE - 3 + topPadding, middleDot, mainTextbox.Height + LINE_SPACE - 3 + topPadding);
                graphics.DrawLine(new Pen(new SolidBrush((hasError) ? Color.Red : primaryColor), 2.6f),
                    middleDot, mainTextbox.Height + LINE_SPACE - 3 + topPadding, lastDot, mainTextbox.Height + LINE_SPACE - 3 + topPadding);
            }
            if (style == TextfieldStyle.HintAsFloatingLabel)
                lbHint.Location = new Point(fixOnMultilin + leftPadding, hintLocation);

            mainSketch.Dispose();
            pen.Dispose();
        }
        public void RaiseError(string errorMessage, bool setFocus = true)
        {
            if (!handleError)
                throw new Exception("HandleError not set to true.");
            errorText = errorMessage;
            if (!hasError)
            {
                hasError = true;
                if (style == TextfieldStyle.HintAsFloatingLabel)
                    lbHint.Text = string.Format("{0}*", lbHint.Text);
                else if (style == TextfieldStyle.HasFloatingLabel)
                    lbFloating.Text = string.Format("{0}*", lbFloating.Text);
            }
            Invalidate(true);
            if (!this.Focused && !mainTextbox.Focused && setFocus)
                Focus();
            else if (!setFocus)
            {
                lbFloating.ForeColor = (hasError) ? Color.Red : primaryColor;
                if (style == TextfieldStyle.HintAsFloatingLabel)
                    lbHint.ForeColor = (hasError) ? Color.Red : primaryColor;
            }
        }
        public void RemoveError()
        {
            if (hasError)
            {
                hasError = false;
                if (style == TextfieldStyle.HintAsFloatingLabel)
                {
                    lbHint.Text = lbHint.Text.Replace("*", "");
                    lbHint.ForeColor = (mainTextbox.Focused) ? ((mainTextbox.Text.Length > 0) ? primaryColor : Color.Gray) : ((mainTextbox.Text.Length > 0) ? Color.Gray : Color.Silver);
                }
                else if (style == TextfieldStyle.HasFloatingLabel)
                {
                    lbFloating.Text = lbFloating.Text.Replace("*", "");
                    lbFloating.ForeColor = (mainTextbox.Focused) ? primaryColor : Color.Gray;
                }
                Invalidate(true);
            }
        }
        void OnCreate()
        {
            mainTextbox.ForeColor = this.ForeColor;
            mainTextbox.Location = new Point(0 + leftPadding, topPadding);
            mainTextbox.Width = Width - leftPadding * 2;
            mainTextbox.BorderStyle = BorderStyle.None;
            mainTextbox.Text = Text;
            mainTextbox.SizeChanged += mainTextbox_SizeChanged;
            mainTextbox.GotFocus += mainTextbox_GotFocus;
            mainTextbox.LostFocus += mainTextbox_LostFocus;
            mainTextbox.KeyDown += mainTextbox_KeyDown;
            mainTextbox.KeyPress += mainTextbox_KeyPress;
            mainTextbox.KeyUp += mainTextbox_KeyUp;
            mainTextbox.TextChanged += mainTextbox_TextChanged;
            mainTextbox.Click += mainTextbox_Click;
            mainTextbox.MouseDown += mainTextbox_MouseDown;
            mainTextbox.MouseUp += mainTextbox_MouseUp;
            mainTextbox.MouseMove += mainTextbox_MouseMove;
            mainTextbox.MouseEnter += mainTextbox_MouseEnter;
            mainTextbox.MouseHover += mainTextbox_MouseHover;
            mainTextbox.MouseLeave += mainTextbox_MouseLeave;
            mainTextbox.ContextMenuStrip = contextMenuStrip1;

            lbHint.Text = "Hint Text";
            lbHint.Width = (int)this.CreateGraphics().MeasureString(lbHint.Text, lbHint.Font).Width;
            lbHint.Multiline = false;
            lbHint.ShortcutsEnabled = false;
            lbHint.TabStop = false;
            lbHint.BackColor = this.BackColor;
            lbHint.BorderStyle = BorderStyle.None;
            lbHint.Click += lbHint_Click;
            lbHint.MouseMove += lbHint_MouseMove;
            lbHint.MouseDown += lbHint_MouseDown;
            lbHint.MouseEnter += LbHint_MouseEnter;
            lbHint.MouseLeave += LbHint_MouseLeave;
            lbHint.ReadOnly = true;
            lbHint.ForeColor = Color.Silver;
            lbHint.GotFocus += lbHint_GotFocus;
            lbHint.Location = new Point(1 + leftPadding, mainTextbox.Location.Y);

            lbFloating.Text = "FloatingLabel";
            lbFloating.ShortcutsEnabled = false;
            lbFloating.TabStop = false;
            lbFloating.BackColor = this.BackColor;
            lbFloating.BorderStyle = BorderStyle.None;
            lbFloating.Font = floatingLabelFont;
            lbFloating.ReadOnly = true;
            lbFloating.GotFocus += lbFloating_GotFocus;
            lbFloating.Click += LbFloating_Click;
            lbFloating.MouseMove += lbFloating_MouseMove;
            lbFloating.ForeColor = Color.Gray;
            lbFloating.Location = new Point(1 + leftPadding, 0);
            lbFloating.Width = (int)this.CreateGraphics().MeasureString(lbFloating.Text, lbFloating.Font).Width;


            this.Controls.Add(lbHint);
            this.Controls.Add(mainTextbox);

            #region Menu
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.BackColor = System.Drawing.Color.White;
            this.contextMenuStrip1.AutoSize = false;
            this.contextMenuStrip1.Font = new System.Drawing.Font("Segoe UI Semibold", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripSeparator2,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(110, 130);
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.copyToolStripMenuItem.Text = "   Copy";
            copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.pasteToolStripMenuItem.Text = "   Paste";
            pasteToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.cutToolStripMenuItem.Text = "   Cut";
            cutToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(179, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.toolStripMenuItem1.Text = "   Delete";
            toolStripMenuItem1.Click += copyToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(120, 6);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(127, 22);
            this.toolStripMenuItem2.Text = "   Select All";
            toolStripMenuItem2.Click += copyToolStripMenuItem_Click;
            #endregion
        }

        private void LbHint_MouseLeave(object sender, EventArgs e)
        {
            MouseState = MouseState.OUT;
            this.Invalidate(true);
        }

        private void LbHint_MouseEnter(object sender, EventArgs e)
        {
            MouseState = MouseState.HOVER;
            this.Invalidate(true);
        }
        #region Events
        protected override void OnEnabledChanged(EventArgs e)
        {
            lbHint.Enabled = lbFloating.Enabled = this.Enabled;
            base.OnEnabledChanged(e);
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            floatingLabelFont = new Font(this.Font.FontFamily, this.Font.Size * 0.75f, this.Font.Style);
            incFontSize = (this.Font.Size * 0.25f) / 8;
            lbFloating.Font = floatingLabelFont;
            lbHint.Font = (mainTextbox.Text.Length == 0) ? this.Font : floatingLabelFont;
            fixOnMultilin = (int)floatingLabelFont.Size / 2 + 1;
            topPadding = (int)floatingLabelFont.Size * 2;
            incLocation = topPadding / 6;
            LINE_SPACE = topPadding / 2;
            if (Text.Length == 0)
                hintLocation = topPadding;
            if (multiline)
            {
                lbHint.Location = new Point(fixOnMultilin + leftPadding, mainTextbox.Location.Y);
                lbFloating.Location = new Point(fixOnMultilin + leftPadding, 0);
            }
            else
            {
                lbHint.Location = new Point(1 + leftPadding, mainTextbox.Location.Y);
                lbFloating.Location = new Point(1 + leftPadding, 0);
                this.Height = mainTextbox.Height + LINE_SPACE + topPadding;
                mainTextbox.Width = this.Width - leftPadding * 2;
                mainTextbox.Location = new Point(0 + leftPadding, topPadding);
            }
            lbFloating.Width = (int)this.CreateGraphics().MeasureString(lbFloating.Text, lbFloating.Font).Width;
            lbHint.Width = (int)this.CreateGraphics().MeasureString(lbHint.Text, lbHint.Font).Width;
            if (lbHint.Width > mainTextbox.Width)
                lbHint.Width = mainTextbox.Width;
            this.Invalidate(true);
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (mainTextbox.Focused == false)
                mainTextbox.Focus();
        }
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            if (style == TextfieldStyle.HintAsFloatingLabel && !this.Focused)
            {
                if (Text.Length > 0)
                {
                    if (!mainTextbox.Focused)
                        lbHint.ForeColor = Color.Gray;
                    lbHint.Font = floatingLabelFont;
                    hintLocation = 0;
                    lbHint.Location = new Point(fixOnMultilin + leftPadding, 0);
                }
                else if (!mainTextbox.Focused)
                {
                    lbHint.ForeColor = Color.Silver;
                    lbHint.Location = new Point(fixOnMultilin + leftPadding, mainTextbox.Location.Y);
                    if (Text.Length == 0)
                        hintLocation = topPadding;
                    lbHint.Font = Font;
                    lbHint.BringToFront();
                }
            }
            mainTextbox.Text = Text;
            if (Text.Length > 0 && style != TextfieldStyle.HintAsFloatingLabel)
                lbHint.SendToBack();
            else
            {
                lbHint.Width = (int)this.CreateGraphics().MeasureString(lbHint.Text, lbHint.Font).Width;
                if (lbHint.Width > mainTextbox.Width)
                    lbHint.Width = mainTextbox.Width;
                lbHint.BringToFront();
            }
            Invalidate(true);
        }
        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            if (textFieldType == BoxType.Filled)
                return;
            mainTextbox.BackColor = this.BackColor;
            lbHint.BackColor = this.BackColor;
            lbFloating.BackColor = this.BackColor;
        }
        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            mainTextbox.ForeColor = ForeColor;
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            incSize = Width / 32;
            firstDot = Width / 2;
            middleDot = firstDot;
            lastDot = firstDot;
            base.OnSizeChanged(e);
            if (!multiline)
                this.Height = mainTextbox.Height + LINE_SPACE + topPadding + ((handleError) ? topPadding : 0);
            else
                mainTextbox.Height = this.Height - LINE_SPACE - topPadding - ((handleError) ? topPadding : 0);
            mainTextbox.Width = this.Width - leftPadding * 2;
            if (lbHint.Width > mainTextbox.Width)
                lbHint.Width = mainTextbox.Width;
        }
        void mainTextbox_GotFocus(object sender, EventArgs e)
        {
            lbFloating.ForeColor = (hasError) ? Color.Red : primaryColor;
            if (style == TextfieldStyle.HintAsFloatingLabel)
                lbHint.ForeColor = (hasError) ? Color.Red : primaryColor;
            firstDot -= incSize * 4;
            lastDot += incSize * 4;
            animationDirector.Start();
            if (hideCaret)
                HideCaret(mainTextbox.Handle);
            GotFocus?.Invoke(sender, e);
        }
        void mainTextbox_SizeChanged(object sender, EventArgs e)
        {
            //this.Height = mainTextbox.Height + LINE_SPACE + topPadding;
        }
        void mainTextbox_Click(object sender, EventArgs e)
        {
            Click?.Invoke(sender, e);
        }

        void mainTextbox_TextChanged(object sender, EventArgs e)
        {
            Text = mainTextbox.Text;
        }

        void mainTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            KeyDown?.Invoke(sender, e);
        }
        void mainTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPress?.Invoke(sender, e);
        }
        void mainTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            KeyUp?.Invoke(sender, e);
        }
        void mainTextbox_KeyPress(object sender, KeyEventArgs e)
        {
            KeyUp?.Invoke(sender, e);
        }
        void mainTextbox_LostFocus(object sender, EventArgs e)
        {
            if (!lbHint.Focused)
                animationDirector.Start();
            lbHint.ForeColor = (hasError && mainTextbox.Text.Length != 0) ? Color.Red : (mainTextbox.Text.Length == 0) ? Color.Silver : Color.Gray;
            lbFloating.ForeColor = (hasError) ? Color.Red : Color.Gray;
            LostFocus?.Invoke(sender, e);
        }

        #region Mouse Events
        void mainTextbox_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDown?.Invoke(sender, e);
        }
        void mainTextbox_MouseUp(object sender, MouseEventArgs e)
        {
            MouseUp?.Invoke(sender, e);
        }

        void mainTextbox_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMove?.Invoke(sender, e);
        }
        void mainTextbox_MouseLeave(object sender, EventArgs e)
        {
            MouseState = MouseState.OUT;
            this.Invalidate(true);
            MouseLeave?.Invoke(sender, e);
        }

        void mainTextbox_MouseHover(object sender, EventArgs e)
        {
            MouseHover?.Invoke(sender, e);
        }

        void mainTextbox_MouseEnter(object sender, EventArgs e)
        {
            MouseState = MouseState.HOVER;
            this.Invalidate(true);
            MouseEnter?.Invoke(sender, e);
        }
        #endregion
        #endregion
        void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem ts = (ToolStripMenuItem)sender;
            switch (ts.Name)
            {
                case "copyToolStripMenuItem":
                    mainTextbox.Copy();
                    break;
                case "pasteToolStripMenuItem":
                    mainTextbox.AppendText(Clipboard.GetText());
                    mainTextbox.SelectionStart = mainTextbox.SelectionStart + Clipboard.GetText().Length;
                    break;
                case "cutToolStripMenuItem":
                    mainTextbox.Cut();
                    break;
                case "toolStripMenuItem2":
                    mainTextbox.Focus();
                    mainTextbox.SelectAll();
                    break;
                case "toolStripMenuItem1":
                    int i = mainTextbox.SelectionStart;
                    mainTextbox.Text = mainTextbox.Text.Remove(mainTextbox.SelectionStart, mainTextbox.SelectionLength);
                    mainTextbox.SelectionStart = i;
                    break;
            }
        }
        void lbHint_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip1.Show(mainTextbox, new Point(e.X, e.Y));
                mainTextbox.Focus();
            }
        }
        void lbFloating_MouseMove(object sender, MouseEventArgs e)
        {
            if (lbFloating.Focused && !mainTextbox.Focused)
                mainTextbox.Focus();
        }
        void LbFloating_Click(object sender, EventArgs e)
        {
            if (!mainTextbox.Focused)
                mainTextbox.Focus();
        }

        void lbFloating_GotFocus(object sender, EventArgs e)
        {
            HideCaret(lbFloating.Handle);
        }

        void lbHint_MouseMove(object sender, MouseEventArgs e)
        {
            if (lbHint.Focused && !mainTextbox.Focused)
                mainTextbox.Focus();
        }


        void lbHint_GotFocus(object sender, EventArgs e)
        {
            HideCaret(lbHint.Handle);
        }

        void lbHint_Click(object sender, EventArgs e)
        {
            if (!mainTextbox.Focused)
                mainTextbox.Focus();
        }
        public void Undo()
        {
            mainTextbox.Undo();
        }
        /// <summary>
        /// Clears all text from the text box control.
        /// </summary>
        public void Clear()
        {
            mainTextbox.Clear();
        }
        /// <summary>
        /// Clears information about the most recent operation from the undo buffer of the text box.
        /// </summary>
        public void ClearUndo()
        {
            mainTextbox.ClearUndo();
        }
        /// <summary>
        /// Appends text to the current text of a text box.
        /// </summary>
        /// <param name="str">String to append.</param>
        public void AppendText(string str)
        {
            mainTextbox.AppendText(str);
        }
        /// <summary>
        /// Moves the current selection in the text box to the Clipboard.
        /// </summary>
        public void Cut()
        {
            mainTextbox.Cut();
        }
        /// <summary>
        /// Copies the current selection in the text box to the Clipboard.
        /// </summary>
        public void Copy()
        {
            mainTextbox.Copy();
        }
        /// <summary>
        /// Specifies that the value of the SelectionLength property is zero so that no characters are selected in the control.
        /// </summary>
        public void DeselectAll()
        {
            mainTextbox.DeselectAll();
        }
        public void ScrollToCaret()
        {
            mainTextbox.ScrollToCaret();
        }
        public void Select(int start, int length)
        {
            mainTextbox.Select(start, length);
        }
        public void SelectAll()
        {
            mainTextbox.SelectAll();
        }
        public void ResetText()
        {
            mainTextbox.ResetText();
        }
    }

}
