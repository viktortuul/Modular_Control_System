using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Threading;
using Communication;

namespace Model_DoubleWatertank
{
    class Program
    {
        // measurement noise


        // data containers (dictionaries)
        static Dictionary<string, string> package_last = new Dictionary<string, string>(); // contains control signals

        static void Main(string[] args)
        {
            // parse the command line arguments
            string ip_controller_send = args[0];
            string ip_plant_recieve = args[1];
            int port_controller_send = Convert.ToInt16(args[2]);
            int port_plant_recieve = Convert.ToInt16(args[3]);

            // initialize the DoubleWatertank model
            double dt = 0.01;

            DoubleWatertank watertank = new DoubleWatertank(dt);
            //QuadWatertank watertank = new QuadWatertank(dt);

            Plant plant = new Plant(watertank);

            // create a thread for listening on the controller
            Thread thread_listen_controller = new Thread(() => listen_controller(ip_plant_recieve, port_plant_recieve, plant));
            thread_listen_controller.Start();

            // create a thread for communication with the controller
            Thread thread_send_controller = new Thread(() => send_controller(ip_controller_send, port_controller_send, plant));
            thread_send_controller.Start();

            // create a thread for the DoubleWatertank simulation
            Thread DoubleWatertank = new Thread(() => process(plant));
            DoubleWatertank.Start();

        }

        static public void listen_controller(string IP, int port, Plant plant)
        {
            // initialize a connection to the controller
            Server server = new Server(IP, port);

            while (true)
            {
                Thread.Sleep(5);
                try
                {
                    server.listen();
                    parse_message(server.last_recieved);

                    // parse dictionary to proper input
                    double[] u = new double[package_last.Count];
                    int i = -1;
                    foreach (string key in package_last.Keys)
                    {
                        i++;
                        u[i] = Convert.ToDouble(package_last[key]);
                        Console.WriteLine(key + ":" + u[i]);
                    }
                    plant.set_u(u);
                }
                catch { }
            }
        }

        static public void send_controller(string IP, int port, Plant plant)
        {
            // initialize a connection to the controller
            Client client = new Client(IP, port);

            while (true)
            {
                Thread.Sleep(10);

                // send measurements y      
                string message = "";
                int counter = -1;
                for (int i = 0; i < plant.get_ly().Length; i++)
                {
                    counter++;
                    message += "yo" + (i + 1) + "_" + (plant.get_ly()[i]).ToString() + "#";
                }
                counter = -1;
                for (int i = 0; i < plant.get_cy().Length; i++)
                {
                    counter++;
                    message += "yc" + (i + 1) + "_" + plant.get_cy()[i].ToString() + "#";
                }

                message = message.Substring(0, message.LastIndexOf('#'));
                Console.WriteLine("sent: " + message);
                client.send(message);
            }
        }

        static public void process(Plant plant)
        {
            while (true)
            {
                Thread.Sleep(10);
                plant.update_state();
                // Console.WriteLine("y1: " + Math.Round(plant.get_y()[0], 2) + "  y2: " + Math.Round(plant.get_y()[1], 2));
            }
        }

        public static void parse_message(string message)
        {
            string text = message;
            // split the message with the delimiter '#'
            string[] container = text.Split('#');

            foreach (string item in container)
            {
                // split each subtext (key and value)
                string[] subitem = item.Split('_');

                // extract key and value
                string key = subitem[0];
                string value = subitem[1];

                if (package_last.ContainsKey(key) == false)
                {
                    package_last.Add(key, value);
                }
                package_last[key] = value;
            }
        }
    }

    class Plant
    {
        string model_type = "";

        DoubleWatertank dWT;
        QuadWatertank qWT;

        public Plant(DoubleWatertank watertank)
        {
            model_type = "DoubleWatertank";
            dWT = watertank;
        }

        public Plant(QuadWatertank watertank)
        {
            model_type = "QuadWatertank";
            qWT = watertank;
        }

        public void update_state()
        {
            switch (model_type)
            {
                case "DoubleWatertank":
                    dWT.update_state();
                    break;
                case "QuadWatertank":
                    qWT.update_state();
                    break;
            }
        }

        public double[] get_ly()
        {
            switch (model_type)
            {
                case "DoubleWatertank":
                    return dWT.get_ly();
                case "QuadWatertank":
                    return qWT.get_ly();
                default:
                    return new double[2];

            }
        }

        public double[] get_cy()
        {
            switch (model_type)
            {
                case "DoubleWatertank":
                    return dWT.get_cy();
                case "QuadWatertank":
                    return qWT.get_cy();
                default:
                    return new double[2];
            }
        }

        public void set_u(double[] u_)
        {
            switch (model_type)
            {
                case "DoubleWatertank":
                    dWT.set_u(u_);
                    break;        
                case "QuadWatertank":
                    qWT.set_u(u_);
                    break;  
            }
        }
    }

    class DoubleWatertank
    {
        // internal states
        double h1 = 0; // height tank 1 (top)
        double h2 = 0; // height tank 2 (bottom)
        double h1_max = 20; // tank height
        // signals
        double u = 0; // control signal

        // parameters
        double g = 9.81 * 100;

        // Operating actuator proportional constants [cm^3/(Vs)]
        double k = 5.32;

