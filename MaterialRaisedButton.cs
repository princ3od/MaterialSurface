using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Material_Design_for_Winform
{
    public partial class MaterialRaisedButton : Control
    {
        #region Variables
        public event EventHandler Click;
        Timer doAnimation = new Timer { Interval = 15 };


        StringFormat SF = new StringFormat();
        Rectangle R, R_icon;

        int MouseX, MouseY, _shadowdepth = 6, _radius = 2, _opacity = 35;
        int op, temp, decAni;

        float animationSize = 0, maxSize, incSize;

        bool Clicked = false, isIn = false, tabIn = false;
        bool isMouseIn = false;


        Image _icon;
        Bitmap bt;

        Color _backcolor = Color.DodgerBlue, _effectcolor;

        public enum ET { Light, Dark }
        ET _effectType = ET.Light;
        #endregion
        #region Properties
        [Category("Appearance Material")]
        public int ShadowOpacity
        { get { return _opacity; } set { if (value < 35) _opacity = 35; else if (value > 50) _opacity = 50; else _opacity = value; this.Invalidate(); } }

        [Category("Appearance Material")]
        public int ShadowDepth
        { get { return _shadowdepth; } set { if (value > 8) _shadowdepth = 8; else _shadowdepth = value; this.Invalidate(); } }

        [Category("Appearance Material"), Description("Radius of shadow.")]
        public int Radius
        { get { return _radius; } set { _radius = value; this.Invalidate(); } }

        [Category("Appearance Material")]
        public Color ButtonColor
        { get { return _backcolor; } set { _backcolor = value; this.Invalidate(); } }

        [Category("Appearance Material"), Description("Effect style when button is clicked.")]
        public ET EffectType
        {
            get { return _effectType; }
            set
            {
                decAni = 4;
                op = 102;
                _effectType = value;
                if (_effectType == ET.Dark) _effectcolor = Color.DimGray; else if (_effectType == ET.Light) { _effectcolor = Color.White; op = 55; decAni = 2; }
                temp = op;
                this.Invalidate();
            }
        }

        [Category("Appearance Material"), Description("Button icon.")]
        public Image Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                if (_icon != null)
                {
                    bt = new Bitmap(_icon);
                    R_icon = new Rectangle((Width - 8) / 15 + 8, (Height - 8) / 6 + 8, Height / 2 + Height / 5 - 16, Height / 2 + Height / 5 - 16);
                    R = new Rectangle(R_icon.X + R_icon.Height + 2, 8, Width - R_icon.Height - 18, Height - 16);
                    SF.Alignment = StringAlignment.Near;
                }
                else R = new Rectangle(8, 8, this.Width - 16, this.Height - 16);

                this.Invalidate();
            }
        }
        [Category("Appearance Material")]
        public StringAlignment TextAlign
        {
            get { return SF.Alignment; }
            set
            {
                SF.Alignment = value;
                this.Invalidate();
            }
        }
        #endregion
        #region Events
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Enter && this.Focused)
                this.PerformClick();
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            isMouseIn = true;
            this.Invalidate();
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            isMouseIn = false;
            this.Invalidate();
        }
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            this.Invalidate();
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (_shadowdepth != 0)
                _opacity += 45 / _shadowdepth;
            isMouseIn = true;
            this.Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_shadowdepth != 0)
                _opacity -= 45 / _shadowdepth;
            if (!this.Focused)
                isMouseIn = false;
            this.Invalidate();
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            this.Invalidate();
            op = temp;
            MouseX = e.X;
            MouseY = e.Y;
            if (MouseX >= 7 && MouseY >= 7
                && MouseX <= this.Width - 7 && MouseY <= Height - 7)
            {
                animationSize = 0;
                doAnimation.Start();
                this.Focus();
                Clicked = true;
            }
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Width > Height) maxSize = Width + 2;
            else maxSize = Height + 2;
            incSize = maxSize / 13;
            if (_icon != null)
            {
                R_icon = new Rectangle(Helper.Round((Width - 8) / 15 + 7), Helper.Round((Height - 8) / 6 + 7), Helper.Round(Height / 2 + Height / 4 - 14), Helper.Round(Height / 2 + Height / 4 - 14));
                R = new Rectangle(R_icon.X + R_icon.Height + 6, 8, Width - R_icon.Height - 14, Height - 16);
            }
            else
                R = new Rectangle(8, 8, this.Width - 16, this.Height - 16);
            this.Invalidate();
        }
        #endregion
        public MaterialRaisedButton()
        {
            SetStyle((ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);
            DoubleBuffered = true;

            Size = new Size(190, 50);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            BackColor = Color.Transparent;

            SF.Alignment = StringAlignment.Center;
            SF.LineAlignment = StringAlignment.Center;
            R = new Rectangle(8, 8, this.Width - 16, this.Height - 16);
            maxSize = Width + 2;
            incSize = maxSize / 12;

            doAnimation.Tick += doAnimation_Tick;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            BackColor = Color.Transparent;

            if (_radius <= 0) _radius = 2;
            var G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;

            var BG = Helper.drawRoundedRec(8, 8, this.Width - 16, this.Height - 16, _radius);
            Region region = new Region(BG);

            if (this.Enabled)
            {
                #region Vẽ bóng
                for (int i = 0; i < _shadowdepth; i++)
                {
                    var BG_sd = Helper.drawRoundedRec(8 - i, 8 - i, Width - 16 + i * 2, Height - 16 + i * 2, _radius + _shadowdepth);
                    G.FillPath(new SolidBrush(Color.FromArgb(_opacity - i * 5, Color.Black)), BG_sd);
                }
                #endregion
                G.FillPath(new SolidBrush(_backcolor), BG);
                if (isMouseIn)
                    G.FillPath(new SolidBrush(Color.FromArgb(70, _effectcolor)), BG);

            }
            else
            {
                G.FillPath(new SolidBrush(Color.FromArgb(95, Color.Gray)), BG);
            }

            G.SetClip(region, CombineMode.Replace);

            G.FillEllipse(new SolidBrush(Color.FromArgb(op * 2 + 50, _effectcolor)), MouseX - (animationSize / 2), MouseY - (animationSize / 2), animationSize, animationSize);
            G.SmoothingMode = SmoothingMode.HighQuality;
            if (_icon != null)
                G.DrawImage(bt, R_icon);
            if (this.Enabled)
                G.DrawString(Text, this.Font, new SolidBrush(this.ForeColor), R, SF);
            else G.DrawString(Text, this.Font, new SolidBrush(Color.FromArgb(160, Color.Gray)), R, SF);
        }
        void doAnimation_Tick(object sender, EventArgs e)
        {
            if (animationSize < maxSize * 2.2)
            {
                op -= decAni;
                animationSize += incSize;
            }
            else
            {
                animationSize = 0;
                op = 0;
                Clicked = false;
                doAnimation.Stop();
                isIn = true;
                DoEvent();
            }
            this.Invalidate();

        }

        public void PerformClick()
        {
            if (doAnimation.Enabled)
                return;
            this.Invalidate();
            op = temp;
            MouseX = Width / 2;
            MouseY = Height / 2;
            animationSize = 0;
            Clicked = true;
            isIn = true;
            doAnimation.Start();
        }
        private void DoEvent()
        {
            if (Click != null && isIn)
            {
                this.Invoke(Click);
                isIn = false;
            }
        }

    }
}
