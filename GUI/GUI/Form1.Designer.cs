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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
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
            this.textBox_port_recieve = new System.Windows.Forms.TextBox();
            this.textBox_ip_send = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.listBox_controllers = new System.Windows.Forms.ListBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_port_send = new System.Windows.Forms.TextBox();
            this.textBox_ip_recieve = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
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
            chartArea2.Name = "ChartArea1";
            this.dataChart.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.dataChart.Legends.Add(legend2);
            this.dataChart.Location = new System.Drawing.Point(322, 128);
            this.dataChart.Name = "dataChart";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Legend = "Legend1";
            series2.Name = "reference";
            this.dataChart.Series.Add(series2);
            this.dataChart.Size = new System.Drawing.Size(872, 488);
            this.dataChart.TabIndex = 2;
            this.dataChart.Text = "chart1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(319, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Reference";
            // 
            // trackBarReference
            // 
            this.trackBarReference.Location = new System.Drawing.Point(322, 38);
            this.trackBarReference.Maximum = 4;
            this.trackBarReference.Name = "trackBarReference";
            this.trackBarReference.Size = new System.Drawing.Size(233, 45);
            this.trackBarReference.TabIndex = 4;
            this.trackBarReference.Scroll += new System.EventHandler(this.trackBarReference_Scroll);
            // 
            // labelReference
            // 
            this.labelReference.AutoSize = true;
            this.labelReference.Location = new System.Drawing.Point(561, 38);
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
            this.groupBox1.Location = new System.Drawing.Point(600, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(102, 110);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PID settings";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
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
            this.label_time.Location = new System.Drawing.Point(1126, 9);
            this.label_time.Name = "label_time";
            this.label_time.Size = new System.Drawing.Size(54, 13);
            this.label_time.TabIndex = 11;
            this.label_time.Text = "label_time";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBox_ip_recieve);
            this.groupBox2.Controls.Add(this.textBox_port_send);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBox_port_recieve);
            this.groupBox2.Controls.Add(this.textBox_ip_send);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.listBox_controllers);
            this.groupBox2.Location = new System.Drawing.Point(722, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(377, 114);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Controller";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(123, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Port (recieve)";
            // 
            // textBox_port_recieve
            // 
            this.textBox_port_recieve.Location = new System.Drawing.Point(195, 37);
            this.textBox_port_recieve.Name = "textBox_port_recieve";
            this.textBox_port_recieve.Size = new System.Drawing.Size(42, 20);
            this.textBox_port_recieve.TabIndex = 3;
            this.textBox_port_recieve.Text = "8100";
            // 
            // textBox_ip_send
            // 
            this.textBox_ip_send.Location = new System.Drawing.Point(195, 58);
            this.textBox_ip_send.Name = "textBox_ip_send";
            this.textBox_ip_send.Size = new System.Drawing.Size(100, 20);
            this.textBox_ip_send.TabIndex = 2;
            this.textBox_ip_send.Text = "127.0.0.1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(144, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "IP (send)";
            // 
            // listBox_controllers
            // 
            this.listBox_controllers.FormattingEnabled = true;
            this.listBox_controllers.Location = new System.Drawing.Point(6, 19);
            this.listBox_controllers.Name = "listBox_controllers";
            this.listBox_controllers.Size = new System.Drawing.Size(111, 69);
            this.listBox_controllers.TabIndex = 0;
            this.listBox_controllers.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(300, 42);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(71, 36);
            this.button1.TabIndex = 5;
            this.button1.Text = "Allow connection";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(131, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Port (send)";
            // 
            // textBox_port_send
            // 
            this.textBox_port_send.Location = new System.Drawing.Point(195, 79);
            this.textBox_port_send.Name = "textBox_port_send";
            this.textBox_port_send.Size = new System.Drawing.Size(42, 20);
            this.textBox_port_send.TabIndex = 7;
            this.textBox_port_send.Text = "8200";
            // 
            // textBox_ip_recieve
            // 
            this.textBox_ip_recieve.Location = new System.Drawing.Point(195, 16);
            this.textBox_ip_recieve.Name = "textBox_ip_recieve";
            this.textBox_ip_recieve.Size = new System.Drawing.Size(100, 20);
            this.textBox_ip_recieve.TabIndex = 8;
            this.textBox_ip_recieve.Text = "127.0.0.1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(132, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "IP (recieve)";
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
        private System.Windows.Forms.ListBox listBox_controllers;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_port_recieve;
        private System.Windows.Forms.TextBox textBox_ip_send;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        public System.Windows.Forms.DataVisualization.Charting.Chart dataChart;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox_port_send;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_ip_recieve;
        private System.Windows.Forms.Label label7;
    }
}

