using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Material_Design_for_Winform
{
    public partial class MaterialInterface : Panel
    {
        #region Variables
        int _opacity = 40, _shadowdepth = 5, _radius = 3;

        Color _bColor =  Color.White;
        #endregion
        #region Properties
         [Category("Appearance Material")]
        public Color MainColor
        { get { return _bColor; } set { _bColor = value; this.Invalidate(); } }
        [Category("Appearance Material")]
        public int ShadowOpacity
        { get { if (_opacity < 40) return 40; else if (_opacity > 80) return 80; else return _opacity; } set { _opacity = value; this.Invalidate(); } }

        [Category("Appearance Material")]
        public int ShadowDepth
        { get { if (_shadowdepth > 10) return 10; else return _shadowdepth; } set { _shadowdepth = value; this.Invalidate(); } }

        [Category("Appearance Material")]
        public int Radius
        { get { return _radius; } set { _radius = value; this.Invalidate(); } }

        #endregion
        #region Events
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.Invalidate();
        }
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            this.Invalidate();
        }
        #endregion
        public MaterialInterface()
        {
            DoubleBuffered = true;
            BackColor = Color.Transparent;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (BackColor != Color.Transparent) BackColor = Color.Transparent;
            var G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;

            var BG = Helper.drawRoundedRec(10, 10, this.Width - 20, this.Height - 20, _radius);

            for (int i = 0; i < _shadowdepth; i++)
            {
                var BG_sd = Helper.drawRoundedRec(10 - i, 10 - i, Width - 20 + i * 2, Height - 20 + i * 2, _radius + _shadowdepth + 2);
                G.FillPath(new SolidBrush(Color.FromArgb(_opacity - i * 5 + _shadowdepth / 2, Color.Black)), BG_sd);
            }

            G.FillPath(new SolidBrush(_bColor), BG);

        }
    }
}
