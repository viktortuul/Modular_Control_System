﻿namespace Canal_GUI
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
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
            this.btnStartListener = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nudStayDrop = new System.Windows.Forms.NumericUpDown();
            this.btnUpdateDropoutModel = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.nudStayPass = new System.Windows.Forms.NumericUpDown();
            this.rbMarkov = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.nudBernoulliPass = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.rbBernoulli = new System.Windows.Forms.RadioButton();
            this.tbCanalPort = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAttack = new System.Windows.Forms.Button();
            this.clbAttackModels = new System.Windows.Forms.CheckedListBox();
            this.perturbationChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
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
            this.saveChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStayDrop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStayPass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBernoulliPass)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.perturbationChart)).BeginInit();
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
            this.label5.Location = new System.Drawing.Point(168, 118);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 17);
            this.label5.TabIndex = 9;
            this.label5.Text = "Amplitude";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(168, 90);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "Duration [s]";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbTransientIncrease);
            this.groupBox2.Controls.Add(this.rbSinusoid);
            this.groupBox2.Controls.Add(this.rbTransientDecrease);
            this.groupBox2.Controls.Add(this.rbBias);
            this.groupBox2.Location = new System.Drawing.Point(8, 79);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(152, 134);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Attack type";
            // 
            // rbTransientIncrease
            // 
            this.rbTransientIncrease.AutoSize = true;
            this.rbTransientIncrease.Location = new System.Drawing.Point(8, 74);
            this.rbTransientIncrease.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbTransientIncrease.Name = "rbTransientIncrease";
            this.rbTransientIncrease.Size = new System.Drawing.Size(130, 21);
            this.rbTransientIncrease.TabIndex = 3;
            this.rbTransientIncrease.Text = "Transient (Incr.)";
            this.rbTransientIncrease.UseVisualStyleBackColor = true;
            this.rbTransientIncrease.CheckedChanged += new System.EventHandler(this.rbTransientIncrease_CheckedChanged);
            // 
            // rbSinusoid
            // 
            this.rbSinusoid.AutoSize = true;
            this.rbSinusoid.Location = new System.Drawing.Point(8, 98);
            this.rbSinusoid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbSinusoid.Name = "rbSinusoid";
            this.rbSinusoid.Size = new System.Drawing.Size(83, 21);
            this.rbSinusoid.TabIndex = 2;
            this.rbSinusoid.Text = "Sinusoid";
            this.rbSinusoid.UseVisualStyleBackColor = true;
            this.rbSinusoid.CheckedChanged += new System.EventHandler(this.rbSinusoid_CheckedChanged);
            // 
            // rbTransientDecrease
            // 
            this.rbTransientDecrease.AutoSize = true;
            this.rbTransientDecrease.Location = new System.Drawing.Point(8, 48);
            this.rbTransientDecrease.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbTransientDecrease.Name = "rbTransientDecrease";
            this.rbTransientDecrease.Size = new System.Drawing.Size(137, 21);
            this.rbTransientDecrease.TabIndex = 1;
            this.rbTransientDecrease.Text = "Transient (Decr.)";
            this.rbTransientDecrease.UseVisualStyleBackColor = true;
            this.rbTransientDecrease.CheckedChanged += new System.EventHandler(this.rbTransientDecrease_CheckedChanged);
            // 
            // rbBias
            // 
            this.rbBias.AutoSize = true;
            this.rbBias.Checked = true;
            this.rbBias.Location = new System.Drawing.Point(8, 23);
            this.rbBias.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbBias.Name = "rbBias";
            this.rbBias.Size = new System.Drawing.Size(56, 21);
            this.rbBias.TabIndex = 0;
            this.rbBias.TabStop = true;
            this.rbBias.Text = "Bias";
            this.rbBias.UseVisualStyleBackColor = true;
            this.rbBias.CheckedChanged += new System.EventHandler(this.rbBias_CheckedChanged);
            // 
            // btnAddAttackModel
            // 
            this.btnAddAttackModel.Location = new System.Drawing.Point(352, 170);
            this.btnAddAttackModel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAddAttackModel.Name = "btnAddAttackModel";
            this.btnAddAttackModel.Size = new System.Drawing.Size(95, 28);
            this.btnAddAttackModel.TabIndex = 6;
            this.btnAddAttackModel.Text = "Add/Update attack model";
            this.btnAddAttackModel.UseVisualStyleBackColor = true;
            this.btnAddAttackModel.Click += new System.EventHandler(this.btnAddAttackModel_Click);
            // 
            // tbTargetTag
            // 
            this.tbTargetTag.Location = new System.Drawing.Point(108, 33);
            this.tbTargetTag.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbTargetTag.Name = "tbTargetTag";
            this.tbTargetTag.Size = new System.Drawing.Size(51, 22);
            this.tbTargetTag.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 37);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Target tag";
            // 
            // tbTargetPort
            // 
            this.tbTargetPort.Location = new System.Drawing.Point(203, 6);
            this.tbTargetPort.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbTargetPort.Name = "tbTargetPort";
            this.tbTargetPort.Size = new System.Drawing.Size(57, 22);
            this.tbTargetPort.TabIndex = 3;
            this.tbTargetPort.Text = "8300";
            // 
            // tbTargetIP
            // 
            this.tbTargetIP.Location = new System.Drawing.Point(108, 6);
            this.tbTargetIP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbTargetIP.Name = "tbTargetIP";
            this.tbTargetIP.Size = new System.Drawing.Size(92, 22);
            this.tbTargetIP.TabIndex = 2;
            this.tbTargetIP.Text = "127.0.0.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Target IP:Port";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(8, 7);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1144, 645);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnStartListener);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.tbCanalPort);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Size = new System.Drawing.Size(1136, 616);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Canal settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnStartListener
            // 
            this.btnStartListener.Location = new System.Drawing.Point(172, 10);
            this.btnStartListener.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStartListener.Name = "btnStartListener";
            this.btnStartListener.Size = new System.Drawing.Size(100, 28);
            this.btnStartListener.TabIndex = 10;
            this.btnStartListener.Text = "Start";
            this.btnStartListener.UseVisualStyleBackColor = true;
            this.btnStartListener.Click += new System.EventHandler(this.btnStartListener_Click_2);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nudStayDrop);
            this.groupBox1.Controls.Add(this.btnUpdateDropoutModel);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.nudStayPass);
            this.groupBox1.Controls.Add(this.rbMarkov);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.nudBernoulliPass);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.rbBernoulli);
            this.groupBox1.Location = new System.Drawing.Point(9, 49);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(257, 160);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Drop out model";
            // 
            // nudStayDrop
            // 
            this.nudStayDrop.Location = new System.Drawing.Point(184, 87);
            this.nudStayDrop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudStayDrop.Name = "nudStayDrop";
            this.nudStayDrop.Size = new System.Drawing.Size(55, 22);
            this.nudStayDrop.TabIndex = 7;
            this.nudStayDrop.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // btnUpdateDropoutModel
            // 
            this.btnUpdateDropoutModel.Location = new System.Drawing.Point(116, 119);
            this.btnUpdateDropoutModel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnUpdateDropoutModel.Name = "btnUpdateDropoutModel";
            this.btnUpdateDropoutModel.Size = new System.Drawing.Size(123, 28);
            this.btnUpdateDropoutModel.TabIndex = 9;
            this.btnUpdateDropoutModel.Text = "Update";
            this.btnUpdateDropoutModel.UseVisualStyleBackColor = true;
            this.btnUpdateDropoutModel.Click += new System.EventHandler(this.btnUpdateDropoutModel_Click_1);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(103, 90);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 17);
            this.label9.TabIndex = 6;
            this.label9.Text = "p(D->D) [%]";
            // 
            // nudStayPass
            // 
            this.nudStayPass.Location = new System.Drawing.Point(184, 62);
            this.nudStayPass.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudStayPass.Name = "nudStayPass";
            this.nudStayPass.Size = new System.Drawing.Size(55, 22);
            this.nudStayPass.TabIndex = 5;
            this.nudStayPass.Value = new decimal(new int[] {
            98,
            0,
            0,
            0});
            // 
            // rbMarkov
            // 
            this.rbMarkov.AutoSize = true;
            this.rbMarkov.Location = new System.Drawing.Point(8, 62);
            this.rbMarkov.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbMarkov.Name = "rbMarkov";
            this.rbMarkov.Size = new System.Drawing.Size(79, 21);
            this.rbMarkov.TabIndex = 1;
            this.rbMarkov.Text = "Markov ";
            this.rbMarkov.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(103, 64);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 17);
            this.label8.TabIndex = 4;
            this.label8.Text = "p(P->P) [%]";
            // 
            // nudBernoulliPass
            // 
            this.nudBernoulliPass.Location = new System.Drawing.Point(184, 23);
            this.nudBernoulliPass.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudBernoulliPass.Name = "nudBernoulliPass";
            this.nudBernoulliPass.Size = new System.Drawing.Size(55, 22);
            this.nudBernoulliPass.TabIndex = 3;
            this.nudBernoulliPass.Value = new decimal(new int[] {
            95,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(103, 26);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 17);
            this.label7.TabIndex = 2;
            this.label7.Text = "Pass [%]";
            // 
            // rbBernoulli
            // 
            this.rbBernoulli.AutoSize = true;
            this.rbBernoulli.Checked = true;
            this.rbBernoulli.Location = new System.Drawing.Point(8, 23);
            this.rbBernoulli.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbBernoulli.Name = "rbBernoulli";
            this.rbBernoulli.Size = new System.Drawing.Size(84, 21);
            this.rbBernoulli.TabIndex = 0;
            this.rbBernoulli.TabStop = true;
            this.rbBernoulli.Text = "Bernoulli";
            this.rbBernoulli.UseVisualStyleBackColor = true;
            // 
            // tbCanalPort
            // 
            this.tbCanalPort.Location = new System.Drawing.Point(107, 11);
            this.tbCanalPort.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbCanalPort.Name = "tbCanalPort";
            this.tbCanalPort.Size = new System.Drawing.Size(57, 22);
            this.tbCanalPort.TabIndex = 4;
            this.tbCanalPort.Text = "8111";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 15);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 17);
            this.label6.TabIndex = 0;
            this.label6.Text = "Listening port";
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.btnAttack);
            this.tabPage1.Controls.Add(this.clbAttackModels);
            this.tabPage1.Controls.Add(this.perturbationChart);
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
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Size = new System.Drawing.Size(1136, 616);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Attack settings";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(348, 68);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 17);
            this.label2.TabIndex = 24;
            this.label2.Text = "Attack models";
            // 
            // btnAttack
            // 
            this.btnAttack.Location = new System.Drawing.Point(465, 170);
            this.btnAttack.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAttack.Name = "btnAttack";
            this.btnAttack.Size = new System.Drawing.Size(99, 28);
            this.btnAttack.TabIndex = 23;
            this.btnAttack.Text = "Apply attack";
            this.btnAttack.UseVisualStyleBackColor = true;
            this.btnAttack.Click += new System.EventHandler(this.btnAttack_Click);
            // 
            // clbAttackModels
            // 
            this.clbAttackModels.FormattingEnabled = true;
            this.clbAttackModels.Location = new System.Drawing.Point(352, 87);
            this.clbAttackModels.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.clbAttackModels.Name = "clbAttackModels";
            this.clbAttackModels.Size = new System.Drawing.Size(211, 72);
            this.clbAttackModels.TabIndex = 22;
            this.clbAttackModels.SelectedIndexChanged += new System.EventHandler(this.clbAttackModels_SelectedIndexChanged);
            // 
            // perturbationChart
            // 
            chartArea1.Name = "ChartArea1";
            this.perturbationChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.perturbationChart.Legends.Add(legend1);
            this.perturbationChart.Location = new System.Drawing.Point(8, 220);
            this.perturbationChart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.perturbationChart.Name = "perturbationChart";
            this.perturbationChart.Size = new System.Drawing.Size(1117, 385);
            this.perturbationChart.TabIndex = 21;
            this.perturbationChart.Text = "chart1";
            title1.Name = "Title1";
            title1.Text = "Attack";
            this.perturbationChart.Titles.Add(title1);
            // 
            // cbAllPorts
            // 
            this.cbAllPorts.AutoSize = true;
            this.cbAllPorts.Location = new System.Drawing.Point(269, 23);
            this.cbAllPorts.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbAllPorts.Name = "cbAllPorts";
            this.cbAllPorts.Size = new System.Drawing.Size(148, 21);
            this.cbAllPorts.TabIndex = 20;
            this.cbAllPorts.Text = "Target all EP ports";
            this.cbAllPorts.UseVisualStyleBackColor = true;
            // 
            // cbAllIPs
            // 
            this.cbAllIPs.AutoSize = true;
            this.cbAllIPs.Location = new System.Drawing.Point(269, 6);
            this.cbAllIPs.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbAllIPs.Name = "cbAllIPs";
            this.cbAllIPs.Size = new System.Drawing.Size(138, 21);
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
            this.nudFrequency.Location = new System.Drawing.Point(289, 172);
            this.nudFrequency.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudFrequency.Name = "nudFrequency";
            this.nudFrequency.Size = new System.Drawing.Size(55, 22);
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
            this.label11.Location = new System.Drawing.Point(168, 175);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(104, 17);
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
            this.nudTimeConst.Location = new System.Drawing.Point(289, 144);
            this.nudTimeConst.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudTimeConst.Name = "nudTimeConst";
            this.nudTimeConst.Size = new System.Drawing.Size(55, 22);
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
            this.label10.Location = new System.Drawing.Point(168, 146);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(116, 17);
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
            this.nudAmplitude.Location = new System.Drawing.Point(289, 116);
            this.nudAmplitude.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudAmplitude.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.nudAmplitude.Name = "nudAmplitude";
            this.nudAmplitude.Size = new System.Drawing.Size(55, 22);
            this.nudAmplitude.TabIndex = 14;
            this.nudAmplitude.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudDuration
            // 
            this.nudDuration.Location = new System.Drawing.Point(289, 87);
            this.nudDuration.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudDuration.Name = "nudDuration";
            this.nudDuration.Size = new System.Drawing.Size(55, 22);
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
            this.labelStatus.Location = new System.Drawing.Point(572, 87);
            this.labelStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(102, 34);
            this.labelStatus.TabIndex = 12;
            this.labelStatus.Text = "Time left[s]: 0\r\nPerturbation: 0\r\n";
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1136, 616);
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 677);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1232, 26);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripLabel
            // 
            this.toolStripLabel.Name = "toolStripLabel";
            this.toolStripLabel.Size = new System.Drawing.Size(73, 21);
            this.toolStripLabel.Text = "Directory:";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setDirectoryToolStripMenuItem,
            this.saveChartToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(34, 24);
            this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Click += new System.EventHandler(this.toolStripDropDownButton1_Click);
            // 
            // setDirectoryToolStripMenuItem
            // 
            this.setDirectoryToolStripMenuItem.Name = "setDirectoryToolStripMenuItem";
            this.setDirectoryToolStripMenuItem.Size = new System.Drawing.Size(168, 26);
            this.setDirectoryToolStripMenuItem.Text = "Set directory";
            // 
            // saveChartToolStripMenuItem
            // 
            this.saveChartToolStripMenuItem.Name = "saveChartToolStripMenuItem";
            this.saveChartToolStripMenuItem.Size = new System.Drawing.Size(168, 26);
            this.saveChartToolStripMenuItem.Text = "Save chart";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1232, 703);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MainForm";
            this.Text = "Canal GUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStayDrop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStayPass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBernoulliPass)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.perturbationChart)).EndInit();
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
        private System.Windows.Forms.Button btnUpdateDropoutModel;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btnStartListener;
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
        private System.Windows.Forms.ToolStripMenuItem saveChartToolStripMenuItem;
        private System.Windows.Forms.CheckedListBox clbAttackModels;
        private System.Windows.Forms.Button btnAttack;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.RadioButton rbSinusoid;
        public System.Windows.Forms.RadioButton rbTransientDecrease;
        public System.Windows.Forms.RadioButton rbBias;
        public System.Windows.Forms.RadioButton rbTransientIncrease;
        public System.Windows.Forms.NumericUpDown nudAmplitude;
        public System.Windows.Forms.NumericUpDown nudDuration;
        public System.Windows.Forms.NumericUpDown nudFrequency;
        public System.Windows.Forms.NumericUpDown nudTimeConst;
        public System.Windows.Forms.DataVisualization.Charting.Chart perturbationChart;
    }
}
