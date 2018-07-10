namespace Canal_GUI
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea8 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend8 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbTransientIncrease = new System.Windows.Forms.RadioButton();
            this.rbSinusoid = new System.Windows.Forms.RadioButton();
            this.rbTransientDecrease = new System.Windows.Forms.RadioButton();
            this.rbBias = new System.Windows.Forms.RadioButton();
            this.btnStart = new System.Windows.Forms.Button();
            this.tbTargetTag = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbTargetPort = new System.Windows.Forms.TextBox();
            this.tbTargetIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nudStayDrop = new System.Windows.Forms.NumericUpDown();
            this.btnApply = new System.Windows.Forms.Button();
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
            this.button2 = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
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
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.setDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
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
            this.label5.Location = new System.Drawing.Point(120, 96);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Amplitude";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(120, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Duration [s]";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbTransientIncrease);
            this.groupBox2.Controls.Add(this.rbSinusoid);
            this.groupBox2.Controls.Add(this.rbTransientDecrease);
            this.groupBox2.Controls.Add(this.rbBias);
            this.groupBox2.Location = new System.Drawing.Point(6, 64);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(108, 100);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Type";
            // 
            // rbTransientIncrease
            // 
            this.rbTransientIncrease.AutoSize = true;
            this.rbTransientIncrease.Location = new System.Drawing.Point(6, 60);
            this.rbTransientIncrease.Name = "rbTransientIncrease";
            this.rbTransientIncrease.Size = new System.Drawing.Size(81, 17);
            this.rbTransientIncrease.TabIndex = 3;
            this.rbTransientIncrease.Text = "Transient (I)";
            this.rbTransientIncrease.UseVisualStyleBackColor = true;
            this.rbTransientIncrease.CheckedChanged += new System.EventHandler(this.rbTransientIncrease_CheckedChanged);
            // 
            // rbSinusoid
            // 
            this.rbSinusoid.AutoSize = true;
            this.rbSinusoid.Location = new System.Drawing.Point(6, 80);
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
            this.rbTransientDecrease.Size = new System.Drawing.Size(86, 17);
            this.rbTransientDecrease.TabIndex = 1;
            this.rbTransientDecrease.Text = "Transient (D)";
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
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(258, 138);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(159, 23);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Add/Update attack model";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.button1_Click);
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
            this.tbTargetPort.Location = new System.Drawing.Point(152, 5);
            this.tbTargetPort.Name = "tbTargetPort";
            this.tbTargetPort.Size = new System.Drawing.Size(44, 20);
            this.tbTargetPort.TabIndex = 3;
            this.tbTargetPort.Text = "8300";
            // 
            // tbTargetIP
            // 
            this.tbTargetIP.Location = new System.Drawing.Point(81, 5);
            this.tbTargetIP.Name = "tbTargetIP";
            this.tbTargetIP.Size = new System.Drawing.Size(70, 20);
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
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(858, 524);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.tbCanalPort);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(850, 498);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Canal settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(129, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nudStayDrop);
            this.groupBox1.Controls.Add(this.btnApply);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.nudStayPass);
            this.groupBox1.Controls.Add(this.rbMarkov);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.nudBernoulliPass);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.rbBernoulli);
            this.groupBox1.Location = new System.Drawing.Point(7, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(193, 130);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Drop out model";
            // 
            // nudStayDrop
            // 
            this.nudStayDrop.Location = new System.Drawing.Point(138, 71);
            this.nudStayDrop.Name = "nudStayDrop";
            this.nudStayDrop.Size = new System.Drawing.Size(41, 20);
            this.nudStayDrop.TabIndex = 7;
            this.nudStayDrop.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(87, 97);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(92, 23);
            this.btnApply.TabIndex = 9;
            this.btnApply.Text = "Update";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(77, 73);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "p(D->D) [%]";
            // 
            // nudStayPass
            // 
            this.nudStayPass.Location = new System.Drawing.Point(138, 50);
            this.nudStayPass.Name = "nudStayPass";
            this.nudStayPass.Size = new System.Drawing.Size(41, 20);
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
            this.rbMarkov.Location = new System.Drawing.Point(6, 50);
            this.rbMarkov.Name = "rbMarkov";
            this.rbMarkov.Size = new System.Drawing.Size(64, 17);
            this.rbMarkov.TabIndex = 1;
            this.rbMarkov.Text = "Markov ";
            this.rbMarkov.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(77, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "p(P->P) [%]";
            // 
            // nudBernoulliPass
            // 
            this.nudBernoulliPass.Location = new System.Drawing.Point(138, 19);
            this.nudBernoulliPass.Name = "nudBernoulliPass";
            this.nudBernoulliPass.Size = new System.Drawing.Size(41, 20);
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
            // 
            // tbCanalPort
            // 
            this.tbCanalPort.Location = new System.Drawing.Point(80, 9);
            this.tbCanalPort.Name = "tbCanalPort";
            this.tbCanalPort.Size = new System.Drawing.Size(44, 20);
            this.tbCanalPort.TabIndex = 4;
            this.tbCanalPort.Text = "8111";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Listening port";
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.checkedListBox1);
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
            this.tabPage1.Controls.Add(this.btnStart);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(850, 498);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Attack settings";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(423, 138);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(74, 23);
            this.button2.TabIndex = 23;
            this.button2.Text = "Apply attack";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(258, 71);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(159, 64);
            this.checkedListBox1.TabIndex = 22;
            this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
            // 
            // perturbationChart
            // 
            chartArea8.Name = "ChartArea1";
            this.perturbationChart.ChartAreas.Add(chartArea8);
            legend8.Name = "Legend1";
            this.perturbationChart.Legends.Add(legend8);
            this.perturbationChart.Location = new System.Drawing.Point(6, 179);
            this.perturbationChart.Name = "perturbationChart";
            this.perturbationChart.Size = new System.Drawing.Size(631, 313);
            this.perturbationChart.TabIndex = 21;
            this.perturbationChart.Text = "chart1";
            // 
            // cbAllPorts
            // 
            this.cbAllPorts.AutoSize = true;
            this.cbAllPorts.Location = new System.Drawing.Point(202, 19);
            this.cbAllPorts.Name = "cbAllPorts";
            this.cbAllPorts.Size = new System.Drawing.Size(80, 17);
            this.cbAllPorts.TabIndex = 20;
            this.cbAllPorts.Text = "All EP ports";
            this.cbAllPorts.UseVisualStyleBackColor = true;
            // 
            // cbAllIPs
            // 
            this.cbAllIPs.AutoSize = true;
            this.cbAllIPs.Location = new System.Drawing.Point(202, 5);
            this.cbAllIPs.Name = "cbAllIPs";
            this.cbAllIPs.Size = new System.Drawing.Size(74, 17);
            this.cbAllIPs.TabIndex = 19;
            this.cbAllIPs.Text = "All EP IP\'s";
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
            this.nudFrequency.Location = new System.Drawing.Point(211, 140);
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
            this.label11.Location = new System.Drawing.Point(120, 142);
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
            this.nudTimeConst.Location = new System.Drawing.Point(211, 117);
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
            this.label10.Location = new System.Drawing.Point(120, 119);
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
            this.nudAmplitude.Location = new System.Drawing.Point(211, 94);
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
            this.nudDuration.Location = new System.Drawing.Point(211, 71);
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
            this.labelStatus.Location = new System.Drawing.Point(423, 71);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(76, 26);
            this.labelStatus.TabIndex = 12;
            this.labelStatus.Text = "Time left[s]: 0\r\nPerturbation: 0\r\n";
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(850, 498);
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
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripDropDownButton1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 549);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(924, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(58, 17);
            this.toolStripStatusLabel1.Text = "Directory:";
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
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(29, 20);
            this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Click += new System.EventHandler(this.toolStripDropDownButton1_Click);
            // 
            // setDirectoryToolStripMenuItem
            // 
            this.setDirectoryToolStripMenuItem.Name = "setDirectoryToolStripMenuItem";
            this.setDirectoryToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.setDirectoryToolStripMenuItem.Text = "Set directory";
            // 
            // saveChartToolStripMenuItem
            // 
            this.saveChartToolStripMenuItem.Name = "saveChartToolStripMenuItem";
            this.saveChartToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.saveChartToolStripMenuItem.Text = "Save chart";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(255, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Attack models";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 571);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
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
        private System.Windows.Forms.RadioButton rbSinusoid;
        private System.Windows.Forms.RadioButton rbTransientDecrease;
        private System.Windows.Forms.RadioButton rbBias;
        private System.Windows.Forms.Button btnStart;
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
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Timer timerStatus;
        private System.Windows.Forms.NumericUpDown nudAmplitude;
        private System.Windows.Forms.NumericUpDown nudDuration;
        private System.Windows.Forms.NumericUpDown nudFrequency;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown nudTimeConst;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox cbAllPorts;
        private System.Windows.Forms.CheckBox cbAllIPs;
        private System.Windows.Forms.DataVisualization.Charting.Chart perturbationChart;
        private System.Windows.Forms.Timer timerChart;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem setDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveChartToolStripMenuItem;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.RadioButton rbTransientIncrease;
        private System.Windows.Forms.Label label2;
    }
}