        // Outlet areas [cm^2]
        double a1 = 0.1678;
        double a2 = 0.1542;

        // Cross section areas [cm^2]
        double A1 = 15.5179;
        double A2 = 15.5179;

        double dt; // time step

        public DoubleWatertank(double _dt)
        {
            dt = _dt;
        }

        public void update_state()
        {
            // out flows
            double q_out1 = a1 * Math.Sqrt(2 * h1 * g);
            double q_out2 = a2 * Math.Sqrt(2 * h2 * g);

            // top tank
            h1 += dt * (1 / A1) * (k * u - q_out1);
            if (h1 < 0) h1 = 0; // empty tank
            if (h1 > h1_max) h1 = h1_max; // empty tank


            // bottom tank
            h2 += dt * (1 / A2) * (q_out1 - q_out2);
            if (h2 < 0) h2 = 0; // empty tank
        }

        public double[] get_ly()
        {
            var r = new GaussianRandom();
            double n = r.NextGaussian(0, 0.05);

            return new double[] { h1 + n };
        }
        public double[] get_cy()
        {
            var r = new GaussianRandom();
            double n = r.NextGaussian(0, 0.05);

            return new double[] { h2 };
        }

        public void set_u(double[] _u)
        {
            u = _u[0];
        }
    }

    public class QuadWatertank
    {
        // internal states
        double h11 = 0; // height tank 1 (top left)
        double h12 = 0; // height tank 1 (top right)
        double h21 = 0; // height tank 2 (bottom left )
        double h22 = 0; // height tank 2 (bottom right)

        // signals
        double u1 = 0; // control signal 1
        double u2 = 0; // control signal 2

        // parameters
        double g = 9.81 * 100; // [cm/s^2]

        // Operating actuator proportional constants [cm^3/(Vs)]
        double k1 = 3.32;
        double k2 = 3.74;

        // Outlet areas [cm^2]
        double a11 = 0.1678;
        double a12 = 0.1542;
        double a21 = 0.06743;
        double a22 = 0.06504;

        // Valve settings
        double gam1 = 0.625;
        double gam2 = 0.625;

        // Cross section areas [cm^2]
        double A11 = 15.5179;
        double A12 = 15.5179;
        double A21 = 15.5179;
        double A22 = 15.5179;

        // time step
        double dt; 

        public QuadWatertank(double _dt)
        {
            dt = _dt;
        }

        public void update_state()
        {
            double q_out11 = a11 * Math.Sqrt(2 * h11 * g);
            double q_out12 = a12 * Math.Sqrt(2 * h12 * g);
            double q_out21 = a21 * Math.Sqrt(2 * h21 * g);
            double q_out22 = a22 * Math.Sqrt(2 * h22 * g);

            // top left tank 
            h11 += dt * (1/ A11) * ((1 - gam2) * k2 * u2 - q_out11);

            // top right tank
            h12 += dt * (1 / A12) * ((1 - gam1) * k1 * u1 - q_out12);

            // bottom left tank
            h21 += dt * (1 / A21) * (gam1 * k1 * u1 + q_out11 - q_out21);
     
            // bottom right tank
            h22 += dt * (1 / A22) * (gam2 * k2 * u2 + q_out12 - q_out22);

            // saturation
            if (h11 < 0) h11 = 0;
            if (h12 < 0) h12 = 0;
            if (h21 < 0) h21 = 0;
            if (h22 < 0) h22 = 0;
        }


        public double[] get_ly()
        {
            var r = new GaussianRandom();
            double n1 = r.NextGaussian(0, 0.05);
            double n2 = r.NextGaussian(0, 0.05);

            return new double[] { h11 + n1, h12 + n2};
        }

        public double[] get_cy()
        {
            var r = new GaussianRandom();
            double n1 = r.NextGaussian(0, 0.05);
            double n2 = r.NextGaussian(0, 0.05);

            return new double[] { h21 + n1, h22 + n2};
        }

        public void set_u(double[] u_)
        {
            u1 = u_[0];
            u2 = u_[1];
        }
    }

    public sealed class GaussianRandom
    {
        private bool _hasDeviate;
        private double _storedDeviate;
        private readonly Random _random;

        public GaussianRandom(Random random = null)
        {
            _random = random ?? new Random();
        }

        public double NextGaussian(double mu = 0, double sigma = 1)
        {
            if (sigma <= 0)
                throw new ArgumentOutOfRangeException("sigma", "Must be greater than zero.");

            if (_hasDeviate)
            {
                _hasDeviate = false;
                return _storedDeviate * sigma + mu;
            }

            double v1, v2, rSquared;
            do
            {
                // two random values between -1.0 and 1.0
                v1 = 2 * _random.NextDouble() - 1;
                v2 = 2 * _random.NextDouble() - 1;
                rSquared = v1 * v1 + v2 * v2;
                // ensure within the unit circle
            } while (rSquared >= 1 || rSquared == 0);

            // calculate polar tranformation for each deviate
            var polar = Math.Sqrt(-2 * Math.Log(rSquared) / rSquared);
            // store first deviate
            _storedDeviate = v2 * polar;
            _hasDeviate = true;
            // return second deviate
            return v1 * polar * sigma + mu;
        }
    }
}
