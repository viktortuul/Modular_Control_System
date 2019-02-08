using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.Drawing;
using System.Globalization;
using GlobalComponents;

namespace HMI
{
    public struct TankDimensions
    {
        public double A1, a1, A2, a2;

        public TankDimensions(double A1, double a1, double A2, double a2)
        {
            this.A1 = A1;
            this.a1 = a1;
            this.A2 = A2;
            this.a2 = a2;
        }
    }

    public static class Helpers
    {
        public static void ManageReferencesKeys(int n_contr_states, Dictionary<string, DataContainer> references, int n_steps)
        {
            // add reference key
            if (n_contr_states >= 1)
            {
                if (references.ContainsKey("r1") == false)
                {
                    references.Add("r1", new DataContainer(n_steps));
                    references["r1"].InsertData(DateTime.UtcNow.ToString(Constants.FMT), "0");
                }
            }
            if (n_contr_states >= 2)
            {
                if (references.ContainsKey("r2") == false)
                {
                    references.Add("r2", new DataContainer(n_steps));
                    references["r2"].InsertData(DateTime.UtcNow.ToString(Constants.FMT), "0");
                }
            }
        }

        public static void ManageEstimatesKeys(string key, Dictionary<string, DataContainer> estimates, int n_steps)
        {
            estimates.Add(key + "_hat", new DataContainer(n_steps)); 
        }


        public static void ManageTrackbars(FrameGUI GUI)
        {
            // enable or disable trackbars
            if (GUI.connection_selected.n_controlled_states == 1)
            {
                GUI.trackBarReference1.Enabled = true;
                GUI.numUpDownRef1.Enabled = true;
                GUI.trackBarReference1.Value = Convert.ToInt16(GUI.connection_selected.references["r1"].GetLastValue());
                GUI.numUpDownRef1.Value = Convert.ToInt16(GUI.connection_selected.references["r1"].GetLastValue());
            }
            else if (GUI.connection_selected.n_controlled_states == 2)
            {
                GUI.trackBarReference1.Enabled = true;
                GUI.trackBarReference2.Visible = true;
                GUI.numUpDownRef1.Enabled = true;
                GUI.numUpDownRef2.Visible = true;
                GUI.trackBarReference2.Value = Convert.ToInt16(GUI.connection_selected.references["r2"].GetLastValue());
                GUI.numUpDownRef2.Value = Convert.ToInt16(GUI.connection_selected.references["r2"].GetLastValue());
            }
            else
            {
                GUI.trackBarReference1.Enabled = false;
                GUI.trackBarReference2.Visible = false;
                GUI.numUpDownRef1.Enabled = false;
                GUI.numUpDownRef2.Visible = false;
            }
        }

        public static void UpdateGuiControls(FrameGUI GUI, CommunicationManager connection_selected)
        {
            GUI.numUpDownKp.Value = Convert.ToDecimal(connection_selected.ControllerParameters.Kp);
            GUI.numUpDownKi.Value = Convert.ToDecimal(connection_selected.ControllerParameters.Ki);
            GUI.numUpDownKd.Value = Convert.ToDecimal(connection_selected.ControllerParameters.Kd);
            GUI.textBox_ip_send.Text = connection_selected.Controller_EP.IP;
            GUI.numericUpDown_port_send.Text = connection_selected.Controller_EP.Port.ToString();
            GUI.numericUpDown_port_recieve.Text = connection_selected.Controller_EP.PortThis.ToString();
        }

        public static void UpdateTree(FrameGUI GUI, CommunicationManager controller)
        {
            // update tree
            GUI.treeViewControllers.Nodes[controller.name].Text = controller.name + " (" + controller.GetConnectionStatus() + ")";
            GUI.treeViewControllers.Nodes[controller.name].Nodes["Controller"].Nodes["Kp"].Text = "Kp: " + GUI.connection_selected.ControllerParameters.Kp.ToString();
            GUI.treeViewControllers.Nodes[controller.name].Nodes["Controller"].Nodes["Ki"].Text = "Ki: " + GUI.connection_selected.ControllerParameters.Ki.ToString();
            GUI.treeViewControllers.Nodes[controller.name].Nodes["Controller"].Nodes["Kd"].Text = "Kd: " + GUI.connection_selected.ControllerParameters.Kd.ToString();
        }

        public static void UpdateTimeLabels(FrameGUI GUI, CommunicationManager connection, string FMT)
        {
            try
            {
                DateTime time_sent = DateTime.ParseExact(connection.recieved_packets["u1"].GetLastTime(), FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                TimeSpan timeDiff = connection.time_last_recieved_packet - time_sent;
                GUI.label_time.Text =   "time now:                    " + DateTime.UtcNow.ToString("hh:mm:ss.fff tt") + "\n" +
                                        "last recieved:              " + connection.time_last_recieved_packet.ToString("hh:mm:ss.fff tt") + "\n" +
                                        "when it was sent:        " + time_sent.ToString("hh:mm:ss.fff tt") + "\n" +
                                        "transmission duration [ms]: " + timeDiff.TotalMilliseconds;
            }
            catch
            {
                GUI.label_time.Text = "time now:                  \n" +
                                        "last recieved:           \n" +
                                        "when it was sent:        \n" +
                                        "transmission duration [ms]: ";
            }
        }
    }
}

//this.Invoke((MethodInvoker)delegate ()
//{

//});