namespace MyTextBox
{
    partial class MyTextBox
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.SuspendLayout();
			// 
			// MyTextBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.Name = "MyTextBox";
			this.Load += new System.EventHandler(this.MyTextBox_Load);
			this.Enter += new System.EventHandler(this.MyTextBox_Enter);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MyTextBox_KeyDown);
			this.Leave += new System.EventHandler(this.MyTextBox_Leave);
			this.ResumeLayout(false);

        }

        #endregion
    }
}
