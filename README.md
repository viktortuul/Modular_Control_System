# Modular_Control_System
This project is conducted at the department of automatic control at KTH, Stockholm. The project is to develop a simulation environment for a modular and networked control system. The purpose of the simulation environment is to realistically study attack and defence mechanisms, both well known and to develop new ones. The [Deployment tutorial - Testbed Evaluation of DoS Attack](https://github.com/viktortuul/Modular_Control_System/wiki/Deployment-tutorial) page presents a tutorial on how to replicate the results that were presented in a short-paper using this simulation environment.

Several modules make up the whole system and are each described in sections below. In total there are four modules: HMI (Human Machine Interface), Control, Plant, and Channel. An overview of the modular control system is presented below, where two Channel modules are used to enable integrity and simulated dos attacks on the data being transmitted between the modules. The system can be scaled up as an arbitrary number of plants and can be controlled in parallel from one single HMI, illustrated in the figure.

<p align="center">
  <img src="https://user-images.githubusercontent.com/25713113/52147441-03474480-2667-11e9-9cef-200f0cba1618.png">
</p>

As the system is network based, a module can be running on an arbitrary machine, i.e. all modules can be running on the same machine, or they could be running on individual ones as illustrated in the figure above.

For further documentation and tutorials, please visit the [wiki page](https://github.com/viktortuul/Modular_Control_System/wiki).

## Modules
### HMI
With the HMI module the user can 1) connect to new Control modules, 2) tune parameters such as the reference set-point and controller parameters, 3) select which Control module to monitor. In addition, details of each controller module is presented in a tree view as well as an animation of the process. The chart shows all variables and parameters associated with the control of the plant, that is, the reference set-point, control signal, state measurements, and state estimates using the Kalman filter.

Furthermore, residual and security metric graphs indicates how much the actual state measurements deviate from the estimated one. The deviation is used to quantify the probability of an attack occurring.

Below is a video of the HMI module application showing reference set-point change and PID controller tuning.
<p align="center">
  <img src="https://user-images.githubusercontent.com/25713113/54478156-8f748c00-480f-11e9-85ef-f92e60bc61ee.gif" width="700">
</p>

### Control
The Control module contains PID-controllers which produces the control signals which manipulates the simulated physical process. The inputs to the Control module are the reference set-point **r** and controller parameters **K_P**, **K_I**, **K_D** from the HMI module, along with the measurement signal **y** from the plant. The outputs from the Control module consists of the control signal **u** to the Plant module, along with the same control signal **u** and received measurement signal **y** to the HMI module. The communication from the Control module to the HMI module is only for monitoring purposes. Two types of PID controllers are available in the Control module: a standard PID controller, and a PIDPlus controller.

### Plant
Two plant models are included in the Plant module; a single-output single-input (SISO) double water tank system, and a multi-input multi-output (MIMO) quad water tank system. In the double water tank case water is pumped into the upper tank which flows into the lower one and then exits the system. In the case of the quad water tank, two pumps pump water into the upper tanks which induce coupling between the inputs and outputs.

Below is a video of the Plant module application showing disturbances being applied.
<!--
<img src="https://user-images.githubusercontent.com/25713113/52367378-3a916900-2a4c-11e9-931f-469939304542.png" width="750">
-->
<p align="center">
  <img src="https://user-images.githubusercontent.com/25713113/54478354-6fde6300-4811-11e9-9345-1a0d554005d5.gif" width="700">
</p>

The Plant module can however be substituted with a physical process. Currently, the [Lego Mindstorms EV3](https://www.lego.com/en-us/mindstorms/about-ev3) robot platform is supported.

### Channel
The main purpose of the Channel module is to enable the execution of integrity attacks and drop-out on packets send between the HMI- and Control module as well as between the control- and Plant module. However it is possible to run the modules without the Channel module, but in that case integrity attacks and simulated DoS attacks are not possible, and the packets are directly sent between the modules.

<!--
![channel](https://user-images.githubusercontent.com/25713113/52367673-e3d85f00-2a4c-11e9-8efb-cf4751e7b09a.png)
<img src="https://user-images.githubusercontent.com/25713113/52367673-e3d85f00-2a4c-11e9-8efb-cf4751e7b09a.png" width="650">
-->
Below is a video of the Channel module application, showing the packet drop-out settings.
<p align="center">
  <img src="https://user-images.githubusercontent.com/25713113/54478155-8f748c00-480f-11e9-8f96-28d6897ae4dd.gif" width="700">
</p>

<!--
![channel_atk](https://user-images.githubusercontent.com/25713113/52367674-e470f580-2a4c-11e9-9e09-d4a5f121073e.png)
-->
Below is a figure of the Channel module application, showing the channel- and attack settings.
<p align="center">
  <img src="https://user-images.githubusercontent.com/25713113/52367674-e470f580-2a4c-11e9-9e09-d4a5f121073e.png" width="700">
</p>





