using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MaterialSurface
{
    public class MaterialProgressbar : Control, IMaterialControl
    {
        public enum ProgressBarType
        {
            Normal,
            Cicular
        }
        #region Varibles
        const int WmPaint = 15;
        const int START_ANGLE = 270;
        readonly Timer animationDirector = new Timer { Interval = 1 };

        Color primaryColor = Color.BlueViolet;
        bool isInditermine = false;

        int targetPos, curPos, _value = 0, incPos, changeDelay = 50, marqueeWidth, valueAngle, posAngle = 270, incAngle, targetAngle;
        float cicularWidth = 3.8f;
        int maximum = 100, minimum = 0, step = 10, padding;
        ProgressBarType barType;
        public bool IsAnimating = false;
        #endregion
        #region Properties
        [Category("Appearance Material"), Description("")]
        public float CircularWidth
        {
            get { return cicularWidth; }
            set
            {
                cicularWidth = value;
                padding = Convert.ToInt32((cicularWidth + 2f) / 2);
                Invalidate();
            }
        }
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
        public ProgressBarType Type
        {
            get
            {
                return barType;
            }
            set
            {
                barType = value;
                if (barType == ProgressBarType.Cicular)
                {
                    if (DesignMode && _value < 10)
                    {
                        _value = 25;
                        valueAngle = Convert.ToInt32((float)_value / (float)maximum * 360f);
                    }
                    if (Height < 40)
                    {
                        Height = 40;
                        Width = Height;
                    }
                }
                Invalidate();
            }
        }
        [Category("Appearance Material"), Description("")]
        public bool IsIndetermine
        {
            get
            {
                return isInditermine;
            }
            set
            {
                isInditermine = value;
                if (isInditermine)
                {
                    if (_value < 10)
                    {
                        _value = 25;
                    }
                    marqueeWidth = Convert.ToInt32((float)_value / (float)Maximum * (float)Width);
                    valueAngle = Convert.ToInt32((float)_value / (float)maximum * 360f);
                    animationDirector.Start();
                }
                Invalidate();
            }
        }
        [Category("Appearance Material"), Description("The bigger the slower animation.")]
        public int ChangeDelay
        {
            get
            {
                return changeDelay;
            }
            set
            {
                changeDelay = value;
                if (changeDelay < 10)
                    changeDelay = 10;
                incAngle = 360 / (changeDelay);
                incPos = Width / changeDelay;
            }
        }
        [Category("Behavior"), Description("")]
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                if (_value < minimum)
                    _value = minimum;
                if (_value > maximum)
                    _value = maximum;
                targetAngle = Convert.ToInt32((float)_value / (float)maximum * 360f);
                targetPos = Convert.ToInt32((float)_value / (float)maximum * (float)Width);
                if (isInditermine || DesignMode)
                {
                    marqueeWidth = targetPos;
                    valueAngle = targetAngle;
                }
                else
                    animationDirector.Start();
                Invalidate();

            }
        }
        [Category("Behavior"), Description("")]
        public int Maximum
        {
            get { return maximum; }
            set
            {
                maximum = value;
                marqueeWidth = Convert.ToInt32((float)_value / (float)maximum * (float)Width);
                valueAngle = Convert.ToInt32((float)_value / (float)maximum * 360f);
                Invalidate();
            }
        }
        [Category("Behavior"), Description("")]
        public int Minimum
        {
            get { return minimum; }
            set
            {
                minimum = value;
            }
        }
        [Category("Behavior"), Description("")]
        public int Step
        {
            get { return step; }
            set
            {
                step = value;
            }
        }
        #endregion
        public MouseState MouseState { get; set; }


        public MaterialProgressbar()
        {
            SetStyle((ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);
            DoubleBuffered = true;

            Size = new Size(400, 8);

            padding = Convert.ToInt32((cicularWidth + 0.5f) / 2);
            incAngle = 360 / (changeDelay);
            targetAngle = valueAngle = Convert.ToInt32((float)_value / (float)maximum * 360f);
            targetPos = curPos = Value / Maximum * Width;
            incPos = Width / changeDelay;
            animationDirector.Tick += OnAnimate;
        }

        private void OnAnimate(object sender, EventArgs e)
        {
            IsAnimating = true;
            if (isInditermine)
            {
                if (barType == ProgressBarType.Normal)
                {
                    if (DesignMode)
                    {
                        curPos = 0;
                        return;
                    }
                    if (curPos < Width)
                        curPos += incPos;
                    else
                        curPos = -marqueeWidth;
                }
                else
                {
                    if (DesignMode)
                    {
                        posAngle = 270;
                        return;
                    }
                    if (posAngle <= START_ANGLE + 360)
                        posAngle += incAngle;
                    else
                        posAngle = START_ANGLE;
                }
            }
            else
            {

                if (barType == ProgressBarType.Normal)
                {
                    if (marqueeWidth != targetPos || curPos != 0)
                    {
                        if (curPos != 0)
                        {
                            if (curPos < Width)
                                curPos += incPos;
                            else
                                curPos = -marqueeWidth;
                            if (curPos <= 0 && curPos + incPos > 0)
                                curPos = 0;
                        }
                        if (Math.Abs(marqueeWidth - targetPos) < incPos)
                            marqueeWidth = targetPos;
                        else
                            marqueeWidth += (marqueeWidth < targetPos) ? incPos / 2 : -incPos / 2;
                    }
                    else
                    {
                        IsAnimating = false;
                        animationDirector.Stop();
                    }
                }
                else
                {
                    if (valueAngle != targetAngle || posAngle != START_ANGLE)
                    {
                        if (posAngle >= START_ANGLE + 360 || posAngle == START_ANGLE)
                            posAngle = START_ANGLE;
                        else
                            posAngle += incAngle;
                        if (Math.Abs(valueAngle - targetAngle) < incAngle)
                            valueAngle = targetAngle;
                        else
                            valueAngle += (valueAngle < targetAngle) ? incAngle / 2 : -incAngle / 2;
                    }
                    else
                    {
                        IsAnimating = false;
                        animationDirector.Stop();
                    }
                }
            }
            Invalidate();
        }

        // OnPaint
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            if (barType == ProgressBarType.Normal)
            {
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(80, primaryColor)), 0, 0, Width, Height);
                graphics.FillRectangle(new SolidBrush(primaryColor), curPos, 0, marqueeWidth, Height);
            }
            else
            {
                Color ParentColor;
                if (Parent is MaterialCard)
                    ParentColor = ((MaterialCard)Parent).CardColor;
                else
                    ParentColor = Parent.BackColor;
                graphics.Clear(ParentColor);
                graphics.DrawArc(new Pen(new SolidBrush(primaryColor), cicularWidth),
                    padding, padding, Width - padding * 2, Height - padding * 2, posAngle, valueAngle);
            }
        }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (isInditermine)
                animationDirector.Start();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (barType == ProgressBarType.Cicular)
                Width = Height;
            incPos = Width / changeDelay;
            if (_value == 100)
                curPos = Width;
            else
                targetPos = Convert.ToInt32((float)_value / (float)Maximum * (float)Width);
            Invalidate();
        }
        public void Increment(int value)
        {
            if (Value + value < maximum)
                Value += value;
            else
                Value = maximum;
        }
        public void PerformStep()
        {
            Increment(step);
        }
    }
}
