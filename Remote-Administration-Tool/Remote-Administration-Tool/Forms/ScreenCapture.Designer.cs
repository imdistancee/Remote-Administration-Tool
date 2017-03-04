namespace Remote_Administration_Tool.Forms
{
    partial class ScreenCapture
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScreenCapture));
            this.pictureScreen = new System.Windows.Forms.PictureBox();
            this.btnTakePic = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureScreen
            // 
            this.pictureScreen.BackColor = System.Drawing.Color.White;
            this.pictureScreen.Location = new System.Drawing.Point(12, 13);
            this.pictureScreen.Name = "pictureScreen";
            this.pictureScreen.Size = new System.Drawing.Size(782, 522);
            this.pictureScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureScreen.TabIndex = 0;
            this.pictureScreen.TabStop = false;
            // 
            // btnTakePic
            // 
            this.btnTakePic.Location = new System.Drawing.Point(340, 541);
            this.btnTakePic.Name = "btnTakePic";
            this.btnTakePic.Size = new System.Drawing.Size(127, 30);
            this.btnTakePic.TabIndex = 1;
            this.btnTakePic.Text = "Take Picture";
            this.btnTakePic.UseVisualStyleBackColor = true;
            this.btnTakePic.Click += new System.EventHandler(this.btnTakePic_Click);
            // 
            // ScreenCapture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(806, 579);
            this.Controls.Add(this.btnTakePic);
            this.Controls.Add(this.pictureScreen);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScreenCapture";
            this.Text = "ScreenCapture";
            ((System.ComponentModel.ISupportInitialize)(this.pictureScreen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureScreen;
        private System.Windows.Forms.Button btnTakePic;
    }
}