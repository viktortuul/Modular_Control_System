using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalComponents
{
    public class Configuration
    {
        public KalmanDoubleWatertankModel kalmanDoubleWatertankModel { get; set; }
        public PidParameters pidParameters { get; set; }
        public AnomalyDetector anomalyDetector { get; set; }
        public ControlConfig controlConfig { get; set; }
        public PlantConfig plantConfig { get; set; }
    }

    public class PidParameters
    {
        public double P { get; set; }
        public double I { get; set; }
        public double D { get; set; }
    }

    public class KalmanDoubleWatertankModel
    {
        public double A1 { get; set; }
        public double a1 { get; set; }
        public double A2 { get; set; }
        public double a2 { get; set; }
        public double k { get; set; }
    }

    public class AnomalyDetector
    {
        public double cusumDelta { get; set; }
    }

    public class ControlConfig
    {
        public string type { get; set; } // 'PID_STANDARD', 'PID_PLUS', 'PID_SUPPRESS'
        public double range_min { get; set; }
        public double range_max { get; set; }
    }

    public class PlantConfig
    {
        public string type { get; set; } // 'DOUBLE_WATERTANK', 'QUAD_WATERTANK',
        public double A1 { get; set; }
        public double a1 { get; set; }
        public double A2 { get; set; }
        public double a2 { get; set; }
        public double meas_noise_std { get; set; }
    }
}
