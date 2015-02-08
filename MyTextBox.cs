using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MyTextBox
{
    public partial class MyTextBox : UserControl
    {
        public Color SelectedBackColor;
        private int CurrentLine;
        public SolidBrush BrushActiveLine;

        public MyTextBox()
        {
            InitializeComponent();

            SelectedBackColor = Color.Yellow;
            CurrentLine = 0;
            BrushActiveLine = new SolidBrush(SelectedBackColor); // Kolor tła aktywnej linii
        }

        ~MyTextBox()
        {
            BrushActiveLine.Dispose();
            DestroyCaret();
        }

        private void MyTextBox_Load(object sender, EventArgs e)
        {
            CreateCaret(this.Handle, IntPtr.Zero, 0, Font.Height); // Kareta
        }

        private void MyTextBox_Paint(object sender, PaintEventArgs e)
        {
            int x = 0;
            int y = CurrentLine * Font.Height;
            e.Graphics.FillRectangle(BrushActiveLine, x, y, this.Size.Width, Font.Height);
            SetCaretPos(x, y);
        }

        private void MyTextBox_Enter(object sender, EventArgs e)
        {
            ShowCaret(this.Handle);
        }

        private void MyTextBox_Leave(object sender, EventArgs e)
        {
            HideCaret(this.Handle);
        }
    }
}
