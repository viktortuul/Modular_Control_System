namespace GUI
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tbDebugLog = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.timerChart = new System.Windows.Forms.Timer(this.components);
            this.dataChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBarReference = new System.Windows.Forms.TrackBar();
            this.labelReference = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDown_Kp = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_Ki = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_Kd = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_update_pid = new System.Windows.Forms.Button();
            this.label_time = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarReference)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Kp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Ki)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Kd)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // tbDebugLog
            // 
            this.tbDebugLog.Location = new System.Drawing.Point(16, 24);
            this.tbDebugLog.Multiline = true;
            this.tbDebugLog.Name = "tbDebugLog";
            this.tbDebugLog.ReadOnly = true;
            this.tbDebugLog.Size = new System.Drawing.Size(289, 592);
            this.tbDebugLog.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "debug log";
            // 
            // timerChart
            // 
            this.timerChart.Enabled = true;
            this.timerChart.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // dataChart
            // 
            chartArea1.Name = "ChartArea1";
            this.dataChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.dataChart.Legends.Add(legend1);
            this.dataChart.Location = new System.Drawing.Point(380, 128);
            this.dataChart.Name = "dataChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "reference";
            this.dataChart.Series.Add(series1);
            this.dataChart.Size = new System.Drawing.Size(872, 488);
            this.dataChart.TabIndex = 2;
            this.dataChart.Text = "chart1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(377, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Reference";
            // 
            // trackBarReference
            // 
            this.trackBarReference.Location = new System.Drawing.Point(380, 38);
            this.trackBarReference.Maximum = 4;
            this.trackBarReference.Name = "trackBarReference";
            this.trackBarReference.Size = new System.Drawing.Size(233, 45);
            this.trackBarReference.TabIndex = 4;
            this.trackBarReference.Scroll += new System.EventHandler(this.trackBarReference_Scroll);
            // 
            // labelReference
            // 
            this.labelReference.AutoSize = true;
            this.labelReference.Location = new System.Drawing.Point(619, 38);
            this.labelReference.Name = "labelReference";
            this.labelReference.Size = new System.Drawing.Size(13, 13);
            this.labelReference.TabIndex = 5;
            this.labelReference.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 65);
            this.label3.TabIndex = 6;
            this.label3.Text = "Kp\r\n\r\nKi\r\n\r\nKd\r\n";
            // 
            // numericUpDown_Kp
            // 
            this.numericUpDown_Kp.DecimalPlaces = 1;
            this.numericUpDown_Kp.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numericUpDown_Kp.Location = new System.Drawing.Point(37, 26);
            this.numericUpDown_Kp.Name = "numericUpDown_Kp";
            this.numericUpDown_Kp.Size = new System.Drawing.Size(49, 20);
            this.numericUpDown_Kp.TabIndex = 7;
            this.numericUpDown_Kp.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDown_Kp.ValueChanged += new System.EventHandler(this.numericUpDown_Kp_ValueChanged);
            // 
            // numericUpDown_Ki
            // 
            this.numericUpDown_Ki.Location = new System.Drawing.Point(37, 51);
            this.numericUpDown_Ki.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDown_Ki.Name = "numericUpDown_Ki";
            this.numericUpDown_Ki.Size = new System.Drawing.Size(49, 20);
            this.numericUpDown_Ki.TabIndex = 8;
            this.numericUpDown_Ki.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDown_Ki.ValueChanged += new System.EventHandler(this.numericUpDown_Ki_ValueChanged);
            // 
            // numericUpDown_Kd
            // 
            this.numericUpDown_Kd.DecimalPlaces = 1;
            this.numericUpDown_Kd.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDown_Kd.Location = new System.Drawing.Point(37, 77);
            this.numericUpDown_Kd.Name = "numericUpDown_Kd";
            this.numericUpDown_Kd.Size = new System.Drawing.Size(49, 20);
            this.numericUpDown_Kd.TabIndex = 9;
            this.numericUpDown_Kd.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_Kd.ValueChanged += new System.EventHandler(this.numericUpDown_Kd_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_update_pid);
            this.groupBox1.Controls.Add(this.numericUpDown_Ki);
            this.groupBox1.Controls.Add(this.numericUpDown_Kd);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numericUpDown_Kp);
            this.groupBox1.Location = new System.Drawing.Point(658, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(104, 110);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PID settings";
            // 
            // button_update_pid
            // 
            this.button_update_pid.Location = new System.Drawing.Point(105, 74);
            this.button_update_pid.Name = "button_update_pid";
            this.button_update_pid.Size = new System.Drawing.Size(75, 23);
            this.button_update_pid.TabIndex = 10;
            this.button_update_pid.Text = "Update";
            this.button_update_pid.UseVisualStyleBackColor = true;
            this.button_update_pid.Click += new System.EventHandler(this.button1_Click);
            // 
            // label_time
            // 
            this.label_time.AutoSize = true;
            this.label_time.Location = new System.Drawing.Point(1061, 20);
            this.label_time.Name = "label_time";
            this.label_time.Size = new System.Drawing.Size(54, 13);
            this.label_time.TabIndex = 11;
            this.label_time.Text = "label_time";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.listBox1);
            this.groupBox2.Location = new System.Drawing.Point(780, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(275, 100);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Controller";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(127, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Port";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(159, 35);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(42, 20);
            this.textBox2.TabIndex = 3;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(159, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(136, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "IP";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "PID (1,20,5)",
            "PID (2,20,5)",
            "PID (3,20,5)",
            "PID (3,30,5)",
            "PID (3,30,5)",
            "PID (5,80,5)",
            "PID (5,80,5)"});
            this.listBox1.Location = new System.Drawing.Point(6, 19);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(111, 69);
            this.listBox1.TabIndex = 0;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1274, 632);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label_time);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelReference);
            this.Controls.Add(this.trackBarReference);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataChart);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbDebugLog);
            this.Name = "Form1";
            this.Text = "GUI";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarReference)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Kp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Ki)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Kd)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDebugLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timerChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart dataChart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackBarReference;
        private System.Windows.Forms.Label labelReference;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDown_Kp;
        private System.Windows.Forms.NumericUpDown numericUpDown_Ki;
        private System.Windows.Forms.NumericUpDown numericUpDown_Kd;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label_time;
        private System.Windows.Forms.Button button_update_pid;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}

