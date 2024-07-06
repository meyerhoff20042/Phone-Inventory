namespace ElectronicsInventory
{
    partial class UpdateForm
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
            this.oldLabel = new System.Windows.Forms.Label();
            this.newTextBox = new System.Windows.Forms.TextBox();
            this.oldPromptLabel = new System.Windows.Forms.Label();
            this.newPromptLabel = new System.Windows.Forms.Label();
            this.updateButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // oldLabel
            // 
            this.oldLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.oldLabel.Location = new System.Drawing.Point(88, 19);
            this.oldLabel.Name = "oldLabel";
            this.oldLabel.Size = new System.Drawing.Size(100, 20);
            this.oldLabel.TabIndex = 0;
            // 
            // newTextBox
            // 
            this.newTextBox.Location = new System.Drawing.Point(88, 45);
            this.newTextBox.Name = "newTextBox";
            this.newTextBox.Size = new System.Drawing.Size(100, 20);
            this.newTextBox.TabIndex = 1;
            // 
            // oldPromptLabel
            // 
            this.oldPromptLabel.AutoSize = true;
            this.oldPromptLabel.Location = new System.Drawing.Point(18, 20);
            this.oldPromptLabel.Name = "oldPromptLabel";
            this.oldPromptLabel.Size = new System.Drawing.Size(23, 13);
            this.oldPromptLabel.TabIndex = 2;
            this.oldPromptLabel.Text = "Old";
            // 
            // newPromptLabel
            // 
            this.newPromptLabel.AutoSize = true;
            this.newPromptLabel.Location = new System.Drawing.Point(12, 48);
            this.newPromptLabel.Name = "newPromptLabel";
            this.newPromptLabel.Size = new System.Drawing.Size(29, 13);
            this.newPromptLabel.TabIndex = 3;
            this.newPromptLabel.Text = "New";
            // 
            // updateButton
            // 
            this.updateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateButton.Location = new System.Drawing.Point(21, 87);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(167, 49);
            this.updateButton.TabIndex = 4;
            this.updateButton.Text = "UPDATE";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // UpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(205, 150);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.newPromptLabel);
            this.Controls.Add(this.oldPromptLabel);
            this.Controls.Add(this.newTextBox);
            this.Controls.Add(this.oldLabel);
            this.Name = "UpdateForm";
            this.Text = "Update";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label oldLabel;
        private System.Windows.Forms.TextBox newTextBox;
        private System.Windows.Forms.Label oldPromptLabel;
        private System.Windows.Forms.Label newPromptLabel;
        private System.Windows.Forms.Button updateButton;
    }
}