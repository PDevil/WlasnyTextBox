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
            SelectedBackColor = Color.LightYellow;
            CurrentLine = 0;
            BrushActiveLine = new SolidBrush(SelectedBackColor);
            InitializeComponent();
        }

        ~MyTextBox()
        {
            BrushActiveLine.Dispose();
        }

        private void MyTextBox_Paint(object sender, PaintEventArgs e)
        {
            int x = 0;
            int y = CurrentLine * Font.Height;
            e.Graphics.FillRectangle(BrushActiveLine, x, y, this.Size.Width, Font.Height);
        }
    }
}
