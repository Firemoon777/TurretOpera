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
        WaveChannel32 wave;
        byte[] wavBytes;
        int wavLength;

        Timer gravityTimer;
        bool inHand = false;
        Timer powerOffTimer;

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
            eye.MouseUp += eye_Click;
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

            // Set move listeners to all parts
            head.MouseDown += Turret_MouseDown;
            tripod.MouseDown += Turret_MouseDown;
            rightGun.MouseDown += Turret_MouseDown;
            leftGun.MouseDown += Turret_MouseDown;

            // Setup music magic
            soundTimer = new Timer();
            soundTimer.Interval = 200;
            soundTimer.Tick += soundTimer_Tick;
            wave = new WaveChannel32(new WaveFileReader(operaWav));
            wavBytes = new byte[352750*60*3];
            wavLength = wave.Read(wavBytes, 0, 352750 * 60 * 3);
            Debug.WriteLine("Time = " + wave.CurrentTime);

            // Setup gravity and etc
            gravityTimer = new Timer();
            gravityTimer.Interval = 10;
            gravityTimer.Tick += gravityTimer_Tick;
            gravityTimer.Start();

            powerOffTimer = new Timer();
            powerOffTimer.Interval = 2000;
            powerOffTimer.Tick += powerOff;
        }

        void prepareToRemove(Button b)
        {
            b.BackgroundImage = null;
            b.BackColor = Color.Black;
        }

        void selfDestruct()
        {
            WinAPI.PlaySound("fizz.wav", UIntPtr.Zero, WinAPI.SND_FILENAME | WinAPI.SND_ASYNC);
            openRightGun(0);
            openLeftGun(0);
            prepareToRemove(head);
            prepareToRemove(leftGun);
            prepareToRemove(rightGun);
            prepareToRemove(tripod);

            head.MouseDown -= Turret_MouseDown;
            tripod.MouseDown -= Turret_MouseDown;
            rightGun.MouseDown -= Turret_MouseDown;
            leftGun.MouseDown -= Turret_MouseDown;

            eye.Hide();
            Timer selfDestructTimer = new Timer();
            selfDestructTimer.Interval = 2000;
            selfDestructTimer.Tick += delegate (object _sender, EventArgs _e) {
                Application.Exit();
            };
            selfDestructTimer.Start();
        }

        void gravityTimer_Tick(object sender, EventArgs e)
        {
            if (inHand == true)
                return;
            if (this.Bounds.Height + this.Bounds.Y + 5 < SystemInformation.VirtualScreen.Height)
            {
                WinAPI.SetWindowPos(this.Handle, 0, this.Bounds.X, this.Bounds.Y + 10, 0, 0, WinAPI.SWP_NOZORDER | WinAPI.SWP_NOSIZE);
            }
        }

        long t = 0;

        void soundTimer_Tick(object sender, EventArgs e)
        {
            //Debug.WriteLine(wavBytes[t]);
            t++;
            int data = wavBytes[t * 176375 / 5];
            int left = (data % 128) * 100 / 128;
            int right = (data >> 1) * 100 / 128;
            openLeftGun(left);
            openRightGun(right);
            if (t * soundTimer.Interval > wave.TotalTime.TotalMilliseconds)
            {
                WinAPI.PlaySound(null, UIntPtr.Zero, WinAPI.SND_PURGE);
                soundTimer.Stop();
                openLeftGun(0);
                openRightGun(0);
                eye.BackgroundImage = Properties.Resources.eye_disabled;
            }
        }

        void Turret_MouseDown(object sender, MouseEventArgs e)
        {
            this.inHand = true;
            soundTimer.Stop();
            powerOffTimer.Stop();
            WinAPI.PlaySound("pickup.wav", UIntPtr.Zero, WinAPI.SND_FILENAME | WinAPI.SND_ASYNC);
            eye.BackgroundImage = Properties.Resources.eye_enabled;
            openLeftGun(100);
            openRightGun(100);

            WinAPI.ReleaseCapture();
            WinAPI.SendMessage(this.Handle, WinAPI.WM_NCLBUTTONDOWN, new IntPtr(WinAPI.HT_CAPTION), IntPtr.Zero);

            this.inHand = false;
            powerOffTimer.Start();
        }

        void powerOff(object sender, EventArgs e)
        {
            this.eye.BackgroundImage = Properties.Resources.eye_disabled;
            this.openLeftGun(0);
            this.openRightGun(0);
            powerOffTimer.Stop();
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
            MouseEventArgs me = (MouseEventArgs)e;
            Debug.WriteLine(me.Button);
            if (me.Button == System.Windows.Forms.MouseButtons.Right)
            {
                selfDestruct();
                return;
            }
            this.eye_enabled = !this.eye_enabled;
            if (this.eye_enabled)
            {
                this.eye.BackgroundImage = Properties.Resources.eye_enabled;
                WinAPI.PlaySound(operaWav, UIntPtr.Zero, WinAPI.SND_ASYNC | WinAPI.SND_FILENAME);
                t = 0;
                soundTimer.Start();
            }
            else
            {
                this.eye.BackgroundImage = Properties.Resources.eye_disabled;
                WinAPI.PlaySound(null, UIntPtr.Zero, WinAPI.SND_PURGE);
                soundTimer.Stop();
                openRightGun(0);
                openLeftGun(0);
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
