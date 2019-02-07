# Modular_Control_System
This project is conducted at the department of automatic control at KTH, Stockholm. The project is to develop a simulation environment for a modular and networked control system. The purpose of the simulation environment is to realistically study attack and defence mechanisms, both well known and to develop new ones. Several modules make up the whole system and are each described in sections below. In total there are four modules: HMI (Human Machine Interface), Control, Plant, and Channel.

An overview of the modular control system is presented below, where two Channel modules are used to enable integrity and simulated dos attacks on the data being transmitted between the modules. The system can be scaled up as an arbitrary number of plants and can be controlled in parallel from one single HMI, illustrated in the figure.

![mcs_basic_overview](https://user-images.githubusercontent.com/25713113/52147441-03474480-2667-11e9-9cef-200f0cba1618.png)

As the system is network based, a module can be running on an arbitrary machine, i.e. all modules can be running on the same machine, or they could be running on individual ones as illustrated in the figure above.

## Modules
### HMI
With the HMI module the user can 1) connect to new Control modules, 2) tune parameters such as the reference set-point and controller parameters, 3) select which Control module to monitor. In addition, details of each controller module is presented in a tree view as well as an animation of the process. The chart shows all variables and parameters associated with the control of the plant, that is, the reference set-point, control signal, state measurements, and state estimates using the Kalman filter.

Furthermore, residual and security metric graphs indicates how much the actual state measurements deviate from the estimated one. The deviation is used to quantify the probability of an attack occurring.

### Control
The Control module contains PID-controllers which produces the control signals which manipulates the simulated physical process. The inputs to the Control module are the reference set-point **r** and controller parameters **K_P**, **K_I**, **K_D** from the HMI module, along with the measurement signal **y** from the plant. The outputs from the Control module consists of the control signal **u** to the Plant module, along with the same control signal **u** and received measurement signal **y** to the HMI module. The communication from the Control module to the HMI module is only for monitoring purposes. Two types of PID controllers are available in the Control module: a standard PID controller, and a PIDPlus controller.

### Plant
Two plant models are included in the Plant module; a single-output single-input (SISO) double water tank system, and a multi-input multi-output (MIMO) quad water tank system. In the double water tank case water is pumped into the upper tank which flows into the lower one and then exits the system. In the case of the quad water tank, two pumps pump water into the upper tanks which induce coupling between the inputs and outputs.

### Channel
The main purpose of the Channel module is to enable the execution of integrity attacks and drop-out on packages send between the HMI- and Control module as well as between the control- and Plant module. However it is possible to run the modules without the Channel module, but in that case integrity attacks are not possible, and the packages are directly sent between the modules.

## Executing modules
The modules are executed through the command line. Examples on how to execute the modules are presented below.

### With Channel module
#### HMI
When executing the Channel module, the IP address and port of the Channel module between the HMI and Control module must be specified, e.g.
```
Start HMI.exe channel_plant=127.0.0.1:8111
```
The IP:port pair of the Control module(s) are specified in the GUI.

#### Control
When executing the Control module, the IP:port pairs for both Channel modules (briding to the HMI and Plant respectively) must be specified, e.g. `channel_gui=127.0.0.1:8111` and `channel_plant=127.0.0.1:8222`.

Furthermore the end-points of the HMI and Plant modules must be specified, e.g. `gui_ep=127.0.0.1:8100:8200` and `plant_ep=127.0.0.1:8300:8400`, where the second port entry dentoes to receiving port on the Control module.

The controller type and logging flag must also be specified, e.g. `controller=PID_normal` and `log=false`

Below is an example of how the Controller module can be executed.
```
Start Controller.exe gui_ep=127.0.0.1:8100:8200 plant_ep=127.0.0.1:8300:8400 channel_gui=127.0.0.1:8111 channel_plant=127.0.0.1:8222 controller=PID_normal log=false
```

#### Plant
When executing the Plant module, three arguments are required in total when using the Channel module. Firstly, the Control module end-point must be specified, along with the Channel module IP:port pair, e.g. `controller_ep=127.0.0.1:8400:8300` and `channel_controller=127.0.0.1:8222` respectively. Furthermore, the parameters for the simulated physical model must be specified, e.g. `model=dwt:5,0:0,3:5,0:0,2:0,00001`. The model argument syntax is the following: `model=<process type>:<top tank area>:<top tank outlet area>:<bottom tank area>:<bottom tank outlet area>:<measurement noise std>`.

Below is an example of how the Plant module can be executed.
```
Start Model_GUI.exe controller_ep=127.0.0.1:8400:8300 channel_controller=127.0.0.1:8222 model=dwt:5,0:0,3:5,0:0,2:0,5 log=false
```

#### Channel
When executing the Channel module the receiving port must be specified. Optionally, drop-out parameters for the Markov model can be specified.

Below are two examples of how the Channel module can be executed.
```
Start Channel_GUI.exe port_receive=8111 markov=90:98
```
```
Start Channel_GUI.exe port_receive=8111 bernoulli=80
```
