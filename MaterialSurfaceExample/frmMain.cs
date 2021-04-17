using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSurface;
namespace MaterialSurfaceExample
{
    public partial class frmMain : Form
    {
        bool raiseError = false;
        int second = 0, transitionSec = 3;
        float showTime = 4f;
        Timer timer = new Timer() { Interval = 1000 };
        public frmMain()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            #region Button
            ckbButtonEnable.CheckedChanged += (s, ev) => textButton1.Enabled = outlinedButton1.Enabled = containedButton1.Enabled = ckbButtonEnable.Checked;
            ckbButtonIcon.CheckedChanged += (s, ev) =>
            {
                if (ckbButtonIcon.Checked)
                    textButton1.Icon = containedButton1.Icon = outlinedButton1.Icon = Properties.Resources.heart;
                else
                    textButton1.Icon = containedButton1.Icon = outlinedButton1.Icon = null;
            };
            #endregion

            #region Text Field
            cbbTextfieldStyle.SelectedIndexChanged += (s, ev) =>
            {
                if (cbbTextfieldStyle.SelectedIndex < 0)
                    return;
                txtMainDemo.Style = (MaterialTextfield.TextfieldStyle)cbbTextfieldStyle.SelectedIndex;
            };
            ckbTextfieldEnable.CheckedChanged += (s, ev) => txtMainDemo.Enabled = ckbTextfieldEnable.Checked;
            ckbCountChar.CheckedChanged += (s, ev) => txtMainDemo.CountText = ckbCountChar.Checked;
            ckbSysPassword.CheckedChanged += (s, ev) => txtMainDemo.UseSystemPasswordChar = ckbSysPassword.Checked;
            ckbMultiLine.CheckedChanged += (s, ev) =>
            {
                txtMainDemo.Multiline = ckbMultiLine.Checked;
                if (ckbMultiLine.Checked)
                {
                    txtMainDemo.Location = new Point(55, 50);
                    txtMainDemo.Size = new Size(420, 170);
                }
                else
                {
                    txtMainDemo.Location = new Point(120, 125);
                    txtMainDemo.Size = new Size(300, 67);
                }
            };
            txtTextfieldHint.TextChanged += (s, ev) => txtMainDemo.HintText = txtTextfieldHint.Text;
            txtTextfieldLabel.TextChanged += (s, ev) => txtMainDemo.FloatingLabelText = txtTextfieldLabel.Text;
            ckbShowHelperText.CheckedChanged += (s, ev) =>
            {
                txtHelperText.Enabled = ckbShowHelperText.Checked;
                if (!ckbShowHelperText.Checked)
                    txtMainDemo.HelperText = "";
                else
                    txtMainDemo.HelperText = txtHelperText.Text;
            };
            txtHelperText.TextChanged += (s, ev) =>
            {
                if (ckbShowHelperText.Checked)
                    txtMainDemo.HelperText = txtHelperText.Text;
            };
            btnRaiseError.Click += (s, ev) =>
            {
                if (!raiseError)
                {
                    raiseError = true;
                    Snackbar.MakeSnackbar(this, "You can raise an error with your custom message on Textfield.", "OK");
                }
                txtMainDemo.RaiseError(txtTextfieldError.Text);
            };
            txtMainDemo.TextChanged += (s, ev) => txtMainDemo.RemoveError();
            #endregion

            #region Toggles
            ckbTogglesEnable.CheckedChanged += (s, ev) =>
            {
                materialCheckbox1.Enabled = materialCheckbox2.Enabled = materialCheckbox3.Enabled = materialRadioButton1.Enabled =
                    materialRadioButton2.Enabled = materialRadioButton3.Enabled = materialRadioButton4.Enabled = ckbTogglesEnable.Checked;
            };
            btnClearChoices.Click += (s, ev) =>
            {
                materialCheckbox1.Checked = materialCheckbox2.Checked = materialCheckbox3.Checked = materialRadioButton1.Checked =
                    materialRadioButton2.Checked = materialRadioButton3.Checked = materialRadioButton4.Checked = false;
            };
            ckbChipEnable.CheckedChanged += (s, ev) => choiceChip1.Enabled = choiceChip2.Enabled = choiceChip3.Enabled = ckbChipEnable.Checked;
            btnClearChipChoice.Click += (s, ev) => choiceChip1.Checked = choiceChip2.Checked = choiceChip3.Checked = false;
            #endregion

