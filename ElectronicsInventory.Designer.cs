namespace ElectronicsInventory
{
    partial class ElectronicsInventory
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
            this.titleLabel = new System.Windows.Forms.Label();
            this.phonesButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 17.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(12, 9);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(288, 29);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Inventory Management";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // phonesButton
            // 
            this.phonesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.phonesButton.Location = new System.Drawing.Point(12, 51);
            this.phonesButton.Name = "phonesButton";
            this.phonesButton.Size = new System.Drawing.Size(288, 50);
            this.phonesButton.TabIndex = 1;
            this.phonesButton.Text = "Phones";
            this.phonesButton.UseVisualStyleBackColor = true;
            this.phonesButton.Click += new System.EventHandler(this.phonesButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitButton.Location = new System.Drawing.Point(12, 107);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(288, 50);
            this.exitButton.TabIndex = 2;
            this.exitButton.Text = "EXIT";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // ElectronicsInventory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 165);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.phonesButton);
            this.Controls.Add(this.titleLabel);
            this.Name = "ElectronicsInventory";
            this.Text = "Inventory Management";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button phonesButton;
        private System.Windows.Forms.Button exitButton;
    }
}