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
    }
}
