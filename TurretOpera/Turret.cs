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
        public Turret()
        {
            InitializeComponent();
            Bitmap headRgnBmp = Properties.Resources.turret_head_rgn;
            Bitmap headTextureBmp = Properties.Resources.turret_head_texture;
            WinAPI.SetWindowPos(this.Handle, 0, 0, 0, headTextureBmp.Width, headTextureBmp.Height, WinAPI.SWP_NOMOVE);
            IntPtr headRgn = WinAPIHelper.createRgnFromBmp(headRgnBmp);
            WinAPI.SetWindowRgn(this.Handle, headRgn, true);

            this.BackgroundImage = headTextureBmp;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

        }
    }
}
