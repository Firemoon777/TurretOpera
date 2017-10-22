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
            eye.Click += eye_Click;
            this.Controls.Add(eye);

            // Setup tripod
            tripod = commonSetup(Properties.Resources.turret_tripod_rgn, Properties.Resources.turret_tripod_texture);
            setPosition(this, tripod, 0, 0);
            tripod.Show();

        }

        Form commonSetup(Bitmap bitmapRgn, Bitmap bg)
        {
            Form form = new Form();
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.ShowInTaskbar = false;
            IntPtr rgn = WinAPIHelper.createRgnFromBmp(bitmapRgn);
            WinAPI.SetWindowRgn(form.Handle, rgn, true);
            form.SetBounds(0, 0, bitmapRgn.Width, bitmapRgn.Height);
            form.BackgroundImage = bg;
            return form;
        }

        void setPosition(Form head, Form part, int x, int y)
        {
            WinAPI.SetWindowPos(part.Handle, 0, head.Bounds.X + x, head.Bounds.Y + y, 0, 0, WinAPI.SWP_NOSIZE);
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
