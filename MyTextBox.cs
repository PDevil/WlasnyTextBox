using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MyTextBox
{
    public partial class MyTextBox : UserControl
    {
        public Color SelectedBackColor; // Property
		private int AllLines;
		private int CurrentLine;
		private int CurrentLineIndex;
        private List<string> TextLines;
		private Size ControlSize;
        private SolidBrush BrushActiveLine;

        public MyTextBox()
        {
            InitializeComponent();

            SelectedBackColor = Color.LightYellow;
            AllLines = 0;
            CurrentLine = 0;
			CurrentLineIndex = 0;
            TextLines = new List<string>(1);
            BrushActiveLine = new SolidBrush(SelectedBackColor); // Kolor tła aktywnej linii
        }

        ~MyTextBox()
        {
            BrushActiveLine.Dispose();
            DestroyCaret();
        }

        private void MyTextBox_Load(object sender, EventArgs e)
        {
            CreateCaret(this.Handle, IntPtr.Zero, 0, Font.Height); // Karetka
        }

        private void MyTextBox_Paint(object sender, PaintEventArgs e)
        {
			TextRenderer.MeasureText()
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

        private void MyTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Nowa linia - Enter
            if(e.KeyCode == Keys.Enter)
            {
                TextLines.Insert(CurrentLine, string.Empty);
                AllLines++;
                CurrentLine++;
                this.Invalidate();
            }

            // Usuwanie znaku - BackSp
            else if(e.KeyCode == Keys.Back)
            {
                if (AllLines > 0)
                {
					if(TextLines[CurrentLine] == string.Empty)
					{
						AllLines--;
						CurrentLine--;
						TextLines.RemoveAt(CurrentLine);
					}
					else
					{
						TextLines[CurrentLine].Remove(CurrentLineIndex, 1);
					}
                    this.Invalidate();
                }
            }

			// Normalny znak
            else
            {
                KeysConverter keyConv = new KeysConverter();
				TextLines[CurrentLine].Insert(CurrentLineIndex, keyConv.ConvertToString(e.KeyData));
				CurrentLineIndex++;
				this.Invalidate();
            }
        }
    }
}
