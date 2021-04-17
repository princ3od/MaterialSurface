using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace MaterialSurface
{
    public abstract class MaterialButton : Button, IMaterialControl
    {
        #region Variables
        protected readonly Timer animationDirector = new Timer() { Interval = 1 };
        protected readonly StringFormat textAlignment = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };

        protected Rectangle drawArea, iconArea;
        protected int mouseX, mouseY, radius = 6, aniOpacity = 140, aniDec = 4, defaultAniOpacity = 140;

        protected float animationSize = 0, maxSize, incSize;

        protected Color primaryColor = Color.BlueViolet;
        protected ET effectType = ET.Custom;

        protected Image icon;
        protected EventArgs _eventArgs;
        protected MouseEventArgs _mouseClickArgs;

        protected bool isPerformClick = false;
        #endregion

        #region Properties
        [Category("Appearance Material"), Description("Ripple animation color.")]
        public Color PrimaryColor
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
                ForeColor = value;
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
        [Category("Appearance Material"), Description("Text alignment.")]
        public StringAlignment TextAlignment
        {
            get { return textAlignment.Alignment; }
            set
            {
                textAlignment.Alignment = value;
                Invalidate();
            }
        }
        #endregion
        public MouseState MouseState { get; set; }
        protected abstract void OnAnimate(object sender, EventArgs e);

        public void PerformClick()
        {
            isPerformClick = true;
            base.OnClick(EventArgs.Empty);
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
                MouseState = MouseState.OUT;
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