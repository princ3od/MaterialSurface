using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialSurface
{
    public class Snackbar : Control, IMaterialControl
    {
        public MouseState MouseState { get; set; }
        #region Variables
        const int LEFT_PADDING = 12;
        Size DEFAULT_BUTTON_SIZE = new Size(75, 35);
        readonly Timer animationDirector = new Timer { Interval = 1 };
        readonly StringFormat textAlignment = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        public static Font MessageFont = new Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        Rectangle messageArea;
        float timeAlive;
        int targetHeight, incSize;
        Form mainParent;
        public bool isShowing = false;

        static Snackbar _this;
        static Color primaryColor = Color.BlueViolet;
        static bool darkTheme = false;
        string strMessage = "This is a message.";

        TextButton mainButton;
        public event EventHandler OnButtonClick;
        #endregion
        #region Properties
        public static Color PrimaryColor
        {
            get { return primaryColor; }
            set { primaryColor = value; }
        }
        public static bool DarkTheme
        {
            get { return darkTheme; }
            set { darkTheme = value; }
        }
        #endregion
        public Snackbar()
        {
            SetStyle((ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);
            DoubleBuffered = true;

            Size = new Size(400, 0);
            BackColor = (darkTheme) ? Color.WhiteSmoke : ColorConstant.DarkThemeBackgroundColor;
            animationDirector.Tick += OnAnimate;
        }

        private async void OnAnimate(object sender, EventArgs e)
        {
            if (isShowing)
            {
                if (Height < targetHeight)
                {
                    Height += incSize;
                    this.Location = new Point(this.Location.X, this.Location.Y - incSize);
                }
                else
                {
                    animationDirector.Stop();

                    Height = targetHeight;
                    this.Location = new Point(this.Location.X, mainParent.ClientSize.Height - this.Height);
                    if (mainButton != null)
                    {
                        mainButton.Location = new Point(this.Width - LEFT_PADDING - mainButton.Width, this.Height / 2 - mainButton.Height / 2 + 1);
                        this.Controls.Add(mainButton);
                    }
                    await Task.Delay((int)timeAlive);
                    Close();
                }
            }
            else
            {
                if (this.Location.Y < mainParent.ClientSize.Height)
                {
                    this.Location = new Point(this.Location.X, this.Location.Y + incSize);
                }
                else
                {
                    animationDirector.Stop();
                    mainParent.Controls.Remove(this);
                }
            }
            this.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            if (Parent is MaterialCard)
                graphics.Clear(((MaterialCard)Parent).CardColor);
            else
                graphics.Clear(mainParent.BackColor);
            GraphicsPath mainSketch = GraphicHelper.GetRoundedRectangle(0, 0, Width, Height, 5);
            graphics.FillPath(new SolidBrush(BackColor), mainSketch);

            graphics.DrawString(strMessage, MessageFont, (darkTheme) ? Brushes.DimGray : Brushes.WhiteSmoke, messageArea, textAlignment);
        }
        public void Make(Form parent, string message, string buttonText = "OK", float timeShow = 4.0f, int offSet = 0)
        {
            if (timeShow < 1f)
                timeShow = 1f;
            SizeF messageSize = this.CreateGraphics().MeasureString(message, MessageFont), buttonTextSize = new SizeF(LEFT_PADDING, 0);

            if (!string.IsNullOrEmpty(buttonText))
            {
                buttonTextSize = this.CreateGraphics().MeasureString(buttonText, MessageFont);
                mainButton = new TextButton();
                mainButton.Text = buttonText;
                mainButton.PrimaryColor = primaryColor;
                mainButton.Width = (int)buttonTextSize.Width + LEFT_PADDING * 2;
                mainButton.Height = (int)buttonTextSize.Height + LEFT_PADDING;
                mainButton.Click += OnClick;
            }

            mainParent = parent;
            targetHeight = (int)messageSize.Height + LEFT_PADDING + LEFT_PADDING;
            Width = (int)messageSize.Width + LEFT_PADDING * 4 + ((mainButton == null) ? 0 : mainButton.Width);
            if (Width < mainParent.Width / 4)
                Width = mainParent.Width / 4;
            if (Width > mainParent.Width)
            {
                Width = mainParent.Width - LEFT_PADDING * 4;
                int widthNoButton = Width - ((mainButton == null) ? 0 : mainButton.Width);
                targetHeight += ((int)messageSize.Width / widthNoButton) * (int)messageSize.Height;
            }
            incSize = targetHeight / 14;
            mainParent.Resize += (s, e) =>
            {
                ReCalculate(mainParent, messageSize);
            };
            strMessage = message;
            timeAlive = timeShow * 1000f;
            isShowing = true;
            Height = 0;
            this.Location = new Point(mainParent.ClientSize.Width / 2 - this.Width / 2 + offSet, mainParent.ClientSize.Height);
            messageArea = new Rectangle(LEFT_PADDING, 0,
                Width - LEFT_PADDING * 2 - ((mainButton == null) ? 0 : mainButton.Width), targetHeight);

            mainParent.Controls.Add(this);
            this.BringToFront();

            animationDirector.Start();
        }

        private void ReCalculate(Form mainParent, SizeF messageSize)
        {
            targetHeight = (int)messageSize.Height + LEFT_PADDING + LEFT_PADDING;
            if (Width > mainParent.Width)
            {
                Width = mainParent.Width - LEFT_PADDING * 4;
                int widthNoButton = Width - ((mainButton == null) ? 0 : mainButton.Width);
                Height = targetHeight + ((int)messageSize.Width / widthNoButton) * (int)messageSize.Height;
            }
            else if (Width < mainParent.Width + LEFT_PADDING * 4 && Height > (int)messageSize.Height + LEFT_PADDING + LEFT_PADDING)
            {
                Width = (int)messageSize.Width + LEFT_PADDING * 4 + ((mainButton == null) ? 0 : mainButton.Width);
                if (Width < mainParent.Width / 4)
                    Width = mainParent.Width / 4;
                if (Width > mainParent.Width)
                {
                    Width = mainParent.Width - LEFT_PADDING * 4;
                    int widthNoButton = Width - ((mainButton == null) ? 0 : mainButton.Width);
                    Height = targetHeight + ((int)messageSize.Width / widthNoButton) * (int)messageSize.Height;
                }
                else
                    Height = targetHeight;
            }
            if (mainButton != null)
                mainButton.Location = new Point(this.Width - LEFT_PADDING - mainButton.Width, this.Height / 2 - mainButton.Height / 2);

            this.Location = new Point(mainParent.ClientSize.Width / 2 - this.Width / 2, mainParent.ClientSize.Height - this.Height);
            messageArea = new Rectangle(LEFT_PADDING, 0,
                Width - LEFT_PADDING * 2 - ((mainButton == null) ? 0 : mainButton.Width), Height);
        }

        public static Snackbar MakeSnackbar(Form mainParent, string message, string buttonText = "", float timeShow = 4.0f, int offSet = 0)
        {
            _this = new Snackbar();
            _this.Make(mainParent, message, buttonText, timeShow, offSet);
            return _this;
        }
        public void Close()
        {
            isShowing = false;
            animationDirector.Start();
        }
        private void OnClick(object sender, EventArgs e)
        {
            OnButtonClick?.Invoke(this, EventArgs.Empty);
            Close();
        }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (DesignMode)
                mainParent.Controls.Remove(this);
        }
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }
    }
}
