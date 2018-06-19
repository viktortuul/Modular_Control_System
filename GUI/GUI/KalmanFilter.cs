using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GUI
{
    class KalmanFilter
    {
        private double a1, a2, A1, A2, k, g = 981, dt;
        private double[,] x, G;
        private double[,] P = new double[,] { { 1, 1 }, { 1, 1 } };
        private double[,] R = new double[,] { { 3, 1 }, { 1, 1 } };
        private double Q = 2;
        private double[,] H = new double[1, 2] { {0, 1} };
        private double[,] I = new double[,] { { 1, 0 }, { 0, 1 } };
        private DateTime update_last = DateTime.Now;

        public KalmanFilter(double[,] initial_x, double a1, double a2, double A1, double A2, double k)
        {
            this.x = initial_x; // states (h1, h2)
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

            double h1 = x[0, 0];
            double h2 = x[1, 0];
            double q_out1 = a1 * Math.Sqrt(2 * x[0, 0] * g);
            double q_out2 = a2 * Math.Sqrt(2 * x[1, 0] * g);

            // apply motion 
            x[0, 0] += dt * (1 / A1) * (k * u - q_out1); // top tank
            x[1, 0] += dt * (1 / A2) * (q_out1 - q_out2); // bottom tank

            // saturation
            if (x[0, 0] <= 0) x[0, 0] = 0.01;
            if (x[1, 0] <= 0) x[1, 0] = 0.01;

            G = update_G(x);
            P = ADD(Matrix.Multiply(G, Matrix.Multiply(P, G)), R);

            // measurement
            double[,] K = divide(Matrix.Multiply(P, Transpose(H)), add(Matrix.Multiply(H, Matrix.Multiply(P, Transpose(H))), Q)[0, 0]);
            //x = ADD(x, mult(K, (z - x[1,0])));

            //Debug.WriteLine("dh1: " + (dt * (1 / A1) * (k * u - q_out1)));
            //Debug.WriteLine("dh1: " + (K[0, 0] * (z - x[1, 0])));

            x[0, 0] += K[0, 0] * (z - x[1, 0]);
            x[1, 0] += K[1, 0] * (z - x[1, 0]);
            P = Matrix.Multiply(MINUS(I, Matrix.Multiply(K, H)), P);

            //Debug.WriteLine("x1: " + x[0,0] + " x2: " + x[1,0]);
            //Debug.WriteLine("inno :" + (z - x[1, 0]));
            //Debug.WriteLine("K: " + K[0, 0] + "; " + K[1, 0]);

            // update prior time
            update_last = nowTime;

            return x;
        }

        private double[,] update_G(double[,] x)
        {
            double x1 = x[0, 0];
            double x2 = x[1, 0];
            double[,] G = new double[2, 2];

            G[0, 0] = 1 - 9.81 * dt * a1 / A1 * (1 / (Math.Sqrt(2 * g * x1)));
            G[0, 1] = 0;
            G[1, 0] = 9.81 * dt * a1 / A2 * (1 / (Math.Sqrt(2 * g * x1)));
            G[1, 1] = 1 - 9.81 * dt * a2 / A2 * (1 / (Math.Sqrt(2 * g * x2)));

            return G;
        }

        private double h(double[,] x)
        {
            return x[1, 0];
        }

        //public double[,] MULT(double[,] A, double[,] B)
        //{
        //    int rA = A.GetLength(0);
        //    int cA = A.GetLength(1);
        //    int rB = B.GetLength(0);
        //    int cB = B.GetLength(1);
        //    double temp = 0;
        //    double[,] kHasil = new double[rA, cB];
        //    if (cA != rB)
        //    {
        //    }
        //    else
        //    {
        //        for (int i = 0; i < rA; i++)
        //        {
        //            for (int j = 0; j < cB; j++)
        //            {
        //                temp = 0;
        //                for (int k = 0; k < cA; k++)
        //                {
        //                    temp += A[i, k] * B[k, j];
        //                }
        //                kHasil[i, j] = temp;
        //            }
        //        }
        //    }
        //    return kHasil;
        //}
        //public double[,] mult(double[,] A, double B)
        //{
        //    int rA = A.GetLength(0);
        //    int cA = A.GetLength(1);

        //    double[,] result = new double[rA, cA];
        //    for (int i = 0; i < rA; i++)
        //    {
        //        for (int j = 0; j < cA; j++)
        //        {
        //            result[i, j] = A[i, j] * B;
        //        }
        //    }
        //    return result;
        //}

        public double[,] ADD(double[,] A, double[,] B)
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

        public double[,] add(double[,] A, double B)
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

        public double[,] MINUS(double[,] A, double[,] B)
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

        public double[,] divide(double[,] A, double B)
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

        public double[,] Transpose(double[,] matrix)
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
            if (cA != rB)
            {
            }
            else
            {
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
            }
            return result;
        }

        //public static double[,] Multiply(double[,] A, double B)
        //{
        //    int rA = A.GetLength(0);
        //    int cA = A.GetLength(1);

        //    double[,] result = new double[rA, cA];
        //    for (int i = 0; i < rA; i++)
        //    {
        //        for (int j = 0; j < cA; j++)
        //        {
        //            result[i, j] = A[i, j] * B;
        //        }
        //    }
        //    return result;
        //}

    }
}
