using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HMI
{
    public partial class FormSettings : Form
    {
        // contains the main gui
        FrameGUI MainForm;

        public FormSettings()
        {
            InitializeComponent();
        }

        public FrameGUI Main
        {
            get { return MainForm; }
            set { MainForm = value; }
        }

        private void updateLog()
        {
            tbDebugLog.Text = MainForm.debugLog;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            updateLog();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            numUpDown_A1.Value = (decimal)Main.connection_selected.kalman_filter.getA1();
            numUpDown_a1a.Value = (decimal)Main.connection_selected.kalman_filter.geta1();
            numUpDown_A2.Value = (decimal)Main.connection_selected.kalman_filter.getA2();
            numUpDown_a2a.Value = (decimal)Main.connection_selected.kalman_filter.geta2();
            numUpDown_k.Value = (decimal)Main.connection_selected.kalman_filter.getK();
            numUpDown_delta.Value = (decimal)Main.connection_selected.kalman_filter.getDelta();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Main.connection_selected.kalman_filter.updateKalmanFilter(Convert.ToDouble(numUpDown_A1.Value), Convert.ToDouble(numUpDown_a1a.Value), Convert.ToDouble(numUpDown_A2.Value), Convert.ToDouble(numUpDown_a2a.Value), Convert.ToDouble(numUpDown_k.Value));
            Main.connection_selected.kalman_filter.setAnomalyDetectorSettings(Convert.ToDouble(numUpDown_delta.Value));
            Main.updateThresholdStripLines();
            tbDebugLog.Text += (DateTime.UtcNow + ": " + "Updated parameters" + Environment.NewLine);
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
