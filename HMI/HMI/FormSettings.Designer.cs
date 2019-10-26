namespace HMI
{
    partial class FormSettings
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
            this.btnSave = new System.Windows.Forms.Button();
            this.tbDebugLog = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.numUpDown_A1 = new System.Windows.Forms.NumericUpDown();
            this.numUpDown_a1a = new System.Windows.Forms.NumericUpDown();
            this.numUpDown_A2 = new System.Windows.Forms.NumericUpDown();
            this.numUpDown_a2a = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numUpDown_k = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.numUpDown_delta = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_A1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_a1a)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_A2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_a2a)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_k)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_delta)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(180, 202);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 32;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tbDebugLog
            // 
            this.tbDebugLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDebugLog.Location = new System.Drawing.Point(348, 24);
            this.tbDebugLog.Multiline = true;
            this.tbDebugLog.Name = "tbDebugLog";
            this.tbDebugLog.ReadOnly = true;
            this.tbDebugLog.Size = new System.Drawing.Size(382, 202);
            this.tbDebugLog.TabIndex = 33;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(345, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 34;
            this.label1.Text = "debug log";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(655, 0);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 35;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 153);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(157, 26);
            this.label3.TabIndex = 30;
            this.label3.Text = "Update Kalman filter parameters\r\nfor selected model";
            // 
            // numUpDown_A1
            // 
            this.numUpDown_A1.DecimalPlaces = 1;
            this.numUpDown_A1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numUpDown_A1.Location = new System.Drawing.Point(26, 26);
            this.numUpDown_A1.Name = "numUpDown_A1";
            this.numUpDown_A1.Size = new System.Drawing.Size(73, 20);
            this.numUpDown_A1.TabIndex = 36;
            // 
            // numUpDown_a1a
            // 
            this.numUpDown_a1a.DecimalPlaces = 1;
            this.numUpDown_a1a.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numUpDown_a1a.Location = new System.Drawing.Point(26, 52);
            this.numUpDown_a1a.Name = "numUpDown_a1a";
            this.numUpDown_a1a.Size = new System.Drawing.Size(73, 20);
            this.numUpDown_a1a.TabIndex = 37;
            // 
            // numUpDown_A2
            // 
            this.numUpDown_A2.DecimalPlaces = 1;
            this.numUpDown_A2.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numUpDown_A2.Location = new System.Drawing.Point(26, 78);
            this.numUpDown_A2.Name = "numUpDown_A2";
            this.numUpDown_A2.Size = new System.Drawing.Size(73, 20);
            this.numUpDown_A2.TabIndex = 38;
            // 
            // numUpDown_a2a
            // 
            this.numUpDown_a2a.DecimalPlaces = 1;
            this.numUpDown_a2a.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numUpDown_a2a.Location = new System.Drawing.Point(26, 104);
            this.numUpDown_a2a.Name = "numUpDown_a2a";
            this.numUpDown_a2a.Size = new System.Drawing.Size(73, 20);
            this.numUpDown_a2a.TabIndex = 39;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.numUpDown_k);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numUpDown_a2a);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numUpDown_A2);
            this.groupBox1.Controls.Add(this.numUpDown_A1);
            this.groupBox1.Controls.Add(this.numUpDown_a1a);
            this.groupBox1.Location = new System.Drawing.Point(12, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(162, 215);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Kalman filter";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 106);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(19, 13);
            this.label6.TabIndex = 43;
            this.label6.Text = "a2";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 42;
            this.label5.Text = "A2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 13);
            this.label4.TabIndex = 41;
            this.label4.Text = "a1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 40;
            this.label2.Text = "A1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 132);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(13, 13);
            this.label7.TabIndex = 45;
            this.label7.Text = "k";
            // 
            // numUpDown_k
            // 
            this.numUpDown_k.DecimalPlaces = 1;
            this.numUpDown_k.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numUpDown_k.Location = new System.Drawing.Point(26, 130);
            this.numUpDown_k.Name = "numUpDown_k";
            this.numUpDown_k.Size = new System.Drawing.Size(73, 20);
            this.numUpDown_k.TabIndex = 44;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.numUpDown_delta);
            this.groupBox2.Location = new System.Drawing.Point(180, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(162, 98);
            this.groupBox2.TabIndex = 46;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Anomaly detector";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 28);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(30, 13);
            this.label12.TabIndex = 40;
            this.label12.Text = "delta";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 54);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(139, 26);
            this.label13.TabIndex = 30;
            this.label13.Text = "Update CUSUM parameters\r\nfor selected model";
            // 
            // numUpDown_delta
            // 
            this.numUpDown_delta.DecimalPlaces = 1;
            this.numUpDown_delta.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numUpDown_delta.Location = new System.Drawing.Point(42, 26);
            this.numUpDown_delta.Name = "numUpDown_delta";
            this.numUpDown_delta.Size = new System.Drawing.Size(73, 20);
            this.numUpDown_delta.TabIndex = 36;
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 240);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.tbDebugLog);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSave);
            this.Name = "FormSettings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.FormSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_A1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_a1a)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_A2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_a2a)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_k)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_delta)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox tbDebugLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numUpDown_A1;
        private System.Windows.Forms.NumericUpDown numUpDown_a1a;
        private System.Windows.Forms.NumericUpDown numUpDown_A2;
        private System.Windows.Forms.NumericUpDown numUpDown_a2a;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numUpDown_k;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown numUpDown_delta;
    }
}