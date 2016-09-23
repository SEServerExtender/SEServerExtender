namespace SEModAPI
{
    partial class BlockLimitConfig
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
            this.BTN_Limit_Add = new System.Windows.Forms.Button();
            this.TP_Block_Limits = new System.Windows.Forms.TableLayoutPanel();
            this.BTN_Limit_Save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BTN_Limit_Add
            // 
            this.BTN_Limit_Add.Location = new System.Drawing.Point(12, 12);
            this.BTN_Limit_Add.Name = "BTN_Limit_Add";
            this.BTN_Limit_Add.Size = new System.Drawing.Size(76, 26);
            this.BTN_Limit_Add.TabIndex = 1;
            this.BTN_Limit_Add.Text = "Add Limit";
            this.BTN_Limit_Add.UseVisualStyleBackColor = true;
            this.BTN_Limit_Add.Click += new System.EventHandler(this.BTN_Limit_Add_Click);
            // 
            // TP_Block_Limits
            // 
            this.TP_Block_Limits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TP_Block_Limits.AutoScroll = true;
            this.TP_Block_Limits.ColumnCount = 1;
            this.TP_Block_Limits.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TP_Block_Limits.Location = new System.Drawing.Point(12, 48);
            this.TP_Block_Limits.Name = "TP_Block_Limits";
            this.TP_Block_Limits.RowCount = 1;
            this.TP_Block_Limits.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TP_Block_Limits.Size = new System.Drawing.Size(401, 301);
            this.TP_Block_Limits.TabIndex = 2;
            // 
            // BTN_Limit_Save
            // 
            this.BTN_Limit_Save.Location = new System.Drawing.Point(94, 12);
            this.BTN_Limit_Save.Name = "BTN_Limit_Save";
            this.BTN_Limit_Save.Size = new System.Drawing.Size(76, 26);
            this.BTN_Limit_Save.TabIndex = 3;
            this.BTN_Limit_Save.Text = "Save";
            this.BTN_Limit_Save.UseVisualStyleBackColor = true;
            this.BTN_Limit_Save.Click += new System.EventHandler(this.BTN_Limit_Save_Click);
            // 
            // BlockLimitConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 361);
            this.Controls.Add(this.BTN_Limit_Save);
            this.Controls.Add(this.TP_Block_Limits);
            this.Controls.Add(this.BTN_Limit_Add);
            this.Name = "BlockLimitConfig";
            this.Text = "Block Limit Config";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button BTN_Limit_Add;
        private System.Windows.Forms.TableLayoutPanel TP_Block_Limits;
        private System.Windows.Forms.Button BTN_Limit_Save;
    }
}