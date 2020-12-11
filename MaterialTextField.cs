using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace Material_Design_for_Winform
{
    [DefaultEvent("TextChanged")]
    public partial class MaterialTextField : Control
    {
        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);
        #region Variables
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
        Timer doAnimation = new Timer { Interval = 1 };
        TextBox myTextbox = new TextBox();
        TextBox lbHint = new TextBox();
        TextBox lbFloating = new TextBox();
        Label lbDis = new Label();

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;
        public event KeyPressEventHandler KeyPress;

        public event EventHandler Click;
        public event EventHandler MouseHover;
        public event EventHandler MouseEnter;
        public event EventHandler MouseLeave;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseMove;

        bool _passchar = false, multiline = false, _autoCol = true, _hidecaret = false, _disableTyper = false;
        Color _focusCol = Color.DodgerBlue;

        int x1, x2, x3, incSize;

        Font hintfont;
        StringFormat SF;

        int space, aniLocation, incLocation, _fix = 10;

        public enum ST { None, HasFloatingLabel, HintAsFloatingLabel };
        ST _style = ST.None;
        #endregion
        #region Properties
        [Category("Appearance Material"), Description("Only display when Style property is HasFloatingLabel.")]
        public string FloatingLabelText
        {
            get { return lbFloating.Text; }
            set { lbFloating.Text = value; }
        }
        [Category("Appearance Material"), Description("Hint-text is displayed when textfield is empty.")]
        public string HintText
        {
            get { return lbHint.Text; }
            set { lbHint.Text = value; lbDis.Text = value; }
        }
        [Category("Appearance Material"), Description("TextField style.")]
        public ST Style
        {
            get { return _style; }
            set
            {
                _style = value;
                if (_style == ST.HasFloatingLabel)
                {
                    lbFloating.BackColor = this.BackColor;
                    this.Controls.Add(lbFloating);
                    lbHint.Font = Font;
                    lbHint.ForeColor = Color.Silver;
                    if (multiline)
                    {
                        lbHint.Location = new Point(_fix, myTextbox.Location.Y);
                        lbFloating.Location = new Point(_fix, 0);
                    }
                    else
                    {
                        _fix = 0;
                        lbHint.Location = new Point(1, myTextbox.Location.Y);
                        lbHint.Location = new Point(1, myTextbox.Location.Y);
                    }
                    if (Text.Length > 0) lbHint.SendToBack();
                }
                if (_style == ST.None)
                {
                    if (this.Contains(lbFloating)) this.Controls.Remove(lbFloating);
                    lbHint.Font = Font;
                    lbHint.ForeColor = Color.Silver;
                    if (!multiline) lbHint.Location = new Point(1, myTextbox.Location.Y);
                    if (Text.Length > 0) lbHint.SendToBack();
                }
                if (_style == ST.HintAsFloatingLabel)
                {
                    if (this.Contains(lbFloating)) this.Controls.Remove(lbFloating);
                    if (Text.Length > 0)
                    {
                        aniLocation = 0;
                        lbHint.ForeColor = Color.Gray;
                        lbHint.Font = hintfont;
                        lbHint.Location = new Point(_fix, 0);
                    }
                    else
                    {
                        aniLocation = space;
                        lbHint.Location = new Point(_fix, myTextbox.Location.Y);
                        lbHint.BringToFront();
                    }
                }

            }
        }
        [Category("Appearance Material"), Description("Textfield color when getting focus.")]
        public Color FocusColor
        {
            get { return _focusCol; }
            set { _focusCol = value;
                Invalidate();
                if (_style == ST.HintAsFloatingLabel && lbHint.ForeColor != Color.Silver)
                    lbHint.ForeColor = _focusCol;
                else if (_style == ST.HasFloatingLabel && lbFloating.ForeColor != Color.Gray)
                    lbFloating.ForeColor = _focusCol;
            }
        }
        [Category("Appearance Material"), Description("Textfield backcolor scale with parent back color.")]
        public bool AutoScaleColor
        {
            get { return _autoCol; }
            set { _autoCol = value; this.Invalidate(); }
        }
        [Category("Appearance Material"), Description("Hide or show the text caret.")]
        public bool ShowCaret
        {
            get { return !_hidecaret; }
            set { _hidecaret = !value; this.Focus(); }
        }
        [Category("Behavior")]
        public int MaxLength
        {
            get { return myTextbox.MaxLength; }
            set { myTextbox.MaxLength = value; }
        }
        [Category("Behavior")]
        public char PasswordChar
        {
            get { return myTextbox.PasswordChar; }
            set { myTextbox.PasswordChar = value; }
        }
        [Category("Behavior")]
        public bool ReadOnly
        {
            get { return myTextbox.ReadOnly; }
            set { myTextbox.ReadOnly = value; }
        }
        [Category("Behavior")]
        public bool HideSelection
        {
            get { return myTextbox.HideSelection; }
            set { myTextbox.HideSelection = value; }
        }
        public bool ShortcutsEnable
        {
            get { return myTextbox.ShortcutsEnabled; }
            set { myTextbox.ShortcutsEnabled = value; }
        }
        [Category("Behavior")]
        public bool Multiline
        {
            get
            {
                return multiline;
            }
            set
            {
                multiline = value;
                myTextbox.Multiline = value;
                if (multiline)
                {
                    _fix = space / 4 + 1;
                    lbHint.Location = new Point(_fix, myTextbox.Location.Y);
                    lbFloating.Location = new Point(_fix, 0);
                }
                else
                {
                    _fix = 0;
                    lbHint.Location = new Point(_fix, myTextbox.Location.Y); lbFloating.Location = new Point(1, 0); this.Height = myTextbox.Height + 3 + space;
                    myTextbox.Width = this.Width;
                    myTextbox.Location = new Point(0, space);
                }
                this.Invalidate();
            }
        }
        [Category("Behavior")]
        public bool UseSystemPasswordChar
        {
            get { return _passchar; }
            set
            {
                _passchar = value;
                myTextbox.UseSystemPasswordChar = value;
                Invalidate();
            }
        }
        #endregion
        #region Events
        protected override void OnEnabledChanged(EventArgs e)
        {
            if (!Enabled)
            {
                if (_style != ST.None) lbDis.Location = new Point(-4, space);
                else lbDis.Location = new Point(-3, space);
                this.Controls.Add(lbDis);
                if (Text.Length == 0) lbDis.BringToFront();
                else lbDis.SendToBack();
            }
            else this.Controls.Remove(lbDis);
            base.OnEnabledChanged(e);

        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            hintfont = new Font(this.Font.FontFamily, this.Font.Size - 2.5f, this.Font.Style);
            lbFloating.Font = hintfont;
            lbHint.Font = this.Font;
            _fix = (int)hintfont.Size / 2 + 1;
            space = (int)hintfont.Size * 2;
            incLocation = space / 6;
            if (Text.Length == 0) aniLocation = space;
            if (multiline)
            {
                lbHint.Location = new Point(_fix, myTextbox.Location.Y);
                lbFloating.Location = new Point(_fix, 0);
            }
            else
            {
                _fix = 0;
                lbHint.Location = new Point(1, myTextbox.Location.Y); lbFloating.Location = new Point(1, 0); this.Height = myTextbox.Height + 3 + space;
                myTextbox.Width = this.Width;
                myTextbox.Location = new Point(0, space);
            }
            this.Invalidate();
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (myTextbox.Focused == false) myTextbox.Focus();
        }
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            lbDis.ForeColor = Color.Silver;
            if (_style == ST.HintAsFloatingLabel && !this.Focused)
            {
                if (Text.Length > 0)
                {
                    if (!myTextbox.Focused) lbHint.ForeColor = Color.Gray;
                    lbHint.Font = hintfont;
                    aniLocation = 0;
                    lbHint.Location = new Point(_fix, 0);
                }
                else if (!myTextbox.Focused)
                {
                    lbHint.ForeColor = Color.Silver;
                    lbHint.Location = new Point(_fix, myTextbox.Location.Y);
                    if (Text.Length == 0) aniLocation = space;
                    lbHint.Font = Font;
                    lbHint.BringToFront();
                }
            }
            myTextbox.Text = Text;
            if (Text.Length > 0 && _style != ST.HintAsFloatingLabel) lbHint.SendToBack();
            else lbHint.BringToFront();
            if (!Enabled)
            {
                if (Text.Length == 0) lbDis.BringToFront();
                else lbDis.SendToBack();
            }
        }
        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            myTextbox.BackColor = this.BackColor;
            lbHint.BackColor = this.BackColor;
            lbFloating.BackColor = this.BackColor;
        }
        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            myTextbox.ForeColor = ForeColor;
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            incSize = Width / 28;
            x1 = Width / 2;
            x2 = x1;
            x3 = x1;
            base.OnSizeChanged(e);
            if (!multiline) { this.Height = myTextbox.Height + 3 + space; myTextbox.Width = this.Width; }
            else myTextbox.Size = new Size(this.Width, this.Height - 3 - space);
            lbHint.Width = Width;
        }
        void myTextbox_GotFocus(object sender, EventArgs e)
        {
            lbFloating.ForeColor = _focusCol;
            doAnimation.Start();
            if (_hidecaret) HideCaret(myTextbox.Handle);
        }
        void myTextbox_SizeChanged(object sender, EventArgs e)
        {
            this.Height = myTextbox.Height + 3 + space;
        }
        void myTextbox_Click(object sender, EventArgs e)
        {
            if (Click != null)
                Click(sender, e);
        }

        void myTextbox_TextChanged(object sender, EventArgs e)
        {
            Text = myTextbox.Text;
        }

        void myTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyDown != null)
                KeyDown(sender, e);
        }
        void myTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPress != null)
                KeyPress(sender, e);
        }
        void myTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (KeyUp != null)
                KeyUp(sender, e);
        }
        void myTextbox_LostFocus(object sender, EventArgs e)
        {
            if (!lbHint.Focused) doAnimation.Start();
            lbFloating.ForeColor = Color.Gray;
        }

        #region Mouse Events
        void myTextbox_MouseDown(object sender, MouseEventArgs e)
        {
            if (MouseDown != null)
                MouseDown(sender, e);
        }
        void myTextbox_MouseUp(object sender, MouseEventArgs e)
        {
            if (MouseUp != null)
                MouseUp(sender, e);
        }

        void myTextbox_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseMove != null)
                MouseMove(sender, e);
        }
        void myTextbox_MouseLeave(object sender, EventArgs e)
        {
            if (MouseLeave != null)
                MouseLeave(sender, e);
        }

        void myTextbox_MouseHover(object sender, EventArgs e)
        {
            if (MouseHover != null)
                MouseHover(sender, e);
        }

        void myTextbox_MouseEnter(object sender, EventArgs e)
        {
            if (MouseEnter != null)
                MouseEnter(sender, e);
        }
        #endregion
        #endregion
        public MaterialTextField()
        {
            SetStyle((ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
            BackColor = Color.White;

            incSize = (Width - _fix) / 28;
            x1 = (Width - _fix) / 2;
            x2 = x1;
            x3 = x1;

            hintfont = new Font(this.Font.FontFamily, this.Font.Size - 2.5f, this.Font.Style);
            space = (int)hintfont.Size * 2;
            Size = new Size(200, (int)Font.Size * 2 + 3 + space);
            Create();
            doAnimation.Tick += doAnimation_Tick;

            SF = new StringFormat();
            SF.LineAlignment = StringAlignment.Near;
            SF.Alignment = StringAlignment.Near;
            this.Invalidate();
        }

        void Create()
        {
            myTextbox.ForeColor = this.ForeColor;
            myTextbox.Location = new Point(0, space);
            myTextbox.Width = Width;
            myTextbox.BorderStyle = BorderStyle.None;
            myTextbox.Text = Text;
            myTextbox.SizeChanged += myTextbox_SizeChanged;
            myTextbox.GotFocus += myTextbox_GotFocus;
            myTextbox.LostFocus += myTextbox_LostFocus;
            myTextbox.KeyDown += myTextbox_KeyDown;
            myTextbox.KeyPress += myTextbox_KeyPress;
            myTextbox.KeyUp += myTextbox_KeyUp;
            myTextbox.TextChanged += myTextbox_TextChanged;
            myTextbox.Click += myTextbox_Click;
            myTextbox.MouseDown += myTextbox_MouseDown;
            myTextbox.MouseUp += myTextbox_MouseUp;
            myTextbox.MouseMove += myTextbox_MouseMove;
            myTextbox.MouseEnter += myTextbox_MouseEnter;
            myTextbox.MouseHover += myTextbox_MouseHover;
            myTextbox.MouseLeave += myTextbox_MouseLeave;
            myTextbox.ContextMenuStrip = contextMenuStrip1;

            lbHint.Text = "Hint - PrInc3";
            lbHint.Multiline = false;
            lbHint.ShortcutsEnabled = false;
            lbHint.TabStop = false;
            lbHint.BackColor = this.BackColor;
            lbHint.BorderStyle = BorderStyle.None;
            lbHint.Click += lbHint_Click;
            lbHint.MouseMove += lbHint_MouseMove;
            lbHint.MouseDown += lbHint_MouseDown;
            lbHint.ReadOnly = true;
            lbHint.ForeColor = Color.Silver;
            lbHint.GotFocus += lbHint_GotFocus;
            if (!multiline) lbHint.Location = new Point(1, myTextbox.Location.Y);

            lbFloating.Text = "FloatingLabel";
            lbFloating.ShortcutsEnabled = false;
            lbFloating.TabStop = false;
            lbFloating.BackColor = this.BackColor;
            lbFloating.BorderStyle = BorderStyle.None;
            lbFloating.Font = hintfont;
            lbFloating.ReadOnly = true;
            lbFloating.GotFocus += lbFloating_GotFocus;
            lbFloating.Click += lbFloating_Click;
            lbFloating.MouseMove += lbFloating_MouseMove;
            lbFloating.ForeColor = Color.Gray;
            lbFloating.Location = new Point(0, 0);
            lbFloating.Width = this.Width;

            lbDis.Enabled = false;
            lbDis.Font = Font;
            lbDis.AutoSize = true;

            this.Controls.Add(lbHint);
            this.Controls.Add(myTextbox);

            #region menu
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

        void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem ts = (ToolStripMenuItem)sender;
            switch (ts.Name)
            {
                case "copyToolStripMenuItem":
                    myTextbox.Copy();
                    break;
                case "pasteToolStripMenuItem":
                    myTextbox.AppendText(Clipboard.GetText());
                    myTextbox.SelectionStart = myTextbox.SelectionStart + Clipboard.GetText().Length;
                    break;
                case "cutToolStripMenuItem":
                    myTextbox.Cut();
                    break;
                case "toolStripMenuItem2":
                    myTextbox.SelectAll();
                    break;
                case "toolStripMenuItem1":
                    int i = myTextbox.SelectionStart;
                    myTextbox.Text = myTextbox.Text.Remove(myTextbox.SelectionStart, myTextbox.SelectionLength);
                    myTextbox.SelectionStart = i;
                    break;
            }
        }
        void lbHint_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip1.Show(myTextbox, new Point(e.X, e.Y));
                myTextbox.Focus();
            }
        }
        void lbFloating_MouseMove(object sender, MouseEventArgs e)
        {
            if (lbFloating.Focused && !myTextbox.Focused)
                myTextbox.Focus();
        }

        void lbFloating_Click(object sender, EventArgs e)
        {
            if (!myTextbox.Focused)
                myTextbox.Focus();
        }

        void lbFloating_GotFocus(object sender, EventArgs e)
        {
            HideCaret(lbFloating.Handle);
        }

        void lbHint_MouseMove(object sender, MouseEventArgs e)
        {
            if (lbHint.Focused && !myTextbox.Focused)
                myTextbox.Focus();
        }


        void lbHint_GotFocus(object sender, EventArgs e)
        {
            HideCaret(lbHint.Handle);
        }

        void lbHint_Click(object sender, EventArgs e)
        {
            if (!myTextbox.Focused)
                myTextbox.Focus();
        }


        void doAnimation_Tick(object sender, EventArgs e)
        {
            if (myTextbox.Focused)
            {
                if (x1 > 0)
                {
                    if (_style == ST.HintAsFloatingLabel)
                    {
                        if (aniLocation >= 2) aniLocation -= incLocation;
                        if (lbHint.Font.Size > hintfont.Size) lbHint.Font = new Font(this.Font.FontFamily, lbHint.Font.Size - 0.3125f, this.Font.Style);
                        else lbHint.ForeColor = FocusColor;
                    }
                    x1 -= incSize;
                    x3 += incSize;
                }
                else doAnimation.Stop();
                if (multiline)
                {

                    if (x1 < _fix) x1 = _fix - 2;
                    if (x3 > Width - _fix * 2) x3 = Width - _fix * 2 + 5;
                }
            }
            else
            {
                if (x1 < x2)
                {
                    if (_style == ST.HintAsFloatingLabel && Text.Length == 0)
                    {
                        if (aniLocation < space) aniLocation += incLocation;
                        if (lbHint.Font.Size < this.Font.Size) lbHint.Font = new Font(this.Font.FontFamily, lbHint.Font.Size + 0.3125f, this.Font.Style);
                        else lbHint.ForeColor = Color.Silver;
                    }
                    else lbHint.ForeColor = Color.Silver;
                    x1 += incSize;
                    x3 -= incSize;
                    if (_style == ST.HintAsFloatingLabel && Text.Length > 0) lbHint.ForeColor = Color.Gray;

                }
                else { doAnimation.Stop(); x1 = x2; x3 = x2; }
            }
            this.Invalidate();
        }
        public void Undo()
        {
            myTextbox.Undo();
        }
        /// <summary>
        /// Clears all text from the text box control.
        /// </summary>
        public void Clear()
        {
            myTextbox.Clear();
        }
        /// <summary>
        /// Clears information about the most recent operation from the undo buffer of the text box.
        /// </summary>
        public void ClearUndo()
        {
            myTextbox.ClearUndo();
        }
        /// <summary>
        /// Appends text to the current text of a text box.
        /// </summary>
        /// <param name="str">String to append.</param>
        public void AppendText(string str)
        {
            myTextbox.AppendText(str);
        }
        /// <summary>
        /// Moves the current selection in the text box to the Clipboard.
        /// </summary>
        public void Cut()
        {
            myTextbox.Cut();
        }
        /// <summary>
        /// Copies the current selection in the text box to the Clipboard.
        /// </summary>
        public void Copy()
        {
            myTextbox.Copy();
        }
        /// <summary>
        /// Specifies that the value of the SelectionLength property is zero so that no characters are selected in the control.
        /// </summary>
        public void DeselectAll()
        {
            myTextbox.DeselectAll();
        }
        public void ScrollToCaret()
        {
            myTextbox.ScrollToCaret();
        }
        public void Select(int start, int length)
        {
            myTextbox.Select(start, length);
        }
        public void SelectAll()
        {
            myTextbox.SelectAll();
        }
        public void ResetText()
        {
            myTextbox.ResetText();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            float[] dashValues = { 4, 2 };
            if (Parent is MaterialInterface && _autoCol)
            {
                MaterialInterface mt = (MaterialInterface)Parent;
                this.BackColor = mt.MainColor;
            }
            else if (_autoCol && BackColor != Parent.BackColor)
                BackColor = Parent.BackColor;
            var G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighSpeed;

            Pen pen = new Pen(new SolidBrush(Color.FromArgb(180, Color.Gray)), 1.5f);
            pen.DashPattern = dashValues;
            if (!Focused)
            {
                if (!this.Enabled)
                {
                    G.DrawLine(pen, 0 + _fix - 2, myTextbox.Height + 1 + space, Width - _fix * 2 + 5, myTextbox.Height + 1 + space);
                    if (Text.Length == 0) lbDis.BringToFront();
                }
                else
                    G.DrawLine(new Pen(new SolidBrush(Color.FromArgb(180, Color.Gray)), 1.3f), 0 + _fix - 2, myTextbox.Height + 1 + space, Width - _fix * 2 + 5, myTextbox.Height + 1 + space);
            }
            //Draw animation
            G.DrawLine(new Pen(new SolidBrush(Color.FromArgb(180, _focusCol)), 3.5f), x1, myTextbox.Height + 2 + space, x2, myTextbox.Height + 2 + space);
            G.DrawLine(new Pen(new SolidBrush(Color.FromArgb(180, _focusCol)), 3.5f), x2, myTextbox.Height + 2 + space, x3, myTextbox.Height + 2 + space);

            if (_style == ST.HintAsFloatingLabel)
                lbHint.Location = new Point(_fix, aniLocation);
        }
    }
}
