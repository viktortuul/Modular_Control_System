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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea7 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend7 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea8 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend8 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint5 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 5D);
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea9 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend9 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint6 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 5D);
            this.tbDebugLog = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.timerChart = new System.Windows.Forms.Timer(this.components);
            this.dataChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label2 = new System.Windows.Forms.Label();
            this.tbReference = new System.Windows.Forms.TrackBar();
            this.labelReference = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numUpDownKp = new System.Windows.Forms.NumericUpDown();
            this.nUD_Ki = new System.Windows.Forms.NumericUpDown();
            this.nUD_Kd = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_update_pid = new System.Windows.Forms.Button();
            this.label_time = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_ip_recieve = new System.Windows.Forms.TextBox();
            this.textBox_port_send = new System.Windows.Forms.TextBox();
            this.textBox_port_recieve = new System.Windows.Forms.TextBox();
            this.btnAllowConnection = new System.Windows.Forms.Button();
            this.textBox_ip_send = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.listBoxControllers = new System.Windows.Forms.ListBox();
            this.chartTankBottom = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartTankTop = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.dataChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbReference)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownKp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_Ki)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_Kd)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartTankBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartTankTop)).BeginInit();
            this.SuspendLayout();
            // 
            // tbDebugLog
            // 
            this.tbDebugLog.Location = new System.Drawing.Point(16, 24);
            this.tbDebugLog.Multiline = true;
            this.tbDebugLog.Name = "tbDebugLog";
            this.tbDebugLog.ReadOnly = true;
            this.tbDebugLog.Size = new System.Drawing.Size(189, 592);
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
            this.timerChart.Tick += new System.EventHandler(this.timerChart_Tick);
            // 
            // dataChart
            // 
            chartArea7.Name = "ChartArea1";
            this.dataChart.ChartAreas.Add(chartArea7);
            legend7.Name = "Legend1";
            this.dataChart.Legends.Add(legend7);
            this.dataChart.Location = new System.Drawing.Point(221, 128);
            this.dataChart.Name = "dataChart";
            this.dataChart.Size = new System.Drawing.Size(766, 488);
            this.dataChart.TabIndex = 2;
            this.dataChart.Text = "chart1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(218, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Reference";
            // 
            // tbReference
            // 
            this.tbReference.Location = new System.Drawing.Point(221, 38);
            this.tbReference.Maximum = 4;
            this.tbReference.Name = "tbReference";
            this.tbReference.Size = new System.Drawing.Size(233, 45);
            this.tbReference.TabIndex = 4;
            this.tbReference.Scroll += new System.EventHandler(this.tbReference_Scroll);
            // 
            // labelReference
            // 
            this.labelReference.AutoSize = true;
            this.labelReference.Location = new System.Drawing.Point(460, 38);
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
            // numUpDownKp
            // 
            this.numUpDownKp.DecimalPlaces = 1;
            this.numUpDownKp.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numUpDownKp.Location = new System.Drawing.Point(37, 26);
            this.numUpDownKp.Name = "numUpDownKp";
            this.numUpDownKp.Size = new System.Drawing.Size(49, 20);
            this.numUpDownKp.TabIndex = 7;
            this.numUpDownKp.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numUpDownKp.ValueChanged += new System.EventHandler(this.numUpDownKp_ValueChanged);
            // 
            // nUD_Ki
            // 
            this.nUD_Ki.Location = new System.Drawing.Point(37, 51);
            this.nUD_Ki.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nUD_Ki.Name = "nUD_Ki";
            this.nUD_Ki.Size = new System.Drawing.Size(49, 20);
            this.nUD_Ki.TabIndex = 8;
            this.nUD_Ki.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nUD_Ki.ValueChanged += new System.EventHandler(this.numUpDownKi_ValueChanged);
            // 
            // nUD_Kd
            // 
            this.nUD_Kd.DecimalPlaces = 1;
            this.nUD_Kd.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUD_Kd.Location = new System.Drawing.Point(37, 77);
            this.nUD_Kd.Name = "nUD_Kd";
            this.nUD_Kd.Size = new System.Drawing.Size(49, 20);
            this.nUD_Kd.TabIndex = 9;
            this.nUD_Kd.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUD_Kd.ValueChanged += new System.EventHandler(this.numUpDownKd_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_update_pid);
            this.groupBox1.Controls.Add(this.nUD_Ki);
            this.groupBox1.Controls.Add(this.nUD_Kd);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numUpDownKp);
            this.groupBox1.Location = new System.Drawing.Point(499, 8);
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
            this.label_time.Location = new System.Drawing.Point(993, 23);
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
            this.groupBox2.Controls.Add(this.textBox_port_recieve);
            this.groupBox2.Controls.Add(this.btnAllowConnection);
            this.groupBox2.Controls.Add(this.textBox_ip_send);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.listBoxControllers);
            this.groupBox2.Location = new System.Drawing.Point(621, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(366, 114);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Connected controllers";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(132, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "IP - Port (recieve)";
            // 
            // textBox_ip_recieve
            // 
            this.textBox_ip_recieve.Location = new System.Drawing.Point(227, 15);
            this.textBox_ip_recieve.Name = "textBox_ip_recieve";
            this.textBox_ip_recieve.Size = new System.Drawing.Size(80, 20);
            this.textBox_ip_recieve.TabIndex = 8;
            this.textBox_ip_recieve.Text = "127.0.0.1";
            // 
            // textBox_port_send
            // 
            this.textBox_port_send.Location = new System.Drawing.Point(313, 41);
            this.textBox_port_send.Name = "textBox_port_send";
            this.textBox_port_send.Size = new System.Drawing.Size(42, 20);
            this.textBox_port_send.TabIndex = 7;
            this.textBox_port_send.Text = "8200";
            // 
            // textBox_port_recieve
            // 
            this.textBox_port_recieve.Location = new System.Drawing.Point(313, 15);
            this.textBox_port_recieve.Name = "textBox_port_recieve";
            this.textBox_port_recieve.Size = new System.Drawing.Size(42, 20);
            this.textBox_port_recieve.TabIndex = 3;
            this.textBox_port_recieve.Text = "8100";
            // 
            // btnAllowConnection
            // 
            this.btnAllowConnection.Location = new System.Drawing.Point(135, 76);
            this.btnAllowConnection.Name = "btnAllowConnection";
            this.btnAllowConnection.Size = new System.Drawing.Size(118, 25);
            this.btnAllowConnection.TabIndex = 5;
            this.btnAllowConnection.Text = "Enable connection";
            this.btnAllowConnection.UseVisualStyleBackColor = true;
            this.btnAllowConnection.Click += new System.EventHandler(this.btnAllowConnection_Click_1);
            // 
            // textBox_ip_send
            // 
            this.textBox_ip_send.Location = new System.Drawing.Point(227, 41);
            this.textBox_ip_send.Name = "textBox_ip_send";
            this.textBox_ip_send.Size = new System.Drawing.Size(80, 20);
            this.textBox_ip_send.TabIndex = 2;
            this.textBox_ip_send.Text = "127.0.0.1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(132, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "IP - Port (send)";
            // 
            // listBoxControllers
            // 
            this.listBoxControllers.FormattingEnabled = true;
            this.listBoxControllers.Location = new System.Drawing.Point(6, 19);
            this.listBoxControllers.Name = "listBoxControllers";
            this.listBoxControllers.Size = new System.Drawing.Size(111, 82);
            this.listBoxControllers.TabIndex = 0;
            this.listBoxControllers.SelectedIndexChanged += new System.EventHandler(this.listBoxControllers_SelectedIndexChanged);
            // 
            // chartTankBottom
            // 
            chartArea8.AxisY.Maximum = 10D;
            chartArea8.AxisY.Minimum = 0D;
            chartArea8.Name = "ChartArea1";
            this.chartTankBottom.ChartAreas.Add(chartArea8);
            legend8.Name = "Legend1";
            this.chartTankBottom.Legends.Add(legend8);
            this.chartTankBottom.Location = new System.Drawing.Point(993, 374);
            this.chartTankBottom.Name = "chartTankBottom";
            series5.ChartArea = "ChartArea1";
            series5.Legend = "Legend1";
            series5.Name = "tank_bottom";
            series5.Points.Add(dataPoint5);
            this.chartTankBottom.Series.Add(series5);
            this.chartTankBottom.Size = new System.Drawing.Size(230, 240);
            this.chartTankBottom.TabIndex = 13;
            this.chartTankBottom.Text = "chart1";
            // 
            // chartTankTop
            // 
            chartArea9.AxisY.Maximum = 10D;
            chartArea9.AxisY.Minimum = 0D;
            chartArea9.Name = "ChartArea1";
            this.chartTankTop.ChartAreas.Add(chartArea9);
            legend9.Name = "Legend1";
            this.chartTankTop.Legends.Add(legend9);
            this.chartTankTop.Location = new System.Drawing.Point(993, 128);
            this.chartTankTop.Name = "chartTankTop";
            series6.ChartArea = "ChartArea1";
            series6.Legend = "Legend1";
            series6.Name = "tank_top";
            series6.Points.Add(dataPoint6);
            this.chartTankTop.Series.Add(series6);
            this.chartTankTop.Size = new System.Drawing.Size(230, 240);
            this.chartTankTop.TabIndex = 14;
            this.chartTankTop.Text = "chart2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1274, 632);
            this.Controls.Add(this.chartTankTop);
            this.Controls.Add(this.chartTankBottom);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label_time);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelReference);
            this.Controls.Add(this.tbReference);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataChart);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbDebugLog);
            this.Name = "Form1";
            this.Text = "GUI";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbReference)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownKp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_Ki)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_Kd)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartTankBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartTankTop)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDebugLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timerChart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar tbReference;
        private System.Windows.Forms.Label labelReference;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numUpDownKp;
        private System.Windows.Forms.NumericUpDown nUD_Ki;
        private System.Windows.Forms.NumericUpDown nUD_Kd;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label_time;
        private System.Windows.Forms.Button button_update_pid;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBoxControllers;
        private System.Windows.Forms.TextBox textBox_port_recieve;
        private System.Windows.Forms.TextBox textBox_ip_send;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.DataVisualization.Charting.Chart dataChart;
        private System.Windows.Forms.Button btnAllowConnection;
        private System.Windows.Forms.TextBox textBox_port_send;
        private System.Windows.Forms.TextBox textBox_ip_recieve;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartTankBottom;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartTankTop;
    }
}

