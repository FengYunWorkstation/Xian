namespace ClearCanvas.Samples.Cad.View.WinForms
{
    partial class CadApplicationComponentControl
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
			this._thresholdControl = new ClearCanvas.Desktop.View.WinForms.TrackBarUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this._opacityControl = new ClearCanvas.Desktop.View.WinForms.TrackBarUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this._analyzeButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _thresholdControl
			// 
			this._thresholdControl.AutoSize = true;
			this._thresholdControl.DecimalPlaces = 0;
			this._thresholdControl.Location = new System.Drawing.Point(16, 57);
			this._thresholdControl.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this._thresholdControl.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this._thresholdControl.Name = "_thresholdControl";
			this._thresholdControl.Size = new System.Drawing.Size(280, 26);
			this._thresholdControl.TabIndex = 1;
			this._thresholdControl.TrackBarIncrements = 10;
			this._thresholdControl.Value = new decimal(new int[] {
            99,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(20, 41);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Threshold";
			// 
			// _opacityControl
			// 
			this._opacityControl.AutoSize = true;
			this._opacityControl.DecimalPlaces = 0;
			this._opacityControl.Location = new System.Drawing.Point(16, 128);
			this._opacityControl.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this._opacityControl.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this._opacityControl.Name = "_opacityControl";
			this._opacityControl.Size = new System.Drawing.Size(280, 26);
			this._opacityControl.TabIndex = 3;
			this._opacityControl.TrackBarIncrements = 10;
			this._opacityControl.Value = new decimal(new int[] {
            99,
            0,
            0,
            0});
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(20, 112);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(43, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Opacity";
			// 
			// _analyzeButton
			// 
			this._analyzeButton.Location = new System.Drawing.Point(23, 209);
			this._analyzeButton.Name = "_analyzeButton";
			this._analyzeButton.Size = new System.Drawing.Size(75, 23);
			this._analyzeButton.TabIndex = 5;
			this._analyzeButton.Text = "Analyze";
			this._analyzeButton.UseVisualStyleBackColor = true;
			// 
			// CadApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._analyzeButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._opacityControl);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._thresholdControl);
			this.Name = "CadApplicationComponentControl";
			this.Size = new System.Drawing.Size(314, 424);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TrackBarUpDown _thresholdControl;
		private System.Windows.Forms.Label label1;
		private ClearCanvas.Desktop.View.WinForms.TrackBarUpDown _opacityControl;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button _analyzeButton;
    }
}
