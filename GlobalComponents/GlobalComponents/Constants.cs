using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalComponents
{
    public struct Constants
    {
        // time formats
        public const string FMT = "yyyy-MM-dd HH:mm:ss.fff";
        const string FMT_plot = "HH:mm:ss";

        // number of items in a DataContainer object
        public const int n_datapoints_max = 3000;
        public const int n_datapoints = 3000;
        public const int n_datapoints_controller = 300;
    }

    public struct ControllerType
    {
        // controller types 
        public const string PID_STANDARD = "PID_STANDARD";
        public const string PID_PLUS = "PID_PLUS";
        public const string PID_SUPPRESS = "PID_SUPPRESS";
    }

    public struct PlantType
    {
        // plant types 
        public const string SIMULATION = "SIMULATION";
        public const string PHYSICAL = "PHYSICAL";
    }

    public struct PhysicalProcessType
    {
        // physical process types 
        public const string SEGWAY = "SEGWAY";
        public const string PLATOONING = "PLATOONING";
        public const string INVPENDULUM = "INVPENDULUM";
    }

    public struct GUIViewMode
    {
        // view modes
        public const string CONTROL = "CONTROL";
        public const string OBSERVE = "OBSERVE";
    }

    public struct DataPointSetting
    {
        // add data point modes (plotting)
        public const string ADD_LATEST = "ADD_LATEST";
        public const string LOAD_ALL = "LOAD_ALL";
    }

    public struct PlantVisualization
    {
        // plant visualization in the HMI module
        public const string NONE = "NONE";
        public const string DOUBLE_WATERTANK = "DOUBLE_WATERTANK";
        public const string QUAD_WATERTANK = "QUAD_WATERTANK";
    }
}
