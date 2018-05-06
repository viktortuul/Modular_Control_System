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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea13 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend13 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea14 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend14 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint9 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 5D);
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea15 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend15 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series10 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint10 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 5D);
            this.tbDebugLog = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.timerChart = new System.Windows.Forms.Timer(this.components);
            this.dataChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.trackBarReference = new System.Windows.Forms.TrackBar();
            this.labelReference1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numUpDownKp = new System.Windows.Forms.NumericUpDown();
            this.numUpDownKi = new System.Windows.Forms.NumericUpDown();
            this.numUpDownKd = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_update_pid = new System.Windows.Forms.Button();
            this.label_time = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericUpDown_port_send = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_port_recieve = new System.Windows.Forms.NumericUpDown();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_ip_recieve = new System.Windows.Forms.TextBox();
            this.btnAllowConnection = new System.Windows.Forms.Button();
            this.textBox_ip_send = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.listBoxControllers = new System.Windows.Forms.ListBox();
            this.chartTankBottom = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartTankTop = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.treeViewControllers = new System.Windows.Forms.TreeView();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.labelReference2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarReference)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownKp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownKi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownKd)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_port_send)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_port_recieve)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartTankBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartTankTop)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbDebugLog
            // 
            this.tbDebugLog.Location = new System.Drawing.Point(923, 24);
            this.tbDebugLog.Multiline = true;
            this.tbDebugLog.Name = "tbDebugLog";
            this.tbDebugLog.ReadOnly = true;
            this.tbDebugLog.Size = new System.Drawing.Size(492, 93);
            this.tbDebugLog.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(921, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "debug log";
            // 
            // timerChart
            // 
            this.timerChart.Tick += new System.EventHandler(this.timerChart_Tick);
            // 
            // dataChart
            // 
            this.dataChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea13.Area3DStyle.Inclination = 0;
            chartArea13.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
            chartArea13.Area3DStyle.Rotation = 0;
            chartArea13.Area3DStyle.WallWidth = 1;
            chartArea13.BackColor = System.Drawing.SystemColors.ControlLight;
            chartArea13.Name = "ChartArea1";
            chartArea13.ShadowColor = System.Drawing.Color.Gray;
            this.dataChart.ChartAreas.Add(chartArea13);
            legend13.Name = "Legend1";
            this.dataChart.Legends.Add(legend13);
            this.dataChart.Location = new System.Drawing.Point(191, 150);
            this.dataChart.Name = "dataChart";
            this.dataChart.Size = new System.Drawing.Size(716, 526);
            this.dataChart.TabIndex = 2;
            this.dataChart.Text = "chart1";
            // 
            // trackBarReference
            // 
            this.trackBarReference.Location = new System.Drawing.Point(11, 24);
            this.trackBarReference.Maximum = 20;
            this.trackBarReference.Name = "trackBarReference";
            this.trackBarReference.Size = new System.Drawing.Size(155, 45);
            this.trackBarReference.TabIndex = 4;
            this.trackBarReference.Scroll += new System.EventHandler(this.trackBarReference_Scroll);
            // 
            // labelReference1
            // 
            this.labelReference1.AutoSize = true;
            this.labelReference1.Location = new System.Drawing.Point(172, 27);
            this.labelReference1.Name = "labelReference1";
            this.labelReference1.Size = new System.Drawing.Size(13, 13);
            this.labelReference1.TabIndex = 5;
            this.labelReference1.Text = "0";
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
            // numUpDownKp
            // 
            this.numUpDownKp.DecimalPlaces = 1;
            this.numUpDownKp.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numUpDownKp.Location = new System.Drawing.Point(37, 26);
            this.numUpDownKp.Name = "numUpDownKp";
            this.numUpDownKp.Size = new System.Drawing.Size(49, 20);
            this.numUpDownKp.TabIndex = 7;
            this.numUpDownKp.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numUpDownKp.ValueChanged += new System.EventHandler(this.numUpDownKp_ValueChanged);
            // 
            // numUpDownKi
            // 
            this.numUpDownKi.DecimalPlaces = 1;
            this.numUpDownKi.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numUpDownKi.Location = new System.Drawing.Point(37, 51);
            this.numUpDownKi.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numUpDownKi.Name = "numUpDownKi";
            this.numUpDownKi.Size = new System.Drawing.Size(49, 20);
            this.numUpDownKi.TabIndex = 8;
            this.numUpDownKi.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numUpDownKi.ValueChanged += new System.EventHandler(this.numUpDownKi_ValueChanged);
            // 
            // numUpDownKd
            // 
            this.numUpDownKd.DecimalPlaces = 1;
            this.numUpDownKd.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numUpDownKd.Location = new System.Drawing.Point(37, 77);
            this.numUpDownKd.Name = "numUpDownKd";
            this.numUpDownKd.Size = new System.Drawing.Size(49, 20);
            this.numUpDownKd.TabIndex = 9;
            this.numUpDownKd.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numUpDownKd.ValueChanged += new System.EventHandler(this.numUpDownKd_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_update_pid);
            this.groupBox1.Controls.Add(this.numUpDownKi);
            this.groupBox1.Controls.Add(this.numUpDownKd);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numUpDownKp);
            this.groupBox1.Location = new System.Drawing.Point(191, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(102, 110);
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
            // 
            // label_time
            // 
            this.label_time.AutoSize = true;
            this.label_time.Location = new System.Drawing.Point(751, 18);
            this.label_time.Name = "label_time";
            this.label_time.Size = new System.Drawing.Size(54, 13);
            this.label_time.TabIndex = 11;
            this.label_time.Text = "label_time";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numericUpDown_port_send);
            this.groupBox2.Controls.Add(this.numericUpDown_port_recieve);
            this.groupBox2.Controls.Add(this.textBoxName);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBox_ip_recieve);
            this.groupBox2.Controls.Add(this.btnAllowConnection);
            this.groupBox2.Controls.Add(this.textBox_ip_send);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(503, 11);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(242, 110);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "New controller";
            // 
            // numericUpDown_port_send
            // 
            this.numericUpDown_port_send.Location = new System.Drawing.Point(187, 51);
            this.numericUpDown_port_send.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDown_port_send.Maximum = new decimal(new int[] {
            9000,
            0,
            0,
            0});
            this.numericUpDown_port_send.Minimum = new decimal(new int[] {
            8000,
            0,
            0,
            0});
            this.numericUpDown_port_send.Name = "numericUpDown_port_send";
            this.numericUpDown_port_send.Size = new System.Drawing.Size(50, 20);
            this.numericUpDown_port_send.TabIndex = 20;
            this.numericUpDown_port_send.Value = new decimal(new int[] {
            8200,
            0,
            0,
            0});
            // 
            // numericUpDown_port_recieve
            // 
            this.numericUpDown_port_recieve.Location = new System.Drawing.Point(187, 24);
            this.numericUpDown_port_recieve.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDown_port_recieve.Maximum = new decimal(new int[] {
            9000,
            0,
            0,
            0});
            this.numericUpDown_port_recieve.Minimum = new decimal(new int[] {
            8000,
            0,
            0,
            0});
            this.numericUpDown_port_recieve.Name = "numericUpDown_port_recieve";
            this.numericUpDown_port_recieve.Size = new System.Drawing.Size(50, 20);
            this.numericUpDown_port_recieve.TabIndex = 19;
            this.numericUpDown_port_recieve.Value = new decimal(new int[] {
            8100,
            0,
            0,
            0});
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(46, 78);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(52, 20);
            this.textBoxName.TabIndex = 11;
            this.textBoxName.Text = "PID1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Name";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "IP:Port (this)";
            // 
            // textBox_ip_recieve
            // 
            this.textBox_ip_recieve.Enabled = false;
            this.textBox_ip_recieve.Location = new System.Drawing.Point(103, 24);
            this.textBox_ip_recieve.Name = "textBox_ip_recieve";
            this.textBox_ip_recieve.Size = new System.Drawing.Size(80, 20);
            this.textBox_ip_recieve.TabIndex = 8;
            this.textBox_ip_recieve.Text = "127.0.0.1";
            // 
            // btnAllowConnection
            // 
            this.btnAllowConnection.Location = new System.Drawing.Point(103, 75);
            this.btnAllowConnection.Name = "btnAllowConnection";
            this.btnAllowConnection.Size = new System.Drawing.Size(134, 25);
            this.btnAllowConnection.TabIndex = 5;
            this.btnAllowConnection.Text = "Enable traffic";
            this.btnAllowConnection.UseVisualStyleBackColor = true;
            this.btnAllowConnection.Click += new System.EventHandler(this.btnAllowConnection_Click_1);
            // 
            // textBox_ip_send
            // 
            this.textBox_ip_send.Location = new System.Drawing.Point(103, 50);
            this.textBox_ip_send.Name = "textBox_ip_send";
            this.textBox_ip_send.Size = new System.Drawing.Size(80, 20);
            this.textBox_ip_send.TabIndex = 2;
            this.textBox_ip_send.Text = "127.0.0.1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "IP:Port (controller)";
            // 
            // listBoxControllers
            // 
            this.listBoxControllers.FormattingEnabled = true;
            this.listBoxControllers.Location = new System.Drawing.Point(5, 32);
            this.listBoxControllers.Name = "listBoxControllers";
            this.listBoxControllers.Size = new System.Drawing.Size(168, 108);
            this.listBoxControllers.TabIndex = 0;
            this.listBoxControllers.SelectedIndexChanged += new System.EventHandler(this.listBoxControllers_SelectedIndexChanged);
            // 
            // chartTankBottom
            // 
            this.chartTankBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea14.AxisY.Maximum = 10D;
            chartArea14.AxisY.Minimum = 0D;
            chartArea14.Name = "ChartArea1";
            this.chartTankBottom.ChartAreas.Add(chartArea14);
            legend14.Name = "Legend1";
            this.chartTankBottom.Legends.Add(legend14);
            this.chartTankBottom.Location = new System.Drawing.Point(914, 401);
            this.chartTankBottom.Name = "chartTankBottom";
            series9.ChartArea = "ChartArea1";
            series9.Legend = "Legend1";
            series9.Name = "tank_bottom";
            series9.Points.Add(dataPoint9);
            this.chartTankBottom.Series.Add(series9);
            this.chartTankBottom.Size = new System.Drawing.Size(269, 275);
            this.chartTankBottom.TabIndex = 13;
            this.chartTankBottom.Text = "chart1";
            // 
            // chartTankTop
            // 
            this.chartTankTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea15.AxisY.Maximum = 10D;
            chartArea15.AxisY.Minimum = 0D;
            chartArea15.Name = "ChartArea1";
            this.chartTankTop.ChartAreas.Add(chartArea15);
            legend15.Name = "Legend1";
            this.chartTankTop.Legends.Add(legend15);
            this.chartTankTop.Location = new System.Drawing.Point(914, 150);
            this.chartTankTop.Name = "chartTankTop";
            series10.ChartArea = "ChartArea1";
            series10.Legend = "Legend1";
            series10.Name = "tank_top";
            series10.Points.Add(dataPoint10);
            this.chartTankTop.Series.Add(series10);
            this.chartTankTop.Size = new System.Drawing.Size(269, 245);
            this.chartTankTop.TabIndex = 14;
            this.chartTankTop.Text = "chart2";
            // 
            // treeViewControllers
            // 
            this.treeViewControllers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeViewControllers.Location = new System.Drawing.Point(7, 168);
            this.treeViewControllers.Margin = new System.Windows.Forms.Padding(2);
            this.treeViewControllers.Name = "treeViewControllers";
            this.treeViewControllers.Size = new System.Drawing.Size(166, 489);
            this.treeViewControllers.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 153);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Details";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.listBoxControllers);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.treeViewControllers);
            this.groupBox3.Location = new System.Drawing.Point(9, 10);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(177, 668);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Overview";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Controllers";
            // 
            // trackBar1
            // 
            this.trackBar1.Enabled = false;
            this.trackBar1.Location = new System.Drawing.Point(11, 58);
            this.trackBar1.Maximum = 20;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(155, 45);
            this.trackBar1.TabIndex = 19;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.labelReference2);
            this.groupBox4.Controls.Add(this.trackBar1);
            this.groupBox4.Controls.Add(this.trackBarReference);
            this.groupBox4.Controls.Add(this.labelReference1);
            this.groupBox4.Location = new System.Drawing.Point(298, 11);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(200, 110);
            this.groupBox4.TabIndex = 20;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Reference";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(0, 13);
            this.label9.TabIndex = 21;
            // 
            // labelReference2
            // 
            this.labelReference2.AutoSize = true;
            this.labelReference2.Location = new System.Drawing.Point(172, 62);
            this.labelReference2.Name = "labelReference2";
            this.labelReference2.Size = new System.Drawing.Size(13, 13);
            this.labelReference2.TabIndex = 20;
            this.labelReference2.Text = "0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1195, 696);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.chartTankTop);
            this.Controls.Add(this.chartTankBottom);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label_time);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dataChart);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbDebugLog);
            this.Name = "Form1";
            this.Text = "GUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarReference)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownKp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownKi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownKd)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_port_send)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_port_recieve)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartTankBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartTankTop)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDebugLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timerChart;
        private System.Windows.Forms.TrackBar trackBarReference;
        private System.Windows.Forms.Label labelReference1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numUpDownKp;
        private System.Windows.Forms.NumericUpDown numUpDownKi;
        private System.Windows.Forms.NumericUpDown numUpDownKd;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label_time;
        private System.Windows.Forms.Button button_update_pid;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBoxControllers;
        private System.Windows.Forms.TextBox textBox_ip_send;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.DataVisualization.Charting.Chart dataChart;
        private System.Windows.Forms.Button btnAllowConnection;
        private System.Windows.Forms.TextBox textBox_ip_recieve;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartTankBottom;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartTankTop;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TreeView treeViewControllers;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numericUpDown_port_send;
        private System.Windows.Forms.NumericUpDown numericUpDown_port_recieve;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label labelReference2;
    }
}