            #region ComboBox
            mainComboBox.SizeChanged += (s, ev) => mainComboBox.Location = new Point(265 - mainComboBox.Width / 2, mainComboBox.Location.Y);
            ckbComboboxEnabled.CheckedChanged += (s, ev) => mainComboBox.Enabled = ckbComboboxEnabled.Checked;
            txtComboboxHint.TextChanged += (s, ev) => mainComboBox.HintText = txtComboboxHint.Text;
            string[] os = { "Windows", "Linux", "Android", "iOS" };
            foreach (var item in os)
                txtComboboxItems.Text += string.Format("{0}" + Environment.NewLine, item);
            txtComboboxItems.KeyPress += (s, ev) =>
            {
                if (ev.KeyChar != (char)Keys.Enter)
                    return;
                mainComboBox.Items.Clear();
                string[] items = txtComboboxItems.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in items)
                    mainComboBox.Items.Add(item);
            };
            #endregion

            #region Progressbar
            ckbProgressbarIndetermine.CheckedChanged += (s, ev) => mainProgressBar.IsIndetermine = ckbProgressbarIndetermine.Checked;
            txtProgressbarValue.TextChanged += (s, ev) =>
            {
                if (txtProgressbarValue.hasError)
                    txtProgressbarValue.RemoveError();
                if (Int32.TryParse(txtProgressbarValue.Text, out int value))
                {
                    if (value >= 0 && value <= 100)
                        mainProgressBar.Value = value;
                    else
                        txtProgressbarValue.RaiseError("Value must be between 0 and 100.", false);
                }
                else
                    txtProgressbarValue.RaiseError("Digit only!", false);
            };
            txtProgressbarDelay.TextChanged += (s, ev) =>
             {
                 if (txtProgressbarDelay.hasError)
                     txtProgressbarDelay.RemoveError();
                 if (Int32.TryParse(txtProgressbarDelay.Text, out int value))
                 {
                     if (value > 10)
                         mainProgressBar.ChangeDelay = value;
                     else
                         txtProgressbarDelay.RaiseError("Value must be greater than 10", false);
                 }
                 else
                     txtProgressbarDelay.RaiseError("Digit only!", false);
             };
            timer.Tick += (s, ev) =>
            {
                if (materialProgressbar1.IsIndetermine)
                    second--;
                else if (!materialProgressbar1.IsAnimating)
                {
                    if (transitionSec <= 0)
                    {
                        transitionSec = 3;
                        materialProgressbar1.IsIndetermine = materialProgressbar2.IsIndetermine = true;
                        materialProgressbar1.Value = materialProgressbar2.Value = new Random().Next(20, 40);
                    }
                    else
                        transitionSec--;
                }
                if (second <= 0)
                {
                    second = new Random().Next(3, 10);
                    materialProgressbar1.IsIndetermine = materialProgressbar2.IsIndetermine = false;
                    materialProgressbar1.Value = materialProgressbar2.Value = 100;
                }
            };
            timer.Start();
            btnStopAnimate.Click += (s, ev) =>
            {
                if (timer.Enabled)
                {
                    btnStopAnimate.Text = "Start";
                    materialProgressbar1.IsIndetermine = materialProgressbar2.IsIndetermine = false;
                    timer.Stop();
                }
                else
                {
                    btnStopAnimate.Text = "Stop";
                    materialProgressbar1.IsIndetermine = materialProgressbar2.IsIndetermine = true;
                    timer.Start();
                }
            };
            #endregion

            #region Card
            ckbCardMouseInteract.CheckedChanged += (s, ev) => mainCard.MouseInteract = ckbCardMouseInteract.Checked;
            #endregion

