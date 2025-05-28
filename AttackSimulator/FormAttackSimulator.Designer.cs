namespace AttackSimulator
{
    partial class FormAttackSimulator
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
            cmbDeviceList = new ComboBox();
            cmbAttackType = new ComboBox();
            rtbLog = new RichTextBox();
            numRate = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)numRate).BeginInit();
            SuspendLayout();
            // 
            // cmbDeviceList
            // 
            cmbDeviceList.FormattingEnabled = true;
            cmbDeviceList.Location = new Point(145, 12);
            cmbDeviceList.Name = "cmbDeviceList";
            cmbDeviceList.Size = new Size(172, 23);
            cmbDeviceList.TabIndex = 0;
            // 
            // cmbAttackType
            // 
            cmbAttackType.FormattingEnabled = true;
            cmbAttackType.Items.AddRange(new object[] { "Flood" });
            cmbAttackType.Location = new Point(145, 60);
            cmbAttackType.Name = "cmbAttackType";
            cmbAttackType.Size = new Size(172, 23);
            cmbAttackType.TabIndex = 0;
            // 
            // rtbLog
            // 
            rtbLog.Location = new Point(16, 98);
            rtbLog.Name = "rtbLog";
            rtbLog.Size = new Size(301, 286);
            rtbLog.TabIndex = 1;
            rtbLog.Text = "";
            // 
            // numRate
            // 
            numRate.Location = new Point(197, 390);
            numRate.Name = "numRate";
            numRate.Size = new Size(120, 23);
            numRate.TabIndex = 2;
            // 
            // FormAttackSimulator
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(337, 450);
            Controls.Add(numRate);
            Controls.Add(rtbLog);
            Controls.Add(cmbAttackType);
            Controls.Add(cmbDeviceList);
            Name = "FormAttackSimulator";
            Text = "Attack Simulator";
            Load += FormAttackSimulator_Load;
            ((System.ComponentModel.ISupportInitialize)numRate).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ComboBox cmbDeviceList;
        private ComboBox cmbAttackType;
        private RichTextBox rtbLog;
        private NumericUpDown numRate;
    }
}
