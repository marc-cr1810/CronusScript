namespace RuleGenerator
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textRuleName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textRuleFormat = new System.Windows.Forms.TextBox();
            this.OutputBox = new System.Windows.Forms.RichTextBox();
            this.buttonGenerate = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.ComboBoxResultType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Rule Name";
            // 
            // textRuleName
            // 
            this.textRuleName.Location = new System.Drawing.Point(89, 6);
            this.textRuleName.Name = "textRuleName";
            this.textRuleName.Size = new System.Drawing.Size(244, 23);
            this.textRuleName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Rule Format";
            // 
            // textRuleFormat
            // 
            this.textRuleFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textRuleFormat.Location = new System.Drawing.Point(89, 38);
            this.textRuleFormat.Name = "textRuleFormat";
            this.textRuleFormat.Size = new System.Drawing.Size(735, 23);
            this.textRuleFormat.TabIndex = 3;
            // 
            // OutputBox
            // 
            this.OutputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputBox.BackColor = System.Drawing.Color.DimGray;
            this.OutputBox.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.OutputBox.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.OutputBox.Location = new System.Drawing.Point(12, 141);
            this.OutputBox.Name = "OutputBox";
            this.OutputBox.ReadOnly = true;
            this.OutputBox.Size = new System.Drawing.Size(812, 430);
            this.OutputBox.TabIndex = 4;
            this.OutputBox.Text = "";
            // 
            // buttonGenerate
            // 
            this.buttonGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGenerate.Location = new System.Drawing.Point(749, 112);
            this.buttonGenerate.Name = "buttonGenerate";
            this.buttonGenerate.Size = new System.Drawing.Size(75, 23);
            this.buttonGenerate.TabIndex = 5;
            this.buttonGenerate.Text = "Generate";
            this.buttonGenerate.UseVisualStyleBackColor = true;
            this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label3.Location = new System.Drawing.Point(14, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Result Type:";
            // 
            // ComboBoxResultType
            // 
            this.ComboBoxResultType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxResultType.FormattingEnabled = true;
            this.ComboBoxResultType.Items.AddRange(new object[] {
            "ModType",
            "StmtType",
            "ExprType",
            "AstStmtSeq",
            "AstSeq"});
            this.ComboBoxResultType.Location = new System.Drawing.Point(89, 72);
            this.ComboBoxResultType.Name = "ComboBoxResultType";
            this.ComboBoxResultType.Size = new System.Drawing.Size(244, 23);
            this.ComboBoxResultType.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(836, 583);
            this.Controls.Add(this.ComboBoxResultType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonGenerate);
            this.Controls.Add(this.OutputBox);
            this.Controls.Add(this.textRuleFormat);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textRuleName);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "CronusScript Parser Rule Generator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox textRuleName;
        private Label label2;
        private TextBox textRuleFormat;
        private RichTextBox OutputBox;
        private Button buttonGenerate;
        private Label label3;
        private ComboBox ComboBoxResultType;
    }
}