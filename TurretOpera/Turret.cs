using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TurretOpera
{
    public partial class Turret : Form
    {
        private Form leftGun;
        private Form rightGun;
        private Form tripod;
        private Button eye;
        private bool eye_enabled = false;

        public Turret()
        {
            InitializeComponent();
            // Create custom region 
            Bitmap headRgnBmp = Properties.Resources.turret_head_rgn;
            WinAPI.SetWindowPos(this.Handle, 0, 0, 0, headRgnBmp.Width, headRgnBmp.Height, WinAPI.SWP_NOMOVE);
            IntPtr headRgn = WinAPIHelper.createRgnFromBmp(headRgnBmp);
            WinAPI.SetWindowRgn(this.Handle, headRgn, true);

            // Setup form theme
            Bitmap headTextureBmp = Properties.Resources.turret_head_texture;
            this.BackgroundImage = headTextureBmp;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            // Setup eye button
            eye = new Button();
            eye.SetBounds(98, 205, 37, 42);
            eye.FlatStyle = FlatStyle.Flat;
            eye.FlatAppearance.BorderSize = 0;
            eye.BackgroundImage = Properties.Resources.eye_disabled;
            IntPtr eyeRgn = WinAPI.CreateRoundRectRgn(0, 0, 37, 42, 37, 42);
            //WinAPI.SetWindowRgn(eye.Handle, eyeRgn, true);
            eye.Click += eye_Click;
            this.Controls.Add(eye);

        }

        void eye_Click(object sender, EventArgs e)
        {
            this.eye_enabled = !this.eye_enabled;
            if (this.eye_enabled)
            {
                this.eye.BackgroundImage = Properties.Resources.eye_enabled;
            }
            else
            {
                this.eye.BackgroundImage = Properties.Resources.eye_disabled;
            }
        }
    }
}
