using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MaterialSurface
{
    public enum Buttons
    {
        AbortRetryInorge,
        Ok,
        OkCancel,
        RetryCancel,
        YesNo,
        YesNoCancel
    }
    public class Dialog : Form
    {
        #region Variables
        Size DEFAULT_BUTTON_SIZE = new Size(75, 35);
        readonly int INIT_BUTTON_POSITION = 60;
        readonly int DEFAULT_SPACE = 8;

        float MAX_WIDTH = 650;
        static readonly Dictionary<string, string> buttonText = new Dictionary<string, string>()
        {
            {"Ok", "OK"},
            { "Abort", "Abort"},
            {"Retry", "Retry" },
            {"Inorge", "Inorge" },
            {"Cancel", "Cancel" },
            {"Yes", "Yes" },
            {"No", "No" }
        };

        static Color primaryColor = Color.BlueViolet;
        static DialogResult dialogResult = new DialogResult();
        static Dialog _this;
        static bool darkTheme = false;

        Buttons buttons;

        string strMessage = string.Empty, strHeader = string.Empty;
        Form mainParent;
        readonly StringFormat textAlignment = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
        Rectangle messageArea, headerArea;
        readonly Font messageFont = new Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        readonly Font headerFont = new Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        #endregion

        #region Properties
        public static string ButtonOkText
        {
            get { return buttonText["Ok"]; }
            set { buttonText["Ok"] = value; }
        }
        public static string ButtonAbortText
        {
            get { return buttonText["Abort"]; }
            set { buttonText["Abort"] = value; }
        }
        public static string ButtonRetryText
        {
            get { return buttonText["Retry"]; }
            set { buttonText["Retry"] = value; }
        }
        public static string ButtonInorgeText
        {
            get { return buttonText["Inorge"]; }
            set { buttonText["Inorge"] = value; }
        }
        public static string ButtonCancelText
        {
            get { return buttonText["Cancel"]; }
            set { buttonText["Cancel"] = value; }
        }
        public static string ButtonYesText
        {
            get { return buttonText["Yes"]; }
            set { buttonText["Yes"] = value; }
        }
        public static string ButtonNoText
        {
            get { return buttonText["No"]; }
            set { buttonText["No"] = value; }
        }
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
        void InitButton(Buttons type)
        {
            TextButton okButton, cancelButton, abortButton, retryButton, inorgeButton, yesButton, noButton;
            int addedButtonNum = 0;
            switch (type)
            {
                case Buttons.AbortRetryInorge:
                    abortButton = new TextButton()
                    {
                        Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                        ForeColor = primaryColor,
                        EffectType = ET.Custom,
                        Text = buttonText["Abort"],
                        Size = DEFAULT_BUTTON_SIZE,
                        Location = new Point(INIT_BUTTON_POSITION + (DEFAULT_BUTTON_SIZE.Width + DEFAULT_SPACE) * addedButtonNum, 85),
                        DialogResult = DialogResult.Abort,
                    };
                    abortButton.Click += ButtonClick;
                    addedButtonNum++;
                    retryButton = new TextButton()
                    {
                        Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                        ForeColor = primaryColor,
                        EffectType = ET.Custom,
                        Text = buttonText["Retry"],
                        Size = DEFAULT_BUTTON_SIZE,
                        Location = new Point(INIT_BUTTON_POSITION + (DEFAULT_BUTTON_SIZE.Width + DEFAULT_SPACE) * addedButtonNum, 85),
                        DialogResult = DialogResult.Retry,
                    };
                    retryButton.Click += ButtonClick;
                    addedButtonNum++;
                    inorgeButton = new TextButton()
                    {
                        Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                        ForeColor = primaryColor,
                        EffectType = ET.Custom,
                        Text = buttonText["Inorge"],
                        Size = DEFAULT_BUTTON_SIZE,
                        Location = new Point(INIT_BUTTON_POSITION + (DEFAULT_BUTTON_SIZE.Width + DEFAULT_SPACE) * addedButtonNum, 85),
                        DialogResult = DialogResult.Ignore,
                    };
                    inorgeButton.Click += ButtonClick;
                    this.Controls.Add(abortButton);
                    this.Controls.Add(retryButton);
                    this.Controls.Add(inorgeButton);
                    break;
                case Buttons.Ok:
                    okButton = new TextButton()
                    {
                        Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                        ForeColor = primaryColor,
                        EffectType = ET.Custom,
                        Text = buttonText["Ok"],
                        Size = DEFAULT_BUTTON_SIZE,
                        Location = new Point(INIT_BUTTON_POSITION + (DEFAULT_BUTTON_SIZE.Width + DEFAULT_SPACE) * 2 - 40, 85),
                        DialogResult = DialogResult.OK,
                    };
                    okButton.Click += ButtonClick;
                    this.Controls.Add(okButton);
                    break;
                case Buttons.OkCancel:
                    addedButtonNum = 1;
                    cancelButton = new TextButton()
                    {
                        Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                        ForeColor = primaryColor,
                        EffectType = ET.Custom,
                        Text = buttonText["Cancel"],
                        Size = DEFAULT_BUTTON_SIZE,
                        Location = new Point(INIT_BUTTON_POSITION + (DEFAULT_BUTTON_SIZE.Width + DEFAULT_SPACE) * addedButtonNum, 85),
                        DialogResult = DialogResult.Cancel
                    };
                    cancelButton.Click += ButtonClick;
                    addedButtonNum++;
                    okButton = new TextButton()
                    {
                        Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                        ForeColor = primaryColor,
                        EffectType = ET.Custom,
                        Text = buttonText["Ok"],
                        Size = DEFAULT_BUTTON_SIZE,
                        Location = new Point(INIT_BUTTON_POSITION + (DEFAULT_BUTTON_SIZE.Width + DEFAULT_SPACE) * addedButtonNum, 85),
                        DialogResult = DialogResult.OK
                    };
                    okButton.Click += ButtonClick;
                    this.Controls.Add(okButton);
                    this.Controls.Add(cancelButton);
                    break;
                case Buttons.RetryCancel:
                    addedButtonNum = 1;
                    retryButton = new TextButton()
                    {
                        Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                        ForeColor = primaryColor,
                        EffectType = ET.Custom,
                        Text = buttonText["Retry"],
                        Size = DEFAULT_BUTTON_SIZE,
                        Location = new Point(140, 85),
                        DialogResult = DialogResult.Retry,
                    };
                    retryButton.Click += ButtonClick;
                    addedButtonNum++;
                    cancelButton = new TextButton()
                    {
                        Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                        ForeColor = primaryColor,
                        EffectType = ET.Custom,
                        Text = buttonText["Cancel"],
                        Size = DEFAULT_BUTTON_SIZE,
                        Location = new Point(INIT_BUTTON_POSITION + (DEFAULT_BUTTON_SIZE.Width + DEFAULT_SPACE) * addedButtonNum, 85),
                        DialogResult = DialogResult.Cancel
                    };
                    cancelButton.Click += ButtonClick;
                    this.Controls.Add(retryButton);
                    this.Controls.Add(cancelButton);
                    break;
                case Buttons.YesNo:
                    addedButtonNum = 1;
                    yesButton = new TextButton()
                    {
                        Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                        ForeColor = primaryColor,
                        EffectType = ET.Custom,
                        Text = buttonText["Yes"],
                        Size = DEFAULT_BUTTON_SIZE,
                        Location = new Point(INIT_BUTTON_POSITION + (DEFAULT_BUTTON_SIZE.Width + DEFAULT_SPACE) * addedButtonNum, 85),
                        DialogResult = DialogResult.Yes
                    };
                    yesButton.Click += ButtonClick;
                    addedButtonNum++;
                    noButton = new TextButton()
                    {
                        Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                        ForeColor = primaryColor,
                        EffectType = ET.Custom,
                        Text = buttonText["No"],
                        Size = DEFAULT_BUTTON_SIZE,
                        Location = new Point(INIT_BUTTON_POSITION + (DEFAULT_BUTTON_SIZE.Width + DEFAULT_SPACE) * addedButtonNum, 85),
                        DialogResult = DialogResult.No
                    };
                    noButton.Click += ButtonClick;
                    this.Controls.Add(yesButton);
                    this.Controls.Add(noButton);
                    break;
                case Buttons.YesNoCancel:
                    yesButton = new TextButton()
                    {
                        Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                        ForeColor = primaryColor,
                        EffectType = ET.Custom,
                        Text = buttonText["Yes"],
                        Size = DEFAULT_BUTTON_SIZE,
                        Location = new Point(INIT_BUTTON_POSITION + (DEFAULT_BUTTON_SIZE.Width + DEFAULT_SPACE) * addedButtonNum, 85),
                        DialogResult = DialogResult.Yes
                    };
                    yesButton.Click += ButtonClick;
                    addedButtonNum++;
                    noButton = new TextButton()
                    {
                        Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                        ForeColor = primaryColor,
                        EffectType = ET.Custom,
                        Text = buttonText["No"],
                        Size = DEFAULT_BUTTON_SIZE,
                        Location = new Point(INIT_BUTTON_POSITION + (DEFAULT_BUTTON_SIZE.Width + DEFAULT_SPACE) * addedButtonNum, 85),
                        DialogResult = DialogResult.No
                    };
                    noButton.Click += ButtonClick;
                    addedButtonNum++;
                    cancelButton = new TextButton()
                    {
                        Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                        ForeColor = primaryColor,
                        EffectType = ET.Custom,
                        Text = buttonText["Cancel"],
                        Size = DEFAULT_BUTTON_SIZE,
                        Location = new Point(INIT_BUTTON_POSITION + (DEFAULT_BUTTON_SIZE.Width + DEFAULT_SPACE) * addedButtonNum, 85),
                        DialogResult = DialogResult.Cancel
                    };
                    cancelButton.Click += ButtonClick;
                    this.Controls.Add(yesButton);
                    this.Controls.Add(noButton);
                    this.Controls.Add(cancelButton);
                    break;
                default:
                    break;
            }
        }
        void ButtonClick(object sender, EventArgs e)
        {
            dialogResult = (sender as Button).DialogResult;
        }
        public Dialog()
        {
            SetStyle((ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);
            DoubleBuffered = true;

            Size = new Size(325, 130);
            messageArea = new Rectangle(30, 40, 290, 45);
            headerArea = new Rectangle(30, 14, 290, 28);

            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.WhiteSmoke;
            StartPosition = FormStartPosition.Manual;

        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitButton(buttons);
            SizingDialog(strMessage);
            if (darkTheme)
                BackColor = ColorConstant.DarkThemeBackgroundColor;
            else
                BackColor = Color.WhiteSmoke;
            if (string.IsNullOrEmpty(strHeader))
            {
                _this.Height -= 15;
                _this.messageArea.Y -= 25;
            }
            else
                headerArea.Width = Width - 60;
        }
        public static void Show(Form parent, string message, string tittle = "", bool dimScreen = true)
        {
            _this = new Dialog
            {
                buttons = Buttons.Ok,
                strMessage = message,
                strHeader = tittle,
                mainParent = parent
            };

            OverlayForm overlayForm = new OverlayForm(parent, _this, blackOut: dimScreen);
            _this.ShowDialog();
        }
        public static DialogResult Show(Form parent, string message, string tittle, Buttons _buttons, bool dimScreen = true)
        {
            dialogResult = DialogResult.None;
            _this = new Dialog
            {
                buttons = _buttons,
                strMessage = message,
                strHeader = tittle,
                mainParent = parent
            };
            OverlayForm overlayForm = new OverlayForm(parent, _this, blackOut: dimScreen);
            _this.ShowDialog();
            return dialogResult;
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
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.DrawRectangle(new Pen(Brushes.Black), 0, 0, Width - 1, Height - 1);
            e.Graphics.DrawString(strMessage, messageFont, (darkTheme) ? Brushes.Gainsboro : Brushes.DimGray, messageArea, textAlignment);
            if (!string.IsNullOrEmpty(strHeader))
                e.Graphics.DrawString(strHeader, headerFont, (darkTheme) ? Brushes.WhiteSmoke : Brushes.Black, headerArea, textAlignment);
        }
        private void SizingDialog(string message)
        {
            Graphics g = this.CreateGraphics();

            MAX_WIDTH = mainParent.Size.Width / 1.6f;
            SizeF messageSize = g.MeasureString(message, messageFont);

            if (messageSize.Width > MAX_WIDTH)
            {
                int addLine = Convert.ToInt32(messageSize.Width / MAX_WIDTH);
                this.Size = new Size(Convert.ToInt32(MAX_WIDTH), this.Height + Convert.ToInt32(messageSize.Height) * addLine);
                messageArea.Height += Convert.ToInt32(messageSize.Height) * addLine;
                messageArea.Width = this.Width - 45;
            }
            else if (messageSize.Width > 290.0)
            {
                this.Size = new Size(Convert.ToInt32(messageSize.Width) + 45, this.Height);
                messageArea.Width = this.Width - 45;
            }

            Centerlize();
        }

        private void Centerlize()
        {
            this.Location = new Point(mainParent.Location.X + mainParent.Width / 2 - this.Width / 2, mainParent.Location.Y + mainParent.Height / 2 - this.Height / 2);
        }
    }
}
