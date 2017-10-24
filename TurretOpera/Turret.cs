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

using NAudio.Wave;

namespace TurretOpera
{
    public partial class Turret : Form
    {
        private Button head;
        private Button leftGun;
        private Button rightGun;
        private Button tripod;
        private Button eye;
        private bool eye_enabled = false;

        Timer soundTimer;
        private readonly string operaWav = "opera.wav";
        byte[] wavBytes;
        int wavLength;

        public Turret()
        {
            InitializeComponent();
            // Setup form theme
            Bitmap headRgnBmp = Properties.Resources.turret_head_rgn;
            WinAPI.SetWindowPos(this.Handle, 0, 0, 0, headRgnBmp.Width, headRgnBmp.Height + 20, WinAPI.SWP_NOMOVE);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            // Enable Transparentcy support
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.TransparencyKey = Color.Green;
            this.BackColor = Color.Green;

            // Setup eye button
            eye = new Button();
            eye.SetBounds(98, 205, 37, 42);
            eye.FlatStyle = FlatStyle.Flat;
            eye.FlatAppearance.BorderSize = 0;
            eye.BackgroundImage = Properties.Resources.eye_disabled;
            eye.Click += eye_Click;
            this.Controls.Add(eye);

            // Setup head
            head = commonSetup(Properties.Resources.turret_head_rgn, Properties.Resources.turret_head_texture);
            this.Controls.Add(head);

            // Setup left gun
            leftGun = commonSetup(Properties.Resources.turret_lgun_rgn, Properties.Resources.turret_lgun_texture);
            openLeftGun(0);
            this.Controls.Add(leftGun);

            // Setup right gun
            rightGun = commonSetup(Properties.Resources.turret_rgun_rgn, Properties.Resources.turret_rgun_texture);
            openRightGun(0);
            this.Controls.Add(rightGun);

            // Setup tripod
            tripod = commonSetup(Properties.Resources.turret_tripod_rgn, Properties.Resources.turret_tripod_texture);
            this.Controls.Add(tripod);

            // Set listeners to all parts
            head.MouseDown += Turret_MouseDown;
            tripod.MouseDown += Turret_MouseDown;
            rightGun.MouseDown += Turret_MouseDown;
            leftGun.MouseDown += Turret_MouseDown;

            // Setup music magic
            soundTimer = new Timer();
            soundTimer.Interval = 200;
            soundTimer.Tick += soundTimer_Tick;
            WaveChannel32 wave = new WaveChannel32(new WaveFileReader(operaWav));
            wavBytes = new byte[352750*60*3];
            wavLength = wave.Read(wavBytes, 0, 352750 * 60 * 3);
            Debug.WriteLine("Time = " + wave.CurrentTime);
        }

        long t = 0;

        void soundTimer_Tick(object sender, EventArgs e)
        {
            //Debug.WriteLine(wavBytes[t]);
            t += 176375 / 5;
            int data = wavBytes[t] * 100 / 255;
            openLeftGun(data);

        }

        void Turret_MouseDown(object sender, MouseEventArgs e)
        {
            WinAPI.PlaySound("pickup.wav", UIntPtr.Zero, WinAPI.SND_FILENAME | WinAPI.SND_ASYNC);
            eye.BackgroundImage = Properties.Resources.eye_enabled;
            openLeftGun(100);
            openRightGun(100);

            WinAPI.ReleaseCapture();
            WinAPI.SendMessage(this.Handle, WinAPI.WM_NCLBUTTONDOWN, new IntPtr(WinAPI.HT_CAPTION), IntPtr.Zero);
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

        void eye_Click(object sender, EventArgs e)
        {
            this.eye_enabled = !this.eye_enabled;
            if (this.eye_enabled)
            {
                this.eye.BackgroundImage = Properties.Resources.eye_enabled;
                WinAPI.PlaySound(operaWav, UIntPtr.Zero, WinAPI.SND_ASYNC | WinAPI.SND_FILENAME);
                soundTimer.Start();
            }
            else
            {
                this.eye.BackgroundImage = Properties.Resources.eye_disabled;
                WinAPI.PlaySound(null, UIntPtr.Zero, WinAPI.SND_PURGE);
                soundTimer.Stop();
            }
            
        }

        void openLeftGun(int percent)
        {
            if(percent < 0) {
                percent = 0;
            }
            if(percent > 100) {
                percent = 100;
            }
            int x = 39 - (int)Math.Truncate(0.39 * percent);
            this.leftGun.Location = new Point(x, 0);
        }

        void openRightGun(int percent)
        {
            if (percent < 0)
            {
                percent = 0;
            }
            if (percent > 100)
            {
                percent = 100;
            }
            percent = 100 - percent;
            int x = -(int)Math.Truncate(0.39 * percent);
            this.rightGun.Location = new Point(x, 0);
        }
    }
}
