﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicalProcesses
{
    public class Plant
    {
        string model_type = "";

        DoubleWatertank doubleWatertank;
        QuadWatertank quadWatertank;
        InvertedPendulumSISO InvPendulumSISO;
        InvertedPendulumMIMO InvPendulumMIMO;

        // identify what type of plant is considered
        public Plant() { }

        public Plant(DoubleWatertank watertank)
        {
            model_type = "DoubleWatertank";
            doubleWatertank = watertank;
        }

        public Plant(QuadWatertank watertank)
        {
            model_type = "QuadWatertank";
            quadWatertank = watertank;
        }

        public Plant(InvertedPendulumSISO invertedpendulum)
        {
            model_type = "InvertedPendulum";
            InvPendulumSISO = invertedpendulum;
        }

        // update the model states
        public void UpdateStates()
        {
            switch (model_type)
            {
                case "DoubleWatertank":
                    doubleWatertank.UpdateStates();
                    break;
                case "QuadWatertank":
                    quadWatertank.UpdateStates();
                    break;
                case "InvertedPendulum":
                    InvPendulumSISO.UpdateStates();
                    break;
            }
        }

        // change the model states (applied disturbance)
        public void ChangeState(string state, double disturbance)
        {
            switch (model_type)
            {
                case "DoubleWatertank":
                    doubleWatertank.ChangeState(state, disturbance);
                    break;
                case "QuadWatertank":
                    quadWatertank.ChangeState(state, disturbance);
                    break;
                case "InvertedPendulum":
                    InvPendulumSISO.ChangeState(state, disturbance);
                    break;
            }
        }

        // measure the observed states
        public double[] get_yo()
        {
            switch (model_type)
            {
                case "DoubleWatertank":
                    return doubleWatertank.get_yo();
                case "QuadWatertank":
                    return quadWatertank.get_yo();
                case "InvertedPendulum":
                    return InvPendulumSISO.get_yo();
                default:
                    return new double[2];
            }
        }

        // measure the controlled states
        public double[] get_yc()
        {
            switch (model_type)
            {
                case "DoubleWatertank":
                    return doubleWatertank.get_yc();
                case "QuadWatertank":
                    return quadWatertank.get_yc();
                case "InvertedPendulum":
                    return InvPendulumSISO.get_yc();
                default:
                    return new double[2];
            }
        }

        // update the control input value
        public void set_u(double[] u_)
        {
            switch (model_type)
            {
                case "DoubleWatertank":
                    doubleWatertank.set_u(u_);
                    break;
                case "QuadWatertank":
                    quadWatertank.set_u(u_);
                    break;
                case "InvertedPendulum":
                    InvPendulumSISO.set_u(u_);
                    break;
            }
        }
    } 
}
