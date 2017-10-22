using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace TurretOpera
{
    public partial class Turret : Form
    {
        private Form leftGun;
        private Form rightGun;
        private Form tripod;
        private Button eye;
        private bool eye_enabled = false;

        private Timer timer;
        private Random rnd;

        private Point lastLocation;

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

            // Setup left gun
            leftGun = commonSetup(Properties.Resources.turret_lgun_rgn, Properties.Resources.turret_lgun_texture);
            setPosition(this, leftGun, 40, 0);
            leftGun.Show();

            // Setup right gun
            rightGun = commonSetup(Properties.Resources.turret_rgun_rgn, Properties.Resources.turret_rgun_texture);
            setPosition(this, rightGun, -40, 0);
            rightGun.Show();

            // Setup timer
            timer = new Timer();
            timer.Interval = 300;
            timer.Tick += timer_Tick;
            rnd = new Random();

            // Set listeners to all parts
            this.MouseDown += Turret_MouseDown;
            tripod.MouseDown += Turret_MouseDown;
            rightGun.MouseDown += Turret_MouseDown;
            leftGun.MouseDown += Turret_MouseDown;

            // Set magic to move all parts at once
            lastLocation = new Point(this.Location.X, this.Location.Y);
            this.LocationChanged += Turret_LocationChanged;
        }

        void Turret_LocationChanged(object sender, EventArgs e)
        {
            Point relativeChange = new Point(this.Location.X - this.lastLocation.X, this.Location.Y - this.lastLocation.Y);
     
            WinAPI.SetWindowPos(tripod.Handle, 0, tripod.Location.X + relativeChange.X, tripod.Location.Y + relativeChange.Y, 0, 0, WinAPI.SWP_NOSIZE | WinAPI.SWP_NOZORDER);
            WinAPI.SetWindowPos(leftGun.Handle, 0, leftGun.Location.X + relativeChange.X, leftGun.Location.Y + relativeChange.Y, 0, 0, WinAPI.SWP_NOSIZE | WinAPI.SWP_NOZORDER);
            WinAPI.SetWindowPos(rightGun.Handle, 0, rightGun.Location.X + relativeChange.X, rightGun.Location.Y + relativeChange.Y, 0, 0, WinAPI.SWP_NOSIZE | WinAPI.SWP_NOZORDER);
            //this.BringToFront();

            lastLocation = new Point(this.Location.X, this.Location.Y);
        }

        void Turret_MouseDown(object sender, MouseEventArgs e)
        {
            WinAPI.ReleaseCapture();
            WinAPI.SendMessage(this.Handle, WinAPI.WM_NCLBUTTONDOWN, new IntPtr(WinAPI.HT_CAPTION), IntPtr.Zero);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            int x;
            x = (this.Bounds.X - rightGun.Bounds.X) + rnd.Next(-10, 10);
            if (x > 0)
            {
                x = 0;
            }
            if (x < -40)
            {
                x = -40;
            }
            setPosition(this, rightGun, x, 0);
            x = rnd.Next(0, 40);
            setPosition(this, leftGun, x, 0);
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
            form.TopMost = true;
            return form;
        }

        void setPosition(Form head, Form part, int x, int y)
        {
            part.SendToBack();
            WinAPI.SetWindowPos(part.Handle, 0, head.Bounds.X + x, head.Bounds.Y + y, 0, 0, WinAPI.SWP_NOSIZE | WinAPI.SWP_NOZORDER);
            head.BringToFront();
        }

        void eye_Click(object sender, EventArgs e)
        {
            this.eye_enabled = !this.eye_enabled;
            if (this.eye_enabled)
            {
                this.eye.BackgroundImage = Properties.Resources.eye_enabled;
                timer.Start();
                WinAPI.PlaySound("opera.wav", UIntPtr.Zero, WinAPI.SND_ASYNC | WinAPI.SND_FILENAME);
            }
            else
            {
                this.eye.BackgroundImage = Properties.Resources.eye_disabled;
                WinAPI.PlaySound(null, UIntPtr.Zero, WinAPI.SND_PURGE);
                timer.Stop();
            }
            
        }
    }
}
