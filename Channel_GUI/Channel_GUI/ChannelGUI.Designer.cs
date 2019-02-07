namespace Channel_GUI
{
    partial class ChannelGUI
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
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChannelGUI));
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbManual = new System.Windows.Forms.RadioButton();
            this.rbTransientIncrease = new System.Windows.Forms.RadioButton();
            this.rbSinusoid = new System.Windows.Forms.RadioButton();
            this.rbTransientDecrease = new System.Windows.Forms.RadioButton();
            this.rbBias = new System.Windows.Forms.RadioButton();
            this.btnAddAttackModel = new System.Windows.Forms.Button();
            this.tbTargetTag = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbTargetPort = new System.Windows.Forms.TextBox();
            this.tbTargetIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label14 = new System.Windows.Forms.Label();
            this.nudHistory1 = new System.Windows.Forms.NumericUpDown();
            this.packageChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.clbDropOutTarget = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nudBernoulliPass = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.rbBernoulli = new System.Windows.Forms.RadioButton();
            this.label15 = new System.Windows.Forms.Label();
            this.nudStayDrop = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.nudStayPass = new System.Windows.Forms.NumericUpDown();
            this.rbMarkov = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.tbCanalPort = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.nudHistory = new System.Windows.Forms.NumericUpDown();
            this.btnUpdateAttackModel = new System.Windows.Forms.Button();
            this.btnStopAttack = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbSetValue = new System.Windows.Forms.RadioButton();
            this.rbAddValue = new System.Windows.Forms.RadioButton();
            this.btnClearCustomAttack = new System.Windows.Forms.Button();
            this.labelTimeSeries = new System.Windows.Forms.Label();
            this.tbTimeSeries = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnStartAttack = new System.Windows.Forms.Button();
            this.clbAttackModels = new System.Windows.Forms.CheckedListBox();
            this.attackChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.cbAllPorts = new System.Windows.Forms.CheckBox();
            this.cbAllIPs = new System.Windows.Forms.CheckBox();
            this.nudFrequency = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.nudTimeConst = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.nudAmplitude = new System.Windows.Forms.NumericUpDown();
            this.nudDuration = new System.Windows.Forms.NumericUpDown();
            this.labelStatus = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.timerStatus = new System.Windows.Forms.Timer(this.components);
            this.timerChart = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.setDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHistory1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.packageChart)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBernoulliPass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStayDrop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStayPass)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHistory)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attackChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrequency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeConst)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAmplitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDuration)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(134, 135);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Amplitude";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(134, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Duration [s]";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbManual);
            this.groupBox2.Controls.Add(this.rbTransientIncrease);
            this.groupBox2.Controls.Add(this.rbSinusoid);
            this.groupBox2.Controls.Add(this.rbTransientDecrease);
            this.groupBox2.Controls.Add(this.rbBias);
            this.groupBox2.Location = new System.Drawing.Point(6, 50);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(122, 139);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Attack characteristic";
            // 
            // rbManual
            // 
            this.rbManual.AutoSize = true;
            this.rbManual.Location = new System.Drawing.Point(6, 102);
            this.rbManual.Name = "rbManual";
            this.rbManual.Size = new System.Drawing.Size(60, 17);
            this.rbManual.TabIndex = 4;
            this.rbManual.Text = "Manual";
            this.rbManual.UseVisualStyleBackColor = true;
            this.rbManual.CheckedChanged += new System.EventHandler(this.rbManual_CheckedChanged);
            // 
            // rbTransientIncrease
            // 
            this.rbTransientIncrease.AutoSize = true;
            this.rbTransientIncrease.Location = new System.Drawing.Point(6, 60);
            this.rbTransientIncrease.Name = "rbTransientIncrease";
            this.rbTransientIncrease.Size = new System.Drawing.Size(99, 17);
            this.rbTransientIncrease.TabIndex = 3;
            this.rbTransientIncrease.Text = "Transient (Incr.)";
            this.rbTransientIncrease.UseVisualStyleBackColor = true;
            this.rbTransientIncrease.CheckedChanged += new System.EventHandler(this.rbTransientIncrease_CheckedChanged);
            // 
            // rbSinusoid
            // 
            this.rbSinusoid.AutoSize = true;
            this.rbSinusoid.Location = new System.Drawing.Point(6, 81);
            this.rbSinusoid.Name = "rbSinusoid";
            this.rbSinusoid.Size = new System.Drawing.Size(65, 17);
            this.rbSinusoid.TabIndex = 2;
            this.rbSinusoid.Text = "Sinusoid";
            this.rbSinusoid.UseVisualStyleBackColor = true;
            this.rbSinusoid.CheckedChanged += new System.EventHandler(this.rbSinusoid_CheckedChanged);
            // 
            // rbTransientDecrease
            // 
            this.rbTransientDecrease.AutoSize = true;
            this.rbTransientDecrease.Location = new System.Drawing.Point(6, 39);
            this.rbTransientDecrease.Name = "rbTransientDecrease";
            this.rbTransientDecrease.Size = new System.Drawing.Size(104, 17);
            this.rbTransientDecrease.TabIndex = 1;
            this.rbTransientDecrease.Text = "Transient (Decr.)";
            this.rbTransientDecrease.UseVisualStyleBackColor = true;
            this.rbTransientDecrease.CheckedChanged += new System.EventHandler(this.rbTransientDecrease_CheckedChanged);
            // 
            // rbBias
            // 
            this.rbBias.AutoSize = true;
            this.rbBias.Checked = true;
            this.rbBias.Location = new System.Drawing.Point(6, 19);
            this.rbBias.Name = "rbBias";
            this.rbBias.Size = new System.Drawing.Size(45, 17);
            this.rbBias.TabIndex = 0;
            this.rbBias.TabStop = true;
            this.rbBias.Text = "Bias";
            this.rbBias.UseVisualStyleBackColor = true;
            this.rbBias.CheckedChanged += new System.EventHandler(this.rbBias_CheckedChanged);
            // 
            // btnAddAttackModel
            // 
            this.btnAddAttackModel.Location = new System.Drawing.Point(273, 89);
            this.btnAddAttackModel.Name = "btnAddAttackModel";
            this.btnAddAttackModel.Size = new System.Drawing.Size(88, 23);
            this.btnAddAttackModel.TabIndex = 6;
            this.btnAddAttackModel.Text = "Add model";
            this.btnAddAttackModel.UseVisualStyleBackColor = true;
            this.btnAddAttackModel.Click += new System.EventHandler(this.btnAddAttackModel_Click);
            // 
            // tbTargetTag
            // 
            this.tbTargetTag.Location = new System.Drawing.Point(81, 27);
            this.tbTargetTag.Name = "tbTargetTag";
            this.tbTargetTag.Size = new System.Drawing.Size(39, 20);
            this.tbTargetTag.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Target tag";
            // 
            // tbTargetPort
            // 
            this.tbTargetPort.Location = new System.Drawing.Point(164, 5);
            this.tbTargetPort.Name = "tbTargetPort";
            this.tbTargetPort.Size = new System.Drawing.Size(44, 20);
            this.tbTargetPort.TabIndex = 3;
            this.tbTargetPort.Text = "8300";
            // 
            // tbTargetIP
            // 
            this.tbTargetIP.Location = new System.Drawing.Point(81, 5);
            this.tbTargetIP.Name = "tbTargetIP";
            this.tbTargetIP.Size = new System.Drawing.Size(82, 20);
            this.tbTargetIP.TabIndex = 2;
            this.tbTargetIP.Text = "127.0.0.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Target IP:Port";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(6, 6);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(875, 442);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.nudHistory1);
            this.tabPage2.Controls.Add(this.packageChart);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.tbCanalPort);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(867, 416);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Channel settings";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.White;
            this.label14.Location = new System.Drawing.Point(11, 141);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(53, 13);
            this.label14.TabIndex = 36;
            this.label14.Text = "History [s]";
            // 
            // nudHistory1
            // 
            this.nudHistory1.Location = new System.Drawing.Point(70, 138);
            this.nudHistory1.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nudHistory1.Name = "nudHistory1";
            this.nudHistory1.Size = new System.Drawing.Size(41, 20);
            this.nudHistory1.TabIndex = 35;
            this.nudHistory1.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nudHistory1.ValueChanged += new System.EventHandler(this.nudHistory1_ValueChanged);
            // 
            // packageChart
            // 
            this.packageChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.BackColor = System.Drawing.Color.WhiteSmoke;
            chartArea1.Name = "ChartArea1";
            this.packageChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.packageChart.Legends.Add(legend1);
            this.packageChart.Location = new System.Drawing.Point(7, 136);
            this.packageChart.Name = "packageChart";
            this.packageChart.Size = new System.Drawing.Size(854, 277);
            this.packageChart.TabIndex = 34;
            this.packageChart.Text = "chart1";
            title1.Name = "Title1";
            title1.Text = "Package status";
            this.packageChart.Titles.Add(title1);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.clbDropOutTarget);
            this.groupBox4.Location = new System.Drawing.Point(253, 5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(232, 125);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Channels";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 90);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(216, 26);
            this.label13.TabIndex = 10;
            this.label13.Text = "Packages in a checked channel are subject\r\nto the drop-out model.";
            // 
            // clbDropOutTarget
            // 
            this.clbDropOutTarget.CheckOnClick = true;
            this.clbDropOutTarget.FormattingEnabled = true;
            this.clbDropOutTarget.Location = new System.Drawing.Point(6, 21);
            this.clbDropOutTarget.Name = "clbDropOutTarget";
            this.clbDropOutTarget.Size = new System.Drawing.Size(220, 64);
            this.clbDropOutTarget.TabIndex = 23;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nudBernoulliPass);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.rbBernoulli);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.nudStayDrop);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.nudStayPass);
            this.groupBox1.Controls.Add(this.rbMarkov);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(7, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 97);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Package drop-out models";
            // 
            // nudBernoulliPass
            // 
            this.nudBernoulliPass.Location = new System.Drawing.Point(172, 19);
            this.nudBernoulliPass.Name = "nudBernoulliPass";
            this.nudBernoulliPass.Size = new System.Drawing.Size(41, 20);
            this.nudBernoulliPass.TabIndex = 3;
            this.nudBernoulliPass.Value = new decimal(new int[] {
            95,
            0,
            0,
            0});
            this.nudBernoulliPass.ValueChanged += new System.EventHandler(this.nudBernoulliPass_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(77, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Pass [%]";
            // 
            // rbBernoulli
            // 
            this.rbBernoulli.AutoSize = true;
            this.rbBernoulli.Checked = true;
            this.rbBernoulli.Location = new System.Drawing.Point(6, 19);
            this.rbBernoulli.Name = "rbBernoulli";
            this.rbBernoulli.Size = new System.Drawing.Size(65, 17);
            this.rbBernoulli.TabIndex = 0;
            this.rbBernoulli.TabStop = true;
            this.rbBernoulli.Text = "Bernoulli";
            this.rbBernoulli.UseVisualStyleBackColor = true;
            this.rbBernoulli.CheckedChanged += new System.EventHandler(this.rbBernoulli_CheckedChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(4, 32);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(211, 13);
            this.label15.TabIndex = 10;
            this.label15.Text = "....................................................................";
            // 
            // nudStayDrop
            // 
            this.nudStayDrop.Location = new System.Drawing.Point(172, 67);
            this.nudStayDrop.Name = "nudStayDrop";
            this.nudStayDrop.Size = new System.Drawing.Size(41, 20);
            this.nudStayDrop.TabIndex = 7;
            this.nudStayDrop.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.nudStayDrop.ValueChanged += new System.EventHandler(this.nudStayDrop_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(77, 69);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(94, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "p(Drop ->Drop) [%]";
            // 
            // nudStayPass
            // 
            this.nudStayPass.Location = new System.Drawing.Point(172, 46);
            this.nudStayPass.Name = "nudStayPass";
            this.nudStayPass.Size = new System.Drawing.Size(41, 20);
            this.nudStayPass.TabIndex = 5;
            this.nudStayPass.Value = new decimal(new int[] {
            98,
            0,
            0,
            0});
            this.nudStayPass.ValueChanged += new System.EventHandler(this.nudStayPass_ValueChanged);
            // 
            // rbMarkov
            // 
            this.rbMarkov.AutoSize = true;
            this.rbMarkov.Location = new System.Drawing.Point(6, 46);
            this.rbMarkov.Name = "rbMarkov";
            this.rbMarkov.Size = new System.Drawing.Size(64, 17);
            this.rbMarkov.TabIndex = 1;
            this.rbMarkov.Text = "Markov ";
            this.rbMarkov.UseVisualStyleBackColor = true;
            this.rbMarkov.CheckedChanged += new System.EventHandler(this.rbMarkov_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(77, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(91, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "p(Pass->Pass) [%]";
            // 
            // tbCanalPort
            // 
            this.tbCanalPort.Enabled = false;
            this.tbCanalPort.Location = new System.Drawing.Point(80, 6);
            this.tbCanalPort.Name = "tbCanalPort";
            this.tbCanalPort.Size = new System.Drawing.Size(44, 20);
            this.tbCanalPort.TabIndex = 4;
            this.tbCanalPort.Text = "8111";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Listening port";
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.nudHistory);
            this.tabPage1.Controls.Add(this.btnUpdateAttackModel);
            this.tabPage1.Controls.Add(this.btnStopAttack);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.btnClearCustomAttack);
            this.tabPage1.Controls.Add(this.labelTimeSeries);
            this.tabPage1.Controls.Add(this.tbTimeSeries);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.btnStartAttack);
            this.tabPage1.Controls.Add(this.clbAttackModels);
            this.tabPage1.Controls.Add(this.attackChart);
            this.tabPage1.Controls.Add(this.cbAllPorts);
            this.tabPage1.Controls.Add(this.cbAllIPs);
            this.tabPage1.Controls.Add(this.nudFrequency);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.tbTargetTag);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.nudTimeConst);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.nudAmplitude);
            this.tabPage1.Controls.Add(this.nudDuration);
            this.tabPage1.Controls.Add(this.labelStatus);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.tbTargetIP);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.tbTargetPort);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.btnAddAttackModel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(867, 416);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Attack settings";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.White;
            this.label12.Location = new System.Drawing.Point(8, 201);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 13);
            this.label12.TabIndex = 33;
            this.label12.Text = "History [s]";
            // 
            // nudHistory
            // 
            this.nudHistory.Location = new System.Drawing.Point(67, 198);
            this.nudHistory.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nudHistory.Name = "nudHistory";
            this.nudHistory.Size = new System.Drawing.Size(41, 20);
            this.nudHistory.TabIndex = 32;
            this.nudHistory.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nudHistory.ValueChanged += new System.EventHandler(this.nudHistory_ValueChanged);
            // 
            // btnUpdateAttackModel
            // 
            this.btnUpdateAttackModel.Location = new System.Drawing.Point(273, 112);
            this.btnUpdateAttackModel.Name = "btnUpdateAttackModel";
            this.btnUpdateAttackModel.Size = new System.Drawing.Size(88, 23);
            this.btnUpdateAttackModel.TabIndex = 31;
            this.btnUpdateAttackModel.Text = "Update model";
            this.btnUpdateAttackModel.UseVisualStyleBackColor = true;
            this.btnUpdateAttackModel.Click += new System.EventHandler(this.btnUpdateAttackModel_Click);
            // 
            // btnStopAttack
            // 
            this.btnStopAttack.Location = new System.Drawing.Point(273, 170);
            this.btnStopAttack.Name = "btnStopAttack";
            this.btnStopAttack.Size = new System.Drawing.Size(88, 23);
            this.btnStopAttack.TabIndex = 30;
            this.btnStopAttack.Text = "Stop attack";
            this.btnStopAttack.UseVisualStyleBackColor = true;
            this.btnStopAttack.Click += new System.EventHandler(this.btnStopAttack_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbSetValue);
            this.groupBox3.Controls.Add(this.rbAddValue);
            this.groupBox3.Location = new System.Drawing.Point(134, 50);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(133, 58);
            this.groupBox3.TabIndex = 28;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Attack type";
            // 
            // rbSetValue
            // 
            this.rbSetValue.AutoSize = true;
            this.rbSetValue.Location = new System.Drawing.Point(5, 35);
            this.rbSetValue.Margin = new System.Windows.Forms.Padding(2);
            this.rbSetValue.Name = "rbSetValue";
            this.rbSetValue.Size = new System.Drawing.Size(70, 17);
            this.rbSetValue.TabIndex = 1;
            this.rbSetValue.TabStop = true;
            this.rbSetValue.Text = "Set value";
            this.rbSetValue.UseVisualStyleBackColor = true;
            // 
            // rbAddValue
            // 
            this.rbAddValue.AutoSize = true;
            this.rbAddValue.Checked = true;
            this.rbAddValue.Location = new System.Drawing.Point(5, 17);
            this.rbAddValue.Margin = new System.Windows.Forms.Padding(2);
            this.rbAddValue.Name = "rbAddValue";
            this.rbAddValue.Size = new System.Drawing.Size(73, 17);
            this.rbAddValue.TabIndex = 0;
            this.rbAddValue.TabStop = true;
            this.rbAddValue.Text = "Add value";
            this.rbAddValue.UseVisualStyleBackColor = true;
            // 
            // btnClearCustomAttack
            // 
            this.btnClearCustomAttack.Location = new System.Drawing.Point(648, 49);
            this.btnClearCustomAttack.Margin = new System.Windows.Forms.Padding(2);
            this.btnClearCustomAttack.Name = "btnClearCustomAttack";
            this.btnClearCustomAttack.Size = new System.Drawing.Size(56, 19);
            this.btnClearCustomAttack.TabIndex = 27;
            this.btnClearCustomAttack.Text = "Clear";
            this.btnClearCustomAttack.UseVisualStyleBackColor = true;
            this.btnClearCustomAttack.Visible = false;
            this.btnClearCustomAttack.Click += new System.EventHandler(this.btnClearCustomAttack_Click);
            // 
            // labelTimeSeries
            // 
            this.labelTimeSeries.AutoSize = true;
            this.labelTimeSeries.Location = new System.Drawing.Point(543, 54);
            this.labelTimeSeries.Name = "labelTimeSeries";
            this.labelTimeSeries.Size = new System.Drawing.Size(94, 13);
            this.labelTimeSeries.TabIndex = 26;
            this.labelTimeSeries.Text = "Manual time series";
            this.labelTimeSeries.Visible = false;
            // 
            // tbTimeSeries
            // 
            this.tbTimeSeries.Location = new System.Drawing.Point(545, 68);
            this.tbTimeSeries.Margin = new System.Windows.Forms.Padding(2);
            this.tbTimeSeries.Multiline = true;
            this.tbTimeSeries.Name = "tbTimeSeries";
            this.tbTimeSeries.Size = new System.Drawing.Size(160, 125);
            this.tbTimeSeries.TabIndex = 25;
            this.tbTimeSeries.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(372, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Saved attack models";
            // 
            // btnStartAttack
            // 
            this.btnStartAttack.Location = new System.Drawing.Point(273, 147);
            this.btnStartAttack.Name = "btnStartAttack";
            this.btnStartAttack.Size = new System.Drawing.Size(88, 23);
            this.btnStartAttack.TabIndex = 23;
            this.btnStartAttack.Text = "Start attack";
            this.btnStartAttack.UseVisualStyleBackColor = true;
            this.btnStartAttack.Click += new System.EventHandler(this.btnStartAttack_Click);
            // 
            // clbAttackModels
            // 
            this.clbAttackModels.FormattingEnabled = true;
            this.clbAttackModels.Location = new System.Drawing.Point(376, 68);
            this.clbAttackModels.Name = "clbAttackModels";
            this.clbAttackModels.Size = new System.Drawing.Size(164, 124);
            this.clbAttackModels.TabIndex = 22;
            this.clbAttackModels.SelectedIndexChanged += new System.EventHandler(this.clbAttackModels_SelectedIndexChanged);
            // 
            // attackChart
            // 
            this.attackChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea2.BackColor = System.Drawing.Color.WhiteSmoke;
            chartArea2.Name = "ChartArea1";
            this.attackChart.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.attackChart.Legends.Add(legend2);
            this.attackChart.Location = new System.Drawing.Point(6, 197);
            this.attackChart.Name = "attackChart";
            this.attackChart.Size = new System.Drawing.Size(854, 213);
            this.attackChart.TabIndex = 21;
            this.attackChart.Text = "chart1";
            title2.Name = "Title1";
            title2.Text = "Attack states";
            this.attackChart.Titles.Add(title2);
            // 
            // cbAllPorts
            // 
            this.cbAllPorts.AutoSize = true;
            this.cbAllPorts.Location = new System.Drawing.Point(214, 20);
            this.cbAllPorts.Name = "cbAllPorts";
            this.cbAllPorts.Size = new System.Drawing.Size(113, 17);
            this.cbAllPorts.TabIndex = 20;
            this.cbAllPorts.Text = "Target all EP ports";
            this.cbAllPorts.UseVisualStyleBackColor = true;
            // 
            // cbAllIPs
            // 
            this.cbAllIPs.AutoSize = true;
            this.cbAllIPs.Location = new System.Drawing.Point(214, 5);
            this.cbAllIPs.Name = "cbAllIPs";
            this.cbAllIPs.Size = new System.Drawing.Size(107, 17);
            this.cbAllIPs.TabIndex = 19;
            this.cbAllIPs.Text = "Target all EP IP\'s";
            this.cbAllIPs.UseVisualStyleBackColor = true;
            // 
            // nudFrequency
            // 
            this.nudFrequency.DecimalPlaces = 1;
            this.nudFrequency.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudFrequency.Location = new System.Drawing.Point(225, 172);
            this.nudFrequency.Name = "nudFrequency";
            this.nudFrequency.Size = new System.Drawing.Size(41, 20);
            this.nudFrequency.TabIndex = 18;
            this.nudFrequency.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(134, 176);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(79, 13);
            this.label11.TabIndex = 17;
            this.label11.Text = "Frequency [Hz]";
            // 
            // nudTimeConst
            // 
            this.nudTimeConst.DecimalPlaces = 1;
            this.nudTimeConst.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudTimeConst.Location = new System.Drawing.Point(225, 151);
            this.nudTimeConst.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudTimeConst.Name = "nudTimeConst";
            this.nudTimeConst.Size = new System.Drawing.Size(41, 20);
            this.nudTimeConst.TabIndex = 16;
            this.nudTimeConst.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(134, 156);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(88, 13);
            this.label10.TabIndex = 15;
            this.label10.Text = "Time constant [s]";
            // 
            // nudAmplitude
            // 
            this.nudAmplitude.DecimalPlaces = 1;
            this.nudAmplitude.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudAmplitude.Location = new System.Drawing.Point(225, 130);
            this.nudAmplitude.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.nudAmplitude.Name = "nudAmplitude";
            this.nudAmplitude.Size = new System.Drawing.Size(41, 20);
            this.nudAmplitude.TabIndex = 14;
            this.nudAmplitude.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudDuration
            // 
            this.nudDuration.Location = new System.Drawing.Point(225, 109);
            this.nudDuration.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudDuration.Name = "nudDuration";
            this.nudDuration.Size = new System.Drawing.Size(41, 20);
            this.nudDuration.TabIndex = 13;
            this.nudDuration.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(272, 55);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(76, 26);
            this.labelStatus.TabIndex = 12;
            this.labelStatus.Text = "Time left[s]: 0\r\nPerturbation: 0\r\n";
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(867, 416);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Statistics";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // timerStatus
            // 
            this.timerStatus.Tick += new System.EventHandler(this.timerStatus_Tick);
            // 
            // timerChart
            // 
            this.timerChart.Tick += new System.EventHandler(this.timerChart_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel,
            this.toolStripDropDownButton1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 451);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(888, 26);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripLabel
            // 
            this.toolStripLabel.Name = "toolStripLabel";
            this.toolStripLabel.Size = new System.Drawing.Size(58, 21);
            this.toolStripLabel.Text = "Directory:";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setDirectoryToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(33, 24);
            this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Click += new System.EventHandler(this.toolStripDropDownButton1_Click);
            // 
            // setDirectoryToolStripMenuItem
            // 
            this.setDirectoryToolStripMenuItem.Name = "setDirectoryToolStripMenuItem";
            this.setDirectoryToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.setDirectoryToolStripMenuItem.Text = "Set directory";
            this.setDirectoryToolStripMenuItem.Click += new System.EventHandler(this.setDirectoryToolStripMenuItem_Click);
            // 
            // CanalGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(888, 477);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(550, 350);
            this.Name = "CanalGUI";
            this.Text = "Channel GUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.ChannelGUI_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHistory1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.packageChart)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBernoulliPass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStayDrop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStayPass)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHistory)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attackChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrequency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeConst)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAmplitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDuration)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tbTargetTag;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbTargetPort;
        private System.Windows.Forms.TextBox tbTargetIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnAddAttackModel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbMarkov;
        private System.Windows.Forms.RadioButton rbBernoulli;
        private System.Windows.Forms.TextBox tbCanalPort;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudStayDrop;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nudStayPass;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown nudBernoulliPass;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Timer timerStatus;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox cbAllPorts;
        private System.Windows.Forms.CheckBox cbAllIPs;
        private System.Windows.Forms.Timer timerChart;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripLabel;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem setDirectoryToolStripMenuItem;
        private System.Windows.Forms.CheckedListBox clbAttackModels;
        private System.Windows.Forms.Button btnStartAttack;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.RadioButton rbSinusoid;
        public System.Windows.Forms.RadioButton rbTransientDecrease;
        public System.Windows.Forms.RadioButton rbBias;
        public System.Windows.Forms.RadioButton rbTransientIncrease;
        public System.Windows.Forms.NumericUpDown nudAmplitude;
        public System.Windows.Forms.NumericUpDown nudDuration;
        public System.Windows.Forms.NumericUpDown nudFrequency;
        public System.Windows.Forms.NumericUpDown nudTimeConst;
        public System.Windows.Forms.DataVisualization.Charting.Chart attackChart;
        public System.Windows.Forms.RadioButton rbManual;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbSetValue;
        private System.Windows.Forms.RadioButton rbAddValue;
        private System.Windows.Forms.Button btnStopAttack;
        private System.Windows.Forms.Button btnUpdateAttackModel;
        public System.Windows.Forms.TextBox tbTimeSeries;
        public System.Windows.Forms.Label labelTimeSeries;
        public System.Windows.Forms.Button btnClearCustomAttack;
        private System.Windows.Forms.Label label12;
        public System.Windows.Forms.NumericUpDown nudHistory;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckedListBox clbDropOutTarget;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        public System.Windows.Forms.NumericUpDown nudHistory1;
        public System.Windows.Forms.DataVisualization.Charting.Chart packageChart;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label15;
    }
}

