namespace CoinCounter
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.picOriginal = new System.Windows.Forms.PictureBox();
            this.picResult = new System.Windows.Forms.PictureBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnCount = new System.Windows.Forms.Button();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.lblOriginal = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picOriginal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picResult)).BeginInit();
            this.SuspendLayout();
            // lblOriginal
            this.lblOriginal.AutoSize = true;
            this.lblOriginal.Location = new System.Drawing.Point(12, 9);
            this.lblOriginal.Text = "Original";
            // lblResult
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(412, 9);
            this.lblResult.Text = "Detected coins";
            // picOriginal
            this.picOriginal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picOriginal.Location = new System.Drawing.Point(12, 28);
            this.picOriginal.Size = new System.Drawing.Size(390, 515);
            this.picOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            // picResult
            this.picResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picResult.Location = new System.Drawing.Point(412, 28);
            this.picResult.Size = new System.Drawing.Size(390, 515);
            this.picResult.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            // btnLoad
            this.btnLoad.Location = new System.Drawing.Point(822, 28);
            this.btnLoad.Size = new System.Drawing.Size(250, 36);
            this.btnLoad.Text = "Load Image";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // btnCount
            this.btnCount.Enabled = false;
            this.btnCount.Location = new System.Drawing.Point(822, 72);
            this.btnCount.Size = new System.Drawing.Size(250, 36);
            this.btnCount.Text = "Count Coins";
            this.btnCount.UseVisualStyleBackColor = true;
            this.btnCount.Click += new System.EventHandler(this.btnCount_Click);
            // txtResults
            this.txtResults.Font = new System.Drawing.Font("Consolas", 11F);
            this.txtResults.Location = new System.Drawing.Point(822, 124);
            this.txtResults.Multiline = true;
            this.txtResults.ReadOnly = true;
            this.txtResults.Size = new System.Drawing.Size(250, 419);
            // Form1
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 556);
            this.Controls.Add(this.lblOriginal);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.picOriginal);
            this.Controls.Add(this.picResult);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnCount);
            this.Controls.Add(this.txtResults);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Text = "Philippine Coin Counter";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picOriginal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        private System.Windows.Forms.PictureBox picOriginal;
        private System.Windows.Forms.PictureBox picResult;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnCount;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.Label lblOriginal;
        private System.Windows.Forms.Label lblResult;
    }
}
