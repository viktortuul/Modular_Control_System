# Modular_Control_System
This project is conducted at the department of automatic control at KTH, Stockholm. The project is to develop a testbed for a modular and networked control system. The purpose of the testbed is to realistically study attack and defence mechanisms, both well known and to develop new ones. The [Deployment tutorial - Testbed Evaluation of DoS Attack](https://github.com/viktortuul/Modular_Control_System/wiki/Deployment-tutorial) page presents a tutorial on how to replicate the results that were presented in a short-paper using this simulation environment.

Several modules make up the system and are each described on the [wiki page](https://github.com/viktortuul/Modular_Control_System/wiki/Modules). In total there are four modules: HMI (Human Machine Interface), Control, Plant, and Channel. An overview of the modular control system is presented below, where two Channel modules are used to enable integrity and simulated dos attacks on the data being transmitted between the modules. The system can be scaled up as an arbitrary number of plants and can be controlled in parallel from one single HMI, illustrated in the figure.

<p align="center">
  <img src="https://user-images.githubusercontent.com/25713113/52147441-03474480-2667-11e9-9cef-200f0cba1618.png">
</p>

As the system is network based, a module can be running on an arbitrary machine, i.e. all modules can be running on the same machine, or they could be running on individual ones as illustrated in the figure above.

For further documentation and tutorials, please visit the [wiki page](https://github.com/viktortuul/Modular_Control_System/wiki).
