using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMI
{
    class Animation
    {
        // drawing settings
        public static Graphics g;
        public static Pen pen_b = new Pen(Color.Black, 4);
        public static Pen pen_r = new Pen(Color.Red, 3);
        public static SolidBrush brush_b = new SolidBrush(Color.LightBlue);
        public static Bitmap bm = new Bitmap(400, 800);

        public static void DrawTanks(FrameGUI GUI, CommunicationManager connection)
        {
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);

            // map tank height in cm to pixels
            double max_height_p = GUI.pictureBox1.Height / 2.5; // max height [pixels]
            double max_inflow_width = 10;
            double max_height_r = 20; // real max height [cm]
            double cm2pix = max_height_p / max_height_r;

            // if there are no estimates, there is nothing to animate 
            if (connection.estimates.ContainsKey("yo1_hat") == false || connection.estimates.ContainsKey("yc1_hat") == false)
            {
                GUI.pictureBox1.Image = bm;
                return;
            }

            // extract signals
            int u = Convert.ToInt16(Convert.ToDouble(connection.received_packets["u1"].GetLastValue()));
            int y1 = Convert.ToInt16(cm2pix * Convert.ToDouble(connection.estimates["yo1_hat"].GetLastValue()));
            int y2 = Convert.ToInt16(cm2pix * Convert.ToDouble(connection.estimates["yc1_hat"].GetLastValue()));

            int reference = 0;

            try { reference = Convert.ToInt16(cm2pix * Convert.ToDouble(connection.references["r1"].GetLastValue())); }
            catch { }

            // dimensions
            double A1 = GUI.tankDimensions.A1;
            double a1 = GUI.tankDimensions.a1;
            double A2 = GUI.tankDimensions.A2;
            double a2 = GUI.tankDimensions.a2;

            // TANK 1 ----------------------------------------------------------------
            Point T1 = new Point(GUI.pictureBox1.Width / 2, Convert.ToInt16(GUI.pictureBox1.Height / 2));
            Point T2 = new Point(T1.X, Convert.ToInt16(GUI.pictureBox1.Height - 20));

            // tank dimensions
            double R1_ = Math.Sqrt(A1 / Math.PI); int R1 = Convert.ToInt16(R1_ * cm2pix);
            double r1_ = Math.Sqrt(a1 / Math.PI); int r1 = Convert.ToInt16(r1_ * cm2pix);
            double h1_ = 20; int h1 = Convert.ToInt16(h1_ * cm2pix);

            // inlet
            if (u > 0)
            {
                Rectangle water_in = new Rectangle(T1.X - R1 + 5, T1.Y - h1 - 50, Convert.ToInt16(max_inflow_width * (u / 7.5)), h1 + 50);
                g.FillRectangle(brush_b, water_in);
            }

            // water 
            Rectangle water1 = new Rectangle(T1.X - R1, T1.Y - y1, 2 * R1, y1);
            g.FillRectangle(brush_b, water1);

            if (y1 > 5)
            {
                Rectangle water_fall = new Rectangle(T1.X - r1, T1.Y, 2 * r1, T2.Y - T1.Y);
                g.FillRectangle(brush_b, water_fall);
            }

            // walls
            Point w1_top = new Point(T1.X - R1, T1.Y - h1); Point w1_bot = new Point(T1.X - R1, T1.Y);
            Point w2_top = new Point(T1.X + R1, T1.Y - h1); Point w2_bot = new Point(T1.X + R1, T1.Y);
            Point wb1_l = new Point(T1.X - R1, T1.Y); Point wb1_r = new Point(T1.X - r1, T1.Y);
            Point wb2_l = new Point(T1.X + r1, T1.Y); Point wb2_r = new Point(T1.X + R1, T1.Y);
            g.DrawLine(pen_b, w1_top, w1_bot);
            g.DrawLine(pen_b, w2_top, w2_bot);
            g.DrawLine(pen_b, wb1_l, wb1_r);
            g.DrawLine(pen_b, wb2_l, wb2_r);

            // TANK 2 ---------------------------------------------------------------

            // tank dimensions
            double R2_ = Math.Sqrt(A2 / Math.PI); int R2 = Convert.ToInt16(R2_ * cm2pix);
            double r2_ = Math.Sqrt(a2 / Math.PI); int r2 = Convert.ToInt16(r2_ * cm2pix);
            double h2_ = 20; int h2 = Convert.ToInt16(h2_ * cm2pix);

            // water 
            Rectangle water2 = new Rectangle(T2.X - R2, T2.Y - y2, 2 * R2, y2);
            g.FillRectangle(brush_b, water2);

            if (y2 > 5)
            {
                Rectangle water_fall = new Rectangle(T2.X - r2, T2.Y, 2 * r2, 200);
                g.FillRectangle(brush_b, water_fall);
            }

            // walls
            w1_top = new Point(T2.X - R2, T2.Y - h2); w1_bot = new Point(T2.X - R2, T2.Y);
            w2_top = new Point(T2.X + R2, T2.Y - h2); w2_bot = new Point(T2.X + R2, T2.Y);
            wb1_l = new Point(T2.X - R2, T2.Y); wb1_r = new Point(T2.X - r2, T2.Y);
            wb2_l = new Point(T2.X + r2, T2.Y); wb2_r = new Point(T2.X + R2, T2.Y);

            g.DrawLine(pen_b, w1_top, w1_bot);
            g.DrawLine(pen_b, w2_top, w2_bot);
            g.DrawLine(pen_b, wb1_l, wb1_r);
            g.DrawLine(pen_b, wb2_l, wb2_r);

            // draw reference
            Point reference_l = new Point(T2.X - R2 - 15, T2.Y - reference); Point reference_r = new Point(T2.X - R2 - 3, T2.Y - reference);
            g.DrawLine(pen_r, reference_l, reference_r);

            GUI.pictureBox1.Image = bm;
        }
    }
}
