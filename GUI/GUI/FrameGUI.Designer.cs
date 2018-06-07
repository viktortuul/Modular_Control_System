namespace GUI
{
    partial class FrameGUI
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
            this.button_thisIP = new System.Windows.Forms.Button();
            this.numericUpDown_port_send = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_port_recieve = new System.Windows.Forms.NumericUpDown();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_ip_recieve = new System.Windows.Forms.TextBox();
            this.btnAllowConnection = new System.Windows.Forms.Button();
            this.textBox_ip_send = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.listBoxModules = new System.Windows.Forms.ListBox();
            this.treeViewControllers = new System.Windows.Forms.TreeView();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.labelReference2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.numUpDown_A1 = new System.Windows.Forms.NumericUpDown();
            this.numUpDown_a1a = new System.Windows.Forms.NumericUpDown();
            this.numUpDown_A2 = new System.Windows.Forms.NumericUpDown();
            this.numUpDown_a2a = new System.Windows.Forms.NumericUpDown();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarReference)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownKp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownKi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownKd)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_port_send)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_port_recieve)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_A1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_a1a)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_A2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_a2a)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbDebugLog
            // 
            this.tbDebugLog.Location = new System.Drawing.Point(1152, 12);
            this.tbDebugLog.Multiline = true;
            this.tbDebugLog.Name = "tbDebugLog";
            this.tbDebugLog.ReadOnly = true;
            this.tbDebugLog.Size = new System.Drawing.Size(414, 545);
            this.tbDebugLog.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(981, 7);
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
            chartArea1.Area3DStyle.Inclination = 0;
            chartArea1.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
            chartArea1.Area3DStyle.Rotation = 0;
            chartArea1.Area3DStyle.WallWidth = 1;
            chartArea1.BackColor = System.Drawing.Color.WhiteSmoke;
            chartArea1.Name = "ChartArea1";
            chartArea1.ShadowColor = System.Drawing.Color.Gray;
            this.dataChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.dataChart.Legends.Add(legend1);
            this.dataChart.Location = new System.Drawing.Point(191, 150);
            this.dataChart.Name = "dataChart";
            this.dataChart.Size = new System.Drawing.Size(404, 522);
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
            2,
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
            1,
            0,
            0,
            65536});
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
            3,
            0,
            0,
            0});
            this.numUpDownKd.ValueChanged += new System.EventHandler(this.numUpDownKd_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_update_pid);
            this.groupBox1.Controls.Add(this.numUpDownKi);
            this.groupBox1.Controls.Add(this.numUpDownKd);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numUpDownKp);
            this.groupBox1.Location = new System.Drawing.Point(478, 10);
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
            this.label_time.Location = new System.Drawing.Point(791, 10);
            this.label_time.Name = "label_time";
            this.label_time.Size = new System.Drawing.Size(54, 13);
            this.label_time.TabIndex = 11;
            this.label_time.Text = "label_time";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_thisIP);
            this.groupBox2.Controls.Add(this.numericUpDown_port_send);
            this.groupBox2.Controls.Add(this.numericUpDown_port_recieve);
            this.groupBox2.Controls.Add(this.textBoxName);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBox_ip_recieve);
            this.groupBox2.Controls.Add(this.btnAllowConnection);
            this.groupBox2.Controls.Add(this.textBox_ip_send);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(191, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(281, 110);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "New controller module";
            // 
            // button_thisIP
            // 
            this.button_thisIP.Location = new System.Drawing.Point(70, 22);
            this.button_thisIP.Name = "button_thisIP";
            this.button_thisIP.Size = new System.Drawing.Size(44, 25);
            this.button_thisIP.TabIndex = 21;
            this.button_thisIP.Text = "get IP";
            this.button_thisIP.UseVisualStyleBackColor = true;
            this.button_thisIP.Click += new System.EventHandler(this.button_thisIP_Click);
            // 
            // numericUpDown_port_send
            // 
            this.numericUpDown_port_send.Location = new System.Drawing.Point(222, 51);
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
            this.numericUpDown_port_recieve.Location = new System.Drawing.Point(222, 24);
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
            this.textBoxName.Text = "Module1";
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
            this.textBox_ip_recieve.Location = new System.Drawing.Point(119, 24);
            this.textBox_ip_recieve.Name = "textBox_ip_recieve";
            this.textBox_ip_recieve.Size = new System.Drawing.Size(98, 20);
            this.textBox_ip_recieve.TabIndex = 8;
            this.textBox_ip_recieve.Text = "127.0.0.1";
            // 
            // btnAllowConnection
            // 
            this.btnAllowConnection.Location = new System.Drawing.Point(119, 73);
            this.btnAllowConnection.Name = "btnAllowConnection";
            this.btnAllowConnection.Size = new System.Drawing.Size(98, 25);
            this.btnAllowConnection.TabIndex = 5;
            this.btnAllowConnection.Text = "Enable traffic";
            this.btnAllowConnection.UseVisualStyleBackColor = true;
            this.btnAllowConnection.Click += new System.EventHandler(this.btnAllowConnection_Click_1);
            // 
            // textBox_ip_send
            // 
            this.textBox_ip_send.Location = new System.Drawing.Point(122, 50);
            this.textBox_ip_send.Name = "textBox_ip_send";
            this.textBox_ip_send.Size = new System.Drawing.Size(98, 20);
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
            // listBoxModules
            // 
            this.listBoxModules.FormattingEnabled = true;
            this.listBoxModules.Location = new System.Drawing.Point(5, 32);
            this.listBoxModules.Name = "listBoxModules";
            this.listBoxModules.Size = new System.Drawing.Size(168, 108);
            this.listBoxModules.TabIndex = 0;
            this.listBoxModules.SelectedIndexChanged += new System.EventHandler(this.listBoxControllers_SelectedIndexChanged);
            // 
            // treeViewControllers
            // 
            this.treeViewControllers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeViewControllers.Location = new System.Drawing.Point(7, 168);
            this.treeViewControllers.Margin = new System.Windows.Forms.Padding(2);
            this.treeViewControllers.Name = "treeViewControllers";
            this.treeViewControllers.Size = new System.Drawing.Size(166, 486);
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
            this.groupBox3.Controls.Add(this.listBoxModules);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.treeViewControllers);
            this.groupBox3.Location = new System.Drawing.Point(9, 10);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(177, 665);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Controller modules overview";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Modules";
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
            this.groupBox4.Location = new System.Drawing.Point(585, 10);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(200, 110);
            this.groupBox4.TabIndex = 20;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Reference values";
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
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(602, 150);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(353, 522);
            this.pictureBox1.TabIndex = 23;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Tank1 A";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(5, 37);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(47, 13);
            this.label10.TabIndex = 24;
            this.label10.Text = "Tank1 a";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 79);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 13);
            this.label11.TabIndex = 26;
            this.label11.Text = "Tank2 a";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 58);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Tank2 A";
            // 
            // numUpDown_A1
            // 
            this.numUpDown_A1.DecimalPlaces = 1;
            this.numUpDown_A1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numUpDown_A1.Location = new System.Drawing.Point(60, 14);
            this.numUpDown_A1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numUpDown_A1.Name = "numUpDown_A1";
            this.numUpDown_A1.Size = new System.Drawing.Size(59, 20);
            this.numUpDown_A1.TabIndex = 11;
            this.numUpDown_A1.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // numUpDown_a1a
            // 
            this.numUpDown_a1a.DecimalPlaces = 2;
            this.numUpDown_a1a.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numUpDown_a1a.Location = new System.Drawing.Point(60, 35);
            this.numUpDown_a1a.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numUpDown_a1a.Name = "numUpDown_a1a";
            this.numUpDown_a1a.Size = new System.Drawing.Size(59, 20);
            this.numUpDown_a1a.TabIndex = 27;
            this.numUpDown_a1a.Value = new decimal(new int[] {
            16,
            0,
            0,
            131072});
            // 
            // numUpDown_A2
            // 
            this.numUpDown_A2.DecimalPlaces = 1;
            this.numUpDown_A2.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numUpDown_A2.Location = new System.Drawing.Point(60, 56);
            this.numUpDown_A2.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numUpDown_A2.Name = "numUpDown_A2";
            this.numUpDown_A2.Size = new System.Drawing.Size(59, 20);
            this.numUpDown_A2.TabIndex = 28;
            this.numUpDown_A2.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // numUpDown_a2a
            // 
            this.numUpDown_a2a.DecimalPlaces = 2;
            this.numUpDown_a2a.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numUpDown_a2a.Location = new System.Drawing.Point(60, 77);
            this.numUpDown_a2a.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numUpDown_a2a.Name = "numUpDown_a2a";
            this.numUpDown_a2a.Size = new System.Drawing.Size(59, 20);
            this.numUpDown_a2a.TabIndex = 29;
            this.numUpDown_a2a.Value = new decimal(new int[] {
            16,
            0,
            0,
            131072});
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.numUpDown_a2a);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.numUpDown_A2);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.numUpDown_a1a);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.numUpDown_A1);
            this.groupBox5.Location = new System.Drawing.Point(820, 150);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(135, 108);
            this.groupBox5.TabIndex = 30;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Visual settings";
            // 
            // FrameGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1336, 693);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label_time);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dataChart);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbDebugLog);
            this.Name = "FrameGUI";
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
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_A1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_a1a)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_A2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_a2a)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
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
        private System.Windows.Forms.ListBox listBoxModules;
        private System.Windows.Forms.TextBox textBox_ip_send;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.DataVisualization.Charting.Chart dataChart;
        private System.Windows.Forms.Button btnAllowConnection;
        private System.Windows.Forms.TextBox textBox_ip_recieve;
        private System.Windows.Forms.Label label7;
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
        private System.Windows.Forms.Button button_thisIP;
        public System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown numUpDown_A1;
        private System.Windows.Forms.NumericUpDown numUpDown_a1a;
        private System.Windows.Forms.NumericUpDown numUpDown_A2;
        private System.Windows.Forms.NumericUpDown numUpDown_a2a;
        private System.Windows.Forms.GroupBox groupBox5;
    }
}

