namespace ElectronicsInventory
{
    partial class ListForm
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
            this.exitButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.inventoryLabel = new System.Windows.Forms.Label();
            this.networkLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // exitButton
            // 
            this.exitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitButton.Location = new System.Drawing.Point(12, 574);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(214, 33);
            this.exitButton.TabIndex = 1;
            this.exitButton.Text = "EXIT";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(214, 27);
            this.label1.TabIndex = 2;
            this.label1.Text = "PHONE INVENTORY";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // inventoryLabel
            // 
            this.inventoryLabel.BackColor = System.Drawing.SystemColors.HighlightText;
            this.inventoryLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inventoryLabel.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.inventoryLabel.Location = new System.Drawing.Point(12, 61);
            this.inventoryLabel.Name = "inventoryLabel";
            this.inventoryLabel.Size = new System.Drawing.Size(214, 497);
            this.inventoryLabel.TabIndex = 3;
            // 
            // networkLabel
            // 
            this.networkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.networkLabel.Location = new System.Drawing.Point(12, 32);
            this.networkLabel.Name = "networkLabel";
            this.networkLabel.Size = new System.Drawing.Size(214, 27);
            this.networkLabel.TabIndex = 4;
            this.networkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(238, 619);
            this.Controls.Add(this.networkLabel);
            this.Controls.Add(this.inventoryLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.exitButton);
            this.Name = "ListForm";
            this.Text = "Phone Inventory";
            this.Load += new System.EventHandler(this.ListForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label inventoryLabel;
        private System.Windows.Forms.Label networkLabel;
    }
}