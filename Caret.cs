using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MyTextBox
{
    public partial class MyTextBox
    {
        [DllImport("user32.dll")]
        static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

        [DllImport("user32.dll")]
        static extern bool DestroyCaret();

        [DllImport("user32.dll")]
        static extern bool SetCaretPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern bool SetCaretBlinkTime(uint uMSeconds);

        [DllImport("user32.dll")]
        static extern bool GetCaretPos(out Point lpPoint);

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowCaret(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern uint GetCaretBlinkTime();
    }
}