namespace ElectronicsInventory
{
    partial class WarningForm
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
            this.warningLabel = new System.Windows.Forms.Label();
            this.yesButton = new System.Windows.Forms.Button();
            this.noButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // warningLabel
            // 
            this.warningLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.warningLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.warningLabel.Location = new System.Drawing.Point(12, 9);
            this.warningLabel.Name = "warningLabel";
            this.warningLabel.Size = new System.Drawing.Size(171, 88);
            this.warningLabel.TabIndex = 0;
            this.warningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // yesButton
            // 
            this.yesButton.Location = new System.Drawing.Point(12, 100);
            this.yesButton.Name = "yesButton";
            this.yesButton.Size = new System.Drawing.Size(75, 44);
            this.yesButton.TabIndex = 1;
            this.yesButton.Text = "YES";
            this.yesButton.UseVisualStyleBackColor = true;
            this.yesButton.Click += new System.EventHandler(this.yesButton_Click);
            // 
            // noButton
            // 
            this.noButton.Location = new System.Drawing.Point(103, 100);
            this.noButton.Name = "noButton";
            this.noButton.Size = new System.Drawing.Size(75, 44);
            this.noButton.TabIndex = 2;
            this.noButton.Text = "NO";
            this.noButton.UseVisualStyleBackColor = true;
            this.noButton.Click += new System.EventHandler(this.noButton_Click);
            // 
            // WarningForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(190, 156);
            this.Controls.Add(this.noButton);
            this.Controls.Add(this.yesButton);
            this.Controls.Add(this.warningLabel);
            this.Name = "WarningForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label warningLabel;
        private System.Windows.Forms.Button yesButton;
        private System.Windows.Forms.Button noButton;
    }
}