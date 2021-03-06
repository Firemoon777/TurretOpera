﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace TurretOpera
{
    class WinAPI
    {
        [DllImport("user32.dll")]
        public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll")]
        public static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int fnCombineMode);
        public const int RGN_OR = 2;

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_SHOWWINDOW = 0x0040;
        public const int SWP_FRAMECHANGED = 0x0020;

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int cx, int cy);

        [DllImport("winmm.dll", SetLastError = true)]
        public static extern bool PlaySound(string pszSound, UIntPtr hmod, uint fdwSound);
        public const uint SND_ASYNC = 0x0001;
        public const uint SND_PURGE = 0x0040;
        public const uint SND_FILENAME = 0x00020000;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        public const int HT_CAPTION = 0x2;
        public const int WM_NCLBUTTONDOWN = 0xA1;

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern IntPtr BeginDeferWindowPos(int nNumWindows);
        [DllImport("user32.dll")]
        public static extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);
        [DllImport("user32.dll")]
        public static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const uint WS_THICKFRAME = 0x00040000;
        public const uint WS_MINIMIZE = 0x20000000;
        public const uint WS_MAXIMIZE = 0x01000000;
        public const uint WS_SYSMENU = 0x00080000;
        public const uint WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }
    }

    class WinAPIHelper
    {

        private static IntPtr applyRgn(IntPtr rgn, IntPtr newRgn)
        {
            if (rgn == IntPtr.Zero)
            {
                return newRgn;
            }
            else
            {
                WinAPI.CombineRgn(rgn, rgn, newRgn, WinAPI.RGN_OR);
                return rgn;
            }
        }

        public static IntPtr createRgnFromBmp(Bitmap bitmap) {
            IntPtr result = IntPtr.Zero;
            Rectangle bitmapSize = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            Debug.WriteLine("W = " + bitmap.Width + "; H = " + bitmap.Height);
            BitmapData data = bitmap.LockBits(bitmapSize, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                for (int i = 0; i < bitmap.Height; i++)
                {
                    int start = -1;
                    for (int j = 0; j < bitmap.Width; j++)
                    {
                        if (ptr[0] == 0)
                        {
                            // Чёрный пиксель, регион закончился или не начался
                            if (start != -1)
                            {
                                IntPtr currentRegion = WinAPI.CreateRectRgn(start, i, j - 1, i+1);
                                result = applyRgn(result, currentRegion);
                                start = -1;
                            }
                        }
                        else
                        {
                            // Не чёрный пиксель, регион продолжается
                            if (start == -1)
                            {
                                start = j;
                            }
                        }
                        ptr += 1;
                    }
                    // Регион до конца битмапа
                    if (start != -1)
                    {
                        IntPtr currentRegion = WinAPI.CreateRectRgn(start, i, bitmap.Width-1, i + 1);
                        result = applyRgn(result, currentRegion);
                        start = -1;
                    }
                    ptr += 4 - (bitmap.Width % 4);
                }
            }
            bitmap.UnlockBits(data);
            return result;
        }
    }
}
