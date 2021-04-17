using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MaterialSurface
{
    class OverlayForm : Form
    {
        private OverlayForm()
        {

        }
        public OverlayForm(Form parent, Form child, float opacity = 0.65f, int offSet = 5, bool blackOut = true)
        {
            DoubleBuffered = true;
            ControlBox = false;
            StartPosition = FormStartPosition.Manual;
            FormBorderStyle = FormBorderStyle.None;
            AllowTransparency = true;
            ShowInTaskbar = false;
            Opacity = opacity;
            if (blackOut)
                BackColor = Color.Black;
            else
            {
                BackColor = Color.White;
                Opacity = 0.1f;
            }
            Size = parent.Size;
            Location = parent.Location;
            if (parent.FormBorderStyle != FormBorderStyle.None)
            {
                this.Location = new Point(this.Location.X + 7, this.Location.Y);
                this.Width -= 14;
                this.Height -= 7;
            }
            Enabled = false;
            Owner = parent;
            Activated += (s, e) =>
            {
                child.Activate();
            };
            child.StartPosition = FormStartPosition.Manual;
            child.Owner = parent;
            child.ShowInTaskbar = false;
            child.Location = new Point(Location.X + Width / 2 - child.Width / 2, Location.Y + Height / 2 - child.Height / 2 - offSet);
            child.FormClosed += (s, e) =>
            {
                this.Close();
                parent.Activate();
            };
            this.Show();
        }
    }
}
