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
            WinAPI.SetWindowPos(this.Handle, 0, 0, 0, 400, 400, WinAPI.SWP_NOMOVE);
            IntPtr headRgn = WinAPIHelper.createRgnFromBmp(Properties.Resources.turret_head_rgn);
            WinAPI.SetWindowRgn(this.Handle, headRgn, true);
        }
    }
}
