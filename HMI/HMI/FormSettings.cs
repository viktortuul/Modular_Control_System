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
        FrameGUI form;

        public FormSettings()
        {
            InitializeComponent();
        }

        public FrameGUI Main
        {
            get { return form; }
            set { form = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // save settings
            double A1 = Convert.ToDouble(numUpDown_A1.Value);
            double a1 = Convert.ToDouble(numUpDown_a1a.Value);
            double A2 = Convert.ToDouble(numUpDown_A2.Value);
            double a2 = Convert.ToDouble(numUpDown_a2a.Value);
            Properties.Settings.Default.BA1 = A1;
            Properties.Settings.Default.SA1 = a1;
            Properties.Settings.Default.BA2 = A2;
            Properties.Settings.Default.SA2 = a2;
            Properties.Settings.Default.Save();

            // update tank dimensions
            form.tankDimensions = new TankDimensions(A1, a1, A2, a2);

            // update kalman filter
            form.connection_current.UpdateKalmanFilter(A1, a1, A2, a2);

            form.log("Settings updated"); updateLog();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            // load settings
            numUpDown_A1.Value = (decimal)Properties.Settings.Default.BA1;
            numUpDown_a1a.Value = (decimal)Properties.Settings.Default.SA1;
            numUpDown_A2.Value = (decimal)Properties.Settings.Default.BA2;
            numUpDown_a2a.Value = (decimal)Properties.Settings.Default.SA2;
            updateLog();
        }

        private void updateLog()
        {
            tbDebugLog.Text = form.debugLog;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            updateLog();
        }
    }
}
