namespace Model_GUI
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.buttonApplyDisturbance = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numUpDownDuration = new System.Windows.Forms.NumericUpDown();
            this.radioButtonConstant = new System.Windows.Forms.RadioButton();
            this.radioButtonTransient = new System.Windows.Forms.RadioButton();
            this.radioButtonSinusoid = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numUpDownAmplitude11 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numUpDownFrequency = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.labelDebug = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.labelDisturbance = new System.Windows.Forms.Label();
            this.numUpDownAmplitude21 = new System.Windows.Forms.NumericUpDown();
            this.numUpDownAmplitude12 = new System.Windows.Forms.NumericUpDown();
            this.numUpDownAmplitude22 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownDuration)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownAmplitude11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownFrequency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownAmplitude21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownAmplitude12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownAmplitude22)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonApplyDisturbance
            // 
            this.buttonApplyDisturbance.Location = new System.Drawing.Point(12, 147);
            this.buttonApplyDisturbance.Name = "buttonApplyDisturbance";
            this.buttonApplyDisturbance.Size = new System.Drawing.Size(70, 31);
            this.buttonApplyDisturbance.TabIndex = 0;
            this.buttonApplyDisturbance.Text = "Apply";
            this.buttonApplyDisturbance.UseVisualStyleBackColor = true;
            this.buttonApplyDisturbance.Click += new System.EventHandler(this.buttonApplyDisturbance_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(94, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Duration [s]";
            // 
            // numUpDownDuration
            // 
            this.numUpDownDuration.Location = new System.Drawing.Point(193, 26);
            this.numUpDownDuration.Name = "numUpDownDuration";
            this.numUpDownDuration.Size = new System.Drawing.Size(47, 20);
            this.numUpDownDuration.TabIndex = 3;
            this.numUpDownDuration.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // radioButtonConstant
            // 
            this.radioButtonConstant.AutoSize = true;
            this.radioButtonConstant.Location = new System.Drawing.Point(5, 26);
            this.radioButtonConstant.Name = "radioButtonConstant";
            this.radioButtonConstant.Size = new System.Drawing.Size(67, 17);
            this.radioButtonConstant.TabIndex = 9;
            this.radioButtonConstant.TabStop = true;
            this.radioButtonConstant.Text = "Constant";
            this.radioButtonConstant.UseVisualStyleBackColor = true;
            // 
            // radioButtonTransient
            // 
            this.radioButtonTransient.AutoSize = true;
            this.radioButtonTransient.Location = new System.Drawing.Point(5, 49);
            this.radioButtonTransient.Name = "radioButtonTransient";
            this.radioButtonTransient.Size = new System.Drawing.Size(69, 17);
            this.radioButtonTransient.TabIndex = 10;
            this.radioButtonTransient.TabStop = true;
            this.radioButtonTransient.Text = "Transient";
            this.radioButtonTransient.UseVisualStyleBackColor = true;
            // 
            // radioButtonSinusoid
            // 
            this.radioButtonSinusoid.AutoSize = true;
            this.radioButtonSinusoid.Location = new System.Drawing.Point(5, 72);
            this.radioButtonSinusoid.Name = "radioButtonSinusoid";
            this.radioButtonSinusoid.Size = new System.Drawing.Size(65, 17);
            this.radioButtonSinusoid.TabIndex = 11;
            this.radioButtonSinusoid.TabStop = true;
            this.radioButtonSinusoid.Text = "Sinusoid";
            this.radioButtonSinusoid.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numUpDownAmplitude22);
            this.groupBox1.Controls.Add(this.numUpDownAmplitude12);
            this.groupBox1.Controls.Add(this.numUpDownAmplitude21);
            this.groupBox1.Controls.Add(this.numUpDownAmplitude11);
            this.groupBox1.Controls.Add(this.radioButtonTransient);
            this.groupBox1.Controls.Add(this.radioButtonSinusoid);
            this.groupBox1.Controls.Add(this.radioButtonConstant);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numUpDownFrequency);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numUpDownDuration);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(318, 129);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Disturbance type";
            // 
            // numUpDownAmplitude11
            // 
            this.numUpDownAmplitude11.Location = new System.Drawing.Point(192, 72);
            this.numUpDownAmplitude11.Name = "numUpDownAmplitude11";
            this.numUpDownAmplitude11.Size = new System.Drawing.Size(47, 20);
            this.numUpDownAmplitude11.TabIndex = 16;
            this.numUpDownAmplitude11.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(94, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Amplitude [cm3/s]";
            // 
            // numUpDownFrequency
            // 
            this.numUpDownFrequency.DecimalPlaces = 1;
            this.numUpDownFrequency.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numUpDownFrequency.Location = new System.Drawing.Point(193, 49);
            this.numUpDownFrequency.Name = "numUpDownFrequency";
            this.numUpDownFrequency.Size = new System.Drawing.Size(47, 20);
            this.numUpDownFrequency.TabIndex = 12;
            this.numUpDownFrequency.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(94, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Frequency [Hz]";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(83, 147);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(176, 31);
            this.progressBar1.TabIndex = 14;
            // 
            // labelDebug
            // 
            this.labelDebug.AutoSize = true;
            this.labelDebug.Location = new System.Drawing.Point(336, 21);
            this.labelDebug.Name = "labelDebug";
            this.labelDebug.Size = new System.Drawing.Size(61, 13);
            this.labelDebug.TabIndex = 17;
            this.labelDebug.Text = "Duration [s]";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // labelDisturbance
            // 
            this.labelDisturbance.AutoSize = true;
            this.labelDisturbance.Location = new System.Drawing.Point(336, 44);
            this.labelDisturbance.Name = "labelDisturbance";
            this.labelDisturbance.Size = new System.Drawing.Size(64, 13);
            this.labelDisturbance.TabIndex = 18;
            this.labelDisturbance.Text = "Disturbance";
            // 
            // numUpDownAmplitude21
            // 
            this.numUpDownAmplitude21.Location = new System.Drawing.Point(193, 98);
            this.numUpDownAmplitude21.Name = "numUpDownAmplitude21";
            this.numUpDownAmplitude21.Size = new System.Drawing.Size(47, 20);
            this.numUpDownAmplitude21.TabIndex = 17;
            // 
            // numUpDownAmplitude12
            // 
            this.numUpDownAmplitude12.Location = new System.Drawing.Point(245, 72);
            this.numUpDownAmplitude12.Name = "numUpDownAmplitude12";
            this.numUpDownAmplitude12.Size = new System.Drawing.Size(47, 20);
            this.numUpDownAmplitude12.TabIndex = 18;
            // 
            // numUpDownAmplitude22
            // 
            this.numUpDownAmplitude22.Location = new System.Drawing.Point(245, 98);
            this.numUpDownAmplitude22.Name = "numUpDownAmplitude22";
            this.numUpDownAmplitude22.Size = new System.Drawing.Size(47, 20);
            this.numUpDownAmplitude22.TabIndex = 19;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 333);
            this.Controls.Add(this.labelDisturbance);
            this.Controls.Add(this.labelDebug);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonApplyDisturbance);
            this.Name = "Form1";
            this.Text = "Model";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownDuration)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownAmplitude11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownFrequency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownAmplitude21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownAmplitude12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownAmplitude22)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonApplyDisturbance;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numUpDownDuration;
        private System.Windows.Forms.RadioButton radioButtonConstant;
        private System.Windows.Forms.RadioButton radioButtonTransient;
        private System.Windows.Forms.RadioButton radioButtonSinusoid;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numUpDownFrequency;
        private System.Windows.Forms.NumericUpDown numUpDownAmplitude11;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label labelDebug;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label labelDisturbance;
        private System.Windows.Forms.NumericUpDown numUpDownAmplitude22;
        private System.Windows.Forms.NumericUpDown numUpDownAmplitude12;
        private System.Windows.Forms.NumericUpDown numUpDownAmplitude21;
    }
}

