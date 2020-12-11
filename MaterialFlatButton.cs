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
    public partial class MaterialFlatButton : Control
    {
        #region Variables
        Timer doAnimation = new Timer { Interval = 15 };

        public event EventHandler Click;
        StringFormat SF = new StringFormat();
        Rectangle R, R_icon;

        int MouseX, MouseY, _opacity = 45, temp = 45, aniDec = 4;

        float animationSize = 0, maxSize, incSize;

        bool Clicked = false, isOver = false;

        Image _icon;
        Bitmap bt;

        Color _effectClick;
        public enum ET { Light, Dark, ScaleTextColor }
        ET _effectType = ET.Dark;
        #endregion
        #region Properties
        [Category("Appearance Material"), Description("Hiệu ứng click.")]
        public ET EffectType
        {
            get { return _effectType; }
            set
            {
                _effectType = value;
                _opacity = 70;
                aniDec = 4;
                if (_effectType == ET.Light) { _effectClick = Color.WhiteSmoke; _opacity = 70; }
                else if (_effectType == ET.Dark) _effectClick = Color.Gray;
                else { _effectClick = ForeColor; _opacity = 35; aniDec = 3; }
                temp = _opacity;
            }
        }

        [Category("Appearance Material"), Description("Icon này sẽ nằm trước dòng text. Ảnh có thể là *.png, *.jpg, ... đều được. :D")]
        public Image Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                if (_icon != null)
                {
                    bt = new Bitmap(_icon);
                    R_icon = new Rectangle(Width / 15, Height / 6, Height / 2 + Height / 5, Height / 2 + Height / 5);
                    R = new Rectangle(R_icon.X + R_icon.Height + 2, 0, Width - R_icon.Height - 2, Height);
                    SF.Alignment = StringAlignment.Near;
                }
                else R = new Rectangle(0, 0, Width, Height);

                this.Invalidate();
            }
        }
        [Category("Appearance Material"), Description("Icon này sẽ nằm trước dòng text. Ảnh có thể là *.png, *.jpg, ... đều được. :D")]
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
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            this.Invalidate();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Width > Height) maxSize = Width + 2;
            else maxSize = Height + 2;
            incSize = maxSize / 13;
            R = new Rectangle(0, 0, Width, Height);
            if (_icon != null)
            {
                R_icon = new Rectangle(Helper.Round(Width / 15), Helper.Round(Height / 6), Helper.Round(Height - Height / 3), Helper.Round(Height - Height / 3));
                R = new Rectangle(R_icon.X + R_icon.Height + 4, 0, Width - R_icon.Height - 4, Height);
            }
            this.Invalidate();
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Clicked = true;
            _opacity = temp;
            this.Focus();
            this.Invalidate();
            //MessageBox.Show(incSize.ToString());
            MouseX = e.X;
            MouseY = e.Y;
            animationSize = 0;
            doAnimation.Start();
            Clicked = true;
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.PerformClick();
            base.OnKeyDown(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.Invalidate();
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            isOver = true;
            this.Invalidate();
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            isOver = false;
            this.Invalidate();
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isOver = true;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!this.Focused)
                isOver = false;
            this.Invalidate();
        }

        #endregion
        public MaterialFlatButton()
        {
            SetStyle((ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);
            DoubleBuffered = true;

            Font = new Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Size = new Size(150, 30);
            ForeColor = Color.Blue;
            BackColor = Color.Transparent;

            R = new Rectangle(0, 0, Width, Height);
            SF.Alignment = StringAlignment.Center;
            SF.LineAlignment = StringAlignment.Center;
            maxSize = Width + 2;
            incSize = maxSize / 12;

            doAnimation.Tick += doAnimation_Tick;
        }

        void doAnimation_Tick(object sender, EventArgs e)
        {
            if (animationSize < maxSize * 2.2)
            {
                _opacity -= aniDec;
                animationSize += incSize;
                this.Invalidate();
            }
            else
            {
                _opacity = temp;
                Clicked = false;
                animationSize = 0;
                this.Invalidate();
                doAnimation.Stop();
                if (Click != null)
                    this.Invoke(Click);
                Clicked = false;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            base.OnPaint(e);

            var G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighSpeed;

            var BG = Helper.drawRoundedRec(1, 1, Width - 2, Height - 2, 1);

            Region region = new Region(BG);

            G.DrawPath(new Pen(BackColor), BG);
            G.FillPath(new SolidBrush(BackColor), BG);

            G.SetClip(region, CombineMode.Replace);

            G.FillEllipse(new SolidBrush(Color.FromArgb(_opacity * 2 + 100, _effectClick)), MouseX - (animationSize / 2), MouseY - (animationSize / 2), animationSize, animationSize);
            if (isOver && Enabled)
                G.FillRectangle(new SolidBrush(Color.FromArgb(temp - 30, _effectClick)), 0, 0, this.Width, this.Height);

            G.SmoothingMode = SmoothingMode.HighQuality;
            if (_icon != null)
                G.DrawImage(bt, R_icon);
            if (this.Enabled)
                G.DrawString(Text, this.Font, new SolidBrush(this.ForeColor), R, SF);
            else
                G.DrawString(Text, this.Font, new SolidBrush(Color.FromArgb(125, Color.Gray)), R, SF);

        }
        public void PerformClick()
        {
            if (doAnimation.Enabled)
                return;
            _opacity = temp;
            this.Invalidate();
            //MessageBox.Show(incSize.ToString());
            MouseX = Width / 2;
            MouseY = Height / 2;
            animationSize = 0;
            doAnimation.Start();
        }
    }
}
