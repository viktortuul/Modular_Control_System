using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HMI
{
    public class KalmanFilter
    {
        // plant parameters
        private double a1, a2, A1, A2, k, g = 981, dt;
        private double[,] x, G;                                     // states and jacobian
        private double[,] P = new double[,] { { 1, 1 }, { 1, 1 } }; // state covariance
        private double[,] R = new double[,] { { 1, 1 }, { 1, 1 } }; // process noise covariance
        private double Q = 50;                                      // measurement noise covariance
        private double[,] H = new double[1, 2] { {0, 1} };          // measurement model jacobian
        private double[,] I = new double[,] { { 1, 0 }, { 0, 1 } }; // identity matrix
        private DateTime update_last = DateTime.Now;

        // anomaly detector parameters
        public string security_status = "Low";
        public double innovation = 0;
        public double security_metric = 0;
        private double delta = 0.2;

        public KalmanFilter(double[,] x, double a1, double a2, double A1, double A2, double k)
        {
            this.x = x; // states (h1, h2)
            this.a1 = a1;
            this.a2 = a2;
            this.A1 = A1;
            this.A2 = A2;
            this.k = k;
        }

        public double[,] Update(double z, double u)
        {
            DateTime nowTime = DateTime.Now;
            dt = (nowTime - update_last).TotalSeconds;

            // saturation
            if (x[0, 0] <= 0) x[0, 0] = 0.01;
            if (x[1, 0] <= 0) x[1, 0] = 0.01;

            // calcultate flows
            double q_out1 = a1 * Math.Sqrt(2 * g * x[0, 0]);
            double q_out2 = a2 * Math.Sqrt(2 * g * x[1, 0]);

            // apply motion 
            x[0, 0] += dt * (1 / A1) * (k * u - q_out1); // top tank
            x[1, 0] += dt * (1 / A2) * (q_out1 - q_out2); // bottom tank

            // saturation
            if (x[0, 0] <= 0) x[0, 0] = 0.01;
            if (x[1, 0] <= 0) x[1, 0] = 0.01;

            // update the state uncertainty
            G = get_G(x);
            P = Matrix.Add(Matrix.Multiply(G, Matrix.Multiply(P, G)), R);

            // calculate the kalman gain
            double[,] K = Matrix.Divide(Matrix.Multiply(P, Matrix.Transpose(H)), Matrix.Add(Matrix.Multiply(H, Matrix.Multiply(P, Matrix.Transpose(H))), Q)[0, 0]);

            // measurement update
            x[0, 0] += dt * K[0, 0] * (z - x[1, 0]);
            x[1, 0] += dt * K[1, 0] * (z - x[1, 0]);
            innovation = z - x[1, 0];

            P = Matrix.Multiply(Matrix.Subtract(I, Matrix.Multiply(K, H)), P);

            // update prior time
            update_last = nowTime;

            UpdateAnomalyDetector();

            return x;
        }

        private void UpdateAnomalyDetector()
        {
            security_metric += Math.Abs(innovation) - delta;
            if (security_metric < 0) security_metric = 0;

            if (security_metric < 10) security_status = "High";
            if (security_metric < 5) security_status = "Medium";
            if (security_metric < 2) security_status = "Low";
        }

        private double[,] get_G(double[,] x)
        {
            double x1 = x[0, 0];
            double x2 = x[1, 0];
            double[,] G = new double[2, 2];

            G[0, 0] = 1 - 2 * g * dt * (a1 / A1) * (1 / (Math.Sqrt(2 * g * x1)))/2;
            G[0, 1] = 0;
            G[1, 0] = 2 * g * dt * (a1 / A2) * (1 / (Math.Sqrt(2 * g * x1)))/2;
            G[1, 1] = 1 - 2 * g * dt * (a2 / A2) * (1 / (Math.Sqrt(2 * g * x2)))/2;

            return G;
        }

        private double h(double[,] x)
        {
            return x[1, 0];
        }
    }

    static class Matrix
    {
        public static double[,] Multiply(double[,] A, double[,] B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);
            int rB = B.GetLength(0);
            int cB = B.GetLength(1);
            double temp = 0;
            double[,] result = new double[rA, cB];

            for (int i = 0; i < rA; i++)
            {
                for (int j = 0; j < cB; j++)
                {
                    temp = 0;
                    for (int k = 0; k < cA; k++)
                    {
                        temp += A[i, k] * B[k, j];
                    }
                    result[i, j] = temp;
                }
            }         
            return result;
        }

        public static double[,] Multiply(double[,] A, double b)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);

            double[,] result = new double[rA, cA];
            for (int i = 0; i < rA; i++)
            {
                for (int j = 0; j < cA; j++)
                {
                    result[i, j] = A[i, j] * b;
                }
            }
            return result;
        }

        public static double[,] Add(double[,] A, double[,] B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);

            double[,] dotsum = new double[rA, cA];
            for (int i = 0; i < rA; i++)
            {
                for (int j = 0; j < cA; j++)
                {
                    dotsum[i, j] = A[i, j] + B[i, j];
                }
            }
            return dotsum;
        }

        public static double[,] Add(double[,] A, double B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);

            double[,] dotsum = new double[rA, cA];
            for (int i = 0; i < rA; i++)
            {
                for (int j = 0; j < cA; j++)
                {
                    dotsum[i, j] = A[i, j] + B;
                }
            }
            return dotsum;
        }

        public static double[,] Subtract(double[,] A, double[,] B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);

            double[,] dotsum = new double[rA, cA];
            for (int i = 0; i < rA; i++)
            {
                for (int j = 0; j < cA; j++)
                {
                    dotsum[i, j] = A[i, j] - B[i, j];
                }
            }
            return dotsum;
        }

        public static double[,] Divide(double[,] A, double B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);

            double[,] result = new double[rA, cA];
            for (int i = 0; i < rA; i++)
            {
                for (int j = 0; j < cA; j++)
                {
                    result[i, j] = A[i, j] / B;
                }
            }
            return result;
        }

        public static double[,] Transpose(double[,] matrix)
        {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);

            double[,] result = new double[h, w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    result[j, i] = matrix[i, j];
                }
            }
            return result;
        }
    }
}
