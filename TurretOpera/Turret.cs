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
        private Button head;
        private Button leftGun;
        private Button rightGun;
        private Button tripod;
        private Button eye;
        private Button exit;
        private bool eye_enabled = false;

        private Timer timer;
        private Random rnd;

        private Point lastLocation;

        public Turret()
        {
            InitializeComponent();
            // Setup form theme
            Bitmap headRgnBmp = Properties.Resources.turret_head_rgn;
            WinAPI.SetWindowPos(this.Handle, 0, 0, 0, headRgnBmp.Width, headRgnBmp.Height, WinAPI.SWP_NOMOVE);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            // Enable Transparentcy support
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.TransparencyKey = Color.Green;
            this.BackColor = Color.Green;

            // Setup head
            head = commonSetup(Properties.Resources.turret_head_rgn, Properties.Resources.turret_head_texture);
            this.Controls.Add(head);

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
            this.Controls.Add(tripod);

            // Setup left gun
            leftGun = commonSetup(Properties.Resources.turret_lgun_rgn, Properties.Resources.turret_lgun_texture);
            this.Controls.Add(leftGun);

            // Setup right gun
            rightGun = commonSetup(Properties.Resources.turret_rgun_rgn, Properties.Resources.turret_rgun_texture);
            this.Controls.Add(rightGun);

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
            this.KeyPreview = true;
            this.Activated += Turret_GotFocus;

            // Exit button
            exit = new Button();
            exit.SetBounds(110, 100, 37, 42);
            eye.FlatStyle = FlatStyle.Flat;
            exit.FlatAppearance.BorderSize = 0;
            exit.ForeColor = Color.FromArgb(0, 0, 0, 0);
            exit.Text = "X";
            exit.Click += exit_Click;
            this.Controls.Add(exit);

            
        }

        void exit_Click(object sender, EventArgs e)
        {
            Timer killtimer = new Timer();
            killtimer.Interval = 2000;
            killtimer.Tick += delegate(object _sender, EventArgs _e)
            {
                Application.Exit();
            };
            killtimer.Start();
            WinAPI.PlaySound("fizz.wav", UIntPtr.Zero, WinAPI.SND_ASYNC | WinAPI.SND_FILENAME);

            this.BackgroundImage = null;
            this.BackColor = Color.Black;
            eye.Hide();
            exit.Hide();

            tripod.BackgroundImage = null;
            tripod.BackColor = Color.Black;

            leftGun.BackgroundImage = null;
            leftGun.BackColor = Color.Black;
            rightGun.BackgroundImage = null;
            rightGun.BackColor = Color.Black;

            //Application.Exit();
        }

        void Turret_GotFocus(object sender, EventArgs e)
        {

        }

        void Turret_LocationChanged(object sender, EventArgs e)
        {
            Point relativeChange = new Point(this.Location.X - this.lastLocation.X, this.Location.Y - this.lastLocation.Y);

            IntPtr info = WinAPI.BeginDeferWindowPos(5);

            WinAPI.DeferWindowPos(info, tripod.Handle, IntPtr.Zero, tripod.Location.X + relativeChange.X, tripod.Location.Y + relativeChange.Y, 0, 0, WinAPI.SWP_NOSIZE | WinAPI.SWP_NOZORDER | WinAPI.SWP_SHOWWINDOW);
            WinAPI.DeferWindowPos(info, leftGun.Handle, IntPtr.Zero, leftGun.Location.X + relativeChange.X, leftGun.Location.Y + relativeChange.Y, 0, 0, WinAPI.SWP_NOSIZE | WinAPI.SWP_NOZORDER | WinAPI.SWP_SHOWWINDOW);
            WinAPI.DeferWindowPos(info, rightGun.Handle, IntPtr.Zero, rightGun.Location.X + relativeChange.X, rightGun.Location.Y + relativeChange.Y, 0, 0, WinAPI.SWP_NOSIZE | WinAPI.SWP_NOZORDER | WinAPI.SWP_SHOWWINDOW);

            WinAPI.EndDeferWindowPos(info);

            lastLocation = new Point(this.Location.X, this.Location.Y);
        }

        void Turret_MouseDown(object sender, MouseEventArgs e)
        {
            WinAPI.PlaySound("pickup.wav", UIntPtr.Zero, WinAPI.SND_FILENAME | WinAPI.SND_ASYNC);
            eye.BackgroundImage = Properties.Resources.eye_enabled;

            WinAPI.ReleaseCapture();
            WinAPI.SendMessage(this.Handle, WinAPI.WM_NCLBUTTONDOWN, new IntPtr(WinAPI.HT_CAPTION), IntPtr.Zero);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            /*int x;
            x = (this.Bounds.X - rightGun.Bounds.X) + rnd.Next(-10, 10);
            if (x > 0)
            {
                x = 0;
            }
            if (x < -40)
            {
                x = -40;
            }
            IntPtr info = WinAPI.BeginDeferWindowPos(5);

            setPosition(info, this, rightGun, x, 0);
            x = rnd.Next(0, 40);
            setPosition(info, this, leftGun, x, 0);
            setTop(info, this, leftGun);

            WinAPI.EndDeferWindowPos(info);
            this.BringToFront();*/
        }

        Button commonSetup(Bitmap bitmapRgn, Bitmap bg)
        {
            Button form = new Button();
            IntPtr rgn = WinAPIHelper.createRgnFromBmp(bitmapRgn);
            WinAPI.SetWindowRgn(form.Handle, rgn, true);
            form.SetBounds(0, 0, bitmapRgn.Width, bitmapRgn.Height);
            form.BackgroundImage = bg;
            form.Location = new Point(0, 0);
            return form;
        }

        void setPosition(IntPtr info, Form head, Button part, int x, int y)
        {
            WinAPI.DeferWindowPos(info, part.Handle, this.Handle, head.Bounds.X + x, head.Bounds.Y + y, 0, 0, WinAPI.SWP_NOSIZE);   
        }

        void setTop(IntPtr info, Button form, Button after)
        {
            WinAPI.DeferWindowPos(info, form.Handle, after.Handle, 0, 0, 0, 0, WinAPI.SWP_NOMOVE | WinAPI.SWP_NOSIZE | WinAPI.SWP_SHOWWINDOW);
        }

        void setPosition(Form head, Button part, int x, int y)
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