            #region Snackbar && Dialog
            txtSnackbarTimeShow.TextChanged += (s, ev) =>
            {
                if (txtSnackbarTimeShow.hasError)
                    txtSnackbarTimeShow.RemoveError();
                if (!float.TryParse(txtSnackbarTimeShow.Text, out showTime))
                    txtSnackbarTimeShow.RaiseError("Digit only", false);
                else
                {
                    if (showTime < 1)
                        txtSnackbarTimeShow.RaiseError("Time show of snackbar must be greater than 1 second.", false);
                }
            };
            btnMakeSnackbar.Click += (s, ev) => Snackbar.MakeSnackbar(this, txtSnackbarMessage.Text, txtSnackbarButtonText.Text, showTime);
            cbbDialogsButton.SelectedIndex = 1;
            btnShowDialog.Click += (s, ev) => Dialog.Show(this, txtDialogMessage.Text, txtDialogTittle.Text, (Buttons)cbbDialogsButton.SelectedIndex,
                ckbDialogDimScreen.Checked);
            ckbDialogDark.CheckedChanged += (s, ev) => Dialog.DarkTheme = ckbDialogDark.Checked;
            ckbSnackbarDark.CheckedChanged += (s, ev) => Snackbar.DarkTheme = ckbSnackbarDark.Checked;
            #endregion
        }
        protected async override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            cbbTextfieldStyle.SelectedIndex = 0;
            await Task.Delay(1200);
            Snackbar.MakeSnackbar(this, "Welcome you to MaterialSurface.", "XIN CHÀO", 5).OnButtonClick += (s, ev) =>
            {
                Dialog.Show(this, "\"Xin chào\" is Vietnamese meaning of the word \"hello\".");
            };
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (Dialog.Show(this, "Are you sure you want to exit?", "Exit", Buttons.YesNo) == DialogResult.No)
                e.Cancel = true;
        }
        private void ShowGithub(object sender, EventArgs e)
        {
            Process.Start("https://github.com/princ3od/MaterialSurface");
        }
        private void ShowLinkedIn(object sender, EventArgs e)
        {
            Process.Start("https://www.linkedin.com/in/princ3od/");
        }
        private void CopyMail(object sender, EventArgs e)
        {
            if (Clipboard.GetText() == "princ3od@gmail.com")
            {
                Snackbar.MakeSnackbar(this, "My email has been already copied to clipboard!", "YAH");
                return;
            }
            Clipboard.SetText("princ3od@gmail.com");
            Snackbar.MakeSnackbar(this, "Yay, my email has been copied to your clipboard.", "OKAY");
        }
        private void ChangeButtonType(object sender, EventArgs e)
        {
            textButton1.Visible = rbtnText.Checked;
            containedButton1.Visible = rbtnContained.Checked;
            outlinedButton1.Visible = rbtnOutlined.Checked;
        }
        private void ChangeTextfieldType(object sender, EventArgs e)
        {
            if (rbtnNormalTextfield.Checked)
                txtMainDemo.FieldType = BoxType.Normal;
            else if (rbtnFilledTextfield.Checked)
                txtMainDemo.FieldType = BoxType.Filled;
            else
                txtMainDemo.FieldType = BoxType.Outlined;
        }
        private void ChangeComboBoxType(object sender, EventArgs e)
        {
            if (rbtnNormalCombobox.Checked)
                mainComboBox.ComboBoxType = BoxType.Normal;
            else if (rbtnFilledCombobox.Checked)
                mainComboBox.ComboBoxType = BoxType.Filled;
            else
                mainComboBox.ComboBoxType = BoxType.Outlined;
        }
        private void ChangeChipType(object sender, EventArgs e)
        {
            if (rbtnFilledChip.Checked)
                choiceChip1.ChipType = choiceChip2.ChipType = choiceChip3.ChipType = ChoiceChip.ChipStyle.Filled;
            else
                choiceChip1.ChipType = choiceChip2.ChipType = choiceChip3.ChipType = ChoiceChip.ChipStyle.Outlined;
        }
        private void ChangeCardType(object sender, EventArgs e)
        {
            if (rbtnElevatedCard.Checked)
                mainCard.Style = MaterialCard.CardStyle.Elevated;
            else
                mainCard.Style = MaterialCard.CardStyle.Outlined;
        }

        private void ChangeProgressbarType(object sender, EventArgs e)
        {
            if (rbtnProgressbarNormal.Checked)
            {
                mainProgressBar.Location = new Point(90, 160);
                mainProgressBar.Size = new Size(350, 8);
                mainProgressBar.Type = MaterialProgressbar.ProgressBarType.Normal;
            }
            else
            {
                mainProgressBar.Location = new Point(240, 140);
                mainProgressBar.Size = new Size(60, 60);
                mainProgressBar.Type = MaterialProgressbar.ProgressBarType.Cicular;
            }
        }
    }
}
