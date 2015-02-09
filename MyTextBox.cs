using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
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
        private SolidBrush BrushActiveLine;

		[DllImport("user32.dll")]
		static extern int ToUnicode(uint wVirtKey, uint wScanCode, byte[] lpKeyState,
		  [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)] StringBuilder pwszBuff, int cchBuff,
		  uint wFlags); // Konwertuje VK na znaki Unicode

        public MyTextBox()
        {
            InitializeComponent();

            SelectedBackColor = Color.LightYellow;
            AllLines = 0;
            CurrentLine = 0;
			CurrentLineIndex = 0;
			TextLines = new List<string>(1);
			TextLines.Add(string.Empty);
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
            int x = 0;
            int y = CurrentLine * Font.Height;
            e.Graphics.FillRectangle(BrushActiveLine, 0, y, this.Size.Width, Font.Height);
			for(int i = 0; i < TextLines.Count; i++)
			{
				TextRenderer.DrawText(e.Graphics, TextLines[i], Font, new Rectangle(0, i * Font.Height, this.Size.Width, this.Size.Height), Color.Black, TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding);
			}
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
                TextLines.Insert(CurrentLine + 1, string.Empty);
                AllLines++;
                CurrentLine++;
				if (CurrentLineIndex > TextLines[CurrentLine].Length)
					CurrentLineIndex = TextLines[CurrentLine].Length;

                this.Invalidate();
            }

            // Usuwanie znaku - BackSp
            else if(e.KeyCode == Keys.Back)
            {
				Debug.Write("KeyCode: Backspace Akcja: ");
                if(TextLines[CurrentLine] == string.Empty)
				{
					Debug.Write("Usuwanie linijki tekstu");
					if (AllLines >= 1)
					{
						TextLines.RemoveAt(CurrentLine);
						AllLines--;
						CurrentLine--;
						CurrentLineIndex = TextLines[CurrentLine].Length;
						Debug.WriteLine(string.Format(" Rezultat: Usunięto 1 linię\nVars: -> [AllLines: {0}], [CurrentLine: {1}], [TextLines[{2}]: \"{3}\"]", AllLines, CurrentLine, CurrentLine, TextLines[CurrentLine]));
						this.Invalidate();
					}
					Debug.WriteIf(AllLines == 0, "\n");
				}
				else
				{
					Debug.Write("Usuwanie znaku");
					CurrentLineIndex--;
					TextLines[CurrentLine] =  TextLines[CurrentLine].Remove(CurrentLineIndex, 1);
					Debug.WriteLine(string.Format(" Rezultat: Usunięto znak z indeksu {0}\nVars: [CurrentLineIndex: {1}], [TextLines[{2}]: \"{3}\"{4}]", CurrentLineIndex, CurrentLineIndex, CurrentLine, TextLines[CurrentLine], TextLines[CurrentLine] == string.Empty ? " (sE)" : ""));
					this.Invalidate();
				}
            }

			// Normalny znak
            else
            {
				Debug.Write("Dodawanie znaku");
                KeysConverter keyConv = new KeysConverter();
				string ch = keyConv.ConvertToString(e.KeyCode);
				if (TextLines[CurrentLine].Length == CurrentLineIndex)
					TextLines[CurrentLine] += ch;
				else
					TextLines[CurrentLine].Insert(CurrentLineIndex, ch);

				CurrentLineIndex += ch.Length;
				Debug.WriteLine(string.Format(" Rezultat: Dodano '{0}' do tekstu\nVars: [CurrentLine: {1}], [CurrentLineIndex: {2}], [TextLines[{1}]: \"{3}\"]", ch, CurrentLine, CurrentLineIndex, TextLines[CurrentLine]));
				this.Invalidate();
            }
        }
    }
}
