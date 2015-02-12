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

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetKeyboardState(byte[] lpKeyState);

        public MyTextBox()
        {
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);

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
			SetCaretPos(0, 0);
        }

		protected override void OnPaint(PaintEventArgs e)
		{
			//int x = 0;
			int y = CurrentLine * Font.Height;
			e.Graphics.FillRectangle(BrushActiveLine, 0, y, this.Size.Width, Font.Height);
			for (int i = 0; i < TextLines.Count; i++)
			{
				TextRenderer.DrawText(e.Graphics, TextLines[i], Font, new Rectangle(0, i * Font.Height, this.Size.Width, this.Size.Height), Color.Black, TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.ExpandTabs);
			}

			base.OnPaint(e);
		}

        private void MyTextBox_Enter(object sender, EventArgs e)
        {
            ShowCaret(this.Handle);
        }

        private void MyTextBox_Leave(object sender, EventArgs e)
        {
            HideCaret(this.Handle);
        }

		protected override bool IsInputKey(Keys keyData)
		{
			//if (keyData == Keys.Tab)
			//{
			//	return true;
			//}

			//return base.IsInputKey(keyData);
			return true;
		}

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Nowa linia - Enter
            if(e.KeyCode == Keys.Enter)
            {
                TextLines.Insert(CurrentLine + 1, string.Empty);
                AllLines++;
                CurrentLine++;
				CurrentLineIndex = 0;
				Point p = new Point();
				GetCaretPos(out p);
				SetCaretPos(0, p.Y + this.Font.Height);

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
						using (Graphics g = this.CreateGraphics())
						{
							Size s;
							Point p = new Point();
							GetCaretPos(out p);
							s = TextRenderer.MeasureText(g, TextLines[CurrentLine], this.Font, new Size(), TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.ExpandTabs);
							SetCaretPos(s.Width, p.Y - this.Font.Height);
						}
						Debug.WriteLine(string.Format(" Rezultat: Usunięto 1 linię\nVars: -> [AllLines: {0}], [CurrentLine: {1}], [TextLines[{2}]: \"{3}\"]", AllLines, CurrentLine, CurrentLine, TextLines[CurrentLine]));
						this.Invalidate();
					}
					Debug.WriteIf(AllLines == 0, "\n");
				}
				else
				{
					Debug.Write("Usuwanie znaku");
					CurrentLineIndex--;
					char chr = TextLines[CurrentLine][CurrentLineIndex];
					using (Graphics g = this.CreateGraphics())
					{
						Size s;
						Point p = new Point();
						GetCaretPos(out p);
						if (chr == '\t')
						{
							s = TextRenderer.MeasureText(g, TextLines[CurrentLine].Substring(0, CurrentLineIndex), this.Font, new Size(), TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.ExpandTabs);
							SetCaretPos(s.Width, p.Y);
						}
						else
						{
							s = TextRenderer.MeasureText(g, chr.ToString(), this.Font, new Size(), TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.ExpandTabs);
							SetCaretPos(p.X - s.Width, p.Y);
						}

						Debug.Write(string.Format(" MT: chr=\"{0}\", p={1}, s={2}", chr, p, s));
					}
					TextLines[CurrentLine] = TextLines[CurrentLine].Remove(CurrentLineIndex, 1);
					Debug.WriteLine(string.Format(" Rezultat: Usunięto znak z indeksu {0}\nVars: [CurrentLineIndex: {1}], [TextLines[{2}]: \"{3}\"{4}]", CurrentLineIndex, CurrentLineIndex, CurrentLine, TextLines[CurrentLine], TextLines[CurrentLine] == string.Empty ? " (sE)" : ""));
					this.Invalidate();
				}
            }

			// Lewo [0x25], Góra [0x26], Prawo [0x27], Dół [0x28]
			else if(e.KeyValue >= 37 && e.KeyValue <= 40)
			{
				Debug.Write("Strzałki");
				Debug.WriteLine(" Klawisz: 0x" + Convert.ToString(e.KeyValue, 16));
				Point p = new Point();
				bool leftRight = false;
				char chr = new char();
				GetCaretPos(out p);
				switch (e.KeyCode)
				{
					case Keys.Up:
						if (CurrentLine <= 0)
							goto Process_KeyDown;

						CurrentLine--;
						p.Y -= this.Font.Height;

						if (CurrentLineIndex > TextLines[CurrentLine].Length)
							CurrentLineIndex = TextLines[CurrentLine].Length;

						Debug.Write(string.Format(" (Góra) CurrentLine--[{0}]", CurrentLine));
						break;

					case Keys.Down:
						if (CurrentLine >= AllLines)
							goto Process_KeyDown;

						CurrentLine++;
						p.Y += this.Font.Height;

						if (CurrentLineIndex > TextLines[CurrentLine].Length)
							CurrentLineIndex = TextLines[CurrentLine].Length;

						Debug.Write(string.Format(" (Dół) CurrentLine++[{0}]", CurrentLine));
						break;

					case Keys.Left:
						if (CurrentLineIndex <= 0)
						{
							if (CurrentLine <= 0)
								goto Process_KeyDown;

							CurrentLine--;
							CurrentLineIndex = TextLines[CurrentLine].Length;
							p.Y -= this.Font.Height;
						}
						else
						{
							leftRight = true;
							CurrentLineIndex--;
							chr = TextLines[CurrentLine][CurrentLineIndex];
						}

						Debug.Write(string.Format(" (Lewo) CurrentLineIndex--[{0}] chr={1}", CurrentLineIndex, chr));
						break;

					case Keys.Right:
						if (CurrentLineIndex >= TextLines[CurrentLine].Length)
						{
							if (CurrentLine == AllLines)
								goto Process_KeyDown;

							CurrentLine++;
							CurrentLineIndex = 0;
							p.Y += this.Font.Height;
						}
						else
						{
							leftRight = true;
							chr = TextLines[CurrentLine][CurrentLineIndex];
							CurrentLineIndex++;
						}

						Debug.Write(string.Format(" (Prawo) CurrentLineIndex++[{0}] chr={1}", CurrentLineIndex, chr));
						break;
				}

				Size s;
				using(Graphics g = this.CreateGraphics())
				{
					if(leftRight == false) // Jeśli zmieniana jest linia z góry na dół
					{
						if (CurrentLineIndex == 0) // Nie ma sensu wywoływać funkcji dla pustego ciągu znaków
						{
							SetCaretPos(0, p.Y);
						}
						else // Prawda...?
						{
							s = TextRenderer.MeasureText(g, TextLines[CurrentLine].Substring(0, CurrentLineIndex), this.Font, new Size(), TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.ExpandTabs);
							SetCaretPos(s.Width, p.Y);
							Debug.Write(string.Format(" s={0}", s.ToString()));
						}
					}
					else // Jeśli zmieniany jest tylko indeks prawy/lewy
					{
						s = TextRenderer.MeasureText(g, chr.ToString(), this.Font, new Size(), TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.ExpandTabs);
						SetCaretPos((e.KeyValue == 37) ? (p.X - s.Width) : (p.X + s.Width), p.Y);
						Debug.Write(string.Format(" s={0}", s.ToString()));
					}
				}
				this.Invalidate();
				Debug.WriteLine("");
			}

			// Ctrl <- TODO
			//else if (e.Control == true)
			//{
			//	Debug.Write(string.Format("Control [KeyValue: 0x{0}, KeyData: 0x{1}, KeyChar: 0x{2}]", Convert.ToString(e.KeyValue, 16).PadLeft(8, '0'), Convert.ToString((int)e.KeyData, 16).PadLeft(8, '0'), Convert.ToString((int)e.KeyCode, 16).PadLeft(8, '0')));
			//}

			// Normalny znak
			else
			{
				if (e.KeyCode == Keys.Escape)
					goto Process_KeyDown;

				Debug.Write("Dodawanie znaku");
				byte[] keyb = new byte[256];
				StringBuilder sb = new StringBuilder(64);
				GetKeyboardState(keyb); // Pobranie informacji nt. przycisków
				int result = ToUnicode((uint)e.KeyValue, 0, keyb, sb, 64, 0); // Zamiana bitów na znak Unicode
				if (result != 0)
				{
					string ret = sb[0].ToString();
					if (TextLines[CurrentLine].Length == CurrentLineIndex)
						TextLines[CurrentLine] += ret;
					else
						TextLines[CurrentLine] = TextLines[CurrentLine].Insert(CurrentLineIndex, ret);

					CurrentLineIndex += 1;

					using (Graphics g = this.CreateGraphics())
					{
						Size s;
						Point p = new Point();
						GetCaretPos(out p);
						if (e.KeyCode == Keys.Tab)
						{
							s = TextRenderer.MeasureText(g, TextLines[CurrentLine].Substring(0, CurrentLineIndex), this.Font, new Size(), TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.ExpandTabs);
							SetCaretPos(s.Width, p.Y);
						}
						else
						{
							s = TextRenderer.MeasureText(g, ret, this.Font, new Size(), TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.ExpandTabs);
							SetCaretPos(p.X + s.Width, p.Y);
						}

						Debug.Write(string.Format(" MT: keyb=\"{0}\", p={1}, s={2}, ret={3}", keyb, p, s, ret));
					}

					Debug.WriteLine(string.Format(" Rezultat: Dodano '{0}' do tekstu\nVars: [CurrentLine: {1}], [CurrentLineIndex: {2}], [TextLines[{1}]: \"{3}\"]", ret, CurrentLine, CurrentLineIndex, TextLines[CurrentLine]));
					this.Invalidate();
				}
				Debug.WriteLineIf(result == 0, string.Format(" Rezultat: ToUnicode zwróciło 0 (brak znaku do dodawania)"));
			}

			Process_KeyDown:
			base.OnKeyDown(e);
        }
    }
}